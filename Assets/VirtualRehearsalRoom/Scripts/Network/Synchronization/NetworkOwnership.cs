using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkOwnership : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{

    public void RequestOwnership()
    {
        photonView.RequestOwnership();
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != photonView)
        {
            return;
        }

        Debug.Log("Light ownership requested for: " + targetView.name + " from " + requestingPlayer.NickName);
        photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("Transfer is complete. New owner: " + targetView.Owner.NickName);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        Debug.Log("Ownership transfer failed. Requesting owner: " + senderOfFailedRequest.NickName);
    }

}
