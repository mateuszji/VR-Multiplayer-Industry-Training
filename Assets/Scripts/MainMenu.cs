using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MainMenu : MonoBehaviourPunCallbacks
{
    public GameObject MainMenuElement;
    public GameObject LoginMenuElement;
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Quit app.");
    }
    public void OpenLoginMenu()
    {
        MainMenuElement.SetActive(false);
        LoginMenuElement.SetActive(true);
    }
}
