using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movecamera : MonoBehaviour
{
    Transform sphere;
    Vector3 starting;
    Vector3 difference;
    float angle = 180;
    public float spinSpeed = 10f;
    public float radius;
    void Start()
    {
        starting = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        sphere = GameObject.Find("cursedboss").transform;
        difference = new Vector3(starting.x - sphere.position.x, starting.y - sphere.position.y, starting.z - sphere.position.z);
        radius = difference.z;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.A))
        {
            angle += spinSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            angle -= spinSpeed * Time.deltaTime;
        }
        difference.x = radius * Mathf.Sin(angle);
        difference.z = radius * Mathf.Cos(angle);
        transform.position = new Vector3(sphere.position.x + difference.x, sphere.position.y + difference.y, sphere.position.z + difference.z);
        transform.LookAt(sphere);
    }
}
