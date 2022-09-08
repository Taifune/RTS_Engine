using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Image healthBarFill;
    public Text healthBarText;
    public Gradient gradient;
    public Slider healthBarSlider;
    public float maxHealth;
             
    public void SetMaxHealth(float Health)
    {
        healthBarSlider.maxValue = Health;
        healthBarSlider.value = Health;
        maxHealth = Health;

        gradient.Evaluate(1f);

        int min = (int)Health ;
        int max = (int)maxHealth;

        healthBarText.text = min + "/" + max;

       // Debug.Log(min + "   " + max +"     " + maxHealth);
    }

    public void SetHealth(float currenthealth)
    {
        healthBarSlider.value = currenthealth;
        healthBarFill.color = gradient.Evaluate(healthBarSlider.normalizedValue);
        //Debug.Log(currenthealth + "     " + maxHealth);
        healthBarText.text = (int)currenthealth + "/" + (int)maxHealth;

    }


    public void UpdateHealthBar(float Fill,string Text)
    {
        healthBarFill.fillAmount = Fill;
        healthBarText.text = Text;
    }

}
