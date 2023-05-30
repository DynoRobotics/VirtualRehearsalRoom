using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public bool AutoLogin = false;
    public string DefaultPlayerName = "";
    public int DefaultSimpleAvatarSelectionNumber = 0;
    public int DefaultFullBodyAvatarSelectionNumber = 0;

    public PlayerConfigurator.PlayerTypeEnum PlayerType;

    public bool HidePlayerAvatar = false;
    public bool AudienceMember = false;

    public GameObject SwitchToAudienceMemeberButtonGameObject;
    public Text ModeText;

    public bool LoadPreviousPlayerType = true;

    [HideInInspector]
    public VirtualRehearsalRoom VirtualRehersalRoomScriptableObject;

    private int _simpleAvatarSelectionNumber;
    private int _fullBodyAvatarSelectionNumber;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    void Start()
    {
        LoadPrefs();

        if (AutoLogin)
        {
            ConnectToPhotonServer();
        }
    }

    private void SavePrefs()
    {
    }

    private void LoadPrefs()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", DefaultPlayerName);

        if (LoadPreviousPlayerType)
        {
            string playerTypeString = PlayerPrefs.GetString("PlayerType", "GenericVRAudience");

            switch (playerTypeString)
            {
                case ("FullBodyPerformer"):
                    SetFullBodyPerformer();
                    break;
                case ("GenericVRPerformer"):
                    SetGenericVRPerformer();
                    break;
                case ("GenericVRAudience"):
                    SetGenericVRAudience();
                    break;
                default:
                    Debug.LogWarning("Invalid player type string: " + playerTypeString);
                    break;
            }

        }

        VRRSettings settings = VRRSettingsAccessor.Instance.Settings;
        bool randomizeAudienceAvatars = settings.RandomizeAudienceAvatar;
        if (randomizeAudienceAvatars)
        {
            _simpleAvatarSelectionNumber = Random.Range(0, settings.SimpleAvatars.Length - 1);
        }  else
        {
            _simpleAvatarSelectionNumber = PlayerPrefs.GetInt("SimpleAvatarSelectionNumber", DefaultSimpleAvatarSelectionNumber);
        }

        _fullBodyAvatarSelectionNumber = PlayerPrefs.GetInt("FullBodyAvatarSelectionNumber", DefaultFullBodyAvatarSelectionNumber);
    }

    private void ConfigurePlayerProperties()
    {

        string playerTypeString = "";
        switch (PlayerType)
        {
            case (PlayerConfigurator.PlayerTypeEnum.GenericVR):
                playerTypeString = MultiplayerVRConstants.PLAYER_TYPE_VALUE_GENERIC_VR;
                break;
            case (PlayerConfigurator.PlayerTypeEnum.FullBodyVR):
                playerTypeString = MultiplayerVRConstants.PLAYER_TYPE_VALUE_FULL_BODY_VR;
                break;
            case (PlayerConfigurator.PlayerTypeEnum.Desktop):
                playerTypeString = MultiplayerVRConstants.PLAYER_TYPE_VALUE_DESKTOP;
                break;
            case (PlayerConfigurator.PlayerTypeEnum.AI):
                playerTypeString = MultiplayerVRConstants.PLAYER_TYPE_VALUE_AI;
                break;
            default:
                break;
        }

        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable()
        {
            {MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, _simpleAvatarSelectionNumber },
            {MultiplayerVRConstants.FULL_BODY_AVATAR_SELECTION_NUMBER, _fullBodyAvatarSelectionNumber },
            {MultiplayerVRConstants.PLAYER_TYPE, playerTypeString },
            {MultiplayerVRConstants.PLAYER_HIDE_AVATAR_KEY, HidePlayerAvatar },
            {MultiplayerVRConstants.PLAYER_AUDIENCE_MEMBER_KEY, AudienceMember },
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void SetFullBodyPerformer()
    {
        PlayerPrefs.SetString("PlayerType", "FullBodyPerformer");
        ModeText.text = "Full Body";
        PlayerType = PlayerConfigurator.PlayerTypeEnum.FullBodyVR;
        AudienceMember = false;
    }

    public void SetGenericVRPerformer()
    {
        PlayerPrefs.SetString("PlayerType", "GenericVRPerformer");
        ModeText.text = "Performer";
        PlayerType = PlayerConfigurator.PlayerTypeEnum.GenericVR;
        AudienceMember = false;
    }

    public void SetGenericVRAudience()
    {
        PlayerPrefs.SetString("PlayerType", "GenericVRAudience");
        ModeText.text = "Audience";
        PlayerType = PlayerConfigurator.PlayerTypeEnum.GenericVR;
        AudienceMember = true;
    }

    void Update()
    {
    }

    #region UI Callback Methods
    public void ConnectToPhotonServer()
    {
        Time.timeScale = 1;

        SavePrefs();

        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log("Using defulat palyer name!");
        PhotonNetwork.NickName = DefaultPlayerName;

        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.Disconnect();
        }
        PhotonNetwork.OfflineMode = false;

        ConfigurePlayerProperties();
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region Photon Callback Methods
    public override void OnConnected()
    {
        Debug.Log("OnConnected is called. The server is available.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the Master Server with player name: " + PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarning(cause);
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to Lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created with the name: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("The local player: " + PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);

        object mapType;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.SCENE_NAME_KEY, out mapType))
        {
            string currentSceneName = VRRSettingsAccessor.Instance.Settings.StartingSceneName;
            Debug.Log("Joined room and loading scene: " + currentSceneName);
            PhotonNetwork.LoadLevel(currentSceneName);
        }
    }

    #endregion

    #region Private Methods
    void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 0;

        string[] roomPropsInLobby = { MultiplayerVRConstants.SCENE_NAME_KEY };
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.SCENE_NAME_KEY,  VRRSettingsAccessor.Instance.Settings.StartingSceneName} };

        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        roomOptions.CleanupCacheOnLeave = true;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }
    #endregion
}
