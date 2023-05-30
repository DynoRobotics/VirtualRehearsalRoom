using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInputManager : MonoBehaviour
{
    public DesktopMovement m_DesktopMovement;
    public DesktopFlightMovement m_DesktopFlightMovement;
    public MouseLook m_MouseLook;

    public PlayerNetworkManager m_PlayerNetworkManager;

    public bool DisableMovement = false;

    DesktopPlayerControls _controls;
    DesktopPlayerControls.GroundMovementActions _groundMovement;
    DesktopPlayerControls.FlyingMovementActions _flyingMovement;
    DesktopPlayerControls.MultiplayerControlsActions _multiplayerControls;

    Vector2 _horizontalInput;
    Vector3 _mouseInput;

    Vector3 _flightInput;

    private void Awake()
    {
        _controls = new DesktopPlayerControls();
        _groundMovement = _controls.GroundMovement;
        _multiplayerControls = _controls.MultiplayerControls;

        _groundMovement.HorizontalMovement.performed += ctx => _horizontalInput = ctx.ReadValue<Vector2>();

        _groundMovement.Jump.performed += _ => m_DesktopMovement.OnJumpPress();

        _groundMovement.ToggleMouseMode.performed += _ => m_MouseLook.ToggleMouseMode();

        _groundMovement.MouseX.performed += ctx => _mouseInput.x = ctx.ReadValue<float>();
        _groundMovement.MouseY.performed += ctx => _mouseInput.y = ctx.ReadValue<float>();

        _multiplayerControls.SelectNextAvatar.performed += _ => m_PlayerNetworkManager.SelectNextSimpleAvatar();
        _multiplayerControls.SelectPreviousAvatar.performed += _ => m_PlayerNetworkManager.SelectPreviousSimpleAvatar();

        // TODO(sam): Get flight movement working, should not use actions from groundMovement...
        _groundMovement.HorizontalMovement.performed += ctx =>
        {
            Vector2 horizontalInput = ctx.ReadValue<Vector2>();
            _flightInput.z = horizontalInput.x;
            _flightInput.x = horizontalInput.y;
        };

        _groundMovement.VerticalMovement.performed += ctx =>
        {
            _flightInput.y = ctx.ReadValue<float>();
        };

        //_multiplayerControls.ChangeSceneToDanceTheatre.performed += _ => m_PlayerNetworkManager.ChangeSceneToDanceStudio();
        //_multiplayerControls.ChangeSceneToOutdoors.performed += _ => m_PlayerNetworkManager.ChangeSceneToCampFire();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void Update()
    {
        m_DesktopMovement.ReceiveInput(_horizontalInput);
        m_DesktopFlightMovement.ReceiveInput(_flightInput);
        m_MouseLook.ReceiveInput(_mouseInput);
    }

}
