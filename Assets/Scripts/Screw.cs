using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR.InteractionSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Screw : MonoBehaviourPunCallbacks
{
    public GameObject widmo;
    public GameObject tool;

    public Material lockedMaterial;
    public Material unlockedMaterial;
    public GameObject SnappedTool { get; set; }

    private int screwSize;
    private new PhotonView photonView;
    private Replaceable replaceable;

    public bool locked;
    private void Start()
    {
        replaceable = GetComponentInParent<Replaceable>();
        photonView = GetComponent<PhotonView>();
        int.TryParse(gameObject.name.Split('_').Last(), out screwSize);

        if(!locked)
        {
            // trzeba sprawdzic czy nie wrzucic tego w RPC jak ktos odkreci i potem kolejny dolaczy
            this.GetComponentInChildren<Renderer>().material = this.unlockedMaterial;
            this.gameObject.transform.position = this.gameObject.transform.position + new Vector3(0, 0.0975f, 0);
        }

        replaceable.OnScrewChanged();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (SnappedTool) return;

        int.TryParse(other.name.Split('_').Last(), out int toolSize);
        if (other.tag.Equals(tag) && screwSize.Equals(toolSize))
        {
            Throwable toolThrowable = other.GetComponent<Throwable>();

            if (toolThrowable == null) return;

            bool isAttached = toolThrowable.interactable.attachedToHand != null;
            if(isAttached)
                widmo.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        widmo.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        int.TryParse(other.name.Split('_').Last(), out int toolSize);
        if (widmo.activeSelf && other.tag.Equals(tag) && screwSize.Equals(toolSize))
        {
            Throwable toolThrowable = other.GetComponent<Throwable>();

            if(toolThrowable == null) return;

            bool isAttached = toolThrowable.interactable.attachedToHand != null;

            if (!isAttached)
            {
                PhotonView snappedToolPV = other.GetComponent<PhotonView>();
                photonView.RPC("RPC_EnableTool", RpcTarget.AllBuffered, snappedToolPV.ViewID);
            }
        }
    }

    [PunRPC]
    public void RPC_UnScrew()
    {
        this.gameObject.transform.position = this.gameObject.transform.position + new Vector3(0, 0.0975f, 0);
        this.GetComponentInChildren<Renderer>().material = this.unlockedMaterial;
        this.locked = false;
        replaceable.OnScrewChanged();
    }

    [PunRPC]
    public void RPC_Screw()
    {
        this.gameObject.transform.position = this.gameObject.transform.position + new Vector3(0, -0.0975f, 0);
        this.GetComponentInChildren<Renderer>().material = this.lockedMaterial;
        this.locked = true;
        replaceable.OnScrewChanged();
    }

    [PunRPC]
    public void RPC_EnableTool(int pvID)
    {
        SnappedTool = PhotonView.Find(pvID).gameObject;
        SnappedTool.SetActive(false);
        widmo.SetActive(false);
        tool.SetActive(true);
    }

    [PunRPC]
    public void RPC_DisableTool(int pvID)
    {
        widmo.SetActive(false);
        SnappedTool.SetActive(true);
        SnappedTool.transform.position = PhotonView.Find(pvID).gameObject.transform.position;
        SnappedTool.transform.rotation = PhotonView.Find(pvID).gameObject.transform.rotation;
        PhotonView.Find(pvID).gameObject.SetActive(false);
        SnappedTool = null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Screw))]
public class ScrewEditor : Editor
{
    private Screw screw;
    private PhotonView photonView;

    private void OnEnable()
    {
        screw = (Screw)target;
        photonView = screw.GetComponent<PhotonView>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Lock/Unlock screw"))
        {
            if(screw.locked)
                photonView.RPC("RPC_UnScrew", RpcTarget.AllBuffered, null);
            else
                photonView.RPC("RPC_Screw", RpcTarget.AllBuffered, null);
        }
    }
}
#endif