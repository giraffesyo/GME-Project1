from flask import Flask, jsonify, request, abort, send_file, redirect
from werkzeug.utils import secure_filename
from io import BytesIO
import boto3
import os
from botocore.exceptions import ClientError

ACCESS_KEY = os.environ['MOONSPEAK_ACCESS_KEY']
SECRET_KEY = os.environ['MOONSPEAK_SECRET_KEY']
S3_BUCKET = "moonspeak" # idk whatever works


session = boto3.session.Session(aws_access_key_id=ACCESS_KEY,
                                aws_secret_access_key=SECRET_KEY
                                )

dynamodb = session.resource('dynamodb')
table = dynamodb.Table('Scores')
s3_client = session.client('s3')
s3_client.create_bucket(Bucket=S3_BUCKET)

app = Flask(__name__)


@app.route('/')
def index():
    return '<h1>API is online</h1>'


@app.route('/user/', methods=['POST'])
def postScores():
    # Bad request, missing user data
    if request.json is None or 'user' not in request.json:
        abort(400)

    incoming = request.json['user']
    username = incoming['username']

    # Get User
    try:
        response = table.get_item(Key={'username': username})
    except ClientError as e:
        print('Error fetching user!', e)

    userExists = 'Item' in response

    # User exists in DB
    if userExists:
        user = response['Item']

    # User does not exist in DB, create user
    else:
        user = {'username': username, 'scores': {}}

    # Combine incoming user scores with current user scores
    scores = incoming['scores']

    for key, value in scores.items():
        user['scores'][key] = value

    # Post to DB
    if userExists:
        try:
            response = table.update_item(
                Key={'username': user['username']},
                UpdateExpression='set scores=:s',
                ExpressionAttributeValues={':s': user['scores']})
        except ClientError as e:
            print('Error updating user!', e)
    else:
        try:
            response = table.put_item(Item=user)
        except ClientError as e:
            print('Error creating user!', e)

    return response

# Fetches user information
# If username does not exist in DB, create user


@app.route('/user/<username>', methods=['GET'])
def getUserInfo(username):
    username = username.strip()

    try:
        response = table.get_item(Key={'username': username})
    except ClientError as e:
        print('Error fetching user!', e)

    # User exists in DB
    if 'Item' in response:
        user = response['Item']
        for key, value in user['scores'].items():
            user['scores'][key] = int(value)
        return jsonify(user)

    # User does not exist in DB, create user
    user = {'username': username, 'scores': {}}
    try:
        response = table.put_item(Item=user)
    except ClientError as e:
        print('Error creating user!', e)
    return jsonify(user)

# TODO: ensure this works...
# https://boto3.amazonaws.com/v1/documentation/api/latest/guide/s3-examples.html
def upload_to_s3(upload_folder, file, bucket, acl="public-read"):
    try:
        s3_client.upload_fileobj(
            file,
            bucket,
            f'{upload_folder}/{file.filename}',
            ExtraArgs = {
                "ACL": acl,
                "ContentType": file.content_type
            }
        )
    except ClientError as e:
        print(f'Error downloading from s3 {e}')
        return False
    return True

def get_from_s3(filename, bucket):
    try:
        file = s3_client.get_object(Bucket=bucket, Key=filename)
        return file['Body']
    except ClientError as e:
        print(f'Error downloading from s3 {e}')
        return None

# Get the asset files from wherever its being hosted
@app.route('/assets', methods=['GET'])
def getFile():
    try:
        filename = request.args.get('filename')
        file = get_from_s3(filename, S3_BUCKET)
        print(file)
        # return send_file(os.path.join(os.getcwd(), 'assets/' + filename), as_attachment=True, attachment_filename=filename)
        return send_file(file, mimetype="text/plain", as_attachment=True, attachment_filename=filename)
    except FileNotFoundError:
        abort(404)

@app.route('/uploads', methods=['GET', 'POST'])
def upload_file():
    if request.method == 'POST':
        files = request.files.getlist('file')
        upload_folder = request.form.get('buildtarget', 'iOS')
        if len(files) == 0:
            return redirect(request.url)
        if all(allowed_file(file.filename) for file in files):
            for file in files:
                upload_to_s3(upload_folder, file, S3_BUCKET)
            return redirect('/')
    return '''
    <!doctype html>
    <title>Upload new asset build</title>
    <h1>Upload file</h1>
    <form method=post enctype=multipart/form-data>
      <select name=buildtarget>
        <option value=iOS>iOS</option>
        <option value=StandaloneWindows64>Windows x64</option>
      </select>
      <input type=file name=file multiple>
      <input type=submit value=Upload>
    </form>
    '''

def allowed_file(filename):
    return '.' in filename and \
            filename.rsplit('.', 1)[1].lower() in { 'hash', 'json', 'bundle' }

# fails cause vercel is read-only
def save_file(upload_folder, file):
    filename = secure_filename(file.filename)
    upload_path = os.path.join(os.getcwd(), f'assets/{upload_folder}')
    if not os.path.exists(upload_path):
        os.makedirs(upload_path)
    file.save(f'{upload_path}/{filename}')
