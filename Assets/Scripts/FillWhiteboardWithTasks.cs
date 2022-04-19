using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FillWhiteboardWithTasks : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        print("Streaming Assets Path: " + Application.streamingAssetsPath);
        FileInfo[] tasksFile = directoryInfo.GetFiles("tasks.csv");
        foreach (FileInfo file in tasksFile)
        {
            Debug.Log(file.Name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
