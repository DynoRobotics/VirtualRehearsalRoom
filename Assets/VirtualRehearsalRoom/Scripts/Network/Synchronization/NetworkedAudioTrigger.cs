using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkedAudioTrigger : MonoBehaviourPunCallbacks
{
    public AudioSource AudioSourceToTrigger;
    public void TriggerSoundForAll()
    {
        photonView.RPC("TriggerSound", RpcTarget.All);
    }

    [PunRPC]
    public void TriggerSound()
    {
        Debug.Log("Sound triggered!");
        AudioSourceToTrigger.Play();
    }
}
