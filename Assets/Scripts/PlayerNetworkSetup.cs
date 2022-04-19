using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject PhotonPlayer;
    public TMP_Text PlayerNameText;
    // Start is called before the first frame update
    void Start()
    {
        PlayerNameText.text = photonView.Owner.NickName;
        if (photonView.IsMine)
        {
            PhotonPlayer.SetActive(false);
        }
        else
        {
            PhotonPlayer.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
