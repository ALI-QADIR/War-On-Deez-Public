using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public enum CharacterState
    {
        DEFAULT,
    }

    public class MyCharacterController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor motor;

        #region Stable movement variables

        [Header("Stable Movement")]
        [Tooltip("Max Speed when stable on Ground")]
        public float maxStableMoveSpeed = 10f;

        [Tooltip("The movement Sharpness for lerping")]
        public float stableMovementSharpness = 15;

        [Tooltip("The sharpness with which the player turns to face away from camera")]
        public float orientationSharpness = 10;

        #endregion Stable movement variables

        #region Air movement variables

        [Header("Air Movement")]
        [Tooltip("Max Speed when in air jumping")]
        public float maxAirMoveSpeed = 10f;

        [Tooltip("Acceleration when in air")]
        public float airAccelerationSpeed = 5f;

        [Tooltip("The drag acting the character when in air")]
        public float drag = 0.1f;

        #endregion Air movement variables

        #region Jumping variables

        [Header("Jumping")]
        [Tooltip("Allow the player to jump when sliding against a wall")]
        public bool allowJumpingWhenSliding = false;

        [Tooltip("Allow player to double jump")]
        public bool allowDoubleJump = false;

        [Tooltip("Allow player to wall jump or perform wall kick")]
        public bool allowWallJump = false;

        [Tooltip("The speed of the jump")]
        public float jumpSpeed = 10f;

        [Tooltip("The extra time before landing where you can press jump and it’ll still jump once you land")]
        public float jumpPreGroundingGraceTime = 0.1f;

        [Tooltip("the extra time after leaving stable ground where you’ll still be allowed to jump")]
        public float jumpPostGroundingGraceTime = 0.1f;

        [HideInInspector] public bool isJumping;

        #endregion Jumping variables

        #region Miscellaneous variables

        [Header("Miscellaneous")]
        [Tooltip("Should the player be oriented according to gravity acting on it?")]
        public bool orientTowardsGravity = true;

        [Tooltip("Gravity acting on the character")]
        public Vector3 gravity = new Vector3(0, -30f, 0);

        [Tooltip("The root(feet) transform of the character")]
        public Transform meshRoot;

        [Tooltip("List of ignored Colliders")]
        public List<Collider> ignoredColliders = new List<Collider>();

        #endregion Miscellaneous variables

        public CharacterState CurrentCharacterState { get; private set; }

        #region private variables

        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private bool _doubleJumpConsumed = false;
        private bool _canWallJump = false;
        private Vector3 _wallJumpNormal;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        /*private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;
        private Collider[] _probedColliders = new Collider[8];*/

        #endregion private variables

        // Start is called before the first frame update
        private void Start()
        {
            // Assign to motor
            motor.CharacterController = this;

            // Handle initial state
            TransitionToState(CharacterState.DEFAULT);
        }

        /// <summary>
        /// Handles new movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialCharacterStateState = CurrentCharacterState;
            OnStateExit(tmpInitialCharacterStateState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialCharacterStateState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.DEFAULT:
                    {
                        break;
                    }
                    // other states come here
                    // use for dodge
                    /*motor.SetCapsuleCollisionsActivation(false);
                    motor.SetMovementCollisionsSolvingActivation(false);
                    motor.SetGroundSolvingActivation(false);*/
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.DEFAULT:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// This is called every frame by MyPlayer in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.moveAxisRight, 0f, inputs.moveAxisForward), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.DEFAULT:
                    {
                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * moveInputVector;
                        _lookInputVector = cameraPlanarDirection;

                        // Jumping input
                        if (inputs.jumpDown)
                        {
                            _timeSinceJumpRequested = 0f;
                            _jumpRequested = true;
                        }

                        // Crouching input
                        /*if (inputs.CrouchDown)
                        {
                            _shouldBeCrouching = true;

                            if (!_isCrouching)
                            {
                                _isCrouching = true;
                                Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                                MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                            }
                        }
                        else if (inputs.CrouchUp)
                        {
                            _shouldBeCrouching = false;
                        }*/
                        break;
                    }
                    // other states come here
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DEFAULT:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now.
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DEFAULT:
                    {
                        if (_lookInputVector != Vector3.zero && orientationSharpness > 0f)
                        {
                            // Smoothly interpolate from current to target look direction
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector,
                                1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

                            // Set the current rotation (which will be used by the KinematicCharacterMotor)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
                        }

                        if (orientTowardsGravity)
                        {
                            currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -gravity) *
                                              currentRotation;
                        }
                        break;
                    }
                    // other states come here
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now.
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DEFAULT:
                    {
                        Vector3 targetMovementVelocity = Vector3.zero;
                        if (motor.GroundingStatus.IsStableOnGround)
                        {
                            // Reorient velocity on slope
                            currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

                            // Smooth movement Velocity
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-stableMovementSharpness * deltaTime));
                        }
                        else
                        {
                            // Add move input
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                targetMovementVelocity = _moveInputVector * maxAirMoveSpeed;

                                // Prevent climbing on un-stable slopes with air movement
                                if (motor.GroundingStatus.FoundAnyGround)
                                {
                                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                                }

                                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, gravity);
                                currentVelocity += velocityDiff * airAccelerationSpeed * deltaTime;
                            }

                            // Gravity
                            currentVelocity += gravity * deltaTime;

                            // Drag
                            currentVelocity *= (1f / (1f + (drag * deltaTime)));
                        }

                        // Handle jumping
                        {
                            _jumpedThisFrame = false;
                            _timeSinceJumpRequested += deltaTime;
                            if (_jumpRequested)
                            {
                                // Handle double jump
                                if (allowDoubleJump)
                                {
                                    if (_jumpConsumed && !_doubleJumpConsumed && (allowJumpingWhenSliding ? !motor.GroundingStatus.FoundAnyGround : !motor.GroundingStatus.IsStableOnGround))
                                    {
                                        motor.ForceUnground(0.1f);

                                        // Add to the return velocity and reset jump state
                                        currentVelocity += (motor.CharacterUp * jumpSpeed) - Vector3.Project(currentVelocity, motor.CharacterUp);
                                        _jumpRequested = false;
                                        _doubleJumpConsumed = true;
                                        _jumpedThisFrame = true;
                                    }
                                }

                                // See if we actually are allowed to jump
                                if (_canWallJump ||
                                    (!_jumpConsumed && ((allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)))
                                {
                                    // Calculate jump direction before ungrounding
                                    Vector3 jumpDirection = motor.CharacterUp;
                                    if (_canWallJump)
                                    {
                                        jumpDirection = _wallJumpNormal;
                                    }
                                    else if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
                                    {
                                        jumpDirection = motor.GroundingStatus.GroundNormal;
                                    }

                                    // Makes the character skip ground probing/snapping on its next update.
                                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                    motor.ForceUnground(0.1f);

                                    // Add to the return velocity and reset jump state
                                    currentVelocity += (jumpDirection * jumpSpeed) - Vector3.Project(currentVelocity, motor.CharacterUp);
                                    _jumpRequested = false;
                                    _jumpConsumed = true;
                                    _jumpedThisFrame = true;
                                }
                            }

                            // Reset wall jump
                            _canWallJump = false;
                        }

                        // Take into account additive velocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }
                    // other states come here
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DEFAULT:
                    {
                        // Handle jump-related values
                        {
                            // Handle jumping pre-ground grace period
                            if (_jumpRequested && _timeSinceJumpRequested > jumpPreGroundingGraceTime)
                            {
                                _jumpRequested = false;
                            }

                            if (allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
                            {
                                // If we're on a ground surface, reset jumping values
                                if (!_jumpedThisFrame)
                                {
                                    _doubleJumpConsumed = false;
                                    _jumpConsumed = false;
                                }
                                _timeSinceLastAbleToJump = 0f;
                            }
                            else
                            {
                                // Keep track of time since we were last able to jump (for grace period)
                                _timeSinceLastAbleToJump += deltaTime;
                            }
                        }

                        // Handle uncrouching
                        /*if (_isCrouching && !_shouldBeCrouching)
                        {
                            // Do an overlap test with the character's standing height to see if there are any obstructions
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterCollisionsOverlap(
                                    Motor.TransientPosition,
                                    Motor.TransientRotation,
                                    _probedColliders) > 0)
                            {
                                // If obstructions, just stick to crouching dimensions
                                Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                            }
                            else
                            {
                                // If no obstructions, uncrouch
                                MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }*/
                        break;
                    }
                    // other states come here
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return !ignoredColliders.Contains(coll);
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DEFAULT:
                    {
                        // We can wall jump only if we are not stable on ground and are moving against an obstruction
                        if (allowWallJump && !motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                        {
                            _canWallJump = true;
                            _wallJumpNormal = hitNormal;
                        }
                        break;
                    }
                    // other states come here
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DEFAULT:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
                    // other states come here
            }
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (motor.GroundingStatus.IsStableOnGround && !motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!motor.GroundingStatus.IsStableOnGround && motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        protected void OnLanded()
        {
            isJumping = false;
        }

        protected void OnLeaveStableGround()
        {
            isJumping = true;
        }
    }
}