using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class ControlSchemeChanged : UnityEvent<PlayerInput> {}

[RequireComponent(typeof(Rigidbody))]
public class CharacterMotor : MonoBehaviour, IPausable
{
    #pragma warning disable 0649
    [Header("Configuration")]
    [SerializeField] CharacterMotorConfig Config;

    [Header("Events")]
    [SerializeField] UnityEvent OnPlayFootstepAudio;
    [SerializeField] UnityEvent OnPlayJumpAudio;
    [SerializeField] UnityEvent OnPlayDoubleJumpAudio;
    [SerializeField] UnityEvent OnPlayLandAudio;

    [SerializeField] UnityEvent OnToggleSettingsMenu;

    [SerializeField] ControlSchemeChanged OnControlSchemeChanged;

    [Header("Debugging")]
    [SerializeField] bool DEBUG_ShowStepRays = false;
    #pragma warning restore 0649

    protected bool IsRunning = false;
    protected bool IsGrounded = true;
    protected bool IsJumping = false;

    protected int JumpCount = 0;
    protected float JumpTargetY = float.MinValue;
    protected float OriginalDrag = 0f;

    protected bool LockCursor = true;
    protected bool EnableUpdates = true;

    protected float TimeUntilNextFootstep = -1f;

    protected Rigidbody CharacterRB;
    protected Collider CharacterCollider;
    protected CinemachineVirtualCamera Camera;
    protected float CameraPitch = 0f;

    protected bool IsCameraLocked
    {
        get
        {
            return !Cursor.visible && Cursor.lockState == CursorLockMode.Locked;
        }
    }

    void Awake()
    {
    }

    
    

    #region Movement Handling
    

    protected virtual RaycastHit UpdateIsGrounded()
    {
        // raycast to check where the ground is
        RaycastHit hitResult;     
        float groundCheckDistance = Config.GroundedThreshold + (Config.CharacterHeight * 0.5f) - Config.CharacterRadius;
        float workingRadius = Config.CharacterRadius * (1f - Config.CollisionBuffer);
        if (Physics.SphereCast(transform.position, workingRadius, Vector3.down, out hitResult, groundCheckDistance, Config.WalkableMask, QueryTriggerInteraction.Ignore))
        {
            // check if the character is grounded
            IsGrounded = true;
        }
        else
            IsGrounded = false;

        return hitResult;
    }

    protected virtual void FixedUpdate()
    {
        // do nothing if updating is turned off or if paused
        if (!EnableUpdates || PauseManager.IsPaused)
            return;

        // Update if grounded
        bool wasGrounded = IsGrounded;
        RaycastHit hitResult = UpdateIsGrounded();


        // has a jump been requested?
        bool jumpRequested = Config.CanJump;

        // jump can only happen if either we're grounded or we're on the first jump of a double jump
        jumpRequested &= IsGrounded || (Config.CanDoubleJump && (JumpCount < 2));

        // are we at rest?
        if (!jumpRequested && !IsJumping && IsGrounded && CharacterRB.velocity.sqrMagnitude < 0.1f)
        {
           // CharacterRB.velocity = Vector3.zero;
            CharacterRB.Sleep();
            IsRunning = false;
        }
      
    }

    public float CurrentSpeed
    {
        get
        {
            return IsGrounded ? (IsRunning ? Config.RunSpeed : Config.WalkSpeed) : 
                                (Config.AirControl ? Config.InAirSpeed : 0);
        }
    }


    protected void UpdateFootstepAudio()
    {
        // update the time until the next footstep
        if (TimeUntilNextFootstep > 0)
        {
            float footstepTimeScale = 1f + Config.FootstepFrequencyWithSpeed.Evaluate(CharacterRB.velocity.magnitude / Config.RunSpeed);

            TimeUntilNextFootstep -= Time.deltaTime * footstepTimeScale;
        }

        // time to play the sound?
        if (TimeUntilNextFootstep <= 0)
        {
            OnPlayFootstepAudio?.Invoke();
            HearingManager.Instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EFootstep, IsRunning ? 2f : 1f);

            TimeUntilNextFootstep = Config.FootstepInterval;

            return;
        }
    }
    #endregion

    #region Cursor Handling
    public void UpdateCursorLock(bool newValue)
    {
        LockCursor = newValue;

        // update the cursor state
        Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !LockCursor;
    }
    #endregion

    #region Update Toggling
    public void SetCanUpdate(bool newValue)
    {
        EnableUpdates = newValue;
    }
    #endregion

    #region IPausable
    public bool OnPauseRequested()  { return true; }
    public bool OnResumeRequested() { return true; }

    public void OnPause() { }
    public void OnResume() { }
    #endregion    
}
