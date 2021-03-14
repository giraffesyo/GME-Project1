using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyparthit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "bosspiece")
        {
            Debug.Log(other.gameObject.name);
            GameObject parent = transform.parent.gameObject;
            parent.GetComponent<bodyparttakedamage>().TakeDamage();
            Destroy(other.transform.gameObject);
        }
        
    }
}
