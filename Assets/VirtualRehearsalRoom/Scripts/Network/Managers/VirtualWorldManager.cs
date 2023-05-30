using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class VirtualWorldManager : MonoBehaviourPunCallbacks
{
    public static VirtualWorldManager Instance;

    public bool isInResettingScene = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (isInResettingScene && PhotonNetwork.IsMasterClient)
        {
            LoadCurrentMapType();
        }
    }

    private void LoadCurrentMapType()
    {
        object currentSceneName;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.SCENE_NAME_KEY, out currentSceneName))
        {
            Debug.Log("Loading current scene");
            ChangeScene((string)currentSceneName);
        } else
        {
            Debug.LogWarning("Could not find current map type");
        }
    }

    public void ChangeSceneForAllPlayers(string newSceneName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object currentSceneName;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.SCENE_NAME_KEY, out currentSceneName))
            {
                if((string)currentSceneName != newSceneName)
                {
                    SetSceneNameInCurrentRoomProperties(newSceneName);
                    ChangeScene(newSceneName);
                }
                else
                {
                    Debug.LogWarning("Map type is already " + newSceneName + ", not doing anything");
                }
            }
        }
        else
        {
            Debug.LogWarning("Only the master player can change scene!");
        }
    }

    public void ChangeScene(string newSceneName)
    {
        PhotonNetwork.LoadLevel(newSceneName);
    }

    public void ResetSceneForAllPlayers()
    {
        ResetScene();
    }

    public void ResetScene()
    {
        Debug.Log("Resetting scene");
        //PhotonNetwork.LoadLevel("ResettingScene");
        SceneManager.LoadSceneAsync("ResettingScene", LoadSceneMode.Single);
    }

    private void SetSceneNameInCurrentRoomProperties(string sceneName)
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { 
            {MultiplayerVRConstants.SCENE_NAME_KEY, sceneName } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }


    #region Photon Callback Methods
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to: " + PhotonNetwork.CurrentRoom.Name + " Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Login");
        Debug.LogWarning("Disconnected from Photon: " + cause.ToString());
    }
    #endregion

}
