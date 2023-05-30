using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Examples;
using UnityEngine.InputSystem;

public class VRPlayerManager : MonoBehaviour
{
    public PlayerConfigurator.PlayerTypeEnum PlayerType;

    public bool UseDirectInteractors = false;

    public float GenericVRCameraNearClipPlane = 0.01f;
    public float FullBodyVRCameraNearClipPlane = 0.25f;

    public Camera MainVRCamera;
    public ActionBasedControllerManager LeftHandControllerManager;
    public GameObject LeftHandBaseControllerGameObject;
    public GameObject LeftHandDirectControllerGameObject;
    public ActionBasedControllerManager RightHandControllerManager;
    public GameObject RightHandBaseControllerGameObject;
    public GameObject RightHandDirectControllerGameObject;

    public LocomotionSchemeManager m_LocomotionSchemeManager;
    public ActionBasedContinuousMoveProvider m_ContinuousMoveProvider;

    [SerializeField]
    InputActionReference TeleportAudienceAction;

    private bool _invalidLineVisable = true;

    public void ToggleInvalidLineVisible()
    {
        _invalidLineVisable = !_invalidLineVisable;

        float transparancyValue = 0.0f;
        if (_invalidLineVisable)
            transparancyValue = 0.04f;

        XRInteractorLineVisual leftLineVisual = LeftHandBaseControllerGameObject.GetComponent<XRInteractorLineVisual>();
        GradientAlphaKey[] leftGradientAlphaKeys = leftLineVisual.invalidColorGradient.alphaKeys;
        GradientColorKey[] leftGradientColorKeys = leftLineVisual.invalidColorGradient.colorKeys;
        leftGradientAlphaKeys[0].alpha = transparancyValue;
        leftLineVisual.invalidColorGradient.SetKeys(leftGradientColorKeys, leftGradientAlphaKeys);

        XRInteractorLineVisual rightLineVisual = RightHandBaseControllerGameObject.GetComponent<XRInteractorLineVisual>();
        GradientAlphaKey[] rightGradientAlphaKeys = rightLineVisual.invalidColorGradient.alphaKeys;
        GradientColorKey[] rightGradientColorKeys = rightLineVisual.invalidColorGradient.colorKeys;
        rightGradientAlphaKeys[0].alpha = transparancyValue;
        rightLineVisual.invalidColorGradient.SetKeys(rightGradientColorKeys, rightGradientAlphaKeys);
    }

    public void ToggleInteractorType()
    {
        SetUseDirectInteractors(!UseDirectInteractors);
    }

    public void SetUseDirectInteractors(bool value)
    {
        UseDirectInteractors = value;
        ConfigureControllerInteractors();
    }

    public void ConfigureUsingCurrentSettings()
    {
        ConfigureCameraClipPlane();
        ConfigureControllerInteractors();
        ConfigureLocomotion();
    }

    private void ConfigureLocomotion()
    {
        if (PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR)
        {
            m_LocomotionSchemeManager.turnStyle = LocomotionSchemeManager.TurnStyle.Snap;
        }
        else if (PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            m_LocomotionSchemeManager.turnStyle = LocomotionSchemeManager.TurnStyle.Continuous;
        }

        m_ContinuousMoveProvider.moveSpeed = VRRSettingsAccessor.Instance.Settings.VRMoveSpeed;
    }

    private void ConfigureCameraClipPlane()
    {
        if (PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR)
        {
            MainVRCamera.nearClipPlane = GenericVRCameraNearClipPlane;
        }
        else if (PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            MainVRCamera.nearClipPlane = FullBodyVRCameraNearClipPlane;
        }
    }

    public void ConfigureControllerInteractors()
    {
        if (UseDirectInteractors)
        {
            LeftHandControllerManager.useDirectController = true;
            RightHandControllerManager.useDirectController = true;
        } else
        {
            LeftHandControllerManager.useDirectController = false;
            RightHandControllerManager.useDirectController = false;
        }
    }


    public void TeleportAudience()
    {
        Debug.Log("Teleporting audience");
    }

    public void UseLeftHandDirectController(bool value)
    {
        LeftHandControllerManager.useDirectController = value;
    }

    public void UseRightHandDirectController(bool value)
    {
        RightHandControllerManager.useDirectController = value;
    }

}
