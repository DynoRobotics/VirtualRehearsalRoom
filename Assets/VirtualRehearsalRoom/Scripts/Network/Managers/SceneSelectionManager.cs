using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneSelectionManager : MonoBehaviourPunCallbacks
{
    public void ResetScene()
    {
        photonView.RPC("ResetSceneForAllPlayers", RpcTarget.AllBuffered);
    }

    public void ChangeSceneForAll(string sceneName)
    {
        photonView.RPC("ChangeSceneForAllPlayers", RpcTarget.AllBuffered, sceneName);
    }

    [PunRPC]
    public void ChangeSceneForAllPlayers(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient) {
            VirtualWorldManager.Instance.ChangeSceneForAllPlayers(sceneName);
        }
    }

    [PunRPC]
    public void ResetSceneForAllPlayers()
    {
        if (PhotonNetwork.IsMasterClient) {
            VirtualWorldManager.Instance.ResetSceneForAllPlayers();
        }
    }
}
