using UnityEngine;
using Photon.Pun;

public class NetworkedAudioVolume : MonoBehaviour, IPunObservable
{
    public AudioSource m_AudioSource;
    public float m_FadeOutSpeed = 0.1f;

    private bool _fadingOut = false;

    void Start()
    {
        if (m_AudioSource == null)
        {
            m_AudioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (_fadingOut)
        {
            float currentVolume = m_AudioSource.volume;
            if (currentVolume > 0.001f)
            {
                float newVolume = currentVolume - m_FadeOutSpeed * Time.deltaTime;
                if (newVolume < 0.0f)
                    newVolume = 0.0f;
                m_AudioSource.volume = newVolume;
            }
        }
    }
 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_AudioSource.enabled);
            stream.SendNext(m_AudioSource.volume == 0.0f);
            stream.SendNext(m_AudioSource.volume);
        }
        else
        {
            m_AudioSource.enabled = (bool)stream.ReceiveNext();

            if ((bool)stream.ReceiveNext() && m_AudioSource.volume != 0.0f)
            {
                _fadingOut = true;
            }
            else
            {
                _fadingOut = false;
                m_AudioSource.volume = (float)stream.ReceiveNext();
            }    

        }
    }

}
