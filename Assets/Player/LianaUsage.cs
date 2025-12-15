using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class LianaUsage : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRigidbody;
    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float jumpHorizontalForce = 3f;
    [SerializeField] JumpHandler jumpHandler;
    [SerializeField] PlayerController playerController;
    [SerializeField] private float dropHoldTime = 0.2f;

    private bool canGrabLiana = true;

    private float dropTimer = 0f;

    private float vertical;
    private float horizontal;
    private bool isLiana;
    private bool isClimbing;
    private bool jumpedFromLiana = false;

    private Transform currentLiana;

    private float originalGravityScale;

    private RigidbodyConstraints2D originalConstraints;

    private PlayerInputActions inputActions;
    private InputAction jumpAction;
    private InputAction moveAction;

    #region Input System

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        jumpAction = inputActions.Player.Jump;
        moveAction = inputActions.Player.Move;
        jumpAction.Enable();
        moveAction.Enable();
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        moveAction.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }


    #endregion


    void Start()
    {

        originalGravityScale = playerRigidbody.gravityScale;    

        if (playerRigidbody == null)
        {
            enabled = false;
            return;
        }

        jumpHandler = GetComponentInChildren<JumpHandler>();
        if (jumpHandler == null)
        {
            Debug.LogError("LianaUsage: Skrypt JumpHandler nie znaleziono na tym samym obiekcie!");
        }

        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("LianaUsage: Skrypt PlayerController nie znaleziono na tym samym obiekcie!");
        }

        originalConstraints = playerRigidbody.constraints;

        Debug.Log($"LianaUsage START - originalConstraints = {originalConstraints}, bodyType = {playerRigidbody.bodyType}");
    }

    void Update()
    {
        vertical = moveAction.ReadValue<Vector2>().y;
        horizontal = moveAction.ReadValue<Vector2>().x;

        // wspinanie jeśli na lianie i poruszamy się w pionie, ale nie wciśnięto Space
        if (isLiana && canGrabLiana && Mathf.Abs(vertical) > 0.3f && !jumpAction.WasPressedThisFrame())
        {
            isClimbing = true;

            SnapToLianaCenter();

            // poprawka lian jest tu
            playerController.IsClimbing = true;
            playerController.SetVelocity(Vector2.zero);

            if (jumpHandler != null) jumpHandler.SetEnabled(false);
            // koniec poprawki
        }

        // Zejście/skok z liany po wciśnięciu Space
        if (isClimbing && jumpAction.WasPressedThisFrame() && isClimbing)
        {
            ExitLiana();
        }

        if (isClimbing && canGrabLiana && Mathf.Abs(horizontal) > 0.3f)
        {
            dropTimer += Time.deltaTime;

            if (dropTimer >= dropHoldTime)
            {
                DropFromLiana();
                dropTimer = 0f;
            }
        }
        else
        {
            dropTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            playerRigidbody.gravityScale = 0f;
            playerRigidbody.linearVelocity = new Vector2(0f, vertical * climbSpeed);
            playerController.SetVelocity(new Vector2(0f, vertical * climbSpeed));
        }
        else
        {
            playerRigidbody.gravityScale = originalGravityScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Liana"))
        {
            isLiana = true;
            currentLiana = collision.transform;

            Debug.Log("Wejście na lianę - isLiana = true");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Liana"))
        {
            if (!jumpedFromLiana) // ignoruj exit wywołany skokiem
            {
                isLiana = false;
                isClimbing = false;
                playerController.IsClimbing = false;
                playerRigidbody.gravityScale = originalGravityScale;
                playerRigidbody.linearVelocity = Vector2.zero;

                currentLiana = null;
                Debug.Log("Opuszczenie liany - przywrócono physics");
            }

            if (jumpHandler != null) jumpHandler.SetEnabled(true);

            jumpedFromLiana = false; // reset flagi
        }
    }

    private void ExitLiana()
    {
        jumpedFromLiana = true;
        isClimbing = false;
        canGrabLiana = false;
        playerController.SetVelocity(Vector2.zero);

        // natychmiast przywróć normalną fizykę
        playerRigidbody.gravityScale = originalGravityScale;
        playerRigidbody.linearVelocity = Vector2.zero;

        // poprawka lian jest tu
        jumpHandler._jumpCount = 1; // reset liczby skoków podczas wspinaczki, wejscie na liane traktuje jako dotkniecie ziemii (resetuje skoki)
        playerController.IsClimbing = false;

        playerController.ForceJump(jumpForce);
        // koniec poprawki

        StartCoroutine(PreventClimbForOneFixedUpdate());
    }

    private void DropFromLiana()
    {
        jumpedFromLiana = true;
        isClimbing = false;
        canGrabLiana = false;
        playerController.SetVelocity(new Vector2(0f, -2f)); 

        // natychmiast przywróć normalną fizykę
        playerRigidbody.gravityScale = originalGravityScale;
        playerRigidbody.linearVelocity = Vector2.zero;

        jumpHandler._jumpCount = 0;
        playerController.IsClimbing = false;

        StartCoroutine(PreventClimbForOneFixedUpdate());
    }


    private IEnumerator PreventClimbForOneFixedUpdate()
    {
        isClimbing = false;      // natychmiast wyłącz climbing
        vertical = 0f;           // zerujemy input wspinania
        yield return new WaitForFixedUpdate();
        jumpedFromLiana = false;
        canGrabLiana = true;
    }

    private void SnapToLianaCenter()
    {
        if (currentLiana != null)
        {
            Vector3 lianaPosition = currentLiana.position;
            Vector3 playerPosition = transform.position;
            transform.position = new Vector3(lianaPosition.x, playerPosition.y, -1f);
        }
    }

}
