using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Single : MonoBehaviour
{
    public bool moveToPlayer;
    public bool letGo;

    [SerializeField]
    private GameObject playerMovePos;

    [SerializeField]
    private GameObject player;


    void Start()
    {
        letGo = false;
    }

    void Update()
    {
        if(moveToPlayer)
        {
            MoveTo(playerMovePos.transform.position);
            GetComponent<BoxCollider2D>().enabled = false;
            letGo = false;
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = true;

        }

        if (transform.position == playerMovePos.transform.position)
        {
            transform.parent = player.transform;

            moveToPlayer = false;

        }

        if (letGo)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            transform.parent = null;
        }


    }

    public void MoveTo(Vector3 weaponPos)
    {
        float step = 10 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, weaponPos, step);

    }


}
