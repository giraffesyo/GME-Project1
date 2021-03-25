using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LearnManager : MonoBehaviour
{

    private List<GameObject> objects;
    public List<string> objectNames;


    public GameObject Instructions;
    public int index = 0;
    private string currentName;
    public int maxIndex;
    private GameObject currentFood;


    // TODO: If we setup player preferences we can load this from there
    private bool shouldShowSwipeInstructions = false;

    void Start()
    {
        SwipeDetector.OnSwipe += SwipeHandler;
        // ReviewButtons.SetActive(false);
        objects = RemoteAssetLoader.Instance.GetAssets(AssetLabels.LearningObjects);
        currentFood = GameObject.Instantiate(objects[0], transform);
        currentFood.GetComponentInChildren<TextMeshPro>().text = currentFood.name.Replace("(Clone)", "");

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
        }
        else
        {
            index++;
            if (index >= maxIndex)
            {
                index = 0;
            }
        }
        Destroy(currentFood);
        currentFood = GameObject.Instantiate(objects[index], transform);
        currentFood.GetComponentInChildren<TextMeshPro>().text = currentFood.name.Replace("(Clone)", "");
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            AdvanceLearnItems(backwards: true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AdvanceLearnItems();
        }
    }

}
