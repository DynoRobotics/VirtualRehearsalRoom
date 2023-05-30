using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MusicPlayerManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public enum PlaybackModeEnum 
    {
        LoopTrack,
        LoopPlaylist,
        StopAfterTrack,
        StopAfterPlaylist
    }

    public PlaybackModeEnum PlaybackMode;

    public List<AudioSource> MusicAudioSources;
    public List<float> MusicAudioSourceDefaultVolumes;

    public int SelectedTrack = 0;

    public MusicTracksManager m_MusicTracksManager;


    [Range(0.0f, 1.0f)]
    public float LocalVolume = 1.0f;
    [Range(0.0f, 1.0f)]
    public float NetworkVolume = 1.0f;
    public float SoundIsolationZoneVolume = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Volume = 1.0f;

    [Range(0.0f, 1.0f)]
    public float DefaultVolume = 1.0f;
    [Range(0.0f, 1.0f)]
    public float LowVolumePreset = 0.5f;
    [Range(0.0f, 1.0f)]
    public float FullVolumePreset = 1.0f;

    public bool AutoresumeMusicInNewScene = false;

    private void Awake()
    {
        SelectedTrack = 0;
    }

    public string SelectedTrackName
    {
        get
        {
            if (SelectedTrack < m_MusicTracksManager.MusicTracks.Count)
            {
                return m_MusicTracksManager.MusicTracks[SelectedTrack].name;
            } else
            {
                return "";
            }
        }
    }

    void Start()
    {
        foreach (AudioSource MusicAudioSource in MusicAudioSources)
        {
            MusicAudioSourceDefaultVolumes.Add(MusicAudioSource.volume);            
        }

        if (PhotonNetwork.IsConnected)
        {
            SetNetworkVolume(GetNetworkedVolume());
            UpdateSelectedTrackFromNetwork();

            if(AutoresumeMusicInNewScene && IsMusicPlaying())
            {
                PlayMusicFromNetworkTime();
            }
        }
    }

    private void Update()
    {
        Volume = Mathf.Clamp(NetworkVolume * LocalVolume * SoundIsolationZoneVolume, 0.0f, 1.0f);
        for (int i = 0; i < MusicAudioSources.Count; i++)
        {
            MusicAudioSources[i].volume = Volume;
        }
    }

    public void IncreaseLocalVolume()
    {
        LocalVolume = System.Math.Clamp(LocalVolume + 0.1f, 0.0f, 1.0f);
    }
    public void DecreaseLocalVolume()
    {
        LocalVolume = System.Math.Clamp(LocalVolume - 0.1f, 0.0f, 1.0f);
    }

    public void ToggleLowVolumeForAll()
    {
        if (NetworkVolume < FullVolumePreset)
        {
            SetFullVolumeForAll();
        } else
        {
            SetLowVolumeForAll();
        }

    }

    public void SetLowVolumeForAll()
    {
        SetNetworkedVolume(LowVolumePreset);
    }

    public void SetFullVolumeForAll()
    {
        SetNetworkedVolume(FullVolumePreset);
    }

    void SetNetworkVolume(float value)
    {
        NetworkVolume = value;
    }

    private float GetNetworkedVolume()
    {
        object volume;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MUSIC_VOLUME_KEY, out volume))
        {
            Debug.Log("Got music volume: " + ((float)volume).ToString());
        }
        else
        {
            SetNetworkedMusicPlayingStartTime(DefaultVolume);
            return DefaultVolume;
        }

        return (float)volume;
    }
    public void SetNetworkedVolume(float value)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.MUSIC_VOLUME_KEY, value} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    private int GetNetworkedSelectedTrack()
    {
        object selectedTrack;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MUSIC_SELECTED_TRACK_KEY, out selectedTrack))
        {
            Debug.Log("Got selected track: " + ((int)selectedTrack).ToString());
        }
        else
        {
            Debug.Log("No selected track set, setting from local value");
            SetNetworkedSelectedTrack(SelectedTrack);
            return SelectedTrack;
        }
        
        if ((int)selectedTrack >= m_MusicTracksManager.MusicTracks.Count)
        {
            Debug.LogWarning("Got network track index out of range, setting to 0");
            SetNetworkedSelectedTrack(0);
            return 0;

        }
        return (int)selectedTrack;
    }

    private void SetNetworkedSelectedTrack(int value)
    {
        SelectedTrack = value;

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.MUSIC_SELECTED_TRACK_KEY, value} };

        Debug.Log("Setting selected track: " + value.ToString() + ", with name: " + SelectedTrackName);
        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    public void SelectTrackByName(string trackName)
    {
        bool trackFound = false;
        int foundTrackIndex = 0;
        for (int i = 0; i < m_MusicTracksManager.MusicTracks.Count; i++)
        {
            if (m_MusicTracksManager.MusicTracks[i].name == trackName)
            {
                trackFound = true;
                foundTrackIndex = i;
                break;
            }
        }

        if (trackFound)
        {
            SetNetworkedSelectedTrack(foundTrackIndex);
        } else
        {
            Debug.LogWarning("Could not select track with name: " + trackName);
        }
    }

    public void SelectNextTrack()
    {
        int nextIndex = GetNetworkedSelectedTrack() + 1;
        
        if (nextIndex == m_MusicTracksManager.MusicTracks.Count)
        {
            SetNetworkedSelectedTrack(0);
        } 
        else if (nextIndex > m_MusicTracksManager.MusicTracks.Count)
        {
            Debug.LogWarning("Track index out of range (to large), should never happen");
        }
        else
        {
            SetNetworkedSelectedTrack(nextIndex);
        }
    }

    public void SelectPreviousTrack()
    {
        Debug.Log("Selecting previous track");

        int nextIndex = GetNetworkedSelectedTrack() - 1;

        if (nextIndex == -1)
        {
            SetNetworkedSelectedTrack(m_MusicTracksManager.MusicTracks.Count - 1);
        }
        else if (nextIndex < -1)
        {
            Debug.LogWarning("Track index out of range (to small), should never happen");
        } else
        {
            SetNetworkedSelectedTrack(nextIndex);
        }
    }

    bool IsMusicPlaying()
    {
        object musicPlaying;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MUSIC_PLAYING_KEY, out musicPlaying))
        {
            Debug.Log("Got music playing: " + ((bool)musicPlaying).ToString());
        }
        else
        {
            Debug.Log("Music playing not set, setting to false!");
            SetMusicIsPlaying(false);
            return false;
        }

        return (bool)musicPlaying;
    }

    void SetMusicIsPlaying(bool value)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.MUSIC_PLAYING_KEY, value} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    public void PlayMusicForAll()
    {
        photonView.RPC("PlayMusic", RpcTarget.All);
        SetMusicIsPlaying(true);
    }

    public void StopMusicForAll()
    {
        photonView.RPC("StopMusic", RpcTarget.All);
        SetMusicIsPlaying(false);
        SetNetworkedMusicPlayingStartTime(PhotonNetwork.Time);
    }

    private void SetNetworkedMusicPlayingStartTime(double value)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.MUSIC_PLAYING_START_TIME_KEY, value} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    private double GetNetworkedMusicPlayingStartTime()
    {
        object musicPlayingStartTime;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MUSIC_PLAYING_START_TIME_KEY, out musicPlayingStartTime))
        {
            Debug.Log("Got music playing start time: " + ((double)musicPlayingStartTime).ToString());
        }
        else
        {
            Debug.Log("Music playing start time not set, setting to current time!");
            SetNetworkedMusicPlayingStartTime(PhotonNetwork.Time);
            return PhotonNetwork.Time;
        }

        return (double)musicPlayingStartTime;
    }

    private void PlayMusicFromNetworkTime()
    {
        UpdateSelectedTrackFromNetwork();

        double musicStartingTime = GetNetworkedMusicPlayingStartTime();

        float musicOffsetTime = (float)(PhotonNetwork.Time - musicStartingTime);

        // TODO(sam): more checks for if time is reasonable

        foreach (AudioSource MusicAudioSource in MusicAudioSources)
        {
            if (musicOffsetTime < MusicAudioSource.clip.length)
            {
                Debug.Log("Playing music from time: " + musicOffsetTime);
                //MusicAudioSource.PlayDelayed(musicOffsetTime);
                MusicAudioSource.time = musicOffsetTime;
                MusicAudioSource.Play(0);
            } else
            {
                Debug.Log("Music already finished, not playing");
            }
        }

    }

    private void SetSelectedAudioClip()
    {
        foreach (AudioSource MusicAudioSource in MusicAudioSources)
        {
            if (m_MusicTracksManager.MusicTracks.Count > SelectedTrack)
            {
                MusicAudioSource.clip = m_MusicTracksManager.MusicTracks[SelectedTrack];
            }
        }
    }

    private void UpdateSelectedTrackFromNetwork()
    {
        SelectedTrack = GetNetworkedSelectedTrack();
        SetSelectedAudioClip();
    }

    private void ConfigurePlaybackMode()
    {

    }

    [PunRPC]
    public void PlayMusic()
    {
        Debug.Log("Playing Music");

        foreach (AudioSource MusicAudioSource in MusicAudioSources)
        {
            MusicAudioSource.Stop();
        }

        UpdateSelectedTrackFromNetwork();
        SetNetworkedMusicPlayingStartTime(PhotonNetwork.Time);
        ConfigurePlaybackMode();

        foreach (AudioSource MusicAudioSource in MusicAudioSources)
        {
            MusicAudioSource.Play(0);
        }
    }

    [PunRPC]
    public void StopMusic()
    {
        Debug.Log("Stopping Music");
        foreach (AudioSource MusicAudioSource in MusicAudioSources)
        {
            MusicAudioSource.Stop();
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        object selectedTrack;
        if(propertiesThatChanged.TryGetValue(MultiplayerVRConstants.MUSIC_SELECTED_TRACK_KEY, out selectedTrack))
        {
            SelectedTrack = (int)selectedTrack;
        }

        object volume;
        if (propertiesThatChanged.TryGetValue(MultiplayerVRConstants.MUSIC_VOLUME_KEY, out volume))
        {
            SetNetworkVolume((float)volume);
        }
    }
}
