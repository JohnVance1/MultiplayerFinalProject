using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// The main player class
/// </summary>
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

    // Reference to the player's name
    [SyncVar]
    public string playerName;

    [SerializeField]
    private GameObject spawn;
    private Vector3 spawnPoint;

    private int dir;
    public bool notShooting;
    public bool grabbing;
    public bool grabbed;
    public bool stayAttached;

    // Target code
    public bool moveToPlayer;
    public bool letGo;

    [SerializeField]
    public GameObject playerMovePos;
    private GameObject otherMovePos;

    [SerializeField] [SyncVar]
    private Vector3 otherMoveVec;

    // The reference to the other player
    [SerializeField] [SyncVar]
    private Player otherPlayer;

    public override void OnStartServer()
    {
        base.OnStartServer();
        gameObject.name = playerName;        

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        gameObject.name = playerName;


    }

    //[Client]
    void Start()
    {
        dir = 1;
        _playerRB = GetComponent<Rigidbody2D>();
        currentWeaponScript = currentWeapon.GetComponent<WeaponScript>();
        letGo = false;
        playerMovePos = transform.GetChild(2).gameObject;
        moveToPlayer = false;
        currentWeapon.GetComponent<BoxCollider2D>().enabled = false;
        grabbed = false;
        stayAttached = false;

        if (spawn != null)
        {
            spawnPoint = spawn.transform.position;
        }
    }

    //[Client]
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if (grabbed == false)
            {
                if (grabbing)
                {
                    CmdNotShooting();
                }
                else
                {
                    CmdShoot();
                }
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            CmdGrab();
        }

        if(notShooting)
        {
            CmdNotShooting();

        }

        if (letGo && otherPlayer != null)
        {
            CmdLetGo(otherPlayer);
        }

        Flip();
       
    }

    [Command]
    public void CmdLetGo(Player other)
    {
        RpcLetGo(other);
    }

    [ClientRpc]
    public void RpcLetGo(Player other)
    {
        other.stayAttached = false;
        other.GetComponent<BoxCollider2D>().enabled = true;
        other._playerRB.simulated = true;
        other.moveToPlayer = false;
        other.grabbed = false;
    }

    [Command]
    public void CmdGrab()
    {       
        RpcRetract();
    }

    [ClientRpc]
    public void RpcRetract()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = false;
        currentWeaponScript.DistanceRetract(dir);
        letGo = true;
        grabbing = false; 
        notShooting = true;

    }

    [Command]
    public void CmdNotShooting()
    {
        RpcNotShooting();
    }

    [ClientRpc]
    public void RpcNotShooting()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = false;
        currentWeaponScript.DistanceRetract(dir);

    }

    [Command]
    public void CmdShoot()
    {
        RpcExtend();
    }

    [ClientRpc]
    public void RpcExtend()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = true;
        currentWeaponScript.DistanceGrab(dir);
        letGo = false;
        notShooting = false;
    }

    //[Client]
    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (moveToPlayer)
        {
            if (!stayAttached)
            {
                CmdMoveTo(otherMovePos.transform.position);
            }
            else
            {
                transform.position = otherMovePos.transform.position;
            }
        }
        else
        {
            Move();
        }
    }    

    [Client]
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

    [Client]
    public void Move()
    {
        _playerRB.velocity = new Vector2(horizontalInput * _speedHorizontal * Time.deltaTime, _playerRB.velocity.y);

        if (Input.GetKey(KeyCode.W) && IsGrounded())
        {
            _playerRB.velocity = Vector2.up * _speedVertical;

        }
        
    }

    [Client]
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

    [Client]
    public void Dead()
    {
        _playerRB.velocity = Vector2.zero;
        transform.position = spawnPoint;

    }

    [Command]
    public void CmdMoveTo(Vector3 weaponPos)
    {
        RpcMoveTo(weaponPos);
    }

    [ClientRpc]
    public void RpcMoveTo(Vector3 weaponPos)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        _playerRB.simulated = false;

        float step = 0.5f;
        Debug.Log(step);
        transform.position = Vector3.MoveTowards(transform.position, weaponPos, step);

        if (Vector3.Distance(otherMovePos.transform.position, transform.position) <= 0.1f)
        {
            stayAttached = true;
        }

    }

    [Client] 
    void OnTriggerEnter2D(Collider2D col)
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if (col.transform.GetComponent<WeaponScript>() && col.tag != "Weapon")
        {
            otherPlayer = col.transform.parent.gameObject.GetComponent<Player>();
            otherMovePos = otherPlayer.playerMovePos;

            CmdGrabbing(otherPlayer);
            CmdOffColliders();

        }
    }

    [Command]
    public void CmdOffColliders()
    {
        RpcOffColliders();

    }

    [ClientRpc]
    void RpcOffColliders()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        _playerRB.simulated = false;
    }

    [Command]
    public void CmdGrabbing(Player other)
    {
        RpcGrabbing(other);

    }


    [ClientRpc]
    public void RpcGrabbing(Player other)
    {
        otherPlayer = other;
        otherMovePos = other.playerMovePos;
        otherPlayer.otherPlayer = this;

        if (other.grabbing == false)
        {
            other.grabbing = true;
            this.grabbed = true;
            this.moveToPlayer = true;
            
        }

    }

    

}
