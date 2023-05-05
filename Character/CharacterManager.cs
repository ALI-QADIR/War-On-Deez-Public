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
        [SerializeField] private PlayerHitManager _hitManager;
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
            {
                _meleeStateMachine.SetNextState(new GroundEntryState());
            }

            _hitManager.isAttacking = (_meleeStateMachine.currentState.GetType() == typeof(GroundEntryState)) || (_meleeStateMachine.currentState.GetType() == typeof(GroundCombo1State)) || (_meleeStateMachine.currentState.GetType() == typeof(GroundCombo2State)) || (_meleeStateMachine.currentState.GetType() == typeof(GroundFinisherState));
        }

        private void Awake()
        {
            _character = GetComponent<MyCharacterController>();
            _animationManager = GetComponent<AnimationManager>();
            _meleeStateMachine = GetComponent<StateMachine.StateMachine>();

            if (_hitManager == null)
            {
                Debug.LogError("Provide the sword game object to the character manager script");
            }
            else
            {
                _hitManager.damage = 15;
            }
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}