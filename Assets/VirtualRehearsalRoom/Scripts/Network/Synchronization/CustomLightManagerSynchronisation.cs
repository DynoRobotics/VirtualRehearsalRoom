using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CustomLightManagerSynchronisation : MonoBehaviour, IPunObservable
{
    private PhotonView m_PhotonView;
    public CustomLightManager _customLightManager;



    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public void Update()
    {
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_customLightManager.lightTime);
        }
        else
        {
            _customLightManager.lightTime = (float)stream.ReceiveNext();
    
        }
    }

}
