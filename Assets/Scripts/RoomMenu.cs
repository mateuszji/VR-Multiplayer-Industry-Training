using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class RoomMenu : MonoBehaviourPunCallbacks
{
    public GameObject MainMenuElement;
    public GameObject RoomMenuElement;
    public TMP_Text MessageElement;
    public TMP_InputField RoomNumberInput;

    private void Start()
    {
        MessageElement.enabled = false;

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void DisconnectButton()
    {
        Debug.Log("Disconnected.");
        MainMenuElement.SetActive(true);
        RoomMenuElement.SetActive(false);
        PhotonNetwork.Disconnect();
    }

    public void CreateRoom()
    {
        int roomNumber = Random.Range(000000, 999999);
        string roomName = "Room_" + roomNumber;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom()
    {
        if (RoomNumberInput.text.Length > 3)
        {
            Debug.Log("Room number set.");
            string roomName = "Room_" + RoomNumberInput.text;
            Debug.Log(roomName);
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.Log("No room number.");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created with the name: " + PhotonNetwork.CurrentRoom.Name + " by player: " + PhotonNetwork.NickName);
        StartCoroutine(ShowMessage("Room created with number " + PhotonNetwork.CurrentRoom.Name + ".", 5, false));
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("The local player: " + PhotonNetwork.NickName + " joined to room: " + PhotonNetwork.CurrentRoom.Name + ", player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        StartCoroutine(ShowMessage("The room exist, can't create new one.", 5, true));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        StartCoroutine(ShowMessage("The room doesn't exist.", 5, true));
    }

    IEnumerator ShowMessage(string message, float delay, bool isError)
    {
        MessageElement.text = message;
        if(isError)
        {
            MessageElement.color = new Color(15, 98, 230, 255);
        }
        else
        {
            MessageElement.color = new Color(222, 41, 22, 255);
        }
        MessageElement.enabled = true;
        yield return new WaitForSeconds(delay);
        MessageElement.enabled = false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        StartCoroutine(ShowMessage("Player " + newPlayer.NickName + " joined to room.", 5, false));
        Debug.Log("Player " + newPlayer.NickName + " joined to your room.");
    }
}
