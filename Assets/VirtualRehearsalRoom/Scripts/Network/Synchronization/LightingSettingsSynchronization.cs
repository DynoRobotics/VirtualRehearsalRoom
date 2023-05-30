using UnityEngine;
using Photon.Pun;

public class LightingSettingsSynchronization : MonoBehaviour, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Color color = RenderSettings.ambientLight;
            stream.SendNext(color.r);
            stream.SendNext(color.g);
            stream.SendNext(color.b);
            stream.SendNext(color.a);
        }
        else
        {
            Color color;
            color.r = (float)stream.ReceiveNext();
            color.g = (float)stream.ReceiveNext();
            color.b = (float)stream.ReceiveNext();
            color.a = (float)stream.ReceiveNext();
            RenderSettings.ambientLight = color;
        }
    }
}
