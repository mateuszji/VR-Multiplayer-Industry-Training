using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ToolSpawnerManager : MonoBehaviour
{
    [SerializeField] Transform positionToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnTool()
    {
        string[] toolInfo = this.name.Split('_');

        GameObject gm = PhotonNetwork.Instantiate(toolInfo[0], positionToSpawn.position, positionToSpawn.rotation);

        if (toolInfo[0] == "Allen")
            gm.transform.Find("AllenKey").name = "AllenKey_" + toolInfo[1];
        if (toolInfo[0] == "Wrench")
            gm.transform.Find("Wrench").name = "WrenchKey_" + toolInfo[1];
    }
}
