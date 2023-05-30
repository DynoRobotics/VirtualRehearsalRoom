using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class NetworkEnableObjects : MonoBehaviourPunCallbacks
{

    public GameObject[] gameObjects;


    public void EnableObjectsForAll()
    {
        photonView.RPC("EnableObjects", RpcTarget.All);
    }

    public void DisableObjectsForAll()
    {
        photonView.RPC("DisableObjects", RpcTarget.All);
    }

    [PunRPC]
    public void EnableObjects()
    {
       foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(true);
        }
       
    }

    [PunRPC]
    public void DisableObjects()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }

    }

}
