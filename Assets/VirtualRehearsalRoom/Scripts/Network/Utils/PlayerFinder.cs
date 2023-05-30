using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerFinder : MonoBehaviour
{
    public PlayerConfigurator[] PlayerConfigurators;

    public PlayerConfigurator LocalPlayerConfigurator;
    public bool LocalPlayerFound = false;

    private float nextUpdate = 1.0f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Time.time >= nextUpdate)
        {
            FindPlayers();
            nextUpdate = Mathf.FloorToInt(Time.time) + 1.0f;
        }

        if (!LocalPlayerFound)
        {
            FindLocalPlayer();
        }
    }
    
    public PlayerConfigurator GetPlayerByActorNumber(int actorNumber)
    {
        foreach (PlayerConfigurator playerConfigurator in PlayerConfigurators)
        {
            PhotonView playerView = playerConfigurator.NetworkGameObject.GetComponent<PhotonView>();
            if (playerView.Owner.ActorNumber == actorNumber)
            {
                return playerConfigurator;
            }
        }
        return null; 
    }

    void FindPlayers()
    {
        //Debug.Log("Finding players");
        PlayerConfigurators = (PlayerConfigurator[])GameObject.FindObjectsOfType(typeof(PlayerConfigurator));
    }

    private void FindLocalPlayer()
    {
        foreach (PlayerConfigurator playerConfigurator in PlayerConfigurators)
        {
            if (!playerConfigurator.PlayerIsRemote)
            {
                LocalPlayerConfigurator = playerConfigurator;
                LocalPlayerFound = true;
            }
        }
    }
}
