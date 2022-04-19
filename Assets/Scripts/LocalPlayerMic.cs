using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocalPlayerMic : MonoBehaviour
{
    [SerializeField] Image micIcon = null;

    // Start is called before the first frame update
    void Start()
    {
        micIcon.enabled = false;
    }

    public void MicImageVisibility(bool isVisible)
    {
        micIcon.enabled = isVisible;
    }
}
