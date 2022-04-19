using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Voice.PUN;

public class HighlightVoice : MonoBehaviourPunCallbacks
{
    [SerializeField] Image speakerIcon = null;
    [SerializeField] PhotonVoiceView photonVoiceView;

    private LocalPlayerMic playerMic;
    private new PhotonView photonView;
    private void Awake()
    {
        this.speakerIcon.enabled = false;
        SetLocalPlayerMic(false);
    }

    private void Update()
    {
        this.speakerIcon.enabled = this.photonVoiceView.IsSpeaking;
        SetLocalPlayerMic(this.photonVoiceView.IsRecording);
    }

    public void SetLocalPlayerMic(bool isVisible)
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            if (playerMic == null) playerMic = FindObjectOfType<LocalPlayerMic>();

            playerMic.MicImageVisibility(isVisible);
        }
    }
}
