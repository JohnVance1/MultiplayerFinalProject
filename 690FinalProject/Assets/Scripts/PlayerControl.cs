using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControl : NetworkBehaviour
{
    [SyncVar]
    public Vector3 Control; //This is a sync var, mirror automatically shares and syncs this variable across all of the scripts on objects with the same network identity, but it can only be set by the server.

    public Color c;//color to change to if we are controlling this one

    void Update()
    {
        if (GetComponent<NetworkIdentity>().hasAuthority)//make sure this is an object that we ae controlling
        {
            GetComponent<Renderer>().material.color = c;//change color
            Control = new Vector3(Input.GetAxis("Horizontal") * .2f, Input.GetAxis("Vertical") * .2f, 0);//update our control variable
            GetComponent<PlayerPhysics>().ApplyForce(Control, ForceMode2D.Force);//Use our custom force function
            if (Input.GetAxis("Cancel") == 1)//if we press the esc button
            {
                GetComponent<PlayerPhysics>().CmdResetPose();//reset our position
            }
        }
    }
}
