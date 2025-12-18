using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ApplesUI : MonoBehaviour
{
    public GameObject container;
    public Color collectedColor = Color.red;
    public Image applePrefab;

    private List<Image> apples = new List<Image>();
    private Color ogColor;
    
    public void Awake()
    {
        apples.Add(applePrefab);
        ogColor = applePrefab.color;
    }

    public void CreateApples(int amount)
    {
        if (apples.Count > 1)
            apples.RemoveRange(1, apples.Count);

        for (int i = 1; i < amount; i++)
        {
            Image apple = Instantiate(applePrefab, container.transform);
            apple.color = ogColor;
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
