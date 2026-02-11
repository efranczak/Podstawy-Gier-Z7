using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopEnter : MonoBehaviour
{

    public GameObject snake;
    public GameObject[] uselessUI;
    public TMP_Text enterShopText;
    public TMP_Text buyItemText;


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
        enterShopText.enabled = false;        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && canEnter)
        {
            if (!isInsideShop)
            {
                if (enterShopText != null)
                    enterShopText.enabled = false;

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
                if (enterShopText != null)
                    enterShopText.enabled = false;
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
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ShopEntrance"))
        {
            canEnter = true;
            currentDoorTrigger = other.gameObject;

            if (enterShopText != null)
            {
                enterShopText.text = isInsideShop ? EXIT_TEXT : ENTER_TEXT;
                enterShopText.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ShopEntrance"))
        {
            canEnter = false;
            currentDoorTrigger = null;

            if (enterShopText != null)
                enterShopText.enabled = false;
        }
    }
}
