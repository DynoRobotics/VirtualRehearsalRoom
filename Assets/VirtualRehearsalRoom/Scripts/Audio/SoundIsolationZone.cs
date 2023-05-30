using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundIsolationZone : MonoBehaviour
{
    [Header("Dynamicly assigned fields")]
    public GameObject PlayerCameraGameObject;
    public MusicPlayerManager m_MusicPlayerManager;

    [Space]
    [Header("Settings")]
    public float VolumeInZone = 0.5f;
    public float FadeOutDistance = 0.5f;

    [Space]
    [Header("Live Data")]
    public float SoundIsolationVolume = 1.0f;

    private float _distanceToEdge;
    private BoxCollider _collider;
    private PlayerFinder _playerFinder;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _playerFinder = FindObjectOfType<PlayerFinder>();
        m_MusicPlayerManager = FindObjectOfType<MusicPlayerManager>();
    }

    private void Update()
    {
        if (PlayerCameraGameObject != null)
        {
            UpdateVolumeMultiplyer();

            // NOTE(sam): This should allow for multiple zones, but will have issues if they are overlapping
            if (SoundIsolationVolume != 1.0f && m_MusicPlayerManager != null)
            {
                m_MusicPlayerManager.SoundIsolationZoneVolume = SoundIsolationVolume;
            }
        } else
        {
            TryToFindPlayerCamera();
        }
    }

    private void TryToFindPlayerCamera()
    {
        if (_playerFinder != null)
        {
            if (_playerFinder.LocalPlayerFound)
            {
                PlayerCameraGameObject = _playerFinder.LocalPlayerConfigurator.GetPlayerCamera().gameObject;
            }
        }
    }

    private void UpdateVolumeMultiplyer()
    {
        Vector3 cameraPosition = PlayerCameraGameObject.transform.position;
        Vector3 closestPointOnBounds = _collider.ClosestPointOnBounds(cameraPosition);

        _distanceToEdge = Vector3.Distance(cameraPosition, closestPointOnBounds);

        if (_distanceToEdge == 0.0f)
        {
            SoundIsolationVolume = VolumeInZone;
        } else if (_distanceToEdge < FadeOutDistance)
        {
            float FadeOutRange = 1.0f - VolumeInZone;
            SoundIsolationVolume = VolumeInZone + FadeOutRange * (_distanceToEdge / FadeOutDistance);
        } else
        {
            SoundIsolationVolume = 1.0f;
        }

    }
}
