using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bodyparttakedamage : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider HPBar;
    bool exploded = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (HPBar.value <= 0 && exploded == false)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name != "Canvas")
                {
                    transform.GetChild(i).GetComponent<Rigidbody>().velocity = (Random.onUnitSphere) * Random.Range(4, 10);
                    transform.GetChild(i).GetComponent<Rigidbody>().useGravity = true;
                    Destroy(transform.GetChild(i).gameObject, 3);
                }
                else
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
            GameObject.Find("cursedboss").GetComponent<cursedbossfight>().bodypartAlive.Remove(gameObject.name);
            Destroy(gameObject, 3);

            exploded = true;
        }
    }

    public void TakeDamage()
    {
        HPBar.value--;
    }
}
