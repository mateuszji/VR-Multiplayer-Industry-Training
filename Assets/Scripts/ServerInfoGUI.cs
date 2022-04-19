using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Valve.VR;

public class ServerInfoGUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private SteamVR_Action_Boolean interactServerInfo;
    [SerializeField] private GameObject ServerInfoCanvas;
    [SerializeField] private GameObject SinglePlayerInfo;
    [SerializeField] private Transform parentEl;
    [SerializeField] TMP_Text RoomNameText, PlayerCountText;
    private bool isVisible = false;
    private void Start()
    {
        HideGUI();
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }
    void Update()
    {
        if(interactServerInfo.GetStateDown(SteamVR_Input_Sources.LeftHand) || interactServerInfo.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            if (!isVisible)
                ShowGUI();
            else
                HideGUI();
        }
    }

    private void ShowGUI()
    {
        isVisible = true;
        generatePlayersList();
        updatePlayerCount();
        ServerInfoCanvas.SetActive(true);
        SinglePlayerInfo.SetActive(false);
    }

    private void HideGUI()
    {
        isVisible = false;
        removePlayersFromList();
        ServerInfoCanvas.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updatePlayerCount();
        updatePlayersList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updatePlayerCount();
        updatePlayersList();
    }
    private void updatePlayerCount()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 10)
            PlayerCountText.text = "0" + PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/20";
        else
            PlayerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/20";
    }

    private void updatePlayersList()
    {
        removePlayersFromList();
        generatePlayersList();
    }

    private void generatePlayersList()
    {
        float posX = (float)-80;
        float posY = (float)101.5;
        float posZ = 0;
        int i = 0;
        foreach (Player player in PhotonNetwork.PlayerList) 
        {
            GameObject newEl = GameObject.Instantiate(SinglePlayerInfo);
            newEl.transform.SetParent(parentEl.gameObject.transform);
            newEl.transform.localPosition = new Vector3(posX, posY, posZ);
            newEl.transform.localScale = new Vector3(1, 1, 1);
            newEl.transform.localRotation = new Quaternion(0, 0, 0, 0);
            newEl.name = "SinglePlayerName_" + player.NickName;
            TMP_Text TMPText = newEl.GetComponentInChildren<TMP_Text>();
            TMPText.text = player.NickName;

            if(player.IsMasterClient)
                TMPText.color = new Color32(197, 179, 88, 190);

            newEl.tag = "PlayerNameOnList";

            posY -= (float)20.5;
            newEl.SetActive(true);
            if (i == 9)
            {
                posY = (float)101.5;
                posX = 80;
            }
            i++;
        }
        /*
        for (int i = 0; i < 20; i++)
        {
            GameObject newEl = GameObject.Instantiate(SinglePlayerInfo);
            newEl.transform.SetParent(parentEl.gameObject.transform);
            newEl.transform.localPosition = new Vector3(posX, posY, posZ);
            newEl.transform.localScale = new Vector3(1, 1, 1);
            newEl.transform.localRotation = new Quaternion(0, 0, 0, 0);
            newEl.name = "SinglePlayerName_" + ;
            newEl.tag = "PlayerNameOnList";
            posY -= (float)20.5;
            newEl.SetActive(true);
            if(i == 9)
            {
                posY = (float)101.5;
                posX = 80;
            }
        }
        */
    }

    private void removePlayersFromList()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerNameOnList");
        foreach (GameObject player in players)
            GameObject.Destroy(player);
    }
}
