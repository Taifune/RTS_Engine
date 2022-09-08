using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counters : MonoBehaviour
{
    private int EnemyCounter;
    private int CrossCounter;

    public Text EnemyCounterUI;
    public Text CrossCounterUI;


    public void IncreaseEnemyCounter()
    {

        EnemyCounter += 1;

        UpdateCountersUI();

    }
    public void IncreaseCrossCounter()
    {
        CrossCounter += 1;

        UpdateCountersUI();
    }

    public void UpdateCountersUI()
    {
        EnemyCounterUI.text = EnemyCounter.ToString();
        CrossCounterUI.text = CrossCounter.ToString();
    }

}
