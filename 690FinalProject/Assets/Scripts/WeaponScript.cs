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
        //startPos = transform.localPosition;

    }

    void Update()
    {
        

    }

    public void DistanceGrab(int dir)   
    {
        Debug.Log(dir);

        if (transform.localPosition.x <= startPos.x + 4f)
        {
            transform.localPosition += new Vector3((Time.deltaTime * speed), 0, 0);
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
        Debug.Log(dir);
        if (transform.localPosition.x > startPos.x)
        {
            transform.localPosition -= new Vector3((Time.deltaTime * speed), 0, 0);
        }      
    }

    [ClientRpc]
    void RpcPickupItem(NetworkIdentity item)
    {
        item.AssignClientAuthority(connectionToClient);
    }

    [Client]
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Player>())
        {
            Debug.Log("Pickuped Weapon");
            //RpcPickupItem(GetComponent<NetworkIdentity>());

        }

    }

}
