using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour
{
    public Image ProgressBarFill;
    public Text ProgressBarText;
    public Gradient gradient;
    public Slider ProgressBarSlider;

    public void SetMaxAmmount(float Ammount)
    {
        ProgressBarSlider.maxValue = Ammount;
        ProgressBarSlider.value = Ammount;

        gradient.Evaluate(1f);

        int min = (int)Ammount;
        int max = (int)ProgressBarSlider.maxValue;

        //ResourceBarText.text = min + "/" + max;
    }

    public void SetCurrentAmmount(float currentAmmount)
    {
        ProgressBarSlider.value = currentAmmount;
        ProgressBarFill.color = gradient.Evaluate(ProgressBarSlider.normalizedValue);
        //ResourceBarText.text = (int)currentAmmount + "/" + (int)ResourceBarSlider.maxValue;
    }


    public void UpdateResourceBar(float Fill, string Text)
    {
        ProgressBarFill.fillAmount = Fill;
        //ResourceBarText.text = Text;
    }

}
