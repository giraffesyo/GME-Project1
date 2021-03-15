using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ReviewModes
{
    ThreeObjects,
    ThreeWords,
    Speech
}

public class ReviewManager : MonoBehaviour
{

    public List<GameObject> objects;
    public List<string> objectNames;
    public List<TextMeshProUGUI> buttonTexts;
    public int index = 0;
    private string currentName;
    public int maxIndex;
    private GameObject currentFood;
    [SerializeField]
    private GameObject threeObjectsContainer;
    [SerializeField]
    private GameObject threeWordsContainer;
    [SerializeField]
    private GameObject speechContainer;

    // TODO: If we setup player preferences we can load this from there
    private int numberOfReviewModes = System.Enum.GetValues(typeof(ReviewModes)).Cast<int>().Max();

    private List<GameObject> reivewQueue;

    public SceneLoader sceneLoader;

    void Start()
    {
        // ReviewButtons.SetActive(false);
        currentFood = GameObject.Instantiate(objects[0], transform);
        sceneLoader = GetComponent<SceneLoader>();
        maxIndex = objects.Count;
        reivewQueue = new List<GameObject>(objects);
        NextReviewQuestion();
    }



    void Update()
    {
        // override to skip the question
        if (Input.GetKeyDown(KeyCode.F))
        {
            NextReviewQuestion();
        }

        if (GameObject.Find("Name"))
        {
            currentName = currentFood.name.Replace("(Clone)", "");
            Destroy(GameObject.Find("Name"));

        }
    }


    public void CheckAnswer(GameObject buttonText)
    {
        if (currentName == buttonText.GetComponent<TextMeshProUGUI>().text)
        {
            // TODO: they got it right, send message about their score for this item
            NextReviewQuestion();
        }
        else
        {
            // FIXME: they got it wrong, show it agin later, show the right answer 
            //TODO: possible todo: move it back one bucket
        }
    }

    public void NextReviewQuestion()
    {
        // get a random mode to switch to 
        ReviewModes randomMode = (ReviewModes)Random.Range(0, numberOfReviewModes + 1);
        if (randomMode == ReviewModes.Speech)
        {
            speechContainer.SetActive(true);
            threeWordsContainer.SetActive(false);
            threeObjectsContainer.SetActive(false);
        }
        else if (randomMode == ReviewModes.ThreeWords)
        {
            speechContainer.SetActive(false);
            threeWordsContainer.SetActive(true);
            threeObjectsContainer.SetActive(false);
        }
        else if (randomMode == ReviewModes.ThreeObjects)
        {
            speechContainer.SetActive(false);
            threeWordsContainer.SetActive(false);
            threeObjectsContainer.SetActive(true);
        }

        if (reivewQueue.Count <= 0)
        {
            sceneLoader.LoadScene("menu");
            return;
        }

        int randomObject = Random.Range(0, reivewQueue.Count);

        Destroy(currentFood);
        currentFood = GameObject.Instantiate(reivewQueue[randomObject], transform);
        reivewQueue.RemoveAt(randomObject);
        StartCoroutine(InitReviewButtons());
    }



    IEnumerator InitReviewButtons()
    {
        yield return new WaitForEndOfFrame();
        // get the name of the fruit from the cloned prefab
        currentName = currentFood.name.Replace("(Clone)", "");
        // pick a random number which will be used to put the correct answer on the button
        int randomIndex = Random.Range(0, buttonTexts.Count);
        // create a list of ALL possible objects in user's review queue, store it in temp
        List<string> temp = new List<string>(objectNames);
        // remove the correct answer from the list
        temp.Remove(currentName);
        // put three incorrect answers on the buttons
        for (int i = 0; i < 3; i++)
        {
            string tempName = temp[Random.Range(0, temp.Count - 1)];
            temp.Remove(tempName);
            buttonTexts[i].text = tempName;
        }
        // put the correct answer on the previously selected button, overwriting one of the incorrect options
        buttonTexts[randomIndex].text = currentName;

    }

}
