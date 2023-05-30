using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopMovement : MonoBehaviour
{
    public CharacterController Controller;
    public float Speed = 11.0f;
    public float Gravity = -9.82f;
    public float JumpHeight = 1.0f;

    public LayerMask groundMask;

    public bool DisableMovement = false;

    bool _isGrounded;
    bool _jump;

    Vector2 _horizontalInput;
    Vector3 _verticalVelocity;

    private void Update()
    {
        if (!DisableMovement)
        {
            _isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);
            if (_isGrounded)
            {
                // TODO[sam]: fix issue with resetting at start of jump
                _verticalVelocity.y = 0;
            }

            Vector3 horizontalVelocity = Speed * (transform.right * _horizontalInput.x + transform.forward * _horizontalInput.y);
            Controller.Move(horizontalVelocity * Time.deltaTime);

            if (_jump)
            {
                if (_isGrounded)
                {
                    _verticalVelocity.y = Mathf.Sqrt(-2.0f * JumpHeight * Gravity);
                }
                _jump = false;
            }
            _verticalVelocity.y += Gravity * Time.deltaTime;
            Controller.Move(_verticalVelocity * Time.deltaTime);
        }
    }
    public void ReceiveInput (Vector2 horizontalInput)
    {
        _horizontalInput = horizontalInput;
    }

    public void OnJumpPress()
    {
        _jump = true;
    }
}
