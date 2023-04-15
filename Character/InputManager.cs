using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public Vector2 moveInput;

    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    [HideInInspector] public bool jumpInput;

    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();

        _inputActions.Player.Move.started += OnMovementInput;
        _inputActions.Player.Move.performed += OnMovementInput;
        _inputActions.Player.Move.canceled += OnMovementInput;

        _inputActions.Player.Jump.started += OnJumpInput;
        _inputActions.Player.Jump.canceled += OnJumpInput;
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        horizontalInput = moveInput.x;
        verticalInput = moveInput.y;
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        jumpInput = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}