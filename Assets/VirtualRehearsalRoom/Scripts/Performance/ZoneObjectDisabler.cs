using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneObjectDisabler : MonoBehaviour
{
    [Header("Dynamicly assigned fields")]
    public GameObject _playerCameraGameObject;

    [Space]
    [Header("Settings")]
    public bool DisableForDesktopPlayer = true;
    public Collider[] ZoneColliders;
    public GameObject[] ZonedObjects;

    private PlayerFinder _playerFinder;
    private bool _disabled = false;

    void Start()
    {
        _playerFinder = FindObjectOfType<PlayerFinder>();
    }

    void Update()
    {
        if (_disabled)
            return;

        if (_playerCameraGameObject != null)
        {
            bool isPlayerInColliderZone = IsPlayerInColliderZone();
            foreach (GameObject gameObject in ZonedObjects)
            {
                gameObject.SetActive(isPlayerInColliderZone);
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
                _playerCameraGameObject = _playerFinder.LocalPlayerConfigurator.GetPlayerCamera().gameObject;
                if (DisableForDesktopPlayer)
                {
                    if (_playerFinder.LocalPlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.Desktop)
                    {
                        _disabled = true;
                    }
                }
            }
        }
    }

    private bool IsPlayerInColliderZone()
    {
        bool playerInZone = false;
        foreach (Collider collider in ZoneColliders)
        {
            playerInZone = playerInZone || collider.bounds.Contains(_playerCameraGameObject.transform.position);
        }
        return playerInZone;
    }
}
