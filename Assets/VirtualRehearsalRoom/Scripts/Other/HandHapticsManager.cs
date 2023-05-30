using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class HandHapticsManager : MonoBehaviour
{
    public XRBaseController m_Controller;
    public PhotonView m_PlayerPhotonView;
    public float m_VibrationAmplitude = 0.8f;
    public float m_VibrationDuration = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (m_PlayerPhotonView.IsMine)
        {
            m_Controller.SendHapticImpulse(m_VibrationAmplitude, m_VibrationDuration);
        }
    }

}
