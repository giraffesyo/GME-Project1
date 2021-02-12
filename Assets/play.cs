using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class play : MonoBehaviour
{

    public List<GameObject> foods;
    public int index = 0;
    private int maxIndex;
    private GameObject currentFood;
    // Start is called before the first frame update
    void Start()
    {
        currentFood = GameObject.Instantiate(foods[0], transform);
        maxIndex = foods.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            index--;
            if(index < 0)
            {
                index = maxIndex - 1;
            }

            Destroy(currentFood);
            currentFood = GameObject.Instantiate(foods[index], transform);

        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            index++;
            if (index >= maxIndex)
            {
                index = 0;
            }

            Destroy(currentFood);
            currentFood = GameObject.Instantiate(foods[index], transform);
        }
    }
}
