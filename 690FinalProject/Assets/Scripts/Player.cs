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
    public bool notShooting;
    public bool grabbing;
    public bool grabbed;


    // Target code
    public bool moveToPlayer;
    public bool letGo;

    [SerializeField]
    public GameObject playerMovePos;
    private GameObject otherMovePos;

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
            //notShooting = false;
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

        if (Input.GetMouseButtonUp(0))// && grabbing == true)
        {
            CmdGrab();
        }

        if(notShooting)
        {
            CmdNotShooting();

        }

        if (letGo && otherPlayer != null)
        {
            CmdLetGo(otherPlayer, this);
        }


        Flip();



        #region Rest of grab code
        

        if (grabbing)
        {
            //CmdGrab();
            Debug.Log("letGo: " + letGo);
            Debug.Log("grabbed");
            if (otherMovePos.transform.position == transform.position)
            { 
                CmdParent(otherPlayer, gameObject);
                
            }
        }

        

        #endregion
    }

    [Command]
    public void CmdLetGo(Player other, Player current)
    {
        other.GetComponent<BoxCollider2D>().enabled = true;
        other.transform.parent = null;
        other._playerRB.simulated = true;
        other.moveToPlayer = false;
        other.grabbed = false;
        RpcLetGo(other, current);
    }

    [ClientRpc]
    public void RpcLetGo(Player other, Player current)
    {
        other.GetComponent<BoxCollider2D>().enabled = true;
        other.transform.parent = null;
        other._playerRB.simulated = true;
        other.moveToPlayer = false;
        other.grabbed = false;
    }

    [Command]
    public void CmdParent(Player other, GameObject gO)
    {
        gO.transform.parent = other.transform;
        //gO.GetComponent<Player>().grabbing = false;
        RpcParent(other, gO);

    }

    [ClientRpc]
    public void RpcParent(Player other, GameObject gO)
    {
        gO.transform.parent = other.transform;
        //gO.GetComponent<Player>().grabbing = false;

        Debug.Log("Transform Set");

    }

    [ClientRpc]
    public void SetPlayer(Player other)
    {
        otherPlayer = other;
        otherMovePos = otherPlayer.playerMovePos;
        CmdSetPlayer(other);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayer(Player other)
    {
        otherPlayer = other;
        otherMovePos = otherPlayer.playerMovePos;
        
    }


    [Command]
    public void CmdGrab()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = false;
        //notShooting = true;

        RpcRetract();
    }

    [ClientRpc]
    public void RpcRetract()
    {
        currentWeaponScript.DistanceRetract(dir);
        letGo = true;
        grabbing = false; 
        notShooting = true;

    }

    [Command]
    public void CmdNotShooting()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = false;

        RpcNotShooting();
    }

    [ClientRpc]
    public void RpcNotShooting()
    {
        currentWeaponScript.DistanceRetract(dir);


    }

    [Command]
    public void CmdShoot()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = true;

        RpcExtend();
    }

    [ClientRpc]
    public void RpcExtend()
    {
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
            MoveTo(otherMovePos.transform.position);
            GetComponent<BoxCollider2D>().enabled = false;
            _playerRB.simulated = false;
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

    [Client]
    public void MoveTo(Vector3 weaponPos)
    {
        float step = 10 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, weaponPos, step);

    }

    [ServerCallback] 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.GetComponent<WeaponScript>() && col.tag != "Weapon")
        {
            //col.transform.parent.GetComponent<Player>().moveToPlayer = true;
            //col.GetComponent<WeaponScript>().retract = true;
            RpcGrabbing(otherPlayer, this);
            RpcOffColliders(this);

        }
    }

    [ClientRpc]
    public void RpcOffColliders(Player current)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        _playerRB.simulated = false;

    }


    [ClientRpc]
    public void RpcGrabbing(Player other, Player current)
    {
        if (other.grabbing == false)
        {
            other.grabbing = true;
            current.grabbed = true;
            current.moveToPlayer = true;
            
            Debug.Log("HIT");

        }

    }

    

}
