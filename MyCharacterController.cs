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

        [Tooltip("The speed of the jump")]
        public float jumpSpeed = 10f;

        [Tooltip("The extra time before landing where you can press jump and it’ll still jump once you land")]
        public float jumpPreGroundingGraceTime = 0f;

        [Tooltip("the extra time after leaving stable ground where you’ll still be allowed to jump")]
        public float jumpPostGroundingGraceTime = 0f;

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

        #endregion private variables

        // Start is called before the first frame update
        private void Start()
        {
            motor.CharacterController = this;
        }

        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
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

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_lookInputVector != Vector3.zero && orientationSharpness > 0f)
            {
                Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 targetMovementVelocity = Vector3.zero;
            if (motor.GroundingStatus.IsStableOnGround)
            {
                // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
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
                        Vector3 perpendicularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                        targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpendicularObstructionNormal);
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
            _jumpedThisFrame = false;
            _timeSinceJumpRequested += deltaTime;
            if (_jumpRequested)
            {
                // See if we actually are allowed to jump
                if (!_jumpConsumed && ((allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime))
                {
                    // Calculate jump direction before un grounding
                    Vector3 jumpDirection = motor.CharacterUp;
                    if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
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
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (_jumpRequested && _timeSinceJumpRequested > jumpPreGroundingGraceTime)
                {
                    _jumpRequested = false;
                }

                // Handle jumping while sliding
                if (allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!_jumpedThisFrame)
                    {
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

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}