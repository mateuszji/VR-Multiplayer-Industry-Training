using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using System.Linq;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(PhotonView))]
public class PhotonPlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public const float MOVEMENT_SMOOTH = 10f;

    public static PhotonPlayerScript Ins { get; private set; }
    public static GameObject LocalPlayerInstance { get; set; }

    private static List<Transform> listNicknames = new List<Transform>();
    private static new Camera camera;

    [SerializeField] Transform panelNickname = null;
    [SerializeField] PosRotSynch[] posRotSynches = null;

    private new PhotonView photonView;

    public int PlayerActorNumber { get; private set; }

    public override void OnEnable()
    {
        base.OnEnable();
        listNicknames.Add(panelNickname);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        listNicknames.Remove(panelNickname);
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            Ins = this;
            LocalPlayerInstance = gameObject;
            posRotSynches.ForEach(x => x.Setup());
            panelNickname.gameObject.SetActive(false);
        }

        if (photonView?.Owner != null)
        {
            gameObject.name = $"PhotonPlayer_{photonView.Owner.NickName}";
            PlayerActorNumber = photonView.Owner.ActorNumber;
            panelNickname.GetComponentInChildren<TextMeshProUGUI>().text = photonView.Owner.NickName;

            if (photonView.Owner.IsMasterClient)
                panelNickname.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(197, 179, 88, 255);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            posRotSynches.ForEach(x => x.UpdateLerp());
        }

        RotateNicknames();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
    }

    public Transform FindObject(string name)
    {
        return posRotSynches.FirstOrDefault(x => x.GetServerObject.name.Equals(name)).GetServerObject;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        bool isActive;
        foreach (PosRotSynch posRotSynch in posRotSynches)
        {

            if (stream.IsWriting)
            {
                isActive = posRotSynch.IsObjectActive;

                stream.SendNext(isActive);
                if (isActive)
                {
                    stream.SendNext(posRotSynch.GetLocalObjectPosition);
                    stream.SendNext(posRotSynch.GetLocalObjectRotation);
                }
            }
            else if (stream.IsReading)
            {
                isActive = (bool)stream.ReceiveNext();
                posRotSynch.ShowHideServerObject(isActive);
                if (isActive)
                {
                    posRotSynch.MoveServerObjectToPosition = (Vector3)stream.ReceiveNext();
                    posRotSynch.MoveServerObjectToRotation = (Quaternion)stream.ReceiveNext();
                }
            }
        }
    }

    private void RotateNicknames()
    {
        if (camera == null)
        {
            camera = Camera.main;
            return;
        }

        foreach (Transform nickname in listNicknames)
        {
            if (nickname.gameObject.activeSelf)
                nickname.LookAt(camera.transform, Vector3.up);
        }
    }
}

[System.Serializable]
public class PosRotSynch
{
    public enum FindType
    {
        ByName, ByTag
    }

    #region Serialized Variables
    [Tooltip("Obiekt w PlayerServer (który bêdzie synchronizowany)")]
    [SerializeField] Transform serverObject = null;

    [Tooltip("Czy ten obiekt ma byæ widoczny przy uruchomieniu?")]
    [SerializeField] bool isServerObjectVisableOnStart = false;

    [Tooltip("Sposób szukania lokalnego obiektu")]
    [SerializeField] FindType findType = FindType.ByName;

    [Tooltip("Nazwa obiektu lokalnego (w PlayerLocal)")]
    [SerializeField] string localObjectName = string.Empty;

    [Tooltip("Czy lokalny obiekt (PlayerLocal) ma byæ widoczny na start?")]
    [SerializeField] bool isLocalObjectVisableOnStart = false;
    #endregion

    private Transform localObject;

    #region Public Properties
    public Transform GetServerObject => serverObject;

    public bool IsObjectActive => localObject && localObject.gameObject.activeSelf;

    public Vector3 GetLocalObjectPosition => localObject.position;
    public Quaternion GetLocalObjectRotation => localObject.rotation;

    public Vector3 MoveServerObjectToPosition { private get; set; }
    public Quaternion MoveServerObjectToRotation { private get; set; }
    #endregion

    public void UpdateLerp()
    {
        try
        {
            serverObject.position = Vector3.Lerp(serverObject.position, MoveServerObjectToPosition, PhotonPlayerScript.MOVEMENT_SMOOTH * Time.deltaTime);

            if (!IsQuaternionInvalid(MoveServerObjectToRotation))
                serverObject.rotation = Quaternion.Lerp(serverObject.rotation, MoveServerObjectToRotation, PhotonPlayerScript.MOVEMENT_SMOOTH * Time.deltaTime);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void ShowHideServerObject(bool isActive)
    {
        serverObject.gameObject.SetActive(isActive);
    }

    public void Setup()
    {
        try
        {
            localObject = (findType == FindType.ByName) ? GameObject.Find(localObjectName)?.transform : GameObject.FindGameObjectWithTag(localObjectName)?.transform;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }

        if (!localObject) Debug.LogError($"Nie znalezion obiektu o nazwie: {localObjectName} (b³êdna NAZWA lub obiekt jest WY£¥CZONY)");
        else localObject.gameObject.SetActive(isLocalObjectVisableOnStart);

        serverObject.gameObject.SetActive(isServerObjectVisableOnStart);

    }

    private static bool IsQuaternionInvalid(Quaternion q)
    {
        bool check = q.x == 0f;
        check &= q.y == 0;
        check &= q.z == 0;
        check &= q.w == 0;

        return check;
    }
}