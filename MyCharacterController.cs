using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
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

        #endregion Jumping variables

        #region Miscellaneous variables

        [Header("Miscellaneous")]
        [Tooltip("Gravity acting on the character")]
        public Vector3 gravity = new Vector3(0, -30f, 0);

        [Tooltip("The root(feet) transform of the character")]
        public Transform meshRoot;

        #endregion Miscellaneous variables

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

        #endregion private variables

        // Start is called before the first frame update
        private void Start()
        {
            motor.CharacterController = this;
        }

        /// <summary>
        /// This is called every frame by MyPlayer in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

            // Move and look inputs
            _moveInputVector = cameraPlanarRotation * moveInputVector;
            _lookInputVector = cameraPlanarDirection;

            // Jumping input
            if (inputs.JumpDown)
            {
                _timeSinceJumpRequested = 0f;
                _jumpRequested = true;
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now.
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_lookInputVector != Vector3.zero && orientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now.
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
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
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
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
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // We can wall jump only if we are not stable on ground and are moving against an obstruction
            if (allowWallJump && !motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
            {
                _canWallJump = true;
                _wallJumpNormal = hitNormal;
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void AddVelocity(Vector3 velocity)
        {
            _internalVelocityAdd += velocity;
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
            Debug.Log("Landed");
        }

        protected void OnLeaveStableGround()
        {
            Debug.Log("Left ground");
        }
    }
}