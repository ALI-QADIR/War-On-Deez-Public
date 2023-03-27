using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using Assets.Scripts;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts
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
            var characterInputs = new PlayerCharacterInputs();
            characterInputs.moveAxisForward = _inputManager.verticalInput;
            characterInputs.moveAxisRight = _inputManager.horizontalInput;
            characterInputs.cameraRotation = Camera.main.transform.rotation;
            characterInputs.jumpDown = _inputManager.jumpInput;

            character.SetInputs(ref characterInputs);
            characterManager.SetInputs(ref characterInputs);
        }
    }
}