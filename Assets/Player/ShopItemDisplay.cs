using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ShopItemDisplay : MonoBehaviour
{
    private UpgradeData data;

    private bool canBuy = false;
    private PlayerController playerRef;
    


    public void Setup(UpgradeData newData)
    {
        data = newData;
        GetComponent<SpriteRenderer>().sprite = data.iconUI;

        gameObject.name = "ShopItem_" + data.upgradeName;
    }

    private void Update()
    {
        if (canBuy && Input.GetKeyDown(KeyCode.W))
        {
            BuyItem();
        }
    }

    private void BuyItem()
    {
        if (data != null && playerRef != null)
        {
            Debug.Log($"Kupiono ulepszenie: {data.upgradeName}");

            data.ApplyUpgrade(playerRef);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canBuy = true;
            playerRef = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canBuy = false;
            playerRef = null;
        }
    }
}