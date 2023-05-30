using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesLiveAnimator : MonoBehaviour
{
    public Transform Target;

    public Transform LeftEyeBone;
    public Transform RightEyeBone;

    void Start()
    {
        Animator animator = GetComponent<Animator>();

        try
        {
            LeftEyeBone = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            RightEyeBone = animator.GetBoneTransform(HumanBodyBones.RightEye);
        }
        catch (Exception e)
        {
            Debug.Log("Can't find eye bone " + e);
        }
    }

    void Update()
    {
        LeftEyeBone.LookAt(Target);
        RightEyeBone.LookAt(Target);

        if (Mathf.Abs(LeftEyeBone.localRotation.eulerAngles.y) > 30.0f || Mathf.Abs(RightEyeBone.localRotation.eulerAngles.y) > 30.0f)
        {
            LeftEyeBone.localRotation = Quaternion.identity;
            RightEyeBone.localRotation = Quaternion.identity;
        }

    }
}
