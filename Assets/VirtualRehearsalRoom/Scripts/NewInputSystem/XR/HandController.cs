using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    public InputActionReference _rightGripInputAction;
    public InputActionReference _rightTriggerInputAction;

    public InputActionReference _leftGripInputAction;
    public InputActionReference _leftTriggerInputAction;

    public bool FieldsHaveBeenAssigned = false;

    Animator _handAnimator;

    private void Awake()
    {
        _handAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (FieldsHaveBeenAssigned)
        {
            _leftGripInputAction.action.Enable();
            _leftTriggerInputAction.action.Enable();
            _rightGripInputAction.action.Enable();
            _rightTriggerInputAction.action.Enable();

            _leftGripInputAction.action.performed += LeftGripPressed;
            _leftTriggerInputAction.action.performed += LeftTriggerPressed;

            _rightGripInputAction.action.performed += RightGripPressed;
            _rightTriggerInputAction.action.performed += RightTriggerPressed;
        }
    }

    private void OnDisable()
    {
        if (FieldsHaveBeenAssigned)
        {
            _leftGripInputAction.action.performed -= LeftGripPressed;
            _leftTriggerInputAction.action.performed -= LeftTriggerPressed;

            _rightGripInputAction.action.performed -= RightGripPressed;
            _rightTriggerInputAction.action.performed -= RightTriggerPressed;
        }
    }

    private void LeftTriggerPressed(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("LeftTrigger", obj.ReadValue<float>());
    }

    private void LeftGripPressed(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("LeftGrip", obj.ReadValue<float>());
    }
    private void RightTriggerPressed(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("RightTrigger", obj.ReadValue<float>());
    }

    private void RightGripPressed(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("RightGrip", obj.ReadValue<float>());
    }
}
