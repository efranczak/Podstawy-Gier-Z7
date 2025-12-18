using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int sliderMaxValue;
    [SerializeField] private TimeHandler timeHandler;
    [SerializeField] private Image fill;


    void Start()
    {
        slider.maxValue = sliderMaxValue;
        slider.value = Mathf.Min(timeHandler.getTime(), sliderMaxValue);
    }

    void Update()
    {
        if (timeHandler.isSubtractingTime)
        {
            fill.color = Color.red;
        }
        else
        {
            fill.color = Color.blue;
        }

        slider.value = Mathf.Min(timeHandler.getTime(), sliderMaxValue);
    }
}
