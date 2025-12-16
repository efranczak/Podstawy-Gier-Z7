using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

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

    private LianaUsage _lianaUsage;

    private PlayerInputActions playerControls;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private Vector3 _lastCheckpoint;
    private SnakeScript _snake;
    private SnakeLogic _snakeLogic;


    public bool autoRun;


    // Dla wall jump
    [SerializeField] private WallJumpHandler _wallJumpHandler;

    private float _wallJumpTimer;
    private float _wallJumpVelocityX;
    private float _wallJumpDirection;
    private bool _isWallJumpingOverride;

    private float _externalVelocityX;
    private float _externalTimer;

    [Header("Wall Detection")]
    [SerializeField] private float _wallCheckDistance = 0.6f;
    [SerializeField] private LayerMask _wallLayer;

    public Collider2D LastWallCollider { get; private set; }



    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    public bool IsGrappling { get; set; }

    public bool IsDashing { get; private set; }

    public bool IsClimbing { get; set; }

    public bool IsTouchingWall { get; private set; }

    public float WallDirection { get; private set; }

    #endregion

    private float _time;



    private void Awake()
    {
        playerControls = new PlayerInputActions();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        IsGrappling = false;

        if (_wallJumpHandler == null) _wallJumpHandler = GetComponent<WallJumpHandler>();

        _lianaUsage = GetComponent<LianaUsage>();
        _snake = FindAnyObjectByType<SnakeScript>();
        _snakeLogic = FindAnyObjectByType<SnakeLogic>();

        _lastCheckpoint = transform.position;

    }

    private void OnEnable()
    {
        moveAction = playerControls.Player.Move;
        moveAction.Enable();

        jumpAction = playerControls.Player.Jump;
        jumpAction.Enable();

        dashAction = playerControls.Player.Dash;
        dashAction.Enable();

    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (dashAction != null) dashAction.Disable();

        if (playerControls != null)
            playerControls.Player.Disable();
    }

    private void OnDestroy()
    {
        if (playerControls != null)
        {
            playerControls.Player.Disable();
            playerControls.Dispose();
        }
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
            JumpDown = jumpAction.WasPressedThisFrame(),
            JumpHeld = jumpAction.IsPressed(),
            Move = moveAction.ReadValue<Vector2>()
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

        HandleWallJumpLogic();

        if (!_isWallJumpingOverride && _externalTimer <= 0)
        {
            HandleDirection();
            HandleSpriteDirection();
        }

        HandleGravity();

        ApplyMovement();
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        _frameVelocity = newVelocity;
        _rb.linearVelocity = newVelocity;
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            _gravityMultiplier = 1f;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            _lianaUsage.CanGrabLiana(true);

        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }


        WallDirection = GetFacingDirection();
        Vector2 wallCheckOrigin = _col.bounds.center;

        RaycastHit2D wallHit = Physics2D.Raycast(
            wallCheckOrigin,
            Vector2.right * WallDirection,
            _wallCheckDistance,
            _wallLayer
        );

        IsTouchingWall = wallHit.collider != null && !_grounded;

        LastWallCollider = wallHit.collider;


        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion

    #region Wall Jumping

    private void HandleWallJumpLogic()
    {
        if (!_isWallJumpingOverride) return;

        _wallJumpTimer -= Time.fixedDeltaTime;

        //Czy input w ta sama strone co kierunek wall jumpu? (tzn. jak sciana po prawo to kierunek wall jumpu uznajemy jest w lewo)
        bool tryingToMoveWithJump = Mathf.Sign(_frameInput.Move.x) == _wallJumpDirection && Mathf.Abs(_frameInput.Move.x) > 0.1f;

        // Blokada jest zdjeta gdy spelnione jedno z tych trzech:
        // 1. Minął czas timera
        // 2. Dotkneliśmy ziemi
        // 3. Imput gracza jest zgodny z kierunkiem wall jumpu
        if (_wallJumpTimer <= 0 || _grounded || tryingToMoveWithJump)
        {
            _isWallJumpingOverride = false;

            if (tryingToMoveWithJump)
            {
                _frameVelocity.x = _wallJumpVelocityX;
            }
        }
    }

    public void AddWallJumpVelocity(float velocityX, float duration)
    {
        _wallJumpVelocityX = velocityX;
        _wallJumpTimer = duration;
        _wallJumpDirection = Mathf.Sign(velocityX);
        // Aktywujemy blokadę, aby logika Wall Jump zrobila swoje
        _isWallJumpingOverride = true;
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
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_wallJumpHandler != null && _wallJumpHandler.HasCoyoteTime)
        {
            return;
        }

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    public void ForceJump(float jumpPowerOverride = -1)
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = jumpPowerOverride > 0 ? jumpPowerOverride : _stats.JumpPower;
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

    public void AddExternalHorizontalVelocity(float velocityX, float duration)
    {
        _externalVelocityX = velocityX;
        _externalTimer = duration;
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

        if (autoRun)
        {                
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            return;
        }
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
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

        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * _gravityMultiplier * Time.fixedDeltaTime);
        }

        if (_wallJumpHandler != null && _wallJumpHandler.IsWallSliding)
        {
            if (_frameVelocity.y < -_wallJumpHandler.WallSlideSpeed)
            {
                _frameVelocity.y = -_wallJumpHandler.WallSlideSpeed;
            }
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

        Vector2 finalVelocity = _frameVelocity;

        if (_isWallJumpingOverride)
        {
            finalVelocity.x = _wallJumpVelocityX;
        }

        else if (_externalTimer > 0)
        {
            finalVelocity.x = _externalVelocityX;
            _externalTimer -= Time.fixedDeltaTime;
        }

        _rb.linearVelocity = finalVelocity;
    }

    #region Respawn Logic

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        _lastCheckpoint = checkpointPosition;
    }

    public void RespawnAtLastCheckpoint()
    {
        if (_snake.GetPosition().x >= _lastCheckpoint.x) _snakeLogic.PlayerDefeated();
        transform.position = _lastCheckpoint;
    }

    #endregion

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