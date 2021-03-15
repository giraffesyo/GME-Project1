using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class shootfruit : MonoBehaviour
{
    Ray projectilePath;
    public Queue<GameObject> projectile = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(projectile != null)
            {
                if(projectile.Count > 0)
                {
                    projectilePath = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(projectilePath, out hit))
                    {
                        GameObject firedProjectile = Instantiate(projectile.Dequeue(), projectilePath.origin, Quaternion.identity);
                        firedProjectile.SetActive(true);
                        firedProjectile.layer = 0;
                        firedProjectile.tag = "Untagged";
                        firedProjectile.transform.DOMove(hit.transform.position, 2);
                        Destroy(firedProjectile, 2.2f);
                    }
                }
            }  
        }
    }

    public void addProjectile(GameObject proj)
    {
        GameObject temp = Instantiate(proj);
        temp.SetActive(false);
        projectile.Enqueue(temp);
    }
}
