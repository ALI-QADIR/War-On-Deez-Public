using Assets.Scripts.ComboSystem;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public struct PlayerCharacterInputs
    {
        public float moveAxisForward;
        public float moveAxisRight;
        public Quaternion cameraRotation;
        public bool jumpDown;
        public bool attackDown;
    }

    public class CharacterManager : MonoBehaviour
    {
        private MyCharacterController _character;
        private AnimationManager _animationManager;
        private StateMachine.StateMachine _meleeStateMachine;

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

            if (inputs.attackDown && _meleeStateMachine.currentState.GetType() == typeof(IdleCombatState))
                _meleeStateMachine.SetNextState(new GroundEntryState());
        }

        // Start is called before the first frame update
        private void Awake()
        {
            _character = GetComponent<MyCharacterController>();
            _animationManager = GetComponent<AnimationManager>();
            _meleeStateMachine = GetComponent<StateMachine.StateMachine>();
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}