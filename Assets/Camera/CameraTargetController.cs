using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _playerTransform;

    [Header("Horizontal Settings")]
    [SerializeField] private float _xOffset = 2f;

    [Header("Vertical Settings")]
    [SerializeField] private float _yDampTime = 0.2f;
    [SerializeField] private float _verticalOffset = -0.5f;
    [SerializeField] private float _fallThreshold = 2.0f;

    [Header("Look Down Settings")]
    [SerializeField] private float _lookDownAmount = 3f;
    [SerializeField] private float _lookDownSpeed = 20f;

    private float _targetY;
    private float _lastGroundedY;
    private float _currentYVelocity;
    private bool _isGrounded;

    private Rigidbody2D _playerRb;

    private void Start()
    {
        transform.position = _playerTransform.position;
        _targetY = _playerTransform.position.y;
        _lastGroundedY = _playerTransform.position.y;
        _playerRb = _playerController.GetComponent<Rigidbody2D>();

        if (_playerController != null)
        {
            _playerController.GroundedChanged += OnGroundedChanged;
        }
    }

    private void OnDestroy()
    {
        if (_playerController != null)
        {
            _playerController.GroundedChanged -= OnGroundedChanged;
        }
    }

    private void OnGroundedChanged(bool grounded, float verticalVelocity)
    {
        _isGrounded = grounded;

        if (grounded)
        {
            _targetY = _playerTransform.position.y;
            _lastGroundedY = _playerTransform.position.y;
        }
    }

    private void LateUpdate()
    {
        if (_playerTransform == null) return;

        float targetX = _playerTransform.position.x + _xOffset;

        if (_isGrounded)
        {
            _targetY = _playerTransform.position.y;
        }
        else
        {
            if (_playerTransform.position.y < _lastGroundedY - _fallThreshold)
            {
                float lookDownTarget = _playerTransform.position.y - _lookDownAmount;
                _targetY = Mathf.Lerp(_targetY, lookDownTarget, Time.deltaTime * _lookDownSpeed);
            }
            else
            {
                _targetY = _lastGroundedY;
            }
        }

        float smoothY = Mathf.SmoothDamp(transform.position.y, _targetY + _verticalOffset, ref _currentYVelocity, _yDampTime);

        transform.position = new Vector3(targetX, smoothY, transform.position.z);
    }
}