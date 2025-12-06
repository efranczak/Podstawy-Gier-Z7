using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CapsuleCollider2D _col;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    private float _gravityMultiplier = 1f;

    private Vector2 movement = new Vector2(0, 0);
    private float frictionAmount = 0f;

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion

    private float _time;

    public bool IsGrappling{ get; set; }

    public bool IsDashing { get; private set; }

    public bool IsClimbing { get; set; }

    private void Awake()
    {
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        IsGrappling = false;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.Space),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        if (IsGrappling)
        {
            HandleGrappleMovement();
            return;
         }

        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        HandleSpriteDirection();

        ApplyMovement();
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            _gravityMultiplier = 1f;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0f)
        {
            _endedJumpEarly = true;
            _rb.AddForce(Vector2.down * _stats.JumpCutForce, ForceMode2D.Impulse);
        }

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;

        // Reset vertical velocity before jumping for consistency
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);

        // Apply jump as an impulse
        _rb.AddForce(Vector2.up * _stats.JumpPower, ForceMode2D.Impulse);

        Jumped?.Invoke();
    }


    public void ForceJump(float jumpPowerOverride = -1)
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;

        float power = jumpPowerOverride > 0 ? jumpPowerOverride : _stats.JumpPower;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
        _rb.AddForce(Vector2.up * power, ForceMode2D.Impulse);

        Jumped?.Invoke();
    }


    private void HandleGrappleMovement()
    {
        // Allow horizontal input while grappling
        float horizontal = _frameInput.Move.x;
        Vector2 velocity = _rb.linearVelocity;

        // Simple swing control � push tangentially
        velocity.x = Mathf.MoveTowards(
            velocity.x,
            horizontal * _stats.MaxSpeed,
            _stats.Acceleration * Time.fixedDeltaTime
        );

        if (Mathf.Abs(_frameInput.Move.y) > 0.1f)
        {
            float reelSpeed = 5f; 
            _rb.linearVelocity += Vector2.up * (_frameInput.Move.y * reelSpeed);
        }

        _rb.linearVelocity = velocity;
    }


    #endregion

    #region Dashing

    public void AddExternalVelocity(Vector2 velocity, float duration)
    {
        StartCoroutine(ExternalVelocityCoroutine(velocity, duration));
    }

    private IEnumerator ExternalVelocityCoroutine(Vector2 velocity, float duration)
    {
        _frameVelocity.y = 0;
        IsDashing = true;
        float timer = 0f;

        while (timer < duration)
        {
            _rb.linearVelocity = velocity;
            timer += Time.deltaTime;
            yield return null;
        }
        _gravityMultiplier = 0.8f;
        _rb.linearVelocity = Vector2.zero;

        IsDashing = false;
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        #region Run

        // calculate direction
        float targetSpeed = _frameInput.Move.x * _stats.MaxSpeed; // change stat name to moveSpeed?

        // diffetence between current and target
        float speedDiff = targetSpeed - _rb.linearVelocity.x;

        // determine acceletration rate 
        float accelRate = (Math.Abs(targetSpeed) > 0.01f) ? _stats.Acceleration : _stats.GroundDeceleration;

        movement.x = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, _stats.VelocityPower) * Mathf.Sign(speedDiff);

        #endregion

        #region Friction

        if (_grounded && Math.Abs(_frameInput.Move.x) < 0.01f)
        {
            // use either friction or deceleration
            frictionAmount = Math.Min(Math.Abs(_rb.linearVelocity.x), _stats.FrictionAmount);
            // sets direction
            frictionAmount *= Mathf.Sign(_rb.linearVelocity.x);
        }
        else
        {
            frictionAmount = 0f;
        }

        #endregion
    }

    private void HandleSpriteDirection()
    {
        if (_frameInput.Move.x > 0.01f)
            _spriteRenderer.flipX = false; 
        else if (_frameInput.Move.x < -0.01f)
            _spriteRenderer.flipX = true;  
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (IsDashing) return;

        if (IsClimbing) return;

        if (_rb.linearVelocity.y < 0)
        {
            _rb.gravityScale = _stats.GravityScale * _stats.FallGravityMultiplier;
        }
        else 
        {
            _rb.gravityScale = _stats.GravityScale;
        }
        
    }
    #endregion

    public float GetFacingDirection()
{
    return _spriteRenderer.flipX ? -1f : 1f;
}


    private void ApplyMovement()
    {
        if (IsDashing) return;

        // aplies force to rigidbody (x axis)
        _rb.AddForce(movement.x * Vector2.right);
        // applies friction (x axis)
        _rb.AddForce(-frictionAmount * Vector2.right, ForceMode2D.Force);

        // _rb.linearVelocity = _frameVelocity;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
}

