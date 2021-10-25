using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
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
    private GameObject spawn;
    private Vector3 spawnPoint;

    private int dir;
    private bool shooting;

    public bool grabbing;

    // Target code
    public bool moveToPlayer;
    public bool letGo;

    [SerializeField]
    private GameObject playerMovePos;

    [SerializeField]
    private GameObject player;


    void Start()
    {
        dir = 1;
        _playerRB = GetComponent<Rigidbody2D>();
        currentWeaponScript = currentWeapon.GetComponent<WeaponScript>();
        letGo = false;
        playerMovePos = transform.GetChild(1).gameObject;

      

        if (spawn != null)
        {
            spawnPoint = spawn.transform.position;
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetMouseButton(0) && (moveToPlayer == false) && grabbing == false)
            {
                currentWeaponScript.DistanceGrab(dir);
                shooting = true;
                currentWeaponScript.retract = false;
            }

            if (Input.GetMouseButtonUp(0) && grabbing == true)
            {
                letGo = true;
                shooting = false;
                grabbing = false;

            }

            if (Input.GetMouseButtonUp(0) && grabbing == false)
            {
                letGo = true;
                moveToPlayer = false;
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

            if (moveToPlayer == true)
            {
                MoveTo(playerMovePos.transform.position);
                GetComponent<BoxCollider2D>().enabled = false;
                letGo = false;
            }
            else if (GetComponent<Player>().grabbing == true)
            {
                GetComponent<BoxCollider2D>().enabled = false;

            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = true;
                Move();
            }

            if (transform.position == playerMovePos.transform.position && moveToPlayer == true)
            {
                transform.parent = transform;

                moveToPlayer = false;
                GetComponent<Player>().grabbing = true;

            }

            if (letGo)
            {
                GetComponent<BoxCollider2D>().enabled = true;
                transform.parent = null;
                moveToPlayer = false;

            }
        }

    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            _playerRB.velocity = new Vector2(horizontalInput * _speedHorizontal * Time.deltaTime, _playerRB.velocity.y);

            if (Input.GetKey(KeyCode.W) && IsGrounded())
            {
                _playerRB.velocity = Vector2.up * _speedVertical;

            }
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

    public void MoveTo(Vector3 weaponPos)
    {
        float step = 10 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, weaponPos, step);

    }

}
