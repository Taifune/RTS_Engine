using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "ScrObj/AI/UnitStats")]

public class UnitStats : ScriptableObject
{
    //Определение полей для создания потом экземпляров ScriptableObject

    public int playerID;
    public string unitName;
    public Sprite Icon;
    public float attackRange;
    public float attackSpeed;
    public float projectileSpeed;
    public float health;
    public float damage;
    public float minDamage;
    public float maxDamage;
    public float viewRange;
    public float viewAngle;
    public float viewSpeed;
    public float armor;
    public float moveStopRange;
    public int productionCost;
    public float productionTime;
    public bool canGatherResources;
    public bool canConstructBuilding;
    public float workSpeed;
    public float workPower;
    public int carryAmount;
    public int ammoAmount;



}
