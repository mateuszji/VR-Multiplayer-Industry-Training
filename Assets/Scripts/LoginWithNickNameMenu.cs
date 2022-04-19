using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class LoginWithNickNameMenu : MonoBehaviourPunCallbacks
{
    public GameObject LoginMenuElement;
    public GameObject LoginWithNickNameElement;
    public GameObject RoomMenuElement;
    public TMP_InputField NickNameInput;
    public void BackButton()
    {
        LoginMenuElement.SetActive(true);
        LoginWithNickNameElement.SetActive(false);
    }
    public void ConnectToPhotonWithNickName()
    {
        if (NickNameInput.text.Length > 3)
        {
            PhotonNetwork.NickName = NickNameInput.text;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Nickname set.");
        }
        else
        {
            Debug.Log("No nickname.");
        }
    }

    public override void OnConnected()
    {
        Debug.Log("Conntected to server.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server with player name: " + PhotonNetwork.NickName);

        if (PhotonNetwork.IsConnected)
        {
            RoomMenuElement.SetActive(true);
            LoginWithNickNameElement.SetActive(false);
        }
    }
}
