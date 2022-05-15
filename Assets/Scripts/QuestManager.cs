using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] GameObject questGUI;
    [SerializeField] TMP_Text questText;
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    public void SetDoneSingleQuest(int id)
    {

        GameObject[] quests;
        quests = GameObject.FindGameObjectsWithTag("Quest");
        foreach (GameObject quest in quests)
        {            
            if(int.Parse(quest.name) == id)
            {
                quest.GetComponent<QuestItem>().photonView.RPC("RPC_StrikeText", RpcTarget.AllBuffered, null);
                TMP_Text text = quest.GetComponent<TMP_Text>();
                text.fontStyle = FontStyles.Strikethrough;
                photonView.RPC("RPC_ShowGUI", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, text.text);
            }
        }
    }

    IEnumerator ShowGUI(string player, float delay, string questString)
    {
        questText.text = player + " has done quest: " + questString;
        questGUI.SetActive(true);
        yield return new WaitForSeconds(delay);
        questGUI.SetActive(false);
    }

    [PunRPC]
    public void RPC_ShowGUI(string playerName, string questText)
    {
        StartCoroutine(ShowGUI(playerName, 5, questText));
    }
}
