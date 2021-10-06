using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player_Single : MonoBehaviour
{
    Rigidbody2D _playerRB;
    private float _speedHorizontal = 200;
    private float _speedVertical = 20;

    private float horizontalInput;

    [SerializeField]
    private LayerMask layers;

    [SerializeField]
    private GameObject currentWeapon;
    private WeaponScript currentWeaponScript;

    [SerializeField]
    private GameObject target;
    private Target_Single targetScript;

    [SerializeField]
    private GameObject spawn;
    private Vector3 spawnPoint;

    private int dir;
    private bool shooting;

    public bool grabbing;

    void Start()
    {
        dir = 1;
        grabbing = false;
        _playerRB = GetComponent<Rigidbody2D>();
        currentWeaponScript = currentWeapon.GetComponent<WeaponScript>();
        if (target != null)
        {
            targetScript = target.GetComponent<Target_Single>();
        }

        if (spawn != null)
        {
            spawnPoint = spawn.transform.position;
        }
    }

    void Update()
    {
        Move();

        if (Input.GetMouseButton(0) && (targetScript.moveToPlayer == false) && grabbing == false)
        {
            currentWeaponScript.DistanceGrab(dir);
            shooting = true;
            currentWeaponScript.retract = false;
        }

        if (Input.GetMouseButtonUp(0) && grabbing == true)
        {
            targetScript.letGo = true;
            shooting = false;
            grabbing = false;

        }

        if (Input.GetMouseButtonUp(0) && grabbing == false)
        {
            targetScript.letGo = true;
            targetScript.moveToPlayer = false;
            shooting = false;
            currentWeaponScript.retract = true;
        }

        if ((currentWeaponScript.scale.x > 0.01 && shooting == false) || currentWeaponScript.retract == true)
        {
            currentWeaponScript.DistanceRetract(dir);
        }
        else
        {
            currentWeaponScript.retract = false;
            shooting = false;
            grabbing = false;

        }
    }

    private void FixedUpdate()
    {
        _playerRB.velocity = new Vector2(horizontalInput * _speedHorizontal * Time.deltaTime, _playerRB.velocity.y);

        if (Input.GetKey(KeyCode.W) && IsGrounded())
        {
            _playerRB.velocity = Vector2.up * _speedVertical;

        }
       
    }

    public void Move()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        if(horizontalInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.x);
            dir = 1;
        }
        else if(horizontalInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.x);
            dir = -1;
        }

    }

    private bool IsGrounded()
    {
        RaycastHit2D groundCheckLeft = Physics2D.Raycast(new Vector3(transform.position.x - transform.localScale.x / 2, transform.position.y, transform.position.z), Vector2.down, transform.localScale.y/2 + 0.05f, layers);
        RaycastHit2D groundCheckRight = Physics2D.Raycast(new Vector3(transform.position.x + transform.localScale.x / 2, transform.position.y, transform.position.z), Vector2.down, transform.localScale.y / 2 + 0.05f, layers);

        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - transform.localScale.y/2 - 0.01f), Color.red);

        if((groundCheckLeft.collider != null && groundCheckLeft.collider.CompareTag("Ground")) || 
            (groundCheckRight.collider != null && groundCheckRight.collider.CompareTag("Ground")))
        {
            return true;
        }
        return false;
    }

    public void Dead()
    {
        _playerRB.velocity = Vector2.zero;
        transform.position = spawnPoint;

    }

}
