using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LoginMenu : MonoBehaviourPunCallbacks
{
    public GameObject MainMenuElement;
    public GameObject LoginMenuElement;
    public GameObject LoginWithNickNameElement;
    public GameObject RoomMenuElement;
    private int guestNumber;

    public void BackButton()
    {
        MainMenuElement.SetActive(true);
        LoginMenuElement.SetActive(false);
    }

    public void LoginWithNickNameButton()
    {
        LoginMenuElement.SetActive(false);
        LoginWithNickNameElement.SetActive(true);
    }

    public void ConnectToPhotonAnonymously()
    {
        guestNumber = Random.Range(000000, 999999);
        PhotonNetwork.NickName = "Guest" + guestNumber;
        PhotonNetwork.ConnectUsingSettings();

    }
    public override void OnConnected()
    {
        Debug.Log("Conntected to server.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server anonymously with guest number: " + guestNumber);

        if(PhotonNetwork.IsConnected)
        {
            RoomMenuElement.SetActive(true);
            LoginMenuElement.SetActive(false);
        }
    }
}
