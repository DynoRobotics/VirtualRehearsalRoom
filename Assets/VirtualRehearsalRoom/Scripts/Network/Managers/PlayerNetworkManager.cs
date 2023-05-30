using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.SpatialTracking;
using xsens;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerNetworkManager : MonoBehaviourPunCallbacks
{
    public PlayerConfigurator m_PlayerConfigurator;

    public GameObject MainPlayerGameObject;

    public GameObject GenericVRPlayerNetworkGameObject;
    public GameObject FullBodyVRPlayerNetworkGameObject;
    public GameObject DesktopPlayerNetworkGameObject;
    public GameObject AIPlayerNetworkGameObject;

    public PhotonVoiceManager VoiceManager;

    public MusicPlayerManager m_MusicPlayerManager;
    public MutePlayersManager m_MutePlayersManager;

    public Image EmojiImage;
    public Image EmojiOverlayImage;

    public bool IsNonPrimaryAIPlayer;

    public bool IsAudienceMember;

    private float _deltaTime = 0.0f;
    private float _fps = 0.0f;

    [SerializeField]
    InputActionReference TeleportAction;

    private void OnDestroy()
    {
        Destroy(MainPlayerGameObject);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        PlayerConfigurator playerConfigurator = MainPlayerGameObject.GetComponent<PlayerConfigurator>();
        if (playerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR)
        {
            Transform VRCameraTransform = MainPlayerGameObject.GetComponent<PlayerConfigurator>().m_SimpleAvatarManger.SimpleAvatarGameObject.GetComponent<AvatarInputConverter>().AvatarHead.transform;

            Color color;
            if (_fps > 60.0)
            {
                color = Color.green;
            } else if (_fps > 50)
            {
                color = Color.yellow;
            } else
            {
                color = Color.red;
            }

            // TODO(sam): Figure out a way to apply the color to the lable....
            Handles.Label(VRCameraTransform.position + Vector3.up, Mathf.Ceil(_fps).ToString() + " FPS");
        }
#endif
    }

    void Start()
    {
        // Setup the player        
        PhotonNetwork.AutomaticallySyncScene = true;

        if (IsNonPrimaryAIPlayer)
        {
            ConfigureAIPlayer();
        } else
        {
            ConfigurePlayerFromNetwork();
        }

        m_MusicPlayerManager = FindObjectOfType<MusicPlayerManager>();
        m_MutePlayersManager = FindObjectOfType<MutePlayersManager>();

        if (photonView.IsMine)
        {
            StartCoroutine(SyncFpsForAll());
        }
    }

    IEnumerator SyncFpsForAll()
    {
        while (true)
        {
            float fps = 1.0f / _deltaTime;
            photonView.RPC("SyncFps", RpcTarget.All, fps);
            yield return new WaitForSeconds(2.0f);
        }
    }

    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
    }

    void ConfigurePlayerFromNetwork()
    {
        object hideAvatar;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_HIDE_AVATAR_KEY, out hideAvatar))
        {
            m_PlayerConfigurator.SetHideAvatar((bool)hideAvatar);
        }
        else
        {
            Debug.LogWarning("Could not get hide avatar setting for player!");
        }

        object audienceMember;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_AUDIENCE_MEMBER_KEY, out audienceMember))
        {
            IsAudienceMember = (bool)audienceMember;
            if (IsAudienceMember)
            {
                // TODO(sam): Remove UseDirectInteractors from here
                //m_PlayerConfigurator.UseDirectInteractors = true;
                m_PlayerConfigurator.PlayerRole = PlayerConfigurator.PlayerRoleEnum.Audience;
            } else
            {
                // TODO(sam): Remove UseDirectInteractors from here
                //m_PlayerConfigurator.UseDirectInteractors = false;
                m_PlayerConfigurator.PlayerRole = PlayerConfigurator.PlayerRoleEnum.Performer;
            }
        }
        else
        {
            Debug.LogWarning("Could not get is audience member setting for player!");
        }

        object playerType;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_TYPE, out playerType))
        {
            Debug.Log("Player type: " + (string)playerType);
        }

        if (IsAudienceMember)
        {
            SetAudienceTeleportation(GetNetworkedAudienceTeleportation());
        }

        if (IsAudienceMember)
        {
            string voiceType = GetNetworkedAudienceVoiceType();
            VoiceManager.SetVoiceType(voiceType);
        } else
        {
            string voiceType = GetNetworkedPerformersVoiceType();
            VoiceManager.SetVoiceType(voiceType);
            if (photonView.IsMine)
            {
                // FIXME(sam): This could be the cause of a very rare bug that makes individual performers be muted.
                // VoiceManager.UsePerformerVoiceChannels();
            }
        }

        switch ((string)playerType)
        {
            case (MultiplayerVRConstants.PLAYER_TYPE_VALUE_GENERIC_VR):
                ConfigureGenericVRPlayer();
                break;
            case (MultiplayerVRConstants.PLAYER_TYPE_VALUE_FULL_BODY_VR):
                ConfigureFullBodyVRPlayer();
                break;
            case (MultiplayerVRConstants.PLAYER_TYPE_VALUE_DESKTOP):
                ConfigureDesktopPlayer();
                break;
            case (MultiplayerVRConstants.PLAYER_TYPE_VALUE_AI):
                ConfigureAIPlayer();
                break;
            default:
                Debug.Log("Player type: " + (string)playerType + " i not supported");
                break;
        }

    }
    public bool GetNetworkedAudienceTeleportation()
    {
        object audienceTeleportation;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.AUDIENCE_TELEPORTATION_KEY, out audienceTeleportation))
        {
            Debug.Log("Got audience teleportation: " + ((bool)audienceTeleportation).ToString());
        }
        else
        {
            SetNetworkedAudienceTeleportation(false);
            return false;
        }

        return (bool)audienceTeleportation;
    }
    public void SetNetworkedAudienceTeleportation(bool value)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.AUDIENCE_TELEPORTATION_KEY, value} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    void SetAudienceTeleportation(bool value)
    {
        if (IsAudienceMember)
        {
            if (value)
            {
                TeleportAction.action.Enable();
            } else
            {
                TeleportAction.action.Disable();
            }
        }
    }

    public string GetNetworkedPerformersVoiceType()
    {

        object voiceType;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.PERFORMERS_VOICE_TYPE_KEY, out voiceType))
        {
            string defaultVoiceType = VRRSettingsAccessor.Instance.Settings.GetPerformerDefaultVoiceType();
            Debug.Log("Performers voice type not set, setting to: " + defaultVoiceType);
            SetNetworkedPerformersVoiceType(defaultVoiceType);
            return defaultVoiceType;
        }

        return (string)voiceType;
    }

    public string GetNetworkedAudienceVoiceType()
    {

        object voiceType;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.AUDIENCE_VOICE_TYPE_KEY, out voiceType))
        {
            string defaultVoiceType = VRRSettingsAccessor.Instance.Settings.GetAudienceDefaultVoiceType();
            Debug.Log("Audience voice type not set, setting to: " + defaultVoiceType);
            SetNetworkedAudienceVoiceType(defaultVoiceType);
            return defaultVoiceType;
        }

        return (string)voiceType;
    }

    public void SetNetworkedAudienceVoiceType(string voiceType)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.AUDIENCE_VOICE_TYPE_KEY, voiceType} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    public void SetNetworkedPerformersVoiceType(string voiceType)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.PERFORMERS_VOICE_TYPE_KEY, voiceType} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    #region Generic VR Player

    void ConfigureGenericVRPlayer()
    {
        m_PlayerConfigurator.PlayerType = PlayerConfigurator.PlayerTypeEnum.GenericVR;

        if (photonView.IsMine)
        {
            ConfigureLocalPlayerCommon();
        } else
        {
            ConfigureRemotePlayerCommon();
        }

        ConfigureSelectedSimpleAvatarModel();

        GenericVRPlayerNetworkGameObject.SetActive(true);
        Component component = GenericVRPlayerNetworkGameObject.GetComponent<MultiplayerVRSynchronization>();
        AddOrSetEmptyPhotonViewObservedComponent(component);
    }

    #endregion

    #region Desktop Player
    void ConfigureDesktopPlayer()
    {
        m_PlayerConfigurator.PlayerType = PlayerConfigurator.PlayerTypeEnum.Desktop;

        if (photonView.IsMine)
        {
            ConfigureLocalPlayerCommon();

        } else
        {
            ConfigureRemotePlayerCommon();
        }

        ConfigureSelectedSimpleAvatarModel();

        DesktopPlayerNetworkGameObject.SetActive(true);
        Component component = DesktopPlayerNetworkGameObject.GetComponent<MultiplayerVRSynchronization>();
        AddOrSetEmptyPhotonViewObservedComponent(component);
    }

    #endregion

    #region Full Body VR Player
    void ConfigureFullBodyVRPlayer()
    {
        m_PlayerConfigurator.PlayerType = PlayerConfigurator.PlayerTypeEnum.FullBodyVR;

        if (photonView.IsMine)
        {
            ConfigureLocalPlayerCommon();

        } else
        {
            ConfigureRemoteFullBodyVRPlayer();
            ConfigureRemotePlayerCommon();
        }

        ConfigureSelectedFullBodyAvatarModel();

        FullBodyVRPlayerNetworkGameObject.SetActive(true);
        Component component = FullBodyVRPlayerNetworkGameObject.GetComponent<MultiplayerXsensSynchronization>();
        AddOrSetEmptyPhotonViewObservedComponent(component);
    }

    void ConfigureRemoteFullBodyVRPlayer()
    {
    }

    #endregion

    #region AI Player
    void ConfigureAIPlayer()
    {
        m_PlayerConfigurator.PlayerType = PlayerConfigurator.PlayerTypeEnum.AI;

        if (photonView.IsMine)
        {
            ConfigureLocalPlayerCommon();
        }
        else
        {
            ConfigureRemotePlayerCommon();
        }

        ConfigureSelectedSimpleAvatarModel();

        AIPlayerNetworkGameObject.SetActive(true);
        Component component = AIPlayerNetworkGameObject.GetComponent<MultiplayerVRSynchronization>();
        AddOrSetEmptyPhotonViewObservedComponent(component);

    }
    #endregion

    #region Common For All Player Types

    void ConfigureLocalPlayerCommon()
    {
        SetPlayerDisplayName();
        m_PlayerConfigurator.ConfigureLocalPlayer();
    }

    void ConfigureRemotePlayerCommon()
    {
        SetPlayerDisplayName();
        m_PlayerConfigurator.ConfigureRemotePlayer();
    }

    public void SetPlayerDisplayName()
    {
        m_PlayerConfigurator.PlayerDisplayName = photonView.Owner.NickName;
    }

    public void SelectNextAvatar()
    {
        if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR)
        {
            SelectNextSimpleAvatar();
        } else if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            SelectNextFullBodyAvatar();
        } else
        {
            Debug.LogWarning("Player avatar selection is not implemented for this player type");
        }
    }

    public void SelectPreviousAvatar()
    {
        if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR)
        {
            SelectPreviousSimpleAvatar();
        } else if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            SelectPreviousFullBodyAvatar();
        } else
        {
            Debug.LogWarning("Player avatar selection is not implemented for this player type");
        }
    }

    #region Simple Avatar

    public void SelectNextSimpleAvatar()
    {
        int avatarSelectionNumber;
        if (GetSimpleAvatarSelectionNumber(out avatarSelectionNumber))
        {
            int numAvatars = VRRSettingsAccessor.Instance.Settings.SimpleAvatars.Length;
            int nextAvatarSelectionNumber = avatarSelectionNumber + 1;

            if (nextAvatarSelectionNumber >= numAvatars)
            {
                nextAvatarSelectionNumber = 0;
            }

            UpdateSimpleAvatar(nextAvatarSelectionNumber);
        }
    }

    public void SelectPreviousSimpleAvatar()
    {
        int avatarSelectionNumber;
        if (GetSimpleAvatarSelectionNumber(out avatarSelectionNumber))
        {
            int numAvatars = VRRSettingsAccessor.Instance.Settings.SimpleAvatars.Length;
            int previousAvatarSelectionNumber = avatarSelectionNumber - 1;

            if (previousAvatarSelectionNumber < 0)
            {
                previousAvatarSelectionNumber = numAvatars - 1;
            }
            UpdateSimpleAvatar(previousAvatarSelectionNumber);
        }
    }
    public void SelectSimpleAvatarByName(string avatarName)
    {
        string[] avatarNames = VRRSettingsAccessor.Instance.Settings.GetSimpleAvatarNames();
        int avatarIndex = System.Array.IndexOf(avatarNames, avatarName);
        if (avatarIndex != -1)
        {
            UpdateSimpleAvatar(avatarIndex);
        } else
        {
            Debug.LogWarning("Could not find simple avatar with name: " + avatarName);
        }
    }

    public void UpdateSimpleAvatar(int selectionNumber)
    {
        // NOTE(sam): should maybe save to file somewhere else...
        PlayerPrefs.SetInt("AvatarSelectionNumber", selectionNumber);

        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable()
        {
            {MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, selectionNumber}
        };
        photonView.Owner.SetCustomProperties(playerProps);
    }

    private bool GetSimpleAvatarSelectionNumber(out int selectionNumber)
    {
        object avatarSelectionNumber;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
        {
            selectionNumber = (int)avatarSelectionNumber;
            return true;
        }

        selectionNumber = 0;
        return false;
    }

    void ConfigureSelectedSimpleAvatarModel()
    {
        int avatarSelectionNumber;
        if (GetSimpleAvatarSelectionNumber(out avatarSelectionNumber))
        {
            m_PlayerConfigurator.SimpleAvatarSelectionNumber = avatarSelectionNumber;
            m_PlayerConfigurator.UpdateAvatar();
        }
        else
        {
            Debug.LogWarning("Could not get avatar selection number");
        }
    }

    #endregion

    #region Full Body Avatar

    // NOTE(sam): only works for full body avatar at the moment
    public void HideAvatar()
    {
        Debug.Log("Hiding avatar");
        SetHideAvatar(true);
    }
    public void ShowAvatar()
    {
        Debug.Log("Showing avatar");
        SetHideAvatar(false);
    }

    public void ToggleHideAvatar()
    {
        bool avatarIsHidden;
        GetHideAvatar(out avatarIsHidden);
        if (avatarIsHidden)
        {
            ShowAvatar();
        } else
        {
            HideAvatar();
        }
    }

    public void SelectNextFullBodyAvatar()
    {
        Debug.Log("Selecting next full body avatar");
        int avatarSelectionNumber;
        if (GetFullBodyAvatarSelectionNumber(out avatarSelectionNumber))
        {
            int numAvatars = VRRSettingsAccessor.Instance.Settings.FullBodyAvatars.Length;
            int nextAvatarSelectionNumber = avatarSelectionNumber + 1;

            if (nextAvatarSelectionNumber >= numAvatars)
            {
                nextAvatarSelectionNumber = 0;
            }

            UpdateFullBodyAvatar(nextAvatarSelectionNumber);
        }
    }

    public void SelectPreviousFullBodyAvatar()
    {
        int avatarSelectionNumber;
        if (GetFullBodyAvatarSelectionNumber(out avatarSelectionNumber))
        {
            int numAvatars = VRRSettingsAccessor.Instance.Settings.FullBodyAvatars.Length;
            int previousAvatarSelectionNumber = avatarSelectionNumber - 1;

            if (previousAvatarSelectionNumber < 0)
            {
                previousAvatarSelectionNumber = numAvatars - 1;
            }
            UpdateFullBodyAvatar(previousAvatarSelectionNumber);
        }
    }

    public void SelectFullBodyAvatarByName(string avatarName)
    {
        string[] avatarNames = VRRSettingsAccessor.Instance.Settings.GetFullBodyAvatarNames();
        int avatarIndex = System.Array.IndexOf(avatarNames, avatarName);
        if (avatarIndex != -1)
        {
            UpdateFullBodyAvatar(avatarIndex);
        } else
        {
            Debug.LogWarning("Could not find simple avatar with name: " + avatarName);
        }
    }

    private void UpdateFullBodyAvatar(int selectionNumber)
    {
        Debug.Log("Updating full body avatar selection number");
        // NOTE(sam): should maybe save to file somewhere else...
        PlayerPrefs.SetInt("FullBodyAvatarSelectionNumber", selectionNumber);

        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable()
        {
            {MultiplayerVRConstants.FULL_BODY_AVATAR_SELECTION_NUMBER, selectionNumber}
        };
        photonView.Owner.SetCustomProperties(playerProps);
    }

    private bool GetFullBodyAvatarSelectionNumber(out int selectionNumber)
    {
        object avatarSelectionNumber;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.FULL_BODY_AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
        {

            selectionNumber = (int)avatarSelectionNumber;
            return true;
        }

        selectionNumber = 0;
        return false;
    }
    private bool GetHideAvatar(out bool hideAvatar)
    {
        object hideAvatarObject;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_HIDE_AVATAR_KEY, out hideAvatarObject))
        {
            m_PlayerConfigurator.HideAvatar = (bool)hideAvatarObject;
            Debug.Log("Got hide avatar " + ((bool)hideAvatarObject).ToString());
            hideAvatar = (bool)hideAvatarObject;
            return true;
        }
        else
        {
            Debug.LogWarning("Could not get hide avatar setting for player!");
            hideAvatar = false;
            return false;
        }
    }

    private void SetHideAvatar(bool hideAvatar)
    {
        Debug.Log("Setting hide avatar");

        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable()
        {
            {MultiplayerVRConstants.PLAYER_HIDE_AVATAR_KEY, hideAvatar}
        };
        photonView.Owner.SetCustomProperties(playerProps);
    }

    void ConfigureSelectedFullBodyAvatarModel()
    {
        int avatarSelectionNumber;
        if (GetFullBodyAvatarSelectionNumber(out avatarSelectionNumber))
        {
            m_PlayerConfigurator.FullBodyAvatarSelectionNumber = avatarSelectionNumber;
            m_PlayerConfigurator.UpdateAvatar();
        }
        else
        {
            Debug.LogWarning("Could not get avatar selection number");
        }
    }
    #endregion

    void AddOrSetEmptyPhotonViewObservedComponent(Component component)
    {
        if (photonView.ObservedComponents.Count == 1 && photonView.ObservedComponents[0] == null)
        {
            photonView.ObservedComponents[0] = component;
        } else
        {
            photonView.ObservedComponents.Add(component);
        }
    }

    #endregion

    public void ShowEmojiForAll()
    {
        photonView.RPC("ShowEmoji", RpcTarget.All);
    }

    #region Photon Remote Procedure Calls

    [PunRPC]
    public void ShowEmoji()
    {
        if (!EmojiImage.enabled)
        {
            EmojiImage.enabled = true;
            EmojiOverlayImage.enabled = true;
            StartCoroutine(HideEmoji(3.0f));
        }
    }

    IEnumerator HideEmoji(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        EmojiImage.enabled = false;
        EmojiOverlayImage.enabled = false;
    }

    [PunRPC]
    public void SyncFps(float fps)
    {
        _fps = fps;
    }

    #endregion


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (photonView.Owner == targetPlayer)
        {
            object avatarSelectionNumber;
            if (changedProps.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                m_PlayerConfigurator.SimpleAvatarSelectionNumber = (int)avatarSelectionNumber;
                m_PlayerConfigurator.UpdateAvatar();
            }

            if (changedProps.TryGetValue(MultiplayerVRConstants.FULL_BODY_AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                Debug.Log("Full body selection number changed, updating avatar");
                m_PlayerConfigurator.FullBodyAvatarSelectionNumber = (int)avatarSelectionNumber;
                m_PlayerConfigurator.UpdateAvatar();
            }

            object hideAvatar;
            if (changedProps.TryGetValue(MultiplayerVRConstants.PLAYER_HIDE_AVATAR_KEY, out hideAvatar))
            {
                Debug.Log("Hide avatar changed, updating");
                m_PlayerConfigurator.SetHideAvatar((bool)hideAvatar);
            }
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (IsAudienceMember)
        {
            object voiceType;
            if (propertiesThatChanged.TryGetValue(MultiplayerVRConstants.AUDIENCE_VOICE_TYPE_KEY, out voiceType))
            {
                VoiceManager.SetVoiceType((string)voiceType);
            }

            object audienceTeleportation;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.AUDIENCE_TELEPORTATION_KEY, out audienceTeleportation))
            {
                SetAudienceTeleportation((bool)audienceTeleportation);
            }
        } else
        {
            object voiceType;
            if (propertiesThatChanged.TryGetValue(MultiplayerVRConstants.PERFORMERS_VOICE_TYPE_KEY, out voiceType))
            {
                VoiceManager.SetVoiceType((string)voiceType);
            }
        }


    }
}
