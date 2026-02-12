using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Miejsca na pó³kach")]
    public Transform[] spawnPoints;

    public GameObject itemPrefab;

    [Header("Mo¿liwe ulepszenia")]
    public UpgradeData jumpData;
    public UpgradeData dashData;
    public UpgradeData wallData;
    public UpgradeData[] optionalUpgrades;

    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        ClearOldItems();
        SpawnShopItems();
    }

    public void SpawnShopItems()
    {
        PlayerSkills skills = player.GetComponent<PlayerSkills>();
        if (skills == null) return;

        List<UpgradeData> availableData = new List<UpgradeData>(optionalUpgrades);

        int ownedPermTypes = 0;
        if (skills.PlayerJumps > 1) ownedPermTypes++;
        if (skills.PlayerDashes > 0) ownedPermTypes++;
        if (skills.SameWallJumpMaxAmount > 0) ownedPermTypes++;

        int maxPermSlots = 2;

        bool hasJump = skills.PlayerJumps > 1;
        if (hasJump || ownedPermTypes < maxPermSlots)
        {
            availableData.Add(jumpData);
        }

        bool hasDash = skills.PlayerDashes > 0;
        if (hasDash || ownedPermTypes < maxPermSlots)
        {
            availableData.Add(dashData);
        }

        bool hasWall = skills.SameWallJumpMaxAmount > 0;
        if (hasWall || ownedPermTypes < maxPermSlots)
        {
            availableData.Add(wallData);
        }


        foreach (Transform spot in spawnPoints)
        {
            if (availableData.Count == 0) break;

            int randomIndex = Random.Range(0, availableData.Count);
            UpgradeData selectedData = availableData[randomIndex];

            GameObject newItem = Instantiate(itemPrefab, spot.position, Quaternion.identity);

            newItem.transform.SetParent(spot);

            ShopItemDisplay displayScript = newItem.GetComponent<ShopItemDisplay>();
            if (displayScript != null)
            {
                displayScript.Setup(selectedData);
            }

            availableData.RemoveAt(randomIndex);
        }
    }

    private void ClearOldItems()
    {
        ShopItemDisplay[] itemsToDestroy = FindObjectsByType<ShopItemDisplay>(FindObjectsSortMode.None);

        foreach (ShopItemDisplay item in itemsToDestroy)
        {
            Destroy(item.gameObject);
        }
    }
}