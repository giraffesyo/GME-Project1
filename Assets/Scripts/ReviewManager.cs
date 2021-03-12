using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{

    public GameObject ReviewButtons;
    private string correctName;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Instantiate(ReviewButtons, transform);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(GameObject.Find("Name"));
    }

    void UpdateButtonNames()
    {

    }
}
