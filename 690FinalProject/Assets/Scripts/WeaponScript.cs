using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public bool letGo;

    void Start()
    {
        letGo = false;
    }

    void Update()
    {
        
    }


    public void DistanceGrab()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.left, 1000);

        if(hit.collider != null && hit.collider.gameObject.GetComponent<Target>())
        {
            hit.collider.GetComponent<Target>().moveToPlayer = true;

        }

    }

}
