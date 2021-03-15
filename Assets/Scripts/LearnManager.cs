using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// public enum GameModes
// {
//     learn,
//     review
// }

public class LearnManager : MonoBehaviour
{

    public List<GameObject> objects;
    public List<string> objectNames;
    // public List<GameObject> buttonTexts;

    public GameObject Instructions;
    public int index = 0;
    private string currentName;
    public int maxIndex;
    private GameObject currentFood;

    // public GameModes GameMode = GameModes.learn;

    // TODO: If we setup player preferences we can load this from there
    private bool shouldShowSwipeInstructions = false;
    private int numberOfReviewModes = System.Enum.GetValues(typeof(ReviewModes)).Cast<int>().Max();

    void Start()
    {
        SwipeDetector.OnSwipe += SwipeHandler;
        // ReviewButtons.SetActive(false);
        currentFood = GameObject.Instantiate(objects[0], transform);
        maxIndex = objects.Count;
    }

    private void AdvanceLearnItems(bool backwards = false)
    {
        if (backwards)
        {
            index--;
            if (index < 0)
            {
                index = maxIndex - 1;
            }

            Destroy(currentFood);
            currentFood = GameObject.Instantiate(objects[index], transform);
        }
        else
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

    private void SwipeHandler(SwipeData swipe)
    {
        // we're only handling swipes inside the learn game mode right now

        if (swipe.Direction == SwipeDirection.Right)
        {
            AdvanceLearnItems();
        }
        else if (swipe.Direction == SwipeDirection.Left)
        {
            AdvanceLearnItems(backwards: true);
        }
        // hide the instruction text 
        shouldShowSwipeInstructions = false;
        Instructions.SetActive(false);

    }

    void Update()
    {

        // if (GameMode == GameModes.learn)
        // {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AdvanceLearnItems(backwards: true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AdvanceLearnItems();
        }
        // }
        // else if (GameMode == GameModes.review)
        // {
        //     // override to skip the question
        //     if (Input.GetKeyDown(KeyCode.F))
        //     {
        //         NextReviewQuestion();
        //     }

        //     if (GameObject.Find("Name"))
        //     {
        //         currentName = currentFood.name.Replace("(Clone)", "");
        //         Destroy(GameObject.Find("Name"));

        //     }

        //     //GameObject.Find("Object1")
        // }
    }
    // void SetGameMode(GameModes mode)
    // {
    //     GameMode = mode;
    //     if (mode == GameModes.learn)
    //     {
    //         ReviewButtons.SetActive(false);
    //         if (shouldShowSwipeInstructions)
    //         {
    //             Instructions.SetActive(true);
    //         }
    //         Destroy(currentFood);
    //         currentFood = GameObject.Instantiate(objects[0], transform);
    //     }
    //     else if (mode == GameModes.review)
    //     {
    //         ReviewButtons.SetActive(true);
    //         Instructions.SetActive(false);
    //         reviewObjectList = new List<GameObject>(objects);
    //         NextReviewQuestion();
    //     }
    // }

    // public void CheckAnswer(GameObject buttonText)
    // {
    //     if (currentName == buttonText.GetComponent<TextMeshProUGUI>().text)
    //     {
    //         // TODO: they got it right, send message about their score for this item
    //         NextReviewQuestion();
    //     }
    //     else
    //     {
    //         // FIXME: they got it wrong, show it agin later, show the right answer 
    //         //TODO: possible todo: move it back one bucket
    //     }
    // }

    // public void NextReviewQuestion()
    // {
    //     // get a random mode to switch to 
    //     ReviewModes randomMode = (ReviewModes)Random.Range(0, numberOfReviewModes + 1);
    //     if (randomMode == ReviewModes.Speech)
    //     {
    //         SpeechContainer.SetActive(true);
    //         ThreeWordsContainer.SetActive(false);
    //         ThreeObjectsContainer.SetActive(false);
    //     }
    //     else if (randomMode == ReviewModes.ThreeWords)
    //     {
    //         SpeechContainer.SetActive(false);
    //         ThreeWordsContainer.SetActive(true);
    //         ThreeObjectsContainer.SetActive(false);
    //     }
    //     else if (randomMode == ReviewModes.ThreeObjects)
    //     {
    //         SpeechContainer.SetActive(false);
    //         ThreeWordsContainer.SetActive(false);
    //         ThreeObjectsContainer.SetActive(true);
    //     }

    //     if (reviewObjectList.Count <= 0)
    //     {
    //         // FIXME: Return them to main menu, not learn mode
    //         SetGameMode(GameModes.learn);
    //         return;
    //     }

    //     int randomObject = Random.Range(0, reviewObjectList.Count);

    //     Destroy(currentFood);
    //     currentFood = GameObject.Instantiate(reviewObjectList[randomObject], transform);
    //     reviewObjectList.RemoveAt(randomObject);
    //     StartCoroutine(InitReviewButtons());
    // }



    // IEnumerator InitReviewButtons()
    // {
    //     yield return new WaitForEndOfFrame();
    //     // get the name of the fruit from the cloned prefab
    //     currentName = currentFood.name.Replace("(Clone)", "");
    //     // pick a random number which will be used to put the correct answer on the button
    //     int randomIndex = Random.Range(0, buttonTexts.Count);
    //     // create a list of ALL possible objects in user's review queue, store it in temp
    //     List<string> temp = new List<string>(objectNames);
    //     // remove the correct answer from the list
    //     temp.Remove(currentName);
    //     // put three incorrect answers on the buttons
    //     for (int i = 0; i < 3; i++)
    //     {
    //         string tempName = temp[Random.Range(0, temp.Count - 1)];
    //         temp.Remove(tempName);
    //         buttonTexts[i].GetComponent<TextMeshProUGUI>().text = tempName;
    //     }
    //     // put the correct answer on the previously selected button, overwriting one of the incorrect options
    //     buttonTexts[randomIndex].GetComponent<TextMeshProUGUI>().text = currentName;

    // }

    // public void SwapMode()
    // {
    //     if (GameMode == GameModes.learn)
    //     {
    //         SetGameMode(GameModes.review);
    //     }
    //     else if (GameMode == GameModes.review)
    //     {
    //         SetGameMode(GameModes.learn);
    //     }
    // }
}
