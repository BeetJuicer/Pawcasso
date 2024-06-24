using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using System;

namespace KinematicCharacterController.Examples
{
    public enum CharacterState
    {
        Default,
        Charging,
        WallRunning
    }

    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }

    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool ChargingDown;
    }

    public struct AICharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public enum BonusOrientationMethod
    {
        None,
        TowardsGravity,
        TowardsGroundSlopeAndGravity,
    }

    public class ExampleCharacterController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        [Header("Wall Detection")]
        public float rayDetectionLength = 1f;

        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 10f;
        public float StableMovementSharpness = 15f;
        public float OrientationSharpness = 10f;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 15f;
        public float AirAccelerationSpeed = 15f;
        public float Drag = 0.1f;

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding = false;
        public float JumpUpSpeed = 10f;
        public float JumpScalableForwardSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;
        public bool AllowDoubleJump = false;
        private bool _doubleJumpConsumed = false;



        [Header("Wall Jumping")]
        public bool AllowWallJump = false;
        public LayerMask whatIsWall;

        [Header("Charging")]
        public float ChargeSpeed = 15f;
        public float lowChargeTime = 0.5f;
        public float mediumChargeTime = 1f;
        public float highChargeTime = 1.5f;
        public float StoppedTime = 1f;
        private float finalChargeTime;

        [Header("Misc")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public BonusOrientationMethod BonusOrientationMethod = BonusOrientationMethod.None;
        public float BonusOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public Transform CameraFollowPoint;
        public float CrouchedCapsuleHeight = 1f;

        public CharacterState CurrentCharacterState { get; private set; }

        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private bool _canWallJump = false;
        private bool _canWallRun = false;
        private Vector3 _wallJumpNormal;


        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;

        private Vector3 lastInnerNormal = Vector3.zero;
        private Vector3 lastOuterNormal = Vector3.zero;

        private Vector3 _currentChargeVelocity;
        private bool _isStopped;
        private bool _mustStopVelocity = false;
        private float _timeSinceStartedCharge = 0;
        private float _timeSinceStopped = 0;

        private void Awake()
        {
            // Handle initial state
            TransitionToState(CharacterState.Default);

            // Assign the characterController to the motor
            Motor.CharacterController = this;
        }

        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
                case CharacterState.Charging:
                    {
                        _currentChargeVelocity = Motor.CharacterForward * ChargeSpeed;
                        _isStopped = false;
                        _timeSinceStartedCharge = 0f;
                        _timeSinceStopped = 0f;
                        break;
                    }
                case CharacterState.WallRunning:
                    {

                        break;
                    }
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
            }
        }

        public void EnterChargeState(int chargeLevel)
        {
            switch (chargeLevel)
            {
                case 1:
                    finalChargeTime = lowChargeTime;
                    break;
                case 2:
                    finalChargeTime = mediumChargeTime;
                    break;
                case 3:
                    finalChargeTime = highChargeTime;
                    break;
            }

            TransitionToState(CharacterState.Charging);
        }

        /// <summary>
        /// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * moveInputVector;

                        switch (OrientationMethod)
                        {
                            case OrientationMethod.TowardsCamera:
                                _lookInputVector = cameraPlanarDirection;
                                break;
                            case OrientationMethod.TowardsMovement:
                                _lookInputVector = _moveInputVector.normalized;
                                break;
                        }

                        // Jumping input
                        if (inputs.JumpDown)
                        {
                            _timeSinceJumpRequested = 0f;
                            _jumpRequested = true;
                        }

                        // Crouching input
                        if (inputs.CrouchDown)
                        {
                            _shouldBeCrouching = true;

                            if (!_isCrouching)
                            {
                                _isCrouching = true;
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                                MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                            }
                        }
                        else if (inputs.CrouchUp)
                        {
                            _shouldBeCrouching = false;
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// This is called every frame by the AI script in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref AICharacterInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            _lookInputVector = inputs.LookVector;
        }

        private Quaternion _tmpTransientRot;

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
                case CharacterState.Charging:
                    {
                        // Update times
                        _timeSinceStartedCharge += deltaTime;
                        if (_isStopped)
                        {
                            _timeSinceStopped += deltaTime;
                        }
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
                case CharacterState.Default:
                    {
                        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                        {
                            // Smoothly interpolate from current to target look direction
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            // Set the current rotation (which will be used by the KinematicCharacterMotor)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }

                        Vector3 currentUp = (currentRotation * Vector3.up);
                        if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                        {
                            // Rotate from current up to invert gravity
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                        {
                            if (Motor.GroundingStatus.IsStableOnGround)
                            {
                                Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                                Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));
                            }
                            else
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                            }
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        break;
                    }
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
                case CharacterState.Default:
                    {

                        Ray front = new Ray(transform.position, Vector3.forward);
                        Ray left = new Ray(transform.position, Vector3.left);
                        Ray right = new Ray(transform.position, Vector3.right);
                        Ray back = new Ray(transform.position, Vector3.back);
                        Ray[] wallRays = { front, left, right, back };
                        Ray nearestHit = front;
                        float nearestDistance = 0f;

                        for (int i = 0; i < 4; i++)
                        {
                            if (Physics.Raycast(wallRays[i], out RaycastHit hitInfo, rayDetectionLength, whatIsWall))
                            {
                                if (hitInfo.distance > nearestDistance)
                                {
                                    //nearestHit = wallRays[i];
                                }

                                //Issue 1: Ledges that are too short are still considered wall runnable. Add two more rays, one at top and bottom.
                                //Issue 2: I should only be able to wallrun when the wall is either left or right. BUT I should be able to walljump from any wall direction.

                                //TODO: make the _wallJumpNormal either (1) if moving towards a certain direction and there is a hit there, that one.
                                // or (2) if no moveDirection, the nearest.
                                // Right now, this code sets the _wallJumpNormal to the last direction with a hit, so priority is front, left, right, back
                                if (!Motor.GroundingStatus.IsStableOnGround && !hitInfo.collider.isTrigger)
                                {
                                    nearestHit = wallRays[i];//for testing
                                    _wallJumpNormal = hitInfo.normal;
                                    _canWallJump = true;
                                }
                            }
                        }

                        // Ground movement
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            float currentVelocityMagnitude = currentVelocity.magnitude;

                            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                            // Reorient velocity on slope
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;

                            float onPaintSpeedMultiplier = PaintSurfaceChecker.IsOnColoredGround ? 1.5f : 1f;
                            Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed * onPaintSpeedMultiplier;

                            // Smooth movement Velocity
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        }
                        //canwalljump means next to a wall. rename this bullshit.
                        //else if (_canWallJump)
                        //{
                            //only negative depending on the face of the wall?
                            //print("Wall normal:" + _wallJumpNormal);
                            //Vector3 wallInput = new Vector3(_moveInputVector.x, -_moveInputVector.z * _wallJumpNormal.z, _moveInputVector.y);
                            //currentVelocity = wallInput * MaxStableMoveSpeed;

                            // TODO: If we lose detection of a wall in the direction we used to stick to, _canWallJump is false.

                        //}
                        // Air movement
                        else
                        {
                            //if(CheckIfThereIsWall(moveDirection), wallrunDuration > 0)
                            //{
                             //   TransitionToState wallRun
                            //}


                            // Add move input
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

                                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                                // Limit air velocity from inputs
                                if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
                                {
                                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                                }
                                else
                                {
                                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                    {
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                    }
                                }

                                // Prevent air-climbing sloped walls
                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                                    {
                                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                    }
                                }

                                // Apply added velocity
                                currentVelocity += addedVelocity;
                            }

                            // Gravity
                            currentVelocity += Gravity * deltaTime;

                            // Drag
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }

                        {
                            // Handle jumping
                            _jumpedThisFrame = false;
                            _timeSinceJumpRequested += deltaTime;
                            if (_jumpRequested)
                            {
                                // See if we actually are allowed to jump
                                if (_canWallJump ||
                                    (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround)) ||
                                    !_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                                {
                                    // Calculate jump direction before ungrounding
                                    Vector3 jumpDirection = Motor.CharacterUp;

                                    //WallJump
                                    if (_canWallJump)
                                    {
                                        jumpDirection = Vector3.Lerp(Motor.CharacterUp, _wallJumpNormal, 0.5f);
                                        jumpDirection *= 2;
                                        print("wall jumping fr");

                                    }
                                    // Handle double jump
                                    else if (AllowDoubleJump && _jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                                    {
                                        Motor.ForceUnground(0.1f);
                                        print("double jumping fr");
                                        // Add to the return velocity and reset jump state
                                        jumpDirection = Motor.CharacterUp;
                                        _doubleJumpConsumed = true;
                                    }
                                    //Normal Jump
                                    else if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                    {
                                        //TODO: Upon exiting charge state(not here, find it somewhere else).
                                        //Start a small window of time where the player receives a forward boost on their jump.
                                        //If this jump request is within that time window, add a forward boost.
                                        jumpDirection = Motor.GroundingStatus.GroundNormal;
                                    }

                                    // Makes the character skip ground probing/snapping on its next update. 
                                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                    Motor.ForceUnground();

                                    // Add to the return velocity and reset jump state
                                    float onPaintSpeedMultiplier = PaintSurfaceChecker.IsOnColoredGround ? 1.3f : 1f;
                                    currentVelocity += (jumpDirection * JumpUpSpeed * onPaintSpeedMultiplier) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                    currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                                    _jumpRequested = false;
                                    _jumpedThisFrame = true;
                                    _jumpConsumed = true;
                                }
                            }

                            // Reset wall jump
                            _canWallJump = false;

                            // Take into account additive velocity
                            if (_internalVelocityAdd.sqrMagnitude > 0f)
                            {
                                currentVelocity += _internalVelocityAdd;
                                _internalVelocityAdd = Vector3.zero;
                            }
                        }

                        break;
                    }

                case CharacterState.Charging:
                    {
                        // If we have stopped and need to cancel velocity, do it here
                        if (_mustStopVelocity)
                        {
                            currentVelocity = Vector3.zero;
                            _mustStopVelocity = false;
                        }

                        if (_isStopped)
                        {
                            // When stopped, do no velocity handling except gravity
                            currentVelocity += Gravity * deltaTime;
                        }
                        else
                        {
                            float onPaintSpeedMultiplier = PaintSurfaceChecker.IsOnColoredGround ? 1.5f : 1f;
                            // When charging, velocity is always constant
                            float previousY = currentVelocity.y;
                            currentVelocity = _currentChargeVelocity * onPaintSpeedMultiplier;
                            currentVelocity.y = previousY;
                            currentVelocity += Gravity * deltaTime;
                        }
                        break;
                    }
            }
        }

        private void IsTouchingWall()
        {
            Ray front = new Ray(transform.position, Vector3.forward);
            Ray left = new Ray(transform.position, Vector3.left);
            Ray right = new Ray(transform.position, Vector3.right);
            Ray back = new Ray(transform.position, Vector3.back);
            Ray[] wallRays = { front, left, right, back };
            Ray nearestHit = front;
            float nearestDistance = 0f;

            for (int i = 0; i < 4; i++)
            {
                if (Physics.Raycast(wallRays[i], out RaycastHit hitInfo, rayDetectionLength))
                {
                    if (hitInfo.distance > nearestDistance)
                    {
                        //nearestHit = wallRays[i];
                    }

                    //Issue 1: Ledges that are too short are still considered wall runnable. Add two more rays, one at top and bottom.
                    //Issue 2: I should only be able to wallrun when the wall is either left or right. BUT I should be able to walljump from any wall direction.

                    //TODO: make the _wallJumpNormal either (1) if moving towards a certain direction and there is a hit there, that one.
                    // or (2) if no moveDirection, the nearest.
                    // Right now, this code sets the _wallJumpNormal to the last direction with a hit, so priority is front, left, right, back
                    if (!Motor.GroundingStatus.IsStableOnGround)
                    {
                        nearestHit = wallRays[i];//for testing
                        _wallJumpNormal = hitInfo.normal;
                        _canWallJump = true;
                    }
                }
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
                case CharacterState.Default:
                    {
                        // Handle jump-related values
                        {
                            // Handle jumping pre-ground grace period
                            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                            {
                                _jumpRequested = false;
                            }

                            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                            {
                                // If we're on a ground surface, reset jumping values
                                if (!_jumpedThisFrame)
                                {
                                    _jumpConsumed = false;
                                    _doubleJumpConsumed = false;
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
                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            // Do an overlap test with the character's standing height to see if there are any obstructions
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterOverlap(
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders,
                                Motor.CollidableLayers,
                                QueryTriggerInteraction.Ignore) > 0)
                            {
                                // If obstructions, just stick to crouching dimensions
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                            }
                            else
                            {
                                // If no obstructions, uncrouch
                                MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }
                        break;
                    }

                case CharacterState.Charging:
                    {
                        // Detect being stopped by elapsed time
                        if (!_isStopped && _timeSinceStartedCharge > finalChargeTime)
                        {
                            _mustStopVelocity = true;
                            _isStopped = true;
                        }

                        // Detect end of stopping phase and transition back to default movement state
                        if (_timeSinceStopped > StoppedTime)
                        {
                            TransitionToState(CharacterState.Default);
                        }
                        break;
                    }
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Charging:
                    {
                        // Detect being stopped by obstructions
                        if (!_isStopped && !hitStabilityReport.IsStable && Vector3.Dot(-hitNormal, _currentChargeVelocity.normalized) > 0.5f)
                        {
                            _mustStopVelocity = true;
                            _isStopped = true;
                        }
                        break;
                    }
                case CharacterState.Default:
                    {
                        // We can wall jump only if we are not stable on ground and are moving against an obstruction
                        if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                        {
                            _canWallJump = true;
                            _wallJumpNormal = hitNormal;
                        }
                        break;
                    }
            }
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        protected void OnLanded()
        {
        }

        protected void OnLeaveStableGround()
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}