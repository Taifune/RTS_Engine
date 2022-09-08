using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScrObj/AI/BuildingStats")]

public class BuildingStats : ScriptableObject
{
    //Определение полей для создания потом экземпляров ScriptableObject

    public int playerID;
    public int typeID;

    public string Name;
    public Sprite Icon;

    public float health;

    public float attackRange;
    public float attackSpeed;
    
    public float damage;
    public float armor;

    public float buildingSize;
    public float buildingRange;
    public int buildingCost;
    public float buildingSpeed;

    public bool canReciveResources;

    

}
