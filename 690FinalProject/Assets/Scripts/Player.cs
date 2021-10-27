using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public Rigidbody2D _playerRB;
    private float _speedHorizontal = 200;
    private float _speedVertical = 20;

    private float horizontalInput;

    [SerializeField]
    private LayerMask layers;

    [SerializeField]
    private GameObject currentWeapon;
    private WeaponScript currentWeaponScript;

    [SyncVar]
    public string playerName;

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

    private Player otherPlayer;

    public override void OnStartServer()
    {
        base.OnStartServer();

        //_playerRB.simulated = true;

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        gameObject.name = playerName;

        NetworkIdentity ni = NetworkClient.connection.identity;
        otherPlayer = ni.GetComponent<Player>();
        Debug.Log("Spawned Player: " + otherPlayer);

    }

    void Start()
    {
        dir = 1;
        _playerRB = GetComponent<Rigidbody2D>();
        currentWeaponScript = currentWeapon.GetComponent<WeaponScript>();
        letGo = false;
        playerMovePos = transform.GetChild(2).gameObject;
        moveToPlayer = false;

        

        if (spawn != null)
        {
            spawnPoint = spawn.transform.position;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetMouseButton(0))//&& (moveToPlayer == false) && grabbing == false)
        {
            currentWeaponScript.DistanceGrab(dir);
            shooting = true;
            currentWeaponScript.retract = false;
        }

        if (Input.GetMouseButtonUp(0))// && grabbing == true)
        {
            Grab();

            //grabbing = false;

        }

        if (letGo && currentWeaponScript.transform.localPosition.x > 0.73)
        {
            currentWeaponScript.DistanceRetract(dir);

        }

        Flip();


        #region Rest of grab code
        //if (Input.GetMouseButtonUp(0) && grabbing == false)
        //{
        //    letGo = true;
        //    moveToPlayer = false;
        //    shooting = false;
        //    currentWeaponScript.retract = true;
        //}

        //if ((currentWeaponScript.transform.localPosition.x > 0.7 && shooting == false) || currentWeaponScript.retract == true)
        //{
        //    currentWeaponScript.DistanceRetract(dir);
        //}
        //else
        //{
        //    currentWeaponScript.retract = false;
        //    shooting = false;
        //    grabbing = false;

        //}

        
        //else if (grabbing == true)
        //{
        //    GetComponent<BoxCollider2D>().enabled = false;

        //}
        //else
        //{
        //    GetComponent<BoxCollider2D>().enabled = true;
        //    Move();
        //}

        //if (transform.position == playerMovePos.transform.position && moveToPlayer == true)
        //{
        //    transform.parent = transform;

        //    moveToPlayer = false;
        //    grabbing = true;

        //}

        //if (letGo)
        //{
        //    GetComponent<BoxCollider2D>().enabled = true;
        //    transform.parent = null;
        //    moveToPlayer = false;

        //}

        #endregion
    }

    [Command]
    public void Grab()
    {
        currentWeaponScript.DistanceRetract(dir);
        letGo = true;
        shooting = false;
        Retract();
    }

    [ClientRpc]
    public void Retract()
    {
        if (grabbing)
        {
            //NetworkIdentity ni = NetworkClient.connection.identity;
            //Player other = ni.GetComponent<Player>();
            moveToPlayer = true;
            if (otherPlayer != null)
            {
                Debug.Log("Other Player: " + otherPlayer.moveToPlayer + " || Current Player: " + moveToPlayer);
                Debug.Log("Other Player: " + otherPlayer.gameObject + " || Current Player: " + gameObject);
            }
        }

        if (moveToPlayer == true)
        {
            //MoveTo(playerMovePos.transform.position);
            //GetComponent<BoxCollider2D>().enabled = false;
            //letGo = false;
            Debug.Log("Moving");
        }
    }

    
    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Move();

    }

    public void Flip()
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

    public void Move()
    {
        _playerRB.velocity = new Vector2(horizontalInput * _speedHorizontal * Time.deltaTime, _playerRB.velocity.y);

        if (Input.GetKey(KeyCode.W) && IsGrounded())
        {
            _playerRB.velocity = Vector2.up * _speedVertical;

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

    [ServerCallback] 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.GetComponent<WeaponScript>())
        {
            //col.transform.parent.GetComponent<Player>().moveToPlayer = true;
            //col.GetComponent<WeaponScript>().retract = true;
            Grabbing();
        }
    }

   

    [ClientRpc]
    public void Grabbing()
    {        
        grabbing = true;
        
        Debug.Log("HIT");
    }

    

}
