using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MaterialSynchronization : MonoBehaviour, IPunObservable
{
    public Material m_Material;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (m_Material == null)
            return;

        if (stream.IsWriting)
        {
            Color color = m_Material.color;
            stream.SendNext(color.r);
            stream.SendNext(color.g);
            stream.SendNext(color.b);
            stream.SendNext(color.a);

            Color emissionColor = m_Material.GetColor("_EmissionColor");
            stream.SendNext(emissionColor.r);
            stream.SendNext(emissionColor.g);
            stream.SendNext(emissionColor.b);
            stream.SendNext(emissionColor.a);

        } else
        {
            Color color;
            color.r = (float)stream.ReceiveNext();
            color.g = (float)stream.ReceiveNext();
            color.b = (float)stream.ReceiveNext();
            color.a = (float)stream.ReceiveNext();
            m_Material.color = color;

            Color emissionColor;
            emissionColor.r = (float)stream.ReceiveNext();
            emissionColor.g = (float)stream.ReceiveNext();
            emissionColor.b = (float)stream.ReceiveNext();
            emissionColor.a = (float)stream.ReceiveNext();
            m_Material.SetColor("_EmissionColor", emissionColor);

        }
    }
}
