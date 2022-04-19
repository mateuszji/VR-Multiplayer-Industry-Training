using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Replaceable : MonoBehaviourPunCallbacks
{
    [SerializeField] public Screw[] screws;
    [HideInInspector] public bool block;

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