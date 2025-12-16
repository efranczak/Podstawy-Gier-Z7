using UnityEngine;
using UnityEngine.InputSystem;

public class WallJumpHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private bool _enabled = true;

    [SerializeField] private PlayerSkills _playerSkills;

    [Header("Wall Slide Settings")]
    [SerializeField] private float _wallSlideSpeed = 2f;
    public float WallSlideSpeed => _wallSlideSpeed;

    [Header("Wall Jump Settings")]
    [SerializeField] private float _wallJumpPowerX = 3f;
    [SerializeField] private float _wallJumpPowerY = 20f;
    [SerializeField] private float _wallJumpDuration = 0.1f;

    [Header("Input Forgiveness")]
    [SerializeField] private float _wallJumpCoyoteTime = 0.15f;
    [SerializeField] private float _wallJumpBufferTime = 0.15f;

    [Header("Wall Jump Limits")]
    private int _currentSameWallJumpCount = 0;
    private Collider2D _lastWallJumpedFrom;

    private bool _isWallSliding;
    public bool IsWallSliding => _isWallSliding;

    public bool HasCoyoteTime => _wallJumpCoyoteCounter > 0;

    [Header("Handlers")]
    [SerializeField] private DashHandler _dashHandler;
    [SerializeField] private JumpHandler _jumpHandler;

    private float _wallJumpCoyoteCounter;
    private float _wallJumpBufferCounter;
    private float _lastWallDirection;

    private PlayerInputActions inputActions;
    private InputAction jumpAction;

    private void Awake()
    {
        if (_jumpHandler == null) _jumpHandler = GetComponent<JumpHandler>();
        if (_dashHandler == null) _dashHandler = GetComponent<DashHandler>();
        if (_player == null) _player = GetComponent<PlayerController>();
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();

        if (_playerSkills == null) _playerSkills = _player.GetComponent<PlayerSkills>();

        if (_playerSkills == null)
        {
            enabled = false;
        }

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        jumpAction = inputActions.Player.Jump;
        jumpAction.Enable();
        _player.GroundedChanged += OnGroundedChanged;

    }

    private void OnDisable()
    {
        jumpAction.Disable();
        _player.GroundedChanged -= OnGroundedChanged;
    }

    private void OnGroundedChanged(bool grounded, float velocity)
    {
        if (grounded)
        {
            _currentSameWallJumpCount = 0;
            _lastWallJumpedFrom = null;
        }
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }


    private void FixedUpdate()
    {
        if (_playerSkills.SameWallJumpMaxAmount == 0) return;
        HandleWallSlideLogic();
    }

    private void Update()
    {
        if (_playerSkills.SameWallJumpMaxAmount == 0) return;

        if (jumpAction.WasPressedThisFrame())
        {
            _wallJumpBufferCounter = _wallJumpBufferTime;
        }
        else
        {
            _wallJumpBufferCounter -= Time.deltaTime;
        }

        if (_wallJumpBufferCounter > 0 && _wallJumpCoyoteCounter > 0)
        {
            DoWallJump();
            _wallJumpBufferCounter = 0;
        }
    }

    private void HandleWallSlideLogic()
    {
        if (_playerSkills.SameWallJumpMaxAmount == 0) return;

        bool isFalling = _rb.linearVelocity.y <= 0.01f;
        bool inputTowardsWall = Mathf.Sign(_player.FrameInput.x) == _player.WallDirection && Mathf.Abs(_player.FrameInput.x) > 0.01f;

        _isWallSliding = _player.IsTouchingWall && isFalling && inputTowardsWall;

        if (_isWallSliding)
        {
            if (_player.LastWallCollider != _lastWallJumpedFrom)
            {
                _currentSameWallJumpCount = 0;
            }

            _wallJumpCoyoteCounter = _wallJumpCoyoteTime;
            _lastWallDirection = _player.WallDirection;

            if (_jumpHandler != null) _jumpHandler._jumpCount = 1;
            if (_dashHandler != null) _dashHandler.ResetDashChain();
        }
        else
        {
            _wallJumpCoyoteCounter -= Time.deltaTime;
        }
    }

    private void DoWallJump()
    {
        Collider2D currentWall = _player.LastWallCollider;

        if (currentWall != null && currentWall == _lastWallJumpedFrom)
        {
            if (_currentSameWallJumpCount >= _playerSkills.SameWallJumpMaxAmount) return;
            _currentSameWallJumpCount++;
        }
        else
        {
            _currentSameWallJumpCount = 1;
            _lastWallJumpedFrom = currentWall;
        }

        float direction = _isWallSliding ? _player.WallDirection : _lastWallDirection;

        float jumpDirectionX = -direction;

        if (_jumpHandler != null)
        {
            _jumpHandler._jumpCount = 1;
        }

        _player.ForceJump(_wallJumpPowerY);
        _player.AddWallJumpVelocity(jumpDirectionX * _wallJumpPowerX, _wallJumpDuration);
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
    }
}