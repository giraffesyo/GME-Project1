using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class play : MonoBehaviour
{

    public List<GameObject> objects;
    public List<string> objectNames;
    public List<GameObject> buttonTexts;
    public GameObject ReviewButtons;
    public GameObject Instructions;
    public int index = 0;
    private string currentName;
    public int maxIndex;
    private GameObject currentFood;

    public string GameMode = "learn";

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        ReviewButtons.SetActive(false);
        currentFood = GameObject.Instantiate(objects[0], transform);
        maxIndex = objects.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMode == "learn")
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                index--;
                if (index < 0)
                {
                    index = maxIndex - 1;
                }

                Destroy(currentFood);
                currentFood = GameObject.Instantiate(objects[index], transform);

            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                index++;
                if (index >= maxIndex)
                {
                    index = 0;
                }

                Destroy(currentFood);
                currentFood = GameObject.Instantiate(objects[index], transform);
            }
        }
        else if(GameMode == "review")
        {
            if (GameObject.Find("Name"))
            {
                currentName = currentFood.name.Replace("(Clone)", "");
                Destroy(GameObject.Find("Name"));

            }

            //GameObject.Find("Object1")
        }
    }

    void SetReview()
    {
        
        GameMode = "review";
        ReviewButtons.SetActive(true);
        Instructions.SetActive(false);
        NextReviewQuestion();
    }

    public void CheckAnswer(GameObject buttonText)
    {
        if(currentName == buttonText.GetComponent<TextMeshProUGUI>().text)
        {
            NextReviewQuestion();
        }
    }

    public void NextReviewQuestion()
    {
        int randomObject = Random.Range(0, maxIndex);
        Destroy(currentFood);
        currentFood = GameObject.Instantiate(objects[randomObject], transform);
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

    void SetLearn()
    {
        GameMode = "learn";
        ReviewButtons.SetActive(false);
        Instructions.SetActive(true);
        Destroy(currentFood);
        currentFood = GameObject.Instantiate(objects[0], transform);
    }

    public void SwapMode()
    {
        if(GameMode == "learn")
        {
            SetReview();
        }
        else if(GameMode == "review")
        {
            SetLearn();
        }
    }
}
