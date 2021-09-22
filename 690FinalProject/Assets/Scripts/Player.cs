using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D _playerRB;
    private float _speed = 5;
    private float _verticalVelocity;
    private float _terminalVelocity = 10;

    private const float Gravity = -15;


    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.transform.position += Vector3.up * _speed * Time.deltaTime;
            //_playerRB.AddForce(Vector2.up * _speed);

        }

        if(_playerRB.velocity.y != 0)
        {
            Debug.Log("Hello");
        }

        _verticalVelocity = _playerRB.velocity.y;

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position += Vector3.left * _speed * Time.deltaTime;
            //_playerRB.AddForce(Vector2.left * _speed);

        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += Vector3.right * _speed * Time.deltaTime;
            //_playerRB.AddForce(Vector2.right * _speed);

        }

        if(_verticalVelocity > _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
            this.transform.position += new Vector3(0.0f, _verticalVelocity, 0.0f) * _speed * Time.deltaTime;
            

        }



    }
}
