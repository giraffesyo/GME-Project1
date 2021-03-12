using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class setfruitname : MonoBehaviour
{
    public string fruitname;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setName()
    {
        gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = fruitname;
    }

    public void removeName()
    {
        gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
    }
}
