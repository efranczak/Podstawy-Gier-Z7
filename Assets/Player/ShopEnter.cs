using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopEnter : MonoBehaviour
{

    public GameObject snake;
    public GameObject[] uselessUI;

    private bool canEnter;
    private bool isInsideShop = false;

    private Vector3 playerPos;

    private GameObject currentDoorTrigger;
    private GameObject activeShopDoor;

    private void Start()
    {
        snake = GameObject.FindWithTag("Snake");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && canEnter)
        {
            if (!isInsideShop)
            {
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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ShopEntrance"))
        {
            canEnter = false;
            currentDoorTrigger = null;
        }
    }
}
