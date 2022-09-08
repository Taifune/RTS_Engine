using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "ScrObj/AI/TechStats")]

public class TechStats : ScriptableObject
{

    public string techName;
    public Sprite techIcon;
    public int techCost;
    public float techTimeForResearch;
    public int techType;
    public int techEffect;


}
