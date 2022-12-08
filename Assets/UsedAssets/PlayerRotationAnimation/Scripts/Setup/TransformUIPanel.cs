using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformUIPanel : MonoBehaviour
{
    public Text title;
    public Slider xSlider;
    public Slider ySlider;
    public Slider zSlider;

    //Populates the panel
    public void SetPanel(string name, Vector3 xLimits, Vector3 yLimits, Vector3 zLimits, out Slider xSlide, out Slider ySlide, out Slider zSlide)
    {
        title.text = name;
        SetSlider(xSlider, xLimits);
        SetSlider(ySlider, yLimits);
        SetSlider(zSlider, zLimits);
        xSlide = xSlider;
        ySlide = ySlider;
        zSlide = zSlider;
    }
    //Sets slider values
    void SetSlider(Slider slider, Vector3 values)
    {
        slider.minValue = values.x;
        slider.maxValue = values.y;
        slider.value = values.z;
    }
}
