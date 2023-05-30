using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAvatarHandController : MonoBehaviour
{
    [SerializeField] InputActionReference gripInputAction;
    [SerializeField] InputActionReference triggerInputAction;

    Animator _handAnimator;

    private void Awake()
    {
        _handAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        gripInputAction.action.performed += GripPressed;
        triggerInputAction.action.performed += TriggerPressed;
    }

    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("Trigger", obj.ReadValue<float>());
    }

    private void GripPressed(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("Grip", obj.ReadValue<float>());
    }

    private void OnDisable()
    {
        gripInputAction.action.performed -= GripPressed;
        triggerInputAction.action.performed -= TriggerPressed;
    }

    private void Update()
    {
        if (gripInputAction.action.WasReleasedThisFrame())
        {
            _handAnimator.SetFloat("Grip", 0.0f);
        }

        if (triggerInputAction.action.WasReleasedThisFrame())
        {
            _handAnimator.SetFloat("Trigger", 0.0f);
        }
    }



}
