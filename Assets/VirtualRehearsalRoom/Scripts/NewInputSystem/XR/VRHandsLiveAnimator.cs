using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandsLiveAnimator : MonoBehaviour
{
    public Animator HandTrackingAnimator;
    public Animator CharacterAnimator;

    private HumanBodyBones[] _handBones =
    {
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
        HumanBodyBones.RightLittleDistal
    };

    public bool HandModelOk = false;
    public bool XsenseModelOk = false;

    private Transform[] _handTrackingModel;
    private Transform[] _characterModel;

    void Start()
    {
        _handTrackingModel = new Transform[_handBones.Length];
        _characterModel = new Transform[_handBones.Length];

        SetupModel(HandTrackingAnimator, _handTrackingModel);
        SetupModel(CharacterAnimator, _characterModel);
    }

    void SetupModel(Animator animator, Transform[] modelRef)
    {
        for (int i = 0; i < _handBones.Length; i++)
        {
            HumanBodyBones boneID = _handBones[i];
            try
            {
                modelRef[i] = animator.GetBoneTransform(boneID);
                if (modelRef[i] == null)
                {
                    // Debug.Log("Can't find " + boneID + " in the model!");
                }
            }
            catch (Exception)
            {
                // Debug.Log("Can't find " + boneID + " in the model! " + e);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < _handBones.Length; i++)
        {
            if (_characterModel[i] == null || _handTrackingModel[i] == null)
            {
                continue;
            }
            _characterModel[i].localRotation = _handTrackingModel[i].localRotation;
        }
    }
}
