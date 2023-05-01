using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Character
{
    public class InputManager : MonoBehaviour
    {
        [HideInInspector] public Vector2 moveInput;

        [HideInInspector] public float horizontalInput;
        [HideInInspector] public float verticalInput;

        [HideInInspector] public bool jumpInput;

        [HideInInspector] public bool attackInput;

        private InputActions _inputActions;

        private void Awake()
        {
            _inputActions = new InputActions();

            _inputActions.Player.Move.started += OnMovementInput;
            _inputActions.Player.Move.performed += OnMovementInput;
            _inputActions.Player.Move.canceled += OnMovementInput;

            _inputActions.Player.Jump.started += OnJumpInput;
            _inputActions.Player.Jump.canceled += OnJumpInput;

            _inputActions.Player.Attack.started += OnAttackInput;
            _inputActions.Player.Attack.canceled += OnAttackInput;
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

        private void OnAttackInput(InputAction.CallbackContext context)
        {
            attackInput = context.ReadValueAsButton();
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
}