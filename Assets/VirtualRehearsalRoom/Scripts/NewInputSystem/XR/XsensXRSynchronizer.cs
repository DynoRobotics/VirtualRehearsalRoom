using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XsensXRSynchronizer : MonoBehaviour
{
    [Space]
    [Header("XR Rig GameObjects")]
    public GameObject XRRigGameObject;
    public GameObject MainCameraGameObject;
    public GameObject CameraOffsetGameObject;
    public GameObject LeftHandControllerGameObject;
    public GameObject RightHandControllerGameObject;

    public GameObject RightHandBaseControllerGameObject;
    public GameObject RightHandDirectControllerGameObject;
    public GameObject RightHandTeleportControllerGameObject;
    public GameObject LeftHandBaseControllerGameObject;
    public GameObject LeftHandDirectControllerGameObject;
    public GameObject LeftHandTeleportControllerGameObject;

    public GameObject LeftHandPositionOffset;
    public GameObject RightHandPositionOffset;
    public GameObject LeftHandRotationOffset;
    public GameObject RightHandRotationOffset;

    [Space]
    [Header("Humanoid Avatar GameObjects")]
    public Transform HumanoidAvatar;
    public Transform HumanoidHead;
    public Transform HumanoidChest;
    public Transform HumanoidLeftHand;
    public Transform HumanoidRightHand;

    [Space]
    [Header("Other Settings")]
    public GameObject LeftHandControllerOffsetGameObject;
    public GameObject RightHandControllerOffsetGameObject;

    public float CameraToHeadVerticalOffset = 0.1f;
    public float CameraToHeadForwardOffset = 0.1f;

    public float HandRotationOffsetX = 10.0f;
    public float HandRotationOffsetY = 0.0f;
    public float HandRotationOffsetZ = 90.0f;

    private bool _initialized = false;

    private void Start()
    {
        SetupHumanoidTransforms();
        SetupControllerOffsets();

        SetHeadsetToTrackOnlyRotation();
        LoadCameraOffsetRotation();

        MainCameraGameObject.GetComponent<Camera>().enabled = false;
        MainCameraGameObject.GetComponent<Camera>().enabled = true;
    }

    public void Init()
    {
        // NOTE(sam): The transforms will be overwritten even when the tracking is disabled this is moved to Start
        //            Seems like a bug in ActionBasedController
        DisableXRHandTracking();

        _initialized = true;
    }

    public void ReparentAvatarToXRRig()
    {
        transform.parent = XRRigGameObject.transform;
    }

    void SetupHumanoidTransforms()
    {
        Animator animator = GetComponent<Animator>();
        HumanBodyBones boneID = HumanBodyBones.LastBone;

        try
        {
            boneID = HumanBodyBones.Hips;
            HumanoidAvatar = animator.GetBoneTransform(boneID).parent;

            boneID = HumanBodyBones.Head;
            HumanoidHead = animator.GetBoneTransform(boneID);

            boneID = HumanBodyBones.Chest;
            HumanoidChest = animator.GetBoneTransform(boneID);

            boneID = HumanBodyBones.LeftHand;
            HumanoidLeftHand = animator.GetBoneTransform(boneID);

            boneID = HumanBodyBones.RightHand;
            HumanoidRightHand = animator.GetBoneTransform(boneID);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Can't find " + boneID + " in the model!, exception: " + e.ToString());
        }
    }

    private void SetupControllerOffsets()
    {
        LeftHandControllerOffsetGameObject = new GameObject();
        LeftHandControllerOffsetGameObject.name = "LeftHandControllerOffset";
        LeftHandControllerOffsetGameObject.transform.parent = HumanoidLeftHand;
        LeftHandControllerOffsetGameObject.transform.localPosition = new Vector3(0f, 0.1f, 0.05f);
        LeftHandControllerOffsetGameObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);

        RightHandControllerOffsetGameObject = new GameObject();
        RightHandControllerOffsetGameObject.name = "RightHandControllerOffset";
        RightHandControllerOffsetGameObject.transform.parent = HumanoidRightHand;
        RightHandControllerOffsetGameObject.transform.localPosition = new Vector3(0f, 0.1f, 0.05f);
        RightHandControllerOffsetGameObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
    }

    private void SetHeadsetToTrackOnlyRotation()
    {
        TrackedPoseDriver trackedPoseDriver = MainCameraGameObject.GetComponent<TrackedPoseDriver>();
        trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
    }

    private void DisableXRHandTracking()
    {
        LeftHandBaseControllerGameObject.GetComponent<ActionBasedController>().enableInputTracking = false;
        LeftHandDirectControllerGameObject.GetComponent<ActionBasedController>().enableInputTracking = false;
        LeftHandTeleportControllerGameObject.GetComponent<ActionBasedController>().enableInputTracking = false;

        RightHandBaseControllerGameObject.GetComponent<ActionBasedController>().enableInputTracking = false;
        RightHandDirectControllerGameObject.GetComponent<ActionBasedController>().enableInputTracking = false;
        RightHandTeleportControllerGameObject.GetComponent<ActionBasedController>().enableInputTracking = false;

        LeftHandBaseControllerGameObject.transform.localPosition = Vector3.zero;
        LeftHandBaseControllerGameObject.transform.localRotation = Quaternion.identity;

        RightHandBaseControllerGameObject.transform.localPosition = Vector3.zero;
        RightHandBaseControllerGameObject.transform.localRotation = Quaternion.identity;

        LeftHandDirectControllerGameObject.transform.localPosition = Vector3.zero;
        LeftHandDirectControllerGameObject.transform.localRotation = Quaternion.identity;

        RightHandDirectControllerGameObject.transform.localPosition = Vector3.zero;
        RightHandDirectControllerGameObject.transform.localRotation = Quaternion.identity;

        LeftHandTeleportControllerGameObject.transform.localPosition = Vector3.zero;
        LeftHandTeleportControllerGameObject.transform.localRotation = Quaternion.identity;

        RightHandTeleportControllerGameObject.transform.localPosition = Vector3.zero;
        RightHandTeleportControllerGameObject.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (_initialized)
        {
            SynchCamera();
            SynchControllers();
        } else
        {
            Debug.LogError("Not updating xr Synchronizer");
        }    
    }

    void SynchCamera()
    {
        MainCameraGameObject.transform.position = HumanoidHead.transform.position + HumanoidHead.up*CameraToHeadVerticalOffset + HumanoidHead.forward*CameraToHeadForwardOffset;
    }

    void SynchControllers()
    {
        LeftHandControllerGameObject.transform.SetPositionAndRotation(
            LeftHandControllerOffsetGameObject.transform.position,
            LeftHandControllerOffsetGameObject.transform.rotation);

        RightHandControllerGameObject.transform.SetPositionAndRotation(
            RightHandControllerOffsetGameObject.transform.position,
            RightHandControllerOffsetGameObject.transform.rotation);

        LeftHandRotationOffset.transform.localRotation = Quaternion.Euler(HandRotationOffsetX, HandRotationOffsetY, HandRotationOffsetZ);
        RightHandRotationOffset.transform.localRotation = Quaternion.Euler(-HandRotationOffsetX, HandRotationOffsetY, HandRotationOffsetZ);
    }

    public void ResetCameraRotation()
    {
        Quaternion rotationDelta = Quaternion.FromToRotation(MainCameraGameObject.transform.forward,
                                                             HumanoidChest.transform.forward);

        Quaternion cameraOffsetRotation = CameraOffsetGameObject.transform.rotation;
        float newAngleY = cameraOffsetRotation.eulerAngles.y + rotationDelta.eulerAngles.y;

        CameraOffsetGameObject.transform.rotation = Quaternion.Euler(cameraOffsetRotation.eulerAngles.x,
                                                              newAngleY,
                                                              cameraOffsetRotation.eulerAngles.z);

        SaveCameraOffsetRotation();
    }

    private void SaveCameraOffsetRotation()
    {
        PlayerPrefs.SetFloat("CameraOffsetRotationY", CameraOffsetGameObject.transform.localRotation.eulerAngles.y);
        Debug.Log("Saving rotation offset y: " + CameraOffsetGameObject.transform.localRotation.eulerAngles.y);
    }

    private void LoadCameraOffsetRotation()
    {
        Vector3 localOffsetRot = CameraOffsetGameObject.transform.localRotation.eulerAngles;
        float cameraOffsetRotationY = localOffsetRot.y;
        cameraOffsetRotationY = PlayerPrefs.GetFloat("CameraOffsetRotationY", cameraOffsetRotationY); //cameraOffsetRotationY);

        // Debug.Log("Loading camera rotation offset y: " + cameraOffsetRotationY);

        localOffsetRot.y = cameraOffsetRotationY;
        CameraOffsetGameObject.transform.localRotation = Quaternion.Euler(localOffsetRot);
    }
}
