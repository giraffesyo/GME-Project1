using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class setfruitname : MonoBehaviour
{
    public string fruitnameSpanish;
    public string fruitnameEnglish;
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
        gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = fruitnameSpanish;
        gameObject.transform.GetChild(1).GetComponent<TextMeshPro>().text = fruitnameEnglish;

    }

    public void removeName()
    {
        gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
        gameObject.transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
    }
}
