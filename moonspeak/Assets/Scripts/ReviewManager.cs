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

    private List<GameObject> objects;
    public List<string> objectNames;
    public List<TextMeshProUGUI> buttonTexts;
    public int index = 0;
    private string currentName;
    public int maxIndex;
    private ReviewModes currentMode;
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


    void Start()
    {
        // ReviewButtons.SetActive(false);
        objects = RemoteAssetLoader.Instance.GetAssets(AssetLabels.LearningObjects);
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

        if (Input.GetMouseButtonUp(0))
        {
            if (currentMode == ReviewModes.ThreeObjects)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    string answer = hit.transform.parent.name.Replace("(Clone)", "");
                    CheckAnswer(answer);
                }
            }
        }
    }


    public void AnswerClicked(TextMeshProUGUI buttonText)
    {
        string answer = buttonText.text;
        CheckAnswer(answer);
    }

    public void CheckAnswer(string answer)
    {
        if (answer == currentName)
        {
            // TODO: they got it right, send message about their score for this item
            PlayerInfo.playerInfo.UpdateScore(answer, 1);
            NextReviewQuestion();
        }
        else
        {
            // FIXME: they got it wrong, show it agin later, show the right answer 
            //TODO: possible todo: move it back one bucket
            PlayerInfo.playerInfo.UpdateScore(currentName, -1);
        }
    }
    public void NextReviewQuestion()
    {
        // get a random mode to switch to 
        ReviewModes randomMode = (ReviewModes)Random.Range(0, numberOfReviewModes + 1);
        int randomObject = Random.Range(0, reivewQueue.Count);
        currentMode = randomMode;
        if (randomMode == ReviewModes.Speech)
        {
            speechContainer.SetActive(true);
            threeWordsContainer.SetActive(false);
            threeObjectsContainer.SetActive(false);

            StartCoroutine(InitReviewButtons());
        }
        else if (randomMode == ReviewModes.ThreeWords)
        {
            speechContainer.SetActive(false);
            threeWordsContainer.SetActive(true);
            threeObjectsContainer.SetActive(false);

            StartCoroutine(InitReviewButtons());
        }
        else if (randomMode == ReviewModes.ThreeObjects)
        {
            speechContainer.SetActive(false);
            threeWordsContainer.SetActive(false);
            threeObjectsContainer.SetActive(true);

            StartCoroutine(InitThreeObjects());
        }

        if (reivewQueue.Count <= 0)
        {
            SceneLoader.LoadScene("menu");
            return;
        }

        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        currentFood = GameObject.Instantiate(reivewQueue[randomObject], transform);

        reivewQueue.RemoveAt(randomObject);

    }


    IEnumerator InitThreeObjects()
    {
        yield return new WaitForEndOfFrame();
        // get the name of the fruit from the cloned prefab
        currentName = currentFood.name.Replace("(Clone)", "");
        threeObjectsContainer.GetComponentInChildren<TextMeshProUGUI>().text = currentName;

        List<GameObject> temp = new List<GameObject>(objects);
        int currentFoodIndex = temp.FindIndex(obj => obj.name == currentName);
        temp.RemoveAt(currentFoodIndex);

        List<GameObject> objectsToPlace = new List<GameObject>(3);
        int randomIndex = Random.Range(0, temp.Count);
        objectsToPlace.Add(Instantiate(temp[randomIndex], transform));
        temp.RemoveAt(randomIndex);
        randomIndex = Random.Range(0, temp.Count);
        objectsToPlace.Add(Instantiate(temp[randomIndex], transform));
        objectsToPlace.Add(currentFood);

        List<Vector3> positions = new List<Vector3>() { new Vector3(transform.position.x + 5f, transform.position.y, transform.position.z), new Vector3(transform.position.x - 5f, transform.position.y, transform.position.z), transform.position };
        objectsToPlace.ForEach(obj =>
        {
            randomIndex = Random.Range(0, positions.Count);
            obj.transform.position = positions[randomIndex];
            positions.RemoveAt(randomIndex);

        });

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
