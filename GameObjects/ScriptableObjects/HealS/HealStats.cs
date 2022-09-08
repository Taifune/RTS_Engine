using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScrObj/AI/HealStats")]

public class HealStats : ScriptableObject
{
    public float healSpeed;
    public float minHealAmount;
    public float maxHealAmount;
    public float healRange;
    public int healCost;

}
