using UnityEngine;

namespace Assets.Scripts.Character
{
    public class MyPlayer : MonoBehaviour
    {
        public MyCharacterController character;
        public CharacterManager characterManager;

        private InputManager _inputManager;

        private void Awake()
        {
            _inputManager = GetComponent<InputManager>();
        }

        private void Update()
        {
            HandleCharacterInput();
        }

        private void HandleCharacterInput()
        {
            var characterInputs = new PlayerCharacterInputs
            {
                moveAxisForward = _inputManager.verticalInput,
                moveAxisRight = _inputManager.horizontalInput,
                cameraRotation = Camera.main.transform.rotation,
                jumpDown = _inputManager.jumpInput,
                attackDown = _inputManager.attackInput
            };

            character.SetInputs(ref characterInputs);
            characterManager.SetInputs(ref characterInputs);
        }
    }
}