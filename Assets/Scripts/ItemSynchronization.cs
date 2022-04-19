using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class ItemSynchronization : MonoBehaviourPunCallbacks
{
    private new Collider collider;
    private Throwable throwable;
    private new PhotonView photonView;
    private GameObject serverBand;
    private bool IsGrabbed = false;
    private Replaceable replaceable;

    [Tooltip("Czy ten obiekt mo¿e byæ modyfikowany tylko przez Master Client'a?")]
    [SerializeField] bool isOnlyForMaster = false;

    private UnityAction onPickUpAction;
    private UnityAction onDetachFromHandAction;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        collider = GetComponent<Collider>();
        throwable = GetComponent<Throwable>();
        serverBand = GameObject.Find("ServerBand");
        replaceable = GetComponent<Replaceable>();

        if (!collider)
            collider = this.GetComponentInChildren<Collider>();

        onPickUpAction = new UnityAction(onGrab);
        onDetachFromHandAction = new UnityAction(onDrop);
        // do sprawdzenia, czy bez dodawania eventu w Throwable bedzie to w necie sie synchronizowac
        throwable.onPickUp.AddListener(onPickUpAction);
        throwable.onDetachFromHand.AddListener(onDetachFromHandAction);
    }

    private void FixedUpdate()
    {
        bool isAttached = throwable.interactable.attachedToHand != null;
        if(isAttached)
            photonView.RPC("RPC_SetItemTransform", RpcTarget.AllBuffered, this.transform.position.x, this.transform.position.y, this.transform.position.z, this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);

        if ((!PhotonNetwork.IsMasterClient && isOnlyForMaster) || IsGrabbed)
                collider.enabled = false;
        else
        {
            if (replaceable)
            {
                if (replaceable.block)
                    collider.enabled = false;
                else
                    collider.enabled = true;
            }
            else
                collider.enabled = true;
        }
    }

    public void onGrab()
    {
        if (throwable.interactable.attachedToHand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand && serverBand != null)
            serverBand.SetActive(false);

        photonView.RPC("RPC_SetItemTransform", RpcTarget.AllBuffered, this.transform.position.x, this.transform.position.y, this.transform.position.z, this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
        photonView.RPC("RPC_SetIsGrabbed", RpcTarget.OthersBuffered, true);
    }

    public void onDrop()
    {
        if (serverBand != null)
            serverBand.SetActive(true);

        photonView.RPC("RPC_SetItemTransform", RpcTarget.AllBuffered, this.transform.position.x, this.transform.position.y, this.transform.position.z, this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
        photonView.RPC("RPC_SetIsGrabbed", RpcTarget.OthersBuffered, false);
    }

    [PunRPC]
    public void RPC_SetItemTransform(float pX, float pY, float pZ, float rX, float rY, float rZ)
    {
        this.transform.position = new Vector3(pX, pY, pZ);
        this.transform.rotation = Quaternion.Euler(rX, rY, rZ);
    }
    [PunRPC]
    public void RPC_SetIsGrabbed(bool isGrabbed)
    {
        this.IsGrabbed = isGrabbed;
    }

}
