using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScrObj/AI/WorldObjectStats")]

public class WorldObjectStats : ScriptableObject
{
    //Определение полей для создания потом экземпляров ScriptableObject

    public int playerID;

    public bool isCapturable;

    public bool isDestructble;

    public float health;

    public float attackRange;
    public float attackSpeed;

    public float damage;
    public float armor;

    public float Size;

    

    public bool ProduceResources;
    public int ResourceAmmount;
    public float LoadingTime;
    public int ResourceRegeneration;


}

    




