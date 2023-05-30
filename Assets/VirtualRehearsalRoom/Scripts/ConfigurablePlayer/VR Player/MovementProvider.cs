using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementProvider : LocomotionProvider
{
    public float maxFallingSpeed = 1.0f;
    public ActionBasedContinuousMoveProvider continuousMoveProvider;

    private CharacterController characterController = null;

    Vector3 _verticalVelocity;

    protected override void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
    }

    void Update()
    {

        LimitFallingSpeed();
    }

    void LimitFallingSpeed()
    {
        Vector3 current_velocity = characterController.velocity;
        if (current_velocity.y <= -maxFallingSpeed)
        {
            Debug.Log(current_velocity.y);
            continuousMoveProvider.useGravity = false;
            //current_velocity.y = -maxFallingSpeed;
            //characterController.SimpleMove(current_velocity);
        } else
        {
            //continuousMoveProvider.useGravity = true;
        }
    }

    //void PositionController()
    //{
    //    Vector3 newCenter = Vector3.zero;
    //    newCenter.y = characterController.height / 2;
    //    newCenter.x = head.transform.localPosition.x;
    //    newCenter.z = head.transform.localPosition.z;

    //    characterController.center = newCenter;
    //}
}
