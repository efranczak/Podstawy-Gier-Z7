using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float climbIdleThreshold = 0.05f;

    private PlayerController _controller;
    private Rigidbody2D _rb;
    private bool _isGrounded;

    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int YVelocityHash = Animator.StringToHash("YVelocity");
    private static readonly int IsGroundingHash = Animator.StringToHash("isGrounding");
    private static readonly int IsClimbingHash = Animator.StringToHash("isClimbing");

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        if (_controller != null)
        {
            _controller.GroundedChanged += OnGroundedChanged;
        }
    }

    private void OnDisable()
    {
        if (_controller != null)
        {
            _controller.GroundedChanged -= OnGroundedChanged;
        }
    }

    private void Update()
    {
        if (animator == null || _rb == null || _controller == null)
        {
            return;
        }

        float speed = Mathf.Abs(_rb.linearVelocity.x);
        bool isRunning = speed > 0.1f;

        animator.SetBool(IsRunningHash, isRunning);
        animator.SetFloat(YVelocityHash, _rb.linearVelocity.y);
        animator.SetBool(IsGroundingHash, _isGrounded);
        animator.SetBool(IsClimbingHash, _controller.IsClimbing);

        bool isClimbingIdle = _controller.IsClimbing && Mathf.Abs(_rb.linearVelocity.y) <= climbIdleThreshold;
        animator.speed = isClimbingIdle ? 0f : 1f;
    }

    private void OnGroundedChanged(bool isGrounded, float _)
    {
        _isGrounded = isGrounded;
    }
}
