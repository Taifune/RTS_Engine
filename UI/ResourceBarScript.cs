using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarScript : MonoBehaviour
{
    public Image ResourceBarFill;
    public Text ResourceBarText;
    public Gradient gradient;
    public Slider ResourceBarSlider;

    public void SetMaxAmmount(float Ammount)
    {
        ResourceBarSlider.maxValue = Ammount;
        ResourceBarSlider.value = Ammount;

        gradient.Evaluate(1f);

        int min = (int)Ammount;
        int max = (int)ResourceBarSlider.maxValue;

        //ResourceBarText.text = min + "/" + max;
    }

    public void SetCurrentAmmount(float currentAmmount)
    {
        ResourceBarSlider.value = currentAmmount;
        ResourceBarFill.color = gradient.Evaluate(ResourceBarSlider.normalizedValue);
        //ResourceBarText.text = (int)currentAmmount + "/" + (int)ResourceBarSlider.maxValue;
    }


    public void UpdateResourceBar(float Fill, string Text)
    {
        ResourceBarFill.fillAmount = Fill;
        //ResourceBarText.text = Text;
    }

}
