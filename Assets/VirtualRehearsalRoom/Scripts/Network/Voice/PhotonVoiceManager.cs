using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Voice.Unity;
using Photon.Voice.PUN;
using TMPro;

public class PhotonVoiceManager : MonoBehaviour
{
    public PlayerConfigurator m_PlayerConfigurator;
    public PlayerNetworkManager m_PlayerNetworkManager;

    public GameObject m_SimpleAvatarHead;
    public GameObject m_FullBodyAvatarHead;

    public GameObject m_SpeakerGameObject;

    public PlayerConfigurator.PlayerTypeEnum m_PlayerType;

    public Recorder m_PhotonVoiceRecorder;
    public PhotonVoiceNetwork m_PhotonVoiceNetwork;

    public TextMeshProUGUI m_MutedText;

    public List<string> m_Channels = new List<string> { "open", "production" };

    private bool m_isUnmuted = true;

    private bool _foundPhotonVoice = false;

    private void Awake()
    {
        m_PhotonVoiceRecorder = FindObjectOfType<Recorder>();
        m_PhotonVoiceNetwork = FindObjectOfType<PhotonVoiceNetwork>();

        m_MutedText.enabled = false;

        _foundPhotonVoice = (m_PhotonVoiceRecorder != null) & (m_PhotonVoiceNetwork != null);
    }

    private void OnGUI()
    {
        if (VRRSettingsAccessor.Instance.Settings.ShowDesktopOverlayUI)
        {
            if (m_PlayerNetworkManager.photonView.IsMine && m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.Desktop)
            {
                GUI.Label(new Rect(10, 10, 200, 50), "Switch Voice Channel");
                if (GUI.Button(new Rect(10, 40, 50, 30), "open"))
                    SwitchVoiceChannel("open");
                if (GUI.Button(new Rect(60, 40, 100, 30), "production"))
                    SwitchVoiceChannel("production");
            }
        }
    }

    public void UsePerformerVoiceChannels()
    {
        StartCoroutine(UseAllVoiceChannels());
    }

    private IEnumerator UseAllVoiceChannels()
    {
        // Wait for voice client to join
        do
        {
            yield return new WaitForEndOfFrame();
        } while (m_PhotonVoiceNetwork.ClientState != Photon.Realtime.ClientState.Joined);

        Debug.Log("Listening to all available voice channels!");

        byte[] allChannels = new byte[m_Channels.Capacity];
        for (int i = 0; i < m_Channels.Capacity; i++)
        {
            allChannels[i] = (byte)i;
        }
        m_PhotonVoiceNetwork.Client.OpChangeGroups(new byte[0], allChannels);

        yield return null;
    }

    public void SwitchVoiceChannel(string channelName)
    {
        int channel = m_Channels.IndexOf(channelName);

        Debug.Log("Switching voice channel to: " + channelName + " [" + channel + "]");

        m_PhotonVoiceRecorder.InterestGroup = (byte)channel;
    }

    void Update()
    {
        if (!_foundPhotonVoice)
            return;

        m_PlayerType = m_PlayerConfigurator.PlayerType;

        if (m_PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            m_SpeakerGameObject.transform.position = m_FullBodyAvatarHead.transform.position;
            m_SpeakerGameObject.transform.rotation = m_FullBodyAvatarHead.transform.rotation;
        } else
        {
            m_SpeakerGameObject.transform.position = m_SimpleAvatarHead.transform.position;
            m_SpeakerGameObject.transform.rotation = m_SimpleAvatarHead.transform.rotation;
        }
        if (m_PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR || m_PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            if (m_isUnmuted != m_PhotonVoiceRecorder.TransmitEnabled)
            {
                m_isUnmuted = m_PhotonVoiceRecorder.TransmitEnabled;
               if (m_isUnmuted)
                {
                    StartCoroutine(DisplayMuteText("You are now unmuted"));
                } else
                {
                    StartCoroutine(DisplayMuteText("You are now muted"));
                }
            }
        }
    }

    private IEnumerator DisplayMuteText(string textToDisplay)
    {
        m_MutedText.text = textToDisplay;
        m_MutedText.enabled = true;

        yield return new WaitForSeconds(3.0f);

        m_MutedText.enabled = false;
        yield return null;
    }

    public void SetVoiceType(string voiceType)
    {
        AudioSource playerVoiceSource = m_SpeakerGameObject.GetComponent<AudioSource>();

        switch (voiceType)
        {
            case MultiplayerVRConstants.VOICE_TYPE_VALUE_MEGAPHONE:
                SetVoiceTypeMegaphone();
                break;
            case MultiplayerVRConstants.VOICE_TYPE_VALUE_3D:
                SetVoiceType3D();
                break;
            case MultiplayerVRConstants.VOICE_TYPE_VALUE_SUPER_CLOSE:
                SetVoiceTypeSuperClose();
                break;
            default:
                Debug.LogWarning("Unknown voice type: " + voiceType);
                break;
        }
    } 

    void SetVoiceTypeMegaphone()
    {
        Debug.Log("Setting voice type to megaphone");
        AudioSource playerVoiceSource = m_SpeakerGameObject.GetComponent<AudioSource>();

        playerVoiceSource.spatialBlend = 0.0f;
    }
    void SetVoiceType3D()
    {
        Debug.Log("Setting voice type to 3D");
        AudioSource playerVoiceSource = m_SpeakerGameObject.GetComponent<AudioSource>();

        playerVoiceSource.spatialBlend = 1.0f;
        playerVoiceSource.rolloffMode = AudioRolloffMode.Linear;
        playerVoiceSource.minDistance = 0.0f;
        playerVoiceSource.maxDistance = 30.0f;
    }
    void SetVoiceTypeSuperClose()
    {
        Debug.Log("Setting voice type to Super Close");
        AudioSource playerVoiceSource = m_SpeakerGameObject.GetComponent<AudioSource>();

        playerVoiceSource.spatialBlend = 1.0f;
        playerVoiceSource.rolloffMode = AudioRolloffMode.Logarithmic;
        playerVoiceSource.minDistance = 0.0f;
        playerVoiceSource.maxDistance = 12.0f;
    }


}
