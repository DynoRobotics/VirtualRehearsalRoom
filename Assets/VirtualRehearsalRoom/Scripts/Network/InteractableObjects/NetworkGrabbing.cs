using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrabbing : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    public bool DisableActivateEventsForAudience = true;
    public bool ToggleGravityWhenActivated = true;

    public bool IsBeingHeld = false;
    public bool ActivateGravityWhenNotInHand = false;

    Rigidbody m_RigidBody;
    PlayerFinder m_PlayerFinder;
    XRBaseInteractable m_XRGrabInteractable;

    Transform _originalParent;


    void Start()
    {
        _originalParent = transform.parent;
        m_RigidBody = GetComponent<Rigidbody>();
        m_PlayerFinder = FindObjectOfType<PlayerFinder>();
        m_XRGrabInteractable = GetComponent<XRGrabInteractable>();

        // NOTE(sam): Read the initial value from rigidbody for now
        ActivateGravityWhenNotInHand = m_RigidBody.useGravity;
    }

    void Update()
    {
        if (IsBeingHeld)
        {
            gameObject.layer = 13; // Change layer to InHand
        } else
        {
            gameObject.layer = 8; // Change layer to Interactable
            m_RigidBody.useGravity = ActivateGravityWhenNotInHand;
        }
    }

    public void OnActivate()
    {
        if (m_PlayerFinder == null)
        {
            Debug.LogWarning("No player finder found in scene when activating object");
            return;
        }
        if (!m_PlayerFinder.LocalPlayerFound)
        {
            Debug.LogWarning("No local player found when activating object");
            return;
        }

        if (DisableActivateEventsForAudience)
        {
            if (m_PlayerFinder.LocalPlayerConfigurator.PlayerRole == PlayerConfigurator.PlayerRoleEnum.Audience)
                return;
        }

        if (ToggleGravityWhenActivated)
        {
            ToggleGravityForAll();
        }
    }

    private void ToggleGravityForAll()
    {
        photonView.RPC("SetGravity", RpcTarget.All, !ActivateGravityWhenNotInHand);
    }
    
    public void SetGravityForAll(bool value)
    {
        photonView.RPC("SetGravity", RpcTarget.All, value);
    }

    void TransferOwnership()
    {
        photonView.RequestOwnership();    
    }

    public void OnSelectEnter()
    {
        Debug.Log("Grabbed");

        photonView.RPC("StartNetworkedGrabbing", RpcTarget.All);
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Debug.Log("We do not request ownership because it is already mine.");
        } else
        {
            TransferOwnership(); 
        }
    }

    public void OnSelectExit()
    {
        Debug.Log("Released");
        photonView.RPC("StopNetworkedGrabbing", RpcTarget.All);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != photonView)
        {
            return;
        }

        Debug.Log("Ownership requested for: " + targetView.name + " from " + requestingPlayer.NickName);
        photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("Transfer is complete. New owner: " + targetView.Owner.NickName);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }

    [PunRPC]
    public void StartNetworkedGrabbing()
    {
        IsBeingHeld = true;
        m_RigidBody.isKinematic = true;
    }

    [PunRPC]
    public void StopNetworkedGrabbing()
    {
        IsBeingHeld = false;
        m_RigidBody.isKinematic = false;
        //transform.parent = _originalParent;
    }

    [PunRPC]
    public void SetGravity(bool value)
    {
        ActivateGravityWhenNotInHand = value;
    }

    [PunRPC]
    public void ReparentToRightHand(int actorNumber)
    {
        PlayerConfigurator playerConfigurator = m_PlayerFinder.GetPlayerByActorNumber(actorNumber);
        if (playerConfigurator != null)
        {
            transform.parent = playerConfigurator.m_SimpleAvatarManger.SimpleAvatarGameObject.GetComponent<AvatarInputConverter>().AvatarHand_Right;
        }
    }

    [PunRPC]
    public void ReparentToLeftHand(int actorNumber)
    {

    }
}
