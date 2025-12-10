using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class LianaUsage : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRigidbody;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float jumpHorizontalForce = 3f;
    [SerializeField] JumpHandler jumpHandler;
    [SerializeField] PlayerController playerController;


    private float vertical;
    private bool isLiana;
    private bool isClimbing;
    private bool jumpedFromLiana = false;

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
        if (playerRigidbody == null)
        {
            Debug.LogError("LianaUsage: playerRigidbody nie jest przypisane!");
            enabled = false;
            return;
        }

        // poprawka lian jest tu
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
        // koniec poprawki

        originalConstraints = playerRigidbody.constraints;

        Debug.Log($"LianaUsage START - originalConstraints = {originalConstraints}, bodyType = {playerRigidbody.bodyType}");
    }

    void Update()
    {
        vertical = moveAction.ReadValue<Vector2>().y;

        // wspinanie jeśli na lianie i poruszamy się w pionie, ale nie wciśnięto Space
        if (isLiana && Mathf.Abs(vertical) > 0.1f && !jumpAction.WasPressedThisFrame())
        {
            isClimbing = true;

            // poprawka lian jest tu
            playerController.IsClimbing = true;
            // koniec poprawki
        }

        // Zejście/skok z liany po wciśnięciu Space
        if (isLiana && jumpAction.WasPressedThisFrame())
        {
            ExitLiana();
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            playerRigidbody.gravityScale = 0f;
            playerRigidbody.constraints = (originalConstraints | RigidbodyConstraints2D.FreezePositionX) & ~RigidbodyConstraints2D.FreezeRotation;
            playerRigidbody.linearVelocity = new Vector2(0f, vertical * climbSpeed);
        }
        else
        {
            playerRigidbody.gravityScale = 1f;
            playerRigidbody.constraints = originalConstraints;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Liana"))
        {
            isLiana = true;
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
                playerRigidbody.constraints = originalConstraints;
                playerRigidbody.gravityScale = 1f;
                Debug.Log("Opuszczenie liany - przywrócono physics");
            }

            jumpedFromLiana = false; // reset flagi
        }
    }

    private void ExitLiana()
    {
        jumpedFromLiana = true;
        isClimbing = false;
        isLiana = false;

        // natychmiast przywróć normalną fizykę
        playerRigidbody.gravityScale = 1f;
        playerRigidbody.constraints = originalConstraints;

        float h = moveAction.ReadValue<Vector2>().x;
        if (Mathf.Abs(h) < 0.1f) h = 1f;

        playerRigidbody.linearVelocity = new Vector2(
            h * jumpHorizontalForce,
            jumpForce
        );

        // poprawka lian jest tu
        jumpHandler._jumpCount = 0; // reset liczby skoków podczas wspinaczki, wejscie na liane traktuje jako dotkniecie ziemii (resetuje skoki)
        playerController.IsClimbing = false;
        // koniec poprawki

        StartCoroutine(PreventClimbForOneFixedUpdate());
    }


    private IEnumerator PreventClimbForOneFixedUpdate()
    {
        isClimbing = false;      // natychmiast wyłącz climbing
        vertical = 0f;           // zerujemy input wspinania
        yield return new WaitForFixedUpdate();
        jumpedFromLiana = false;
    }

}
