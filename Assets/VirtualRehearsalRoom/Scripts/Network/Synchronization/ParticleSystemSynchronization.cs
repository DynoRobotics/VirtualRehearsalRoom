using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ParticleSystemSynchronization : MonoBehaviour, IPunObservable
{
    public ParticleSystem m_ParticleSystem;
    public void Start()
    {
        if (m_ParticleSystem == null)
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            if (m_ParticleSystem == null)
            {
                Debug.LogError("ParticleSystem not assigned");
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (m_ParticleSystem == null) return;

        var emission = m_ParticleSystem.emission;
        if (stream.IsWriting)
        {
            stream.SendNext(emission.enabled);
        }
        else
        {
            emission.enabled = (bool)stream.ReceiveNext();
        }
    }
}
