using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UIs")]
    public GameObject PerformerMenu_GameObject;
    public GameObject AudienceMenu_GameObject;

    [Space]
    [Header("Configurable player managers")]
    public PlayerNetworkManager m_PlayerNetworkManager;
    public PhotonVoiceManager m_PhotonVoiceManager;
    public SceneSelectionUIManager m_SceneSelectionUIManager;
    public AvatarSelectionUIManager m_AvatarSelectionUIManager;
    public FullBodyAvatarManager m_FullBodyAvatarManager;
    public FullBodyAvatarImporter m_FullBodyAvatarImporter;
    public PlayerConfigurator m_PlayerConfigurator;
    public VRPlayerManager m_VRPlayerManager;

    [Space]
    [Header("Managers to find at runtime")]
    public MusicPlayerManager m_MusicPlayerManager;
    public SceneSelectionManager m_SceneSelectionManager;
    public MutePlayersManager m_MutePlayersManager;
    public SpawnManager m_SpawnManager;
    public SafetyRailManager m_SafetyRailManager;
    public PlayerSettingsManager m_PlayerSettingsManager;

    [Space]
    [Header("Scenes")]
    public Button ResetSceneButton;

    [Space]
    [Header("Music Player")]
    public Button PreviousTrackButton;
    public Button PlayMusicButton;
    public Button StopMusicButton;
    public Button VolumeControlButton;
    public Button NextTrackButton;
    public TextMeshProUGUI VolumControlButtonText;
    public TextMeshProUGUI SelectedTrackText;
    public TextMeshProUGUI ClockText;

    [Space]
    [Header("Voice")]
    public Button ToggleMutePerformersButton;
    public TextMeshProUGUI ToggleMutePerformersText;
    public Button ToggleMuteAudienceButton;
    public TextMeshProUGUI ToggleMuteAudienceText;
    public Button AudienceMegaphoneButton;
    public Button Audience3DButton;
    public Button AudienceSuperCloseButton;
    public Button PerformersMegaphoneButton;
    public Button Performers3DButton;
    public Button PerformersSuperCloseButton;
    public Button SwitchToOpenChannelButton;
    public Button SwitchToProductionChannelButton;

    [Space]
    [Header("Additional features")]
    public Button ToggleInvisibilityButton;
    public Button RespawnButton;
    public Button ToggleSafetyRailButton;
    public Button ActivateAudienceTeleportationButton;
    public Button DeactivateAudienceTeleportationButton;
    public Button ToggleInteractorTypeButton;
    public Button ToggleInvalidLineVisible;

    [Space]
    [Header("User interaction")]
    [SerializeField]
    InputActionReference ShowUserUIAction;
    [SerializeField]
    InputActionReference HideUserUIAction;


    private void OnEnable()
    {
        ShowUserUIAction.action.performed += OnShowPlayerUI;        
        HideUserUIAction.action.performed += OnHidePlayerUI;

        ShowUserUIAction.action.Enable();
        HideUserUIAction.action.Enable();
    }

    private void OnDisable()
    {
        ShowUserUIAction.action.performed -= OnShowPlayerUI;        
        HideUserUIAction.action.performed -= OnHidePlayerUI;

        ShowUserUIAction.action.Disable();
        HideUserUIAction.action.Disable();
    }

    void Start()
    {
        PerformerMenu_GameObject.SetActive(false);
        AudienceMenu_GameObject.SetActive(false);

        m_MusicPlayerManager = FindObjectOfType<MusicPlayerManager>();
        m_SceneSelectionManager = FindObjectOfType<SceneSelectionManager>();
        m_MutePlayersManager = FindObjectOfType<MutePlayersManager>();
        m_SpawnManager = FindObjectOfType<SpawnManager>();
        m_SafetyRailManager = FindObjectOfType<SafetyRailManager>();
        m_PlayerSettingsManager = FindObjectOfType<PlayerSettingsManager>();

        if (ToggleInvisibilityButton != null)
        {
            ToggleInvisibilityButton.onClick.AddListener(ToggleInvisibility);
        }

        m_SceneSelectionUIManager.SceneSelectedEvent.AddListener(ChangeScene);
        m_AvatarSelectionUIManager.AvatarSelectedEvent.AddListener(ChangeAvatar);

        if (m_SceneSelectionManager != null)
        {
            ResetSceneButton.onClick.AddListener(m_SceneSelectionManager.ResetScene);
        }

        if (m_MusicPlayerManager != null)
        {
            NextTrackButton.onClick.AddListener(m_MusicPlayerManager.SelectNextTrack);
            PreviousTrackButton.onClick.AddListener(m_MusicPlayerManager.SelectPreviousTrack);

            PlayMusicButton.onClick.AddListener(m_MusicPlayerManager.PlayMusicForAll);
            StopMusicButton.onClick.AddListener(m_MusicPlayerManager.StopMusicForAll);
            VolumeControlButton.onClick.AddListener(m_MusicPlayerManager.ToggleLowVolumeForAll);
        }

        if (m_MutePlayersManager != null)
        {
            ToggleMuteAudienceButton.onClick.AddListener(m_MutePlayersManager.ToggleAudienceMute);
            ToggleMutePerformersButton.onClick.AddListener(m_MutePlayersManager.TogglePerformersMute);

            AudienceMegaphoneButton.onClick.AddListener(delegate{m_MutePlayersManager.SetNetworkedAudienceVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_MEGAPHONE);});
            Audience3DButton.onClick.AddListener(delegate{m_MutePlayersManager.SetNetworkedAudienceVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_3D);});
            AudienceSuperCloseButton.onClick.AddListener(delegate{m_MutePlayersManager.SetNetworkedAudienceVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_SUPER_CLOSE);});

            PerformersMegaphoneButton.onClick.AddListener(delegate{m_MutePlayersManager.SetNetworkedPerformersVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_MEGAPHONE);});
            Performers3DButton.onClick.AddListener(delegate{m_MutePlayersManager.SetNetworkedPerformersVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_3D);});
            PerformersSuperCloseButton.onClick.AddListener(delegate{m_MutePlayersManager.SetNetworkedPerformersVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_SUPER_CLOSE);});
        }

        if (m_SafetyRailManager != null)
        {
            ToggleSafetyRailButton.onClick.AddListener(m_SafetyRailManager.ToggleSafetyRail);
        }

        if (m_PlayerSettingsManager != null)
        {
            ActivateAudienceTeleportationButton.onClick.AddListener(delegate { m_PlayerSettingsManager.SetNetworkedAudienceTeleportation(true); });
            DeactivateAudienceTeleportationButton.onClick.AddListener(delegate { m_PlayerSettingsManager.SetNetworkedAudienceTeleportation(false); });
        }

        RespawnButton.onClick.AddListener(RespawnPlayer);

        SwitchToOpenChannelButton.onClick.AddListener(delegate{m_PhotonVoiceManager.SwitchVoiceChannel("open");});
        SwitchToProductionChannelButton.onClick.AddListener(delegate{m_PhotonVoiceManager.SwitchVoiceChannel("production");});

        ToggleInteractorTypeButton.onClick.AddListener(m_VRPlayerManager.ToggleInteractorType);
        ToggleInvalidLineVisible.onClick.AddListener(m_VRPlayerManager.ToggleInvalidLineVisible);
    }

    public void ChangeScene(string sceneName)
    {
        m_SceneSelectionManager.ChangeSceneForAll(sceneName);
    }

    public void ChangeAvatar(string avatarName)
    {
        if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR || m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.Desktop)
        {
            m_PlayerNetworkManager.SelectSimpleAvatarByName(avatarName);
        } else if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            m_PlayerNetworkManager.SelectFullBodyAvatarByName(avatarName);
        }
    }

    public void RespawnPlayer()
    {
        if (m_SpawnManager)
        {
            m_SpawnManager.Respawn(m_PlayerConfigurator);
        }
    }

    public void SelectPreviousTrack()
    {
        m_MusicPlayerManager.SelectPreviousTrack();
    }

    public void ToggleInvisibility()
    {
        m_PlayerNetworkManager.ToggleHideAvatar();
    }

    private void Update()
    {
        if (m_MusicPlayerManager != null)
        {
            UpdateSelectedTrackNameText();
        }

        if (m_MutePlayersManager != null)
        {
            UpdateMuteButtonTexts();
        }

        UpdateClockText();
    }


    private void UpdateClockText()
    {
        ClockText.text = System.DateTime.Now.Hour.ToString() + " : " + System.DateTime.Now.Minute.ToString() + " : " + System.DateTime.Now.Second.ToString();
    }

    private void UpdateMuteButtonTexts()
    {
        SetAudienceMutedText(m_MutePlayersManager.AudienceMuted);    
        SetPerformersMutedText(m_MutePlayersManager.PerformersMuted);    
    }

    private void UpdateSelectedTrackNameText()
    {
        SelectedTrackText.text = m_MusicPlayerManager.SelectedTrackName;
    }

    private void SetAudienceMutedText(bool audienceMuted)
    {
        if (audienceMuted)
        {
            ToggleMuteAudienceText.text = "Unmute Audience";
        } else
        {
            ToggleMuteAudienceText.text = "Mute Audience";
        }
    }

    private void SetPerformersMutedText(bool audienceMuted)
    {
        if (audienceMuted)
        {
            ToggleMutePerformersText.text = "Unmute Performers";
        } else
        {
            ToggleMutePerformersText.text = "Mute Performers";
        }
    }

    private void OnShowPlayerUI(InputAction.CallbackContext _)
    {
        Debug.Log("Show Player UI Pressed");
        if (m_PlayerConfigurator.PlayerRole == PlayerConfigurator.PlayerRoleEnum.Performer)
        {
            PerformerMenu_GameObject.SetActive(true);
            UseUIInteractors();
        } 
        
        if (VRRSettingsAccessor.Instance.Settings.EnableAudienceMenu)
        {
            if (m_PlayerConfigurator.PlayerRole == PlayerConfigurator.PlayerRoleEnum.Audience)
            {
                AudienceMenu_GameObject.SetActive(true);
                UseUIInteractors();
            }
        }    
    }

    private void UseUIInteractors()
    {
        m_VRPlayerManager.UseLeftHandDirectController(true);
        m_VRPlayerManager.UseRightHandDirectController(false);
    }

    private void OnHidePlayerUI(InputAction.CallbackContext _)
    {
        Debug.Log("Hide Player UI Pressed");
        m_VRPlayerManager.ConfigureControllerInteractors();
        if (m_PlayerConfigurator.PlayerRole == PlayerConfigurator.PlayerRoleEnum.Performer)
        {
            PerformerMenu_GameObject.SetActive(false);
        } 

        if (m_PlayerConfigurator.PlayerRole == PlayerConfigurator.PlayerRoleEnum.Audience)
        {
            AudienceMenu_GameObject.SetActive(false);
        }
    }

}
