using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurablePlayerTracker : MonoBehaviour
{
    //public LookTargetController m_LookTargetController;

    public Transform MainCameraTransform;

    public PlayerConfigurator[] PlayerConfigurators;

    public List<Transform> VRAvatarHeadTransforms;

    public List<Transform> DesktopAvatarHeadTransforms;

    public Transform HeadToLookAt;

    public GameObject LookAtTargetGameObject;

    public float HeadVerticalLookAtOffset = 0.2f;

    private float nextUpdate = 1.0f;

    void Start()
    {
        LookAtTargetGameObject.SetActive(true);

    }

    void FindPlayers()
    {
        Debug.Log("Finding players");
        PlayerConfigurators = (PlayerConfigurator[])GameObject.FindObjectsOfType(typeof(PlayerConfigurator));

        VRAvatarHeadTransforms.Clear();
        DesktopAvatarHeadTransforms.Clear();

        foreach (PlayerConfigurator playerConfigurator in PlayerConfigurators)
        {
            if (playerConfigurator.PlayerIsRemote)
            {
                if (playerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR)
                {
                    AvatarInputConverter avatarInputConverter = playerConfigurator.m_SimpleAvatarManger.SimpleAvatarGameObject.GetComponent<AvatarInputConverter>();
                    VRAvatarHeadTransforms.Add(avatarInputConverter.AvatarHead);
                }

                if (playerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.Desktop)
                {
                    AvatarInputConverter avatarInputConverter = playerConfigurator.m_SimpleAvatarManger.SimpleAvatarGameObject.GetComponent<AvatarInputConverter>();
                    DesktopAvatarHeadTransforms.Add(avatarInputConverter.AvatarHead);
                }
            }

        }
    }

    void Update()
    {
        if (Time.time >= nextUpdate)
        {
            FindPlayers();
            SelectClosestToSightline();
            nextUpdate = Mathf.FloorToInt(Time.time) + 1.0f;
        }

        if (HeadToLookAt != null)
        {
            Vector3 eyePosition = HeadToLookAt.position + new Vector3(0f, HeadVerticalLookAtOffset, 0f);
            LookAtTargetGameObject.transform.position = eyePosition;
        }
    }

    void SelectClosestToSightline()
    {
        float closestDistanceFromSightline = float.MaxValue;
        VRAvatarHeadTransforms.ForEach(headTransform =>
        {
            Vector3 cameraRelative = MainCameraTransform.InverseTransformPoint(headTransform.position);

            Vector2 sightlineDistanceVector = new Vector2(cameraRelative.x, cameraRelative.y);
            float distanceFromSightline = sightlineDistanceVector.magnitude;

            if (distanceFromSightline <= closestDistanceFromSightline)
            {
                HeadToLookAt = headTransform;
                closestDistanceFromSightline = distanceFromSightline;
            }
        });

        DesktopAvatarHeadTransforms.ForEach(headTransform =>
        {
            Vector3 cameraRelative = MainCameraTransform.InverseTransformPoint(headTransform.position);

            Vector2 sightlineDistanceVector = new Vector2(cameraRelative.x, cameraRelative.y);
            float distanceFromSightline = sightlineDistanceVector.magnitude;

            if (distanceFromSightline <= closestDistanceFromSightline)
            {
                HeadToLookAt = headTransform;
                closestDistanceFromSightline = distanceFromSightline;
            }
        });

    }

}
