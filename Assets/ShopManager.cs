using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Miejsca na pó³kach")]
    public Transform[] spawnPoints;

    public GameObject itemPrefab;

    [Header("Mo¿liwe ulepszenia")]
    public UpgradeData[] possibleUpgrades;

    private void Start()
    {
        SpawnShopItems();
    }

    public void SpawnShopItems()
    {
        List<UpgradeData> availableData = new List<UpgradeData>(possibleUpgrades);

        foreach (Transform spot in spawnPoints)
        {
            if (availableData.Count == 0) break;

            int randomIndex = Random.Range(0, availableData.Count);
            UpgradeData selectedData = availableData[randomIndex];

            GameObject newItem = Instantiate(itemPrefab, spot.position, Quaternion.identity);

            ShopItemDisplay displayScript = newItem.GetComponent<ShopItemDisplay>();
            if (displayScript != null)
            {
                displayScript.Setup(selectedData);
            }


            availableData.RemoveAt(randomIndex);

        }
    }
}