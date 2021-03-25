from flask import Flask, jsonify, request, abort
import boto3
import os
from botocore.exceptions import ClientError

ACCESS_KEY = os.environ['MOONSPEAK_ACCESS_KEY']
SECRET_KEY = os.environ['MOONSPEAK_SECRET_KEY']


dynamodb = boto3.client('dynamodb', aws_access_key_id=ACCESS_KEY,
                        aws_secret_access_key=SECRET_KEY
                        )

#dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('Scores')

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
