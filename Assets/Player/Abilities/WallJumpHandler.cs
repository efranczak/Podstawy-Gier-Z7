using UnityEngine;

public class WallJumpHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private bool _enabled = false;

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

    private bool _isWallSliding;
    public bool IsWallSliding => _isWallSliding;

    public bool HasCoyoteTime => _wallJumpCoyoteCounter > 0;

    [Header("Handlers")]
    [SerializeField] private DashHandler _dashHandler;
    [SerializeField] private JumpHandler _jumpHandler;

    private float _wallJumpCoyoteCounter;
    private float _wallJumpBufferCounter;
    private float _lastWallDirection;

    private void Awake()
    {
        if (_jumpHandler == null) _jumpHandler = GetComponent<JumpHandler>();
        if (_dashHandler == null) _dashHandler = GetComponent<DashHandler>();
        if (_player == null) _player = GetComponent<PlayerController>();
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!_enabled) return;
        HandleWallSlideLogic();
    }

    private void Update()
    {
        if (!_enabled) return;

        if (Input.GetButtonDown("Jump"))
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
        bool isFalling = _rb.linearVelocity.y <= 0.01f;

        bool inputTowardsWall = Mathf.Sign(_player.FrameInput.x) == _player.WallDirection && Mathf.Abs(_player.FrameInput.x) > 0.01f;

        _isWallSliding = _player.IsTouchingWall && isFalling && inputTowardsWall;

        if (_isWallSliding)
        {
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