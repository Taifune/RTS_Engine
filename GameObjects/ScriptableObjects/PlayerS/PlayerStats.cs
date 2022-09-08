using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScrObj/PlayerStats")]

public class PlayerStats : ScriptableObject
{
    public string playerName;
    public int playerColor;
    public int playerID;
    public int Resources;
    public int Capacity;

    
}
