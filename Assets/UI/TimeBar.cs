using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int sliderMaxValue;
    [SerializeField] private TimeHandler timeHandler;

    void Start()
    {
        slider.maxValue = sliderMaxValue;
        slider.value = Mathf.Min(timeHandler.getTime(), sliderMaxValue);
    }

    void Update()
    {
        slider.value = Mathf.Min(timeHandler.getTime(), sliderMaxValue);
    }
}
