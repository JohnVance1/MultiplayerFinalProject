using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// The main player class
/// </summary>
public class Player : NetworkBehaviour
{
    public Rigidbody2D _playerRB;
    private float _speedHorizontal = 200;
    private float _speedVertical = 20;

    private float horizontalInput;

    private Vector3 scale;
    [SerializeField]
    private Transform sprite;
    
    [SerializeField]
    private LayerMask layers;

    [SerializeField]
    private Transform wepPos;

    [SerializeField]
    private GameObject currentWeapon;
    [SerializeField]
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

    /// <summary>
    /// When the Server starts
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        gameObject.name = playerName;

    }

    /// <summary>
    /// When this Client starts
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();

        gameObject.name = playerName;
        //addresses.Add(LocalIPAddress(), RandomCode());

    }

    void Start()
    {
        scale = sprite.localScale;
        dir = 1;
        _playerRB = GetComponent<Rigidbody2D>();
        //currentWeaponScript = currentWeapon.GetComponent<WeaponScript>();
        letGo = false;
        //playerMovePos = transform.GetChild(1).gameObject;
        moveToPlayer = false;
        //currentWeapon.GetComponent<BoxCollider2D>().enabled = false;
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

        //Debug.Log(LocalIPAddress());
        Flip();

        if (currentWeapon != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (grabbed == false)
                {
                    // If this Player is grabbing the other Player
                    if (grabbing)
                    {
                        // Turns off the colliders when grabbing
                        CmdNotShooting();
                    }
                    else
                    {
                        // Moves the weapon out to be able to grab
                        CmdShoot();
                    }
                }

            }

            if (Input.GetMouseButtonUp(0))
            {
                // Moves the weapon back
                CmdGrab();
            }

            if (notShooting)
            {
                // Moves the weapon back when the Player is grabbed
                CmdNotShooting();

            }

            // Checks to see if the Player is letting go of the other Player
            if (letGo && otherPlayer != null)
            {
                // Lets go of the other Player
                CmdLetGo(otherPlayer);
            }

            // Turns the Player correctly

        }

    }

    /// <summary>
    /// The Command call for when the Player lets go of the other Player
    /// </summary>
    /// <param name="other"></param>
    [Command]
    public void CmdLetGo(Player other)
    {
        RpcLetGo(other);
    }

    /// <summary>
    /// The RPC call for when the Player lets go of the other Player
    /// </summary>
    /// <param name="other"></param>
    [ClientRpc]
    public void RpcLetGo(Player other)
    {
        other.stayAttached = false;
        other.GetComponent<BoxCollider2D>().enabled = true;
        other._playerRB.simulated = true;
        other.moveToPlayer = false;
        other.grabbed = false;
    }

    /// <summary>
    /// The Command for retracting the weapon and turning off the weapon's collider
    /// Only called when the other Player isn't grabbed
    /// </summary>
    [Command]
    public void CmdGrab()
    {
        currentWeaponScript.DistanceRetract(dir);
        RpcRetract();
    }

    /// <summary>
    /// The RPC call for retracting the weapon and turning off the weapon's collider
    /// </summary>
    [ClientRpc]
    public void RpcRetract()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = false;
        currentWeaponScript.DistanceRetract(dir);
        letGo = true;
        grabbing = false; 
        notShooting = true;

    }

    /// <summary>
    /// The Command for retracting the weapon and turning off the weapon's collider
    /// Only ran when the the other Player is grabbed
    /// </summary>
    [Command]
    public void CmdNotShooting()
    {
        currentWeaponScript.DistanceRetract(dir);
        RpcNotShooting();
    }

    /// <summary>
    /// The RPC call for retracting the weapon and turning off the weapon's collider
    /// </summary>
    [ClientRpc]
    public void RpcNotShooting()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = false;
        currentWeaponScript.DistanceRetract(dir);

    }

    /// <summary>
    /// The Command call for turning back on the weapon's box collider and moving it forward
    /// This allows for the Player to use their weapon and grab the other Player
    /// </summary>
    [Command]
    public void CmdShoot()
    {
        currentWeaponScript.DistanceGrab(dir);

        RpcExtend();
    }

    /// <summary>
    /// The RPC call for turning back on the weapon's box collider and moving it forward
    /// </summary>
    [ClientRpc]
    public void RpcExtend()
    {
        currentWeapon.GetComponent<BoxCollider2D>().enabled = true;
        currentWeaponScript.DistanceGrab(dir);
        letGo = false;
        notShooting = false;
    }

    private void FixedUpdate()
    {
        // Checks to make sure this is the local player
        if (!isLocalPlayer)
        {
            return;
        }

        // If we can move the Player
        if (moveToPlayer)
        {
            if (!stayAttached)
            {
                // Move to the other Player's grab loc
                CmdMoveTo(otherMovePos.transform.position);
            }
            else
            {
                // Keep the grabbed Player on the Player grabbing
                transform.position = otherMovePos.transform.position;
            }
        }
        else
        {
            // Regular Player movement
            Move();
        }
    }    

    /// <summary>
    /// Allows for the Players to move back and forth
    /// </summary>
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

    /// <summary>
    /// Regular Player movement
    /// </summary>
    [Client]
    public void Move()
    {
        _playerRB.velocity = new Vector2(horizontalInput * _speedHorizontal * Time.deltaTime, _playerRB.velocity.y);

        if (Input.GetKey(KeyCode.W) && IsGrounded())
        {
            _playerRB.velocity = Vector2.up * _speedVertical;

        }
        
    }

    /// <summary>
    /// Checks to see if the Player is grounded or not
    /// </summary>
    /// <returns></returns>
    [Client]
    private bool IsGrounded()
    {
        RaycastHit2D groundCheckLeft = Physics2D.Raycast(new Vector3(transform.position.x - sprite.localScale.x / 2, transform.position.y, transform.position.z), 
            Vector2.down, sprite.localScale.y/2 + 0.05f, layers);
        RaycastHit2D groundCheckRight = Physics2D.Raycast(new Vector3(transform.position.x + sprite.localScale.x / 2, transform.position.y, transform.position.z), 
            Vector2.down, sprite.localScale.y / 2 + 0.05f, layers);

        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - sprite.localScale.y/2 - 0.01f), Color.red);

        if((groundCheckLeft.collider != null && groundCheckLeft.collider.CompareTag("Ground")) || 
            (groundCheckRight.collider != null && groundCheckRight.collider.CompareTag("Ground")))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks to see if the Player is dead and respawns them
    /// </summary>
    [Client]
    public void Dead()
    {
        _playerRB.velocity = Vector2.zero;
        transform.position = spawnPoint;

    }

    /// <summary>
    /// The Command call to move the Player to the other Player's grab pos
    /// </summary>
    /// <param name="weaponPos"></param>
    [Command]
    public void CmdMoveTo(Vector3 weaponPos)
    {
        RpcMoveTo(weaponPos);
    }

    /// <summary>
    /// The RPC call to move the Player to the other Player's grab pos
    /// </summary>
    /// <param name="weaponPos"></param>
    [ClientRpc]
    public void RpcMoveTo(Vector3 weaponPos)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        _playerRB.simulated = false;

        float step = 0.5f;
        transform.position = Vector3.MoveTowards(transform.position, weaponPos, step);

        if (Vector3.Distance(otherMovePos.transform.position, transform.position) <= 0.1f)
        {
            stayAttached = true;
        }

    }

    /// <summary>
    /// Checking collisions between the Player and the weapon
    /// </summary>
    /// <param name="col"></param>
    [Client] 
    void OnTriggerEnter2D(Collider2D col)
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if (currentWeapon == null && col.transform.GetComponent<WeaponScript>() && !col.transform.root.GetComponent<Player>())
        {
            CmdPickupItem(col.GetComponent<NetworkIdentity>(), this.GetComponent<NetworkIdentity>());
        }
        else if (col.transform.GetComponent<WeaponScript>() && col.tag != "Weapon")
        {
            otherPlayer = col.transform.parent.gameObject.GetComponent<Player>();
            otherMovePos = otherPlayer.playerMovePos;

            CmdGrabbing(otherPlayer);
            //RpcGrabbing(otherPlayer);
            CmdOffColliders();
            //RpcOffColliders();

        }
        
    }

    [Client]
    public static string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    //[Server]
    //public string RandomCode()
    //{
    //    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //    string code = "";
    //    for (int i = 0; i < 4; i++)
    //    {
    //        code += letters.Substring(Random.Range(0, 25), 1);
    //    }
    //    return code;

    //}

    [Client]
    public void OnGUI()
    {
        //addresses.TryGetValue(LocalIPAddress(), out string val);
        GUI.Label(new Rect(10, 10, 150, 40), "Local IP:" + LocalIPAddress()); 

    }

    [Command]
    void CmdPickupItem(NetworkIdentity item, NetworkIdentity playerID)
    {
        item.AssignClientAuthority(playerID.connectionToClient);
        //currentWeapon = item.gameObject;
        //currentWeapon.transform.parent = gameObject.transform;
        currentWeapon = item.gameObject;
        currentWeapon.transform.parent = gameObject.transform;
        currentWeapon.transform.localPosition = wepPos.localPosition;
        currentWeaponScript = currentWeapon.GetComponent<WeaponScript>();
        currentWeaponScript.startPos = currentWeapon.transform.localPosition;
        RpcPickupItem(item, playerID);
        item.RemoveClientAuthority();
    }

    [ClientRpc]
    void RpcPickupItem(NetworkIdentity item, NetworkIdentity playerID)
    {
        //item.AssignClientAuthority(playerID.connectionToClient);
        currentWeapon = item.gameObject;
        currentWeapon.transform.parent = gameObject.transform;
        currentWeapon.transform.localPosition = wepPos.localPosition;
        currentWeaponScript = currentWeapon.GetComponent<WeaponScript>();
        currentWeaponScript.startPos = currentWeapon.transform.localPosition;
    }

    /// <summary>
    /// The Command call for turning off the colliders on the Player being grabbed
    /// </summary>
    [Command]
    public void CmdOffColliders()
    {
        RpcOffColliders();

    }

    /// <summary>
    /// The RPC call for turning off the colliders on the Player being grabbed
    /// </summary>
    [ClientRpc]
    void RpcOffColliders()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        _playerRB.simulated = false;
    }

    /// <summary>
    /// The Command call for actually grabbing the other Player
    /// </summary>
    /// <param name="other"></param>
    [Command]
    public void CmdGrabbing(Player other)
    {
        RpcGrabbing(other);

    }

    /// <summary>
    /// The RPC call for actually grabbing the other Player
    /// </summary>
    /// <param name="other"></param>
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
