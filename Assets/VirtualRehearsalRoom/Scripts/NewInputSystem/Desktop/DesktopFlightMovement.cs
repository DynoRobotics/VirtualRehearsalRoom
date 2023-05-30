using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopFlightMovement : MonoBehaviour
{
    public CharacterController Controller;
    public float HorizontalSpeed = 10.0f;
    public float VerticalSpeed = 5.0f;

    public Transform CameraTransform;

    public bool DisableMovement = false;

    Vector3 _flyingInput;

    void Update()
    {
        if (!DisableMovement)
        {
            Vector3 velocity = HorizontalSpeed * (CameraTransform.right * _flyingInput.z + CameraTransform.forward * _flyingInput.x) + VerticalSpeed * (CameraTransform.up * _flyingInput.y);
            Controller.Move(velocity * Time.deltaTime);
        }
    }

    public void ReceiveInput (Vector3 flyingInput)
    {
        _flyingInput = flyingInput;
    }
}
