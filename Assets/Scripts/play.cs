using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameModes
{
    learn,
    review
}
public class play : MonoBehaviour
{

    public List<GameObject> objects;
    public List<string> objectNames;
    public List<GameObject> buttonTexts;
    public List<GameObject> reviewObjectList;
    public GameObject ReviewButtons;
    public GameObject Instructions;
    public int index = 0;
    private string currentName;
    public int maxIndex;
    private GameObject currentFood;

    public GameModes GameMode = GameModes.learn;

    private void Awake()
    {

    }
    void Start()
    {
        ReviewButtons.SetActive(false);
        currentFood = GameObject.Instantiate(objects[0], transform);
        maxIndex = objects.Count;
    }


    void Update()
    {
        if (GameMode == GameModes.learn)
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
        else if (GameMode == GameModes.review)
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

        GameMode = GameModes.review;
        ReviewButtons.SetActive(true);
        Instructions.SetActive(false);
        reviewObjectList = new List<GameObject>(objects);
        NextReviewQuestion();
    }

    public void CheckAnswer(GameObject buttonText)
    {
        if (currentName == buttonText.GetComponent<TextMeshProUGUI>().text)
        {
            NextReviewQuestion();
        }
    }

    public void NextReviewQuestion()
    {
        if (reviewObjectList.Count <= 0)
        {
            SetLearn();
            return;
        }
        int randomObject = Random.Range(0, reviewObjectList.Count);
        Debug.Log(randomObject);
        Destroy(currentFood);
        currentFood = GameObject.Instantiate(reviewObjectList[randomObject], transform);
        reviewObjectList.RemoveAt(randomObject);
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
        GameMode = GameModes.learn;
        ReviewButtons.SetActive(false);
        Instructions.SetActive(true);
        Destroy(currentFood);
        currentFood = GameObject.Instantiate(objects[0], transform);
    }

    public void SwapMode()
    {
        if (GameMode == GameModes.learn)
        {
            SetReview();
        }
        else if (GameMode == GameModes.review)
        {
            SetLearn();
        }
    }
}
