using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerhp : MonoBehaviour
{

    public Slider HPBAR;
    public GameObject dead;
    bool hasDied = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(HPBAR.value <= 0 && hasDied == false)
        {
            GameObject died = Instantiate(dead);
            hasDied = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "bosspiece")
        {
            HPBAR.value--;
            Destroy(other.gameObject);
        }
    }
}
