// Copyright Dyno Robotics 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerXsensSynchronization : MonoBehaviourPun, IPunObservable
{
    public Transform Character;

    public bool SendHeadBlendshapes = true;

    private Animator _animator;

    private Transform[] _characterModel;

    private Vector3 _characterNetworkPosition;
    private Quaternion _characterNetworkRotation;
    private Quaternion[] _characterNetworkBoneRotations;
    private Vector3 _characterHipsNetworkPosition;
    private Vector3 _characterAvatarNetworkPosition;

    private float[] _angleCharacterBone;
    private float _distanceAvatar;
    private float _distanceHips;

    private bool _gotNetworkData;

    public List<GameObject> _missingTransforms;

    private HumanBodyBones[] humanBodyBones =
    {
        HumanBodyBones.Hips,
        HumanBodyBones.LeftUpperLeg,
        HumanBodyBones.LeftLowerLeg,
        HumanBodyBones.LeftFoot,
        HumanBodyBones.LeftToes,
        HumanBodyBones.RightUpperLeg,
        HumanBodyBones.RightLowerLeg,
        HumanBodyBones.RightFoot,
        HumanBodyBones.RightToes,
        HumanBodyBones.Spine,
        HumanBodyBones.Chest,
        HumanBodyBones.LeftShoulder,
        HumanBodyBones.LeftUpperArm,
        HumanBodyBones.LeftLowerArm,
        HumanBodyBones.LeftHand,
        HumanBodyBones.RightShoulder,
        HumanBodyBones.RightUpperArm,
        HumanBodyBones.RightLowerArm,
        HumanBodyBones.RightHand,
        HumanBodyBones.Neck,
        HumanBodyBones.Head,

        HumanBodyBones.LeftThumbProximal,
        HumanBodyBones.LeftThumbIntermediate,
        HumanBodyBones.LeftThumbDistal,
        HumanBodyBones.LeftIndexProximal,
        HumanBodyBones.LeftIndexIntermediate,
        HumanBodyBones.LeftIndexDistal,
        HumanBodyBones.LeftMiddleProximal,
        HumanBodyBones.LeftMiddleIntermediate,
        HumanBodyBones.LeftMiddleDistal,
        HumanBodyBones.LeftRingProximal,
        HumanBodyBones.LeftRingIntermediate,
        HumanBodyBones.LeftRingDistal,
        HumanBodyBones.LeftLittleProximal,
        HumanBodyBones.LeftLittleIntermediate,
        HumanBodyBones.LeftLittleDistal,

        HumanBodyBones.RightThumbProximal,
        HumanBodyBones.RightThumbIntermediate,
        HumanBodyBones.RightThumbDistal,
        HumanBodyBones.RightIndexProximal,
        HumanBodyBones.RightIndexIntermediate,
        HumanBodyBones.RightIndexDistal,
        HumanBodyBones.RightMiddleProximal,
        HumanBodyBones.RightMiddleIntermediate,
        HumanBodyBones.RightMiddleDistal,
        HumanBodyBones.RightRingProximal,
        HumanBodyBones.RightRingIntermediate,
        HumanBodyBones.RightRingDistal,
        HumanBodyBones.RightLittleProximal,
        HumanBodyBones.RightLittleIntermediate,
        HumanBodyBones.RightLittleDistal,

        HumanBodyBones.LeftEye,
        HumanBodyBones.RightEye
    };

    void Start()
    {
        PhotonNetwork.SerializationRate = 20;
        _gotNetworkData = false;

        if (Character != null)
        {
            BeginSetup();
        } else
        {
            Debug.LogWarning("No character set when starting full body network sync");
        }
    }
    
    public void BeginSetup()
    {
        _characterModel = new Transform[humanBodyBones.Length];
        _characterNetworkBoneRotations = new Quaternion[humanBodyBones.Length];
        _angleCharacterBone = new float[humanBodyBones.Length];

        if (!SetupModel(Character, _characterModel))
        {
            return;
        }
    }

    bool SetupModel(Transform model, Transform[] modelRef)
    {
        _animator = model.GetComponent<Animator>();
        if (!_animator)
        {
            return false;
        }

        for (int i = 0; i < humanBodyBones.Length; i++)
        {
            HumanBodyBones boneID = humanBodyBones[i];

            try
            {
                modelRef[i] = _animator.GetBoneTransform(boneID);
                if (modelRef[i] == null)
                {
                    // TODO(sam): Parent and remove the objects properly
                    GameObject missingTransform = new GameObject(boneID.ToString());
                    _missingTransforms.Add(missingTransform);
                    modelRef[i] = missingTransform.transform;
                    missingTransform.transform.parent = model;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Can't find " + boneID + " in the model! " + e);
                return false;
            }
        }

        return true;
    }

    void Update()
    {
        if (!this.photonView.IsMine)
        {
            if (_gotNetworkData)
            {
                Character.position = _characterNetworkPosition;
                Character.rotation = _characterNetworkRotation;

                for (int i = 0; i < humanBodyBones.Length; i++)
                {
                    if (humanBodyBones[i] == HumanBodyBones.Hips)
                    {
                        _characterModel[i].parent.transform.localPosition = _characterAvatarNetworkPosition;
                        _characterModel[i].localPosition = _characterHipsNetworkPosition;
                    }

                    _characterModel[i].localRotation = _characterNetworkBoneRotations[i];

            }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (Character == null) return;

        if (stream.IsWriting)
        {
            // Send Xsens tracking data

            stream.SendNext(Character.position);
            stream.SendNext(Character.rotation);

            for (int i = 0; i < humanBodyBones.Length; i++)
            {
                if (humanBodyBones[i] == HumanBodyBones.Hips)
                {
                    stream.SendNext(_characterModel[i].parent.transform.localPosition);
                    stream.SendNext(_characterModel[i].localPosition);
                }
                stream.SendNext(_characterModel[i].localRotation);
            }

        } else
        {
            // Receive Xsens tracking data 

            _gotNetworkData = true;

            try
            {
                _characterNetworkPosition = (Vector3)stream.ReceiveNext();
                _characterNetworkRotation = (Quaternion)stream.ReceiveNext();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                Debug.LogError("Do some reset here maybe?");
            }

            for (int i = 0; i < humanBodyBones.Length; i++)
            {
                // Store distances to allow smooting through interpolation
                if (humanBodyBones[i] == HumanBodyBones.Hips)
                {
                    Vector3 newAvatarPosition = (Vector3)stream.ReceiveNext();
                    Vector3 oldAvatarPosition = _characterModel[i].parent.transform.localPosition;
                    _characterAvatarNetworkPosition = newAvatarPosition;
                    _distanceAvatar = Vector3.Distance(oldAvatarPosition, newAvatarPosition);

                    Vector3 newHipsPosition = (Vector3)stream.ReceiveNext();
                    Vector3 oldHipsPosition = _characterModel[i].localPosition;
                    _characterHipsNetworkPosition = newHipsPosition;
                    _distanceHips = Vector3.Distance(oldHipsPosition, newHipsPosition);
                }

                Quaternion newBoneRotation = (Quaternion)stream.ReceiveNext();
                Quaternion oldBoneRotation = _characterModel[i].localRotation;
                _characterNetworkBoneRotations[i] = newBoneRotation;
                _angleCharacterBone[i] = Quaternion.Angle(oldBoneRotation, newBoneRotation);

            }
        }
    }
}
