using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AudienceLevitationManager : MonoBehaviour, IPunObservable
{
    [Range(-10.0f, 10.0f)]
    public float LevitationSpeed = 0.0f;

    private PlayerFinder _playerFinder;
    private PlayerConfigurator _localPlayerConfigurator;

    private void Awake()
    {
        _playerFinder = FindObjectOfType<PlayerFinder>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(LevitationSpeed);
        } else
        {
            LevitationSpeed = (float)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (_localPlayerConfigurator == null)
        {
            FindLocalPlayer();
        }
        else if (ShouldUpdateCameraOffset())
        {
            Transform localCameraOffsetTransform = _localPlayerConfigurator.m_SimpleAvatarManger.CameraOffsetTransform;
            localCameraOffsetTransform.Translate(Vector3.up * LevitationSpeed * Time.deltaTime);

        }
        else
        {
            Transform localCameraOffsetTransform = _localPlayerConfigurator.m_SimpleAvatarManger.CameraOffsetTransform;
            if (localCameraOffsetTransform.localPosition.y < 0.0f)
            {
                Vector3 localPosition = localCameraOffsetTransform.localPosition;
                localPosition.y = 0.0f;
                localCameraOffsetTransform.localPosition = localPosition;
            }
        }
    }

    bool ShouldUpdateCameraOffset()
    {
        Transform localCameraOffsetTransform = _localPlayerConfigurator.m_SimpleAvatarManger.CameraOffsetTransform;

        bool shouldUpdate = _localPlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR;
        shouldUpdate = shouldUpdate && Mathf.Abs(LevitationSpeed) > 0.01f;
        shouldUpdate = shouldUpdate && !(localCameraOffsetTransform.localPosition.y <= 0.0f && LevitationSpeed < 0.0f);
        return shouldUpdate;
    }

    void FindLocalPlayer()
    {
        _localPlayerConfigurator = _playerFinder.LocalPlayerConfigurator;
    }
}
