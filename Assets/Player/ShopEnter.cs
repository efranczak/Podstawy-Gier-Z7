using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopEnter : MonoBehaviour
{

    public GameObject snake;
    public GameObject[] uselessUI;
    public RawImage Enter;
    public RawImage Exit;


    private bool canEnter;
    private bool isInsideShop = false;

    private Vector3 playerPos;

    private GameObject currentDoorTrigger;
    private GameObject activeShopDoor;

    private const string ENTER_TEXT = "[W] to enter the shop";
    private const string EXIT_TEXT = "[W] to exit the shop";


    private void Start()
    {
        snake = GameObject.FindWithTag("Snake");
        if (Enter != null)
            Enter.enabled = false;

        if (Exit != null)
            Exit.enabled = false;

    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton4)) && canEnter)
        {
            if (!isInsideShop)
            {
                if (Enter != null) Enter.enabled = false;
                if (Exit != null) Exit.enabled = false;


                playerPos = transform.position;

                activeShopDoor = currentDoorTrigger;

                isInsideShop = true;

                foreach (GameObject uselessElement in uselessUI)
                {
                    if (uselessElement != null)
                    {
                        uselessElement.SetActive(false);
                    }
                }
                SnakeScript.Instance.gameObject.SetActive(false);

                DontDestroyOnLoad(gameObject);
                SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
                transform.position = Vector3.zero;
            }
            else
            {
                if (Enter != null) Enter.enabled = false;
                if (Exit != null) Exit.enabled = false;

                foreach (GameObject uselessElement in uselessUI)
                {
                    if (uselessElement != null)
                    {
                        uselessElement.SetActive(true);
                    }
                }
                isInsideShop = false;
                SceneManager.UnloadSceneAsync("ShopScene");
                SnakeScript.Instance.gameObject.SetActive(true);
                activeShopDoor.GetComponent<Collider2D>().enabled = false;
                transform.position = playerPos;

                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ShopEntrance"))
        {
            canEnter = true;
            currentDoorTrigger = other.gameObject;

            if (!isInsideShop)
            {
                if (Enter != null) Enter.enabled = true;
                if (Exit != null) Exit.enabled = false;
            }
            else
            {
                if (Enter != null) Enter.enabled = false;
                if (Exit != null) Exit.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ShopEntrance"))
        {
            canEnter = false;
            currentDoorTrigger = null;

            if (Enter != null) Enter.enabled = false;
            if (Exit != null) Exit.enabled = false;
        }
    }
}
