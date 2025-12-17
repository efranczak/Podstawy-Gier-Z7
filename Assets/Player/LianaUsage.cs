using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class LianaUsage : MonoBehaviour
{
    [Header("Zachowanie")]
    [SerializeField] private bool allowJumpOff = true;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float jumpOffForce = 15f;
    [SerializeField] private float sideJumpForce = 5f;
    [SerializeField] private float snapSmoothness = 15f;
    [SerializeField] private float climbCooldownTime = 0.2f;

    private PlayerController _player;
    private Transform _currentLiana;
    private PlayerInputActions _inputActions;

    private bool _canClimb = false;
    private bool _isClimbing = false;
    private float _cooldownTimer = 0f;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    private void Update()
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        Vector2 moveInput = _player.FrameInput;
        bool jumpPressed = _inputActions.Player.Jump.WasPressedThisFrame();

        if (_canClimb && !_isClimbing && _cooldownTimer <= 0)
        {
            if (Mathf.Abs(moveInput.y) > 0.4f)
            {
                StartClimbing();
            }
        }

        if (_isClimbing)
        {
            HandleClimbingLogic(moveInput, jumpPressed);
        }
    }

    private void FixedUpdate()
    {
        if (_isClimbing && _currentLiana != null)
        {
            float targetX = _currentLiana.position.x;
            float currentX = transform.position.x;

            float newX = Mathf.Lerp(currentX, targetX, snapSmoothness * Time.fixedDeltaTime);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    private void HandleClimbingLogic(Vector2 moveInput, bool jumpPressed)
    {
        _player.AddExternalHorizontalVelocity(0f, Time.deltaTime * 2f);

        _player.SetVelocity(new Vector2(0f, moveInput.y * climbSpeed));

        if (allowJumpOff && jumpPressed)
        {
            PerformJumpOff(moveInput.x);
        }
    }

    private void StartClimbing()
    {
        _isClimbing = true;
        _player.IsClimbing = true;
        _player.IsGrappling = false;
        _player.SetVelocity(Vector2.zero);
    }

    private void StopClimbing()
    {
        _isClimbing = false;
        _player.IsClimbing = false;
    }

    private void PerformJumpOff(float xInput)
    {
        StopClimbing();

        _cooldownTimer = climbCooldownTime;

        _player.ForceJump(jumpOffForce);

        float xVelocity = 0f;

        if (Mathf.Abs(xInput) > 0.1f)
        {
            float dir = Mathf.Sign(xInput);
            xVelocity = dir * sideJumpForce;
        }

        _player.SetVelocity(new Vector2(xVelocity, jumpOffForce));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Liana"))
        {
            _canClimb = true;
            _currentLiana = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Liana"))
        {
            _canClimb = false;
            _currentLiana = null;

            if (_isClimbing)
            {
                StopClimbing();
            }
        }
    }
}