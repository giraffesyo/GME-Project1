using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class clickonfruit : MonoBehaviour
{

    Ray ray;
    RaycastHit hit;
    GameObject target;
    GameObject previousTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MouseDown");
            // Reset ray with new mouse position
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Fruit")
                {
                    if(previousTarget != null)
                    {
                        previousTarget.GetComponent<Rigidbody>().useGravity = true;
                        previousTarget.GetComponent<Rigidbody>().mass = 1;
                        previousTarget.GetComponent<setfruitname>().removeName();
                        UnFreezeAllConstraints();
                    }
                    target = hit.collider.gameObject;
                    target.transform.DOMove(new Vector3(0f, 1f, 2.5f), 1).OnComplete(FreezeAllConstraints);
                    target.transform.DORotate(new Vector3(0, 0, 0), 1);
                    target.GetComponent<Rigidbody>().useGravity = false;
                    target.GetComponent<Rigidbody>().mass = 1000;
                    target.GetComponent<setfruitname>().setName();
                    previousTarget = target;
                    target = null;
                    Debug.Log("Hit");
                }

            }

        }
    }

    public void FreezeAllConstraints()
    {
        previousTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }
    public void UnFreezeAllConstraints()
    {
        previousTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}
