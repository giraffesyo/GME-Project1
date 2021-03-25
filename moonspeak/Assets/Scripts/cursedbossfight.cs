using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class cursedbossfight : MonoBehaviour
{
    // Start is called before the first frame update\
    public Camera cam;
    public Animator rightArm;
    public Transform throwArm;
    public GameObject thrownProjectile;
    float throwCountdown = 0;
    float throwTimer = 3;
    public List<string> bodypartAlive;
    private List<GameObject> bodyParts;
    bool gameRunning = true;
    public string currentName;
    public GameObject currentFood;
    private List<GameObject> reviewObjectList;
    public List<GameObject> buttonTexts;
    public List<string> objectNames;
    public GameObject WinScreen;
    void Start()
    {
        cam = Camera.main;
        bodyParts = RemoteAssetLoader.Instance.GetAssets(AssetLabels.BossProjectiles);
        reviewObjectList = RemoteAssetLoader.Instance.GetAssets(AssetLabels.LearningObjects);
        NextReviewQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        if (bodypartAlive.Count == 0 && gameRunning == true)
        {
            StartCoroutine(Die());
            gameRunning = false;
        }   

        throwCountdown += Time.deltaTime;
        if (throwArm != null)
        {
            if (throwTimer <= throwCountdown)
            {

                GameObject toss = Instantiate(thrownProjectile, throwArm.position, Quaternion.identity);
                toss.transform.DOMove(Camera.main.transform.position, 3);
                Destroy(toss, 3.2f);
                rightArm.SetTrigger("Throw");
                throwCountdown = 0;


            }
        }
        

        transform.LookAt(cam.transform);
        transform.eulerAngles = transform.eulerAngles + 180f * Vector3.up;

        
    }

    IEnumerator Die()
    {
        int i = 0;
        Destroy(currentFood);
        Destroy(GameObject.Find("ReviewButtons"));
        while (i < 1000)
        {
            i++;
            GameObject temp = Instantiate(bodyParts[Random.Range(0, bodyParts.Count)], transform.position, Random.rotation);
            temp.GetComponent<Rigidbody>().velocity = (Random.onUnitSphere) * Random.Range(8, 20);
            temp.GetComponent<Rigidbody>().useGravity = true;
            temp.tag = "Untagged";
            Destroy(temp, 3);
            yield return null;
        }

        GameObject win = Instantiate(WinScreen);
    }

    public void CheckAnswer(GameObject buttonText)
    {
        
        if (currentName == buttonText.GetComponent<TextMeshProUGUI>().text)
        {
            gameObject.GetComponent<shootfruit>().addProjectile(currentFood);
            NextReviewQuestion();
        }
    }

    public void NextReviewQuestion()
    {
        int randomObject = Random.Range(0, reviewObjectList.Count);
        Debug.Log(randomObject);
        DestroyImmediate(currentFood, true);
        currentFood = GameObject.Instantiate(reviewObjectList[randomObject], new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), Quaternion.identity);
        currentFood.transform.localScale = currentFood.transform.localScale * 4;
        StartCoroutine(InitReviewButtons());

    }

    IEnumerator InitReviewButtons()
    {
        yield return new WaitForEndOfFrame();
        currentName = currentFood.name.Replace("(Clone)", "");

        int randomIndex = Random.Range(0, buttonTexts.Count);
        List<string> temp = new List<string>(objectNames);
        temp.Remove(currentName);
        for (int i = 0; i < 3; i++)
        {
            string tempName = temp[Random.Range(0, temp.Count - 1)];
            temp.Remove(tempName);
            buttonTexts[i].GetComponent<TextMeshProUGUI>().text = tempName;
        }

        buttonTexts[randomIndex].GetComponent<TextMeshProUGUI>().text = currentName;
    }
}
