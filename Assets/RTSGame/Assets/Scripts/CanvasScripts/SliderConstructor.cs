using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class SliderConstructor
{
    public GameObject bar { get; private set; }
    public Image fill { get; private set; }
    public TextMeshProUGUI textMax { get; private set; }
    public TextMeshProUGUI textCurrent { get; private set; }
    public SliderConstructor(GameObject bar)
    {
        this.bar = bar;
        fill = bar.transform.GetChild(0).GetComponent<Image>();
        textMax = bar.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        textCurrent = bar.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

    }
    public void ChangeRotation()
    {
        if(textMax.alignment == TextAlignmentOptions.TopLeft)
        {
            textMax.alignment = TextAlignmentOptions.TopRight;
            textCurrent.alignment = TextAlignmentOptions.TopRight;
            bar.GetComponent<Slider>().direction = Slider.Direction.RightToLeft;
        }
        else
        {
            textMax.alignment = TextAlignmentOptions.TopLeft;
            textCurrent.alignment = TextAlignmentOptions.TopLeft;
            bar.GetComponent<Slider>().direction = Slider.Direction.LeftToRight;
        }
    }
    public void ChangeColor(Color color)
    {
        fill.color = color;
    }

    public void ChangeSlider(int cur, int max)
    {

        float value = (float)cur / (float)max;
        if (value <= 1)
        {
            textMax.text = max.ToString();
            textCurrent.text = cur.ToString();

            bar.GetComponent<Slider>().value = value;
        }

    }
}
