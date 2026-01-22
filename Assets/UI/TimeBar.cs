using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int sliderMaxValue;
    [SerializeField] private SnakeDistanceHandler handler;
    [SerializeField] private Image fill;


    void Start()
    {
        slider.maxValue = sliderMaxValue;
        slider.value = Mathf.Min(handler.GetDistance(), sliderMaxValue);
    }

    void Update()
    {

        slider.value = Mathf.Min(handler.GetDistance(), sliderMaxValue);
    }
}
