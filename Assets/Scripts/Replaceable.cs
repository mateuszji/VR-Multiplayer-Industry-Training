using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Replaceable : MonoBehaviourPunCallbacks
{
    [SerializeField] public Screw[] screws;
    [SerializeField] private CorrectPos correctPos;
    [HideInInspector] public bool block;
    private Throwable objThrowable;

    private UnityAction onPickUpAction;

    private void Start()
    {
        objThrowable = GetComponent<Throwable>();
        foreach (Screw screw in screws)
            if (!screw.locked)
                screw.gameObject.SetActive(false);

        onPickUpAction = new UnityAction(onGrab);
        // do sprawdzenia, czy bez dodawania eventu w Throwable bedzie to w necie sie synchronizowac
        objThrowable.onPickUp.AddListener(onPickUpAction);
    }

    private void onGrab()
    {
        foreach (Screw screw in screws)
        {
            if (screw.gameObject.activeSelf)
                screw.gameObject.SetActive(false);
        }
        correctPos.gameObject.GetComponent<Collider>().enabled = true;
        if (correctPos.replaceable == this)
            correctPos.replaceable = null;
    }
    public void OnScrewChanged()
    {
        block = false;
        foreach (Screw screw in screws)
        {
            if (screw.locked)
            {
                block = true;
                break;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!correctPos) return;

        if (other.gameObject == correctPos.gameObject && correctPos.replaceable == null)
        {
            other.gameObject.transform.localScale = transform.localScale;
            other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!correctPos) return;

        if (other.gameObject == correctPos.gameObject)
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!correctPos) return;

        if(other.gameObject == correctPos.gameObject && correctPos.replaceable == null)
        {
            Throwable objThrowable = GetComponent<Throwable>();

            if (objThrowable == null) return;

            bool isAttached = objThrowable.interactable.attachedToHand != null;

            if (!isAttached)
            {
                correctPos.replaceable = this;
                transform.SetPositionAndRotation(correctPos.transform.position, correctPos.transform.rotation);
                correctPos.GetComponent<MeshRenderer>().enabled = false;
                correctPos.GetComponent<Collider>().enabled = false;
                foreach (Screw screw in screws)
                {
                    if (!screw.gameObject.activeSelf)
                        screw.gameObject.SetActive(true);
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Replaceable))]
public class ReplaceableEditor : Editor
{
    private Replaceable replaceable;
    private string action;

    private void OnEnable()
    {
        replaceable = (Replaceable)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Lock/Unlock ALL screws"))
        {
            if (replaceable.screws.Length == 0) return;

            if (replaceable.block)
                action = "RPC_UnScrew";
            else
                action = "RPC_Screw";

            foreach (Screw screw in replaceable.screws)
            {
                screw.GetComponent<PhotonView>().RPC(action, RpcTarget.AllBuffered, null);
            }
        }
    }
}
#endif