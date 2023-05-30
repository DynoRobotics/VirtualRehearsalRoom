using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;

public class MutePlayersManager : MonoBehaviourPunCallbacks
{
    public PlayerFinder m_PlayerFinder;

    public Recorder m_PhotonVoiceRecorder;

    public bool PerformersMuted;
    public bool AudienceMuted;

    IEnumerator Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            yield return new WaitUntil(() => m_PlayerFinder.LocalPlayerFound);
            SetPerformersMuted(GetNetworkedPerformersMuted());
            SetAudienceMuted(GetNetworkedAudienceMuted());
        }
    }


    private void SetAudienceMuted(bool value)
    {
        AudienceMuted = value;
        SetLocalAudienceTransmitEnabled(!AudienceMuted);
    }

    private void SetPerformersMuted(bool value)
    {
        PerformersMuted = value;
        SetLocalPerformerTransmitEnabled(!PerformersMuted);
    }

    public void TogglePerformersMute()
    {
        SetNetworkedPerformersMutedRoomProperty(!PerformersMuted);
    }

    public void ToggleAudienceMute()
    {
        SetNetworkedAudienceMutedRoomProperty(!AudienceMuted);
    }

    private bool GetNetworkedAudienceMuted()
    {
        object audienceMuted;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.AUDIENCE_MUTED_KEY, out audienceMuted))
        {
            Debug.Log("Got audience muted: " + ((bool)audienceMuted).ToString());
        }
        else
        {
            Debug.Log("Audience muted not set, setting to false");
            SetNetworkedAudienceMutedRoomProperty(false);
            return false;
        }

        return (bool)audienceMuted;
    }

    void SetNetworkedAudienceMutedRoomProperty(bool value)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.AUDIENCE_MUTED_KEY, value} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    public bool GetNetworkedPerformersMuted()
    {
        object performersMuted;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.PERFORMERS_MUTED_KEY, out performersMuted))
        {
            Debug.Log("Got performers muted: " + ((bool)performersMuted).ToString());
        }
        else
        {
            Debug.Log("Performers muted not set, setting to false");
            SetNetworkedPerformersMutedRoomProperty(false);
            return false;
        }

        return (bool)performersMuted;
    }

    void SetNetworkedPerformersMutedRoomProperty(bool value)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.PERFORMERS_MUTED_KEY, value} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
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

    public void SetLocalPerformerTransmitEnabled(bool value)
    {
        if (m_PlayerFinder.LocalPlayerFound)
        {
            bool isAudienceMember = m_PlayerFinder.LocalPlayerConfigurator.NetworkGameObject.GetComponent<PlayerNetworkManager>().IsAudienceMember;
            if (!isAudienceMember)
            {
                m_PhotonVoiceRecorder.TransmitEnabled = value;
            }
        } else
        {
            Debug.LogWarning("Local player not found when trying to set transmit enabled for performer");
        }
    }
    public void SetLocalAudienceTransmitEnabled(bool value)
    {
        if (m_PlayerFinder.LocalPlayerFound)
        {
            bool isAudienceMember = m_PlayerFinder.LocalPlayerConfigurator.NetworkGameObject.GetComponent<PlayerNetworkManager>().IsAudienceMember;
            if (isAudienceMember)
            {
                m_PhotonVoiceRecorder.TransmitEnabled = value;
            }
        } else
        {
            Debug.LogWarning("Local player not found when trying to set transmit enabled for audience");
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        object audienceMuted;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.AUDIENCE_MUTED_KEY, out audienceMuted))
        {
            SetAudienceMuted((bool)audienceMuted);
        }

        object performersMuted;
        if (propertiesThatChanged.TryGetValue(MultiplayerVRConstants.PERFORMERS_MUTED_KEY, out performersMuted))
        {
            SetPerformersMuted((bool)performersMuted);
        }

    }
}
