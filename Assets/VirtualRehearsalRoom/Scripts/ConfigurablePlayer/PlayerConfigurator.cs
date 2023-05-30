using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerConfigurator : MonoBehaviour
{

    [System.Serializable]
    public enum PlayerTypeEnum
    {
        GenericVR,
        FullBodyVR,
        Desktop,
        AI
    }

    [System.Serializable]
    public enum PlayerRoleEnum
    {
        Audience,
        Performer
    }

    public PlayerTypeEnum PlayerType;
    public PlayerRoleEnum PlayerRole;

    public bool HideAvatar = false;

    public bool PlayerIsRemote;

    [Space]
    [Header("Network")]
    public bool UseNetwork;
    public GameObject NetworkGameObject;

    [Space]
    [Header("Common")]
    public string PlayerDisplayName = "";

    [Space]
    [Header("Players")]
    public GameObject DesktopPlayer;
    public GameObject VRPlayer;
    public GameObject AIPlayer;

    [Space]
    [Header("VR Player Config")]
    public VRPlayerManager m_VRPlayerManager;
    public PlayerUIManager m_PlayerUIManager;

    [Space]
    [Header("Simple Avatar")]
    public int SimpleAvatarSelectionNumber = 0;
    public SimpleAvatarManager m_SimpleAvatarManger;

    [Space]
    [Header("Full Body Avatar")]
    public int FullBodyAvatarSelectionNumber = 0;
    public FullBodyAvatarManager m_FullBodyAvatarManager;

    private void Awake()
    {
        if (UseNetwork)
        {
            NetworkGameObject.SetActive(true);
        }
    }

    void Start()
    {
        if (!UseNetwork)
        {
            if (PlayerIsRemote)
            {
                ConfigureRemotePlayer();
            } else
            {
                ConfigureLocalPlayer();
            }
        }
    }

    public void ConfigureLocalPlayer()
    {
        PlayerIsRemote = false;
        m_PlayerUIManager.enabled = true;
        m_VRPlayerManager.PlayerType = PlayerType;

        if (PlayerRole == PlayerRoleEnum.Audience)
        {
            m_VRPlayerManager.SetUseDirectInteractors(true);
        }
        else
        {
            m_VRPlayerManager.SetUseDirectInteractors(false);
        }

        m_VRPlayerManager.ConfigureUsingCurrentSettings();

        switch (PlayerType)
        {
            case (PlayerTypeEnum.GenericVR):
                ConfigureLocalGenericVRPlayer();
                break;
            case (PlayerTypeEnum.FullBodyVR):
                ConfigureLocalFullBodyVRPlayer();
                break;
            case (PlayerTypeEnum.Desktop):
                ConfigureLocalDesktopPlayer();
                break;
            case (PlayerTypeEnum.AI):
                ConfigureLocalAIPlayer();
                break;
            default:
                Debug.Log("Local player type: " + PlayerType.ToString() + " i not supported");
                break;
        }
    }

    public void ConfigureRemotePlayer()
    {
        PlayerIsRemote = true;

        switch (PlayerType)
        {
            case (PlayerTypeEnum.GenericVR):
                ConfigureRemoteGenericVRPlayer();
                break;
            case (PlayerTypeEnum.FullBodyVR):
                ConfigureRemoteFullBodyVRPlayer();
                break;
            case (PlayerTypeEnum.Desktop):
                ConfigureRemoteDesktopPlayer();
                break;
            case (PlayerTypeEnum.AI):
                ConfigureRemoteAIPlayer();
                break;
            default:
                Debug.Log("Remote player type: " + PlayerType.ToString() + " i not supported");
                break;
        }
    }

    public void SetHideAvatar(bool shouldHide)
    {
        HideAvatar = shouldHide;
        Debug.LogWarning("Hide Avatar is not implemented");
        // m_FullBodyAvatarManager.HideAvatar = HideAvatar;
    }

    public Camera GetPlayerCamera()
    {
        if (PlayerType == PlayerTypeEnum.GenericVR || PlayerType == PlayerTypeEnum.FullBodyVR)
        {
            return VRPlayer.GetComponentInChildren<Camera>();
        } else if (PlayerType == PlayerTypeEnum.Desktop)
        {
            return DesktopPlayer.GetComponentInChildren<Camera>();
        }
        return null;
    }

    #region VR Config
    private void ConfigureLocalVRCommon()
    {
        VRPlayer.SetActive(true);
        SetUpTeleportersForVRPlayer();
    }

    private void SetUpTeleportersForVRPlayer()
    {
        TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();

        if (teleportationAreas.Length > 0)
        {
            Debug.Log("Found " + teleportationAreas.Length + " teleportation area.");
            foreach (var item in teleportationAreas)
            {
                item.teleportationProvider = VRPlayer.GetComponent<TeleportationProvider>();
            }
        }
    }
    #endregion

    #region AI Player
    private void ConfigureLocalAIPlayer()
    {
        m_SimpleAvatarManger.SetAvatarIsRemote(false);
        AIPlayer.SetActive(true);

        ConfigureAIPlayerCommon();
    }

    private void ConfigureRemoteAIPlayer()
    {
        m_SimpleAvatarManger.SetAvatarIsRemote(true);

        ConfigureAIPlayerCommon();
    }

    private void ConfigureAIPlayerCommon()
    {
        m_SimpleAvatarManger.enabled = true;
        m_SimpleAvatarManger.SetPlayerType(PlayerTypeEnum.AI);
        m_SimpleAvatarManger.PlayerDisplayName = PlayerDisplayName;
        m_SimpleAvatarManger.SelectAvatar(SimpleAvatarSelectionNumber);
    }

    #endregion

    #region Generic VR Player
    private void ConfigureLocalGenericVRPlayer()
    {
        Debug.Log("Configuring player with name: " + PlayerDisplayName + " as Local Generic VR Player");
        ConfigureLocalVRCommon();

        m_SimpleAvatarManger.SetAvatarIsRemote(false);
        ConfigureGenericVRPlayerCommon();
    }
    private void ConfigureRemoteGenericVRPlayer()
    {
        m_SimpleAvatarManger.SetAvatarIsRemote(true);
        ConfigureGenericVRPlayerCommon();
    }

    private void ConfigureGenericVRPlayerCommon()
    {
        m_SimpleAvatarManger.enabled = true;

        m_SimpleAvatarManger.SetPlayerType(PlayerTypeEnum.GenericVR);
        m_SimpleAvatarManger.PlayerDisplayName = PlayerDisplayName;
        m_SimpleAvatarManger.SelectAvatar(SimpleAvatarSelectionNumber);
        m_SimpleAvatarManger.SetLayersForCameraVisibility();
    }
    #endregion

    #region Desktop Player
    private void ConfigureLocalDesktopPlayer()
    {
        Debug.Log("Configuring player with name: " + PlayerDisplayName + " as Local Desktop Player");
        DesktopPlayer.SetActive(true);
        m_SimpleAvatarManger.SetAvatarIsRemote(false);

        ConfigureDesktopPlayerCommon();
    }

    private void ConfigureRemoteDesktopPlayer()
    {
        m_SimpleAvatarManger.SetAvatarIsRemote(true);

        ConfigureDesktopPlayerCommon();
    }

    private void ConfigureDesktopPlayerCommon()
    {
        m_SimpleAvatarManger.SetPlayerType(PlayerTypeEnum.Desktop);

        if (!HideAvatar)
        {
            m_SimpleAvatarManger.enabled = true;
            m_SimpleAvatarManger.PlayerDisplayName = PlayerDisplayName;
            m_SimpleAvatarManger.SelectAvatar(SimpleAvatarSelectionNumber);
        } else
        {
            m_SimpleAvatarManger.enabled = false;
        }
    }

    #endregion

    #region Full Body VR Player
    private void ConfigureLocalFullBodyVRPlayer()
    {
        Debug.Log("Configuring player with name: " + PlayerDisplayName + " as Local Full Body VR Player");

        ConfigureFullBodyVRPlayerCommon();
        ConfigureLocalVRCommon();

        m_FullBodyAvatarManager.SetAvatarIsRemote(false);
    }

    private void ConfigureRemoteFullBodyVRPlayer()
    {
        Debug.Log("Configuring player with name: " + PlayerDisplayName + " as Remote Full Body VR Player");

        m_FullBodyAvatarManager.SetAvatarIsRemote(true);
        ConfigureFullBodyVRPlayerCommon();
    }

    private void ConfigureFullBodyVRPlayerCommon()
    {
        // Debug.Log("Configuring Full Body Player Common");
        m_FullBodyAvatarManager.enabled = true;
        m_FullBodyAvatarManager.PlayerDisplayName = PlayerDisplayName;
        m_FullBodyAvatarManager.UpdateAvatar();
    }

    #endregion

    public void UpdateAvatar()
    {
        if (SimpleAvatarIsUsed())
        {
            m_SimpleAvatarManger.SetAvtarSelectionNumber(SimpleAvatarSelectionNumber);
            m_SimpleAvatarManger.UpdateAvatar();
        } else if (FullBodyAvatarIsUsed())
        {
            m_FullBodyAvatarManager.SetAvtarSelectionNumber(FullBodyAvatarSelectionNumber);
            m_FullBodyAvatarManager.UpdateAvatar();
        }
    }
    private bool SimpleAvatarIsUsed()
    {
        return PlayerType != PlayerConfigurator.PlayerTypeEnum.FullBodyVR;
    }
    private bool FullBodyAvatarIsUsed()
    {
        return PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR;
    }

}
