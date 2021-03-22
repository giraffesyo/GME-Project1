using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetfruit : MonoBehaviour
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
        if(other.gameObject.tag == "Fruit")
        {
            Debug.Log("what");
            other.gameObject.transform.position = new Vector3(0, 3, 2.5f);
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
}
