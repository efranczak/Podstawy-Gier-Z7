using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ShopItemDisplay : MonoBehaviour
{
    private UpgradeData data;

    private bool canBuy = false;
    private PlayerController playerRef;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    
    public void Setup(UpgradeData newData)
    {
        data = newData;
        GetComponent<SpriteRenderer>().sprite = data.iconUI;

        gameObject.name = "ShopItem_" + data.upgradeName;

        descriptionText = GameObject.Find("Description").GetComponent<TMP_Text>();
        priceText = GameObject.Find("Price").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (canBuy && (Input.GetKeyDown(KeyCode.W)||Input.GetKey(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton4)))
        {
            if (playerRef.GetComponent<ScoreManager>().coins > data.price)
            {
                playerRef.GetComponent<ScoreManager>().coins -= data.price;
                playerRef.GetComponent<ScoreManager>().coinsText.text = "Apples: " + playerRef.GetComponent<ScoreManager>().coins.ToString();
                BuyItem();
            }
        }
    }

    private void BuyItem()
    {
        if (data != null && playerRef != null)
        {
            Debug.Log($"Kupiono ulepszenie: {data.upgradeName}");

            data.ApplyUpgrade(playerRef);

            ActiveUpgradesContainer uiContainer = FindAnyObjectByType<ActiveUpgradesContainer>();

            if (uiContainer != null)
            {
                if (data.category == UpgradeCategory.Permanent)
                {
                    uiContainer.UpdateActiveUpgrades(data);
                }
                else if (data.category == UpgradeCategory.Consumable)
                {
                    uiContainer.UpdateConsumableUpgrades(data);
                }
            }
            else
            {
                Debug.LogWarning("Nie znaleziono ActiveUpgradesContainer na scenie!");
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canBuy = true;
            playerRef = other.GetComponent<PlayerController>();
            descriptionText.text = data.description;
            priceText.text = "Price: " + data.price.ToString() + " apples";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canBuy = false;
            playerRef = null;
            descriptionText.text = null;
            priceText.text = null;
        }
    }
}