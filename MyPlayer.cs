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
            characterInputs.MoveAxisForward = _inputManager.verticalInput;
            characterInputs.MoveAxisRight = _inputManager.horizontalInput;
            characterInputs.CameraRotation = Camera.main.transform.rotation;
            characterInputs.JumpDown = _inputManager.jumpInput;

            character.SetInputs(ref characterInputs);

            // when Q is pressed the character will jump backwards
            // use this logic for dodge
            /*if (Input.GetKeyDown(KeyCode.Q))
            {
                character.motor.ForceUnground(0.1f);
                character.AddVelocity(-Camera.main.transform.forward * 10);
            }*/
        }
    }
}