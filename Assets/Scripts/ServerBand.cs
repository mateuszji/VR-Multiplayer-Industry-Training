using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class ServerBand : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text PlayerNameText, PlayerCountText, RoomNameText;
    [SerializeField] GameObject ServerBandForClient, ServerBandForMaster;
    void Start()
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ServerBandForClient.SetActive(false);
            ServerBandForMaster.SetActive(true);
        }

        PlayerNameText.text = PhotonNetwork.LocalPlayer.NickName;
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        updatePlayerCount();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updatePlayerCount();
    }

    private void updatePlayerCount()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 10)
            PlayerCountText.text = "0" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        else
            PlayerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
}
