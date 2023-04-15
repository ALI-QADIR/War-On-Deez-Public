using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using Assets.Scripts;
using KinematicCharacterController.Examples;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts
{
    public struct PlayerCharacterInputs
    {
        public float moveAxisForward;
        public float moveAxisRight;
        public Quaternion cameraRotation;
        public bool jumpDown;
    }

    public class CharacterManager : MonoBehaviour
    {
        private MyCharacterController _character;
        private AnimationManager _animationManager;

        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            var horizontal = inputs.moveAxisRight;
            var vertical = inputs.moveAxisForward;
            var jumpDown = inputs.jumpDown;

            _animationManager.horizontalInput = horizontal switch
            {
                > 0.1f => 1,
                < -0.1f => -1,
                _ => 0
            };

            _animationManager.verticalInput = vertical switch
            {
                > 0.1f => 1,
                < -0.1f => -1,
                _ => 0
            };

            _animationManager.jumping = _character.isJumping;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            _character = GetComponent<MyCharacterController>();
            _animationManager = GetComponent<AnimationManager>();
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}