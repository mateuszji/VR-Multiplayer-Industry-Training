using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestItem : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonView photonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void RPC_StrikeText()
    {
        TMP_Text text = this.GetComponent<TMP_Text>();
        text.fontStyle = FontStyles.Strikethrough;
    }
}
