using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject ConfigurablePlayerPrefab;

    public List<Transform> AudienceSpawnPoints;
    public List<Transform> PerformerSpawnPoints;

    public UnityEvent OnPlayerSpawned;

    public bool IsAudienceMember;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (OnPlayerSpawned == null)
        {
            OnPlayerSpawned = new UnityEvent();
        }
    }

    public void Respawn(PlayerConfigurator playerConfigurator)
    {
        Transform spawnPoint;
        if (IsAudienceMember)
        {
            spawnPoint= AudienceSpawnPoints[Random.Range(0, AudienceSpawnPoints.Count)]; 
        } else
        {

            spawnPoint= PerformerSpawnPoints[Random.Range(0, PerformerSpawnPoints.Count)]; 
        }

        playerConfigurator.VRPlayer.transform.position = spawnPoint.position;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    public void SpawnAIPlayer()
    {
        //Transform spawnPoint = GetSpawnPoint();
        Transform spawnPoint= AudienceSpawnPoints[Random.Range(0, AudienceSpawnPoints.Count)]; 
        if (PhotonNetwork.IsConnectedAndReady)
        {
            GameObject player = PhotonNetwork.Instantiate(ConfigurablePlayerPrefab.name, spawnPoint.position, spawnPoint.rotation);
            PlayerConfigurator playerConfigurator = player.GetComponent<PlayerConfigurator>();
            playerConfigurator.NetworkGameObject.GetComponent<PlayerNetworkManager>().IsNonPrimaryAIPlayer = true;

            //int numberOfAvatarModels = playerConfigurator.m_SimpleAvatarManger.AvatarModelPrefabs.Length;
            //playerConfigurator.SimpleAvatarSelectionNumber = Random.Range(0, numberOfAvatarModels);
        }
    }

    Transform GetSpawnPoint()
    {
        Transform spawnPoint;
        if (IsAudienceMember)
        {
             spawnPoint= AudienceSpawnPoints[Random.Range(0, AudienceSpawnPoints.Count)]; 
        } else
        {
             spawnPoint= PerformerSpawnPoints[Random.Range(0, PerformerSpawnPoints.Count)]; 
        }

        return spawnPoint;
    }


    private void Start()
    {
        if(photonView.Owner == null)
        {
            return;
        }

        object audienceMember;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_AUDIENCE_MEMBER_KEY, out audienceMember))
        {
            IsAudienceMember = (bool)audienceMember;
        }
        else
        {
            Debug.LogWarning("Could not get is audience member setting for player!");
        }


        Transform spawnPoint = GetSpawnPoint();

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(ConfigurablePlayerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }

        OnPlayerSpawned.Invoke();
    }

}
