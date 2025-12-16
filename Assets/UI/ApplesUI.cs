using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ApplesUI : MonoBehaviour
{
    public GameObject container;
    public Color collectedColor = Color.red;
    public Image applePrefab;

    private List<Image> apples = new List<Image>();
    

    public void CreateApples(int amount)
    {
        foreach (Transform child in container.transform)
            Destroy(child.gameObject);

        apples.Clear();

        for (int i = 0; i < amount; i++)
        {
            Image apple = Instantiate(applePrefab, transform);
            apple.color = Color.white;
            apples.Add(apple);
        }
    }

    public void setApples(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            apples[i].color = collectedColor;
        }

    }

    public void ResetApples()
    {
        foreach (var apple in apples)
            apple.color = Color.white;
    }
}
