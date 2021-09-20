using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D _playerRB;
    private float _speed = 10;




    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            //_playerRB.transform.position += Vector3.up;
            _playerRB.AddForce(Vector2.up * _speed);

        }

        if (Input.GetKey(KeyCode.A))
        {
            //_playerRB.transform.position += Vector3.left;
            _playerRB.AddForce(Vector2.left * _speed);

        }

        if (Input.GetKey(KeyCode.D))
        {
            //_playerRB.transform.position += Vector3.right;
            _playerRB.AddForce(Vector2.right * _speed);

        }


    }
}
