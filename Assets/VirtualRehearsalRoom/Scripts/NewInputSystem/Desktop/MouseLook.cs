using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform PlayerCamera;
    public float XClamp = 85f;

    public float SensitivityX = 11.0f;
    public float SensitivityY = 0.5f;

    float _mouseX, _mouseY;
    float _xRotation;

    bool _mouseCameraMode = true;

    public void ToggleMouseMode()
    {
        _mouseCameraMode = !_mouseCameraMode;

        if(_mouseCameraMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        if (_mouseCameraMode)
        {
            transform.Rotate(Vector3.up, _mouseX * Time.deltaTime);

            _xRotation -= _mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -XClamp, XClamp);
            Vector3 targetRotation = transform.eulerAngles;
            targetRotation.x = _xRotation;

            PlayerCamera.eulerAngles = targetRotation;
        }
    }

    public void ReceiveInput (Vector2 mouseInput)
    {
        _mouseX = mouseInput.x * SensitivityX;
        _mouseY = mouseInput.y * SensitivityY;
    }
}
