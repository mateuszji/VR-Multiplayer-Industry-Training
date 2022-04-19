using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ScrewRotate : MonoBehaviourPunCallbacks
{
    private Screw screw;
    private CircularDrive cd;
    public PhotonView photonViewScrew;

    private void Start()
    {
        cd = GetComponent<CircularDrive>();
        screw = GetComponentInParent<Screw>();
    }
    private void FixedUpdate()
    {
        if (cd == null || !cd.driving || screw.SnappedTool == null) return;

        if (screw.locked && cd.outAngle <= 0)
        {
            cd.outAngle = 0;
            cd.driving = false;
            photonViewScrew.RPC("RPC_UnScrew", RpcTarget.AllBuffered, null);
            photonViewScrew.RPC("RPC_DisableTool", RpcTarget.AllBuffered, photonView.ViewID);
        }
        
        if(!screw.locked && cd.outAngle >= 1080)
        {
            cd.outAngle = 1080;
            cd.driving = false;
            photonViewScrew.RPC("RPC_Screw", RpcTarget.AllBuffered, null);
            photonViewScrew.RPC("RPC_DisableTool", RpcTarget.AllBuffered, photonView.ViewID);
        }

        this.photonView.RPC("RPC_Update_Angle_And_Rotation", RpcTarget.AllBuffered, this.cd.outAngle);
    }

    [PunRPC]
    public void RPC_Update_Angle_And_Rotation(float angle)
    {
        this.transform.localRotation = Quaternion.identity * Quaternion.AngleAxis(angle, Vector3.right);
        this.GetComponent<CircularDrive>().outAngle = angle;
    }
}
