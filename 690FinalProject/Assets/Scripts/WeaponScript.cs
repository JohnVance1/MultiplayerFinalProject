using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponScript : NetworkBehaviour
{
    public bool letGo;
    public float speed;

    public Vector3 startPos;

    public bool retract;

    void Start()
    {
        letGo = false;
        speed = 10;
        retract = false;
        startPos = transform.localPosition;

    }

    void Update()
    {
        

    }

    public void DistanceGrab(int dir)   
    {
        
        if (transform.localPosition.x <= 4)
        {
            transform.position += new Vector3((Time.deltaTime * speed * dir), 0, 0);
        }



        #region Raycast Code
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.left * dir, 3);

        //if((hit.collider != null && hit.collider.gameObject.GetComponent<Target>()) || (hit.collider != null && hit.collider.gameObject.GetComponent<Target_Single>()))
        //{
        //    if (hit.collider.gameObject.GetComponent<Target>())
        //    {
        //        hit.collider.GetComponent<Target>().moveToPlayer = true;
        //    }
        //    else
        //    {
        //        hit.collider.GetComponent<Target_Single>().moveToPlayer = true;

        //    }

        //}
        #endregion
    }

    public void DistanceRetract(int dir)
    {
        if (transform.localPosition.x > 0.7)
        {
            transform.position -= new Vector3((Time.deltaTime * speed * dir), 0, 0);
        }      


    }

    [Server]
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Player>())
        {
            Debug.Log("Pickuped Weapon");

        }

    }

}
