using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public bool letGo;
    public float speed;

    [SerializeField]
    private GameObject shootLine;
    public Vector3 scale;

    public bool retract;

    void Start()
    {
        letGo = false;
        scale = shootLine.transform.localScale;
        speed = 10;
        retract = false;
    }

    void Update()
    {
        scale = shootLine.transform.localScale;
        if(shootLine.transform.localScale.x < 0.01)
        {
            shootLine.transform.localScale = new Vector3(0.01f, 0.08f, 1);
            
            shootLine.SetActive(false);


        }

    }


    public void DistanceGrab(int dir)   
    {
        shootLine.SetActive(true);

        if (shootLine.transform.localScale.x <= 4)
        {
            shootLine.transform.localScale += new Vector3((Time.deltaTime * speed), 0, 0);
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
        if (shootLine.transform.localScale.x > 0.01)
        {
            shootLine.transform.localScale -= new Vector3((Time.deltaTime * speed), 0, 0);
        }

        else
        {
            shootLine.transform.localScale = new Vector3(0.01f, scale.y, scale.z);

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Target_Single>())
        {
            collision.gameObject.GetComponent<Target_Single>().moveToPlayer = true;
            retract = true;
        }


    }


}
