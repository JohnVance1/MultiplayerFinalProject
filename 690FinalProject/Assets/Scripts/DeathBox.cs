using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<Player>() || other.gameObject.GetComponent<Player_Single>())
        {
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<Player>().Dead();
            }
            else
            {
                other.gameObject.GetComponent<Player_Single>().Dead();
            }
        }
    }

}
