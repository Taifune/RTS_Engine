using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSGameManager : MonoBehaviour
{

    

    #region UnitsEvents

    //Событие получения урона. Здесь например можно обсчитывать формулы получения урона.
    //И возвращать в цель уже итоговое значение где вызывается анимация и записываются данные.

    public static void UnitTakeDamage(Unit Attacker,Unit Target)
    {

        UnitHaveActivity(Attacker, FindObjectOfType<UnitManager>());
        UnitHaveActivity(Target, FindObjectOfType<UnitManager>());

        var damage = Random.Range(Attacker.unitStats.minDamage, Attacker.unitStats.maxDamage) - Target.unitStats.armor;

        if(damage<=0)
        {
            damage = 0;
        }

        Target.TakeDamage(Attacker, damage);

    }

    public static void UnitTakeHealingFromStantion(RepairStantion Healer, Unit Target,bool ConsumeResources,ResourceManager Manager)
    {

        if(ConsumeResources)
        {
            Manager.ConsumeResources(Healer.building.buildingStats.playerID,Healer.healStats.healCost);
        }


        var healAmount =   Random.Range(Healer.healStats.minHealAmount, Healer.healStats.maxHealAmount);

        var healthDelta = Target.unitStats.health - Target.currentHealth;

        //Debug.Log(healAmount + "<hA    hD>" + healthDelta);

        if(healAmount>healthDelta)
        {
            healAmount = healthDelta;
            Healer.unitHaveFullHealth = true;
        }

        Target.Healing(Healer.transform, healAmount);


    }


    public static void UnitReciveAmmunitionFromStantion(RepairStantion Reloader,Unit Target,bool ConsumeResources,ResourceManager Manager)
    {
        if (ConsumeResources)
        {
            Manager.ConsumeResources(Reloader.building.buildingStats.playerID, Reloader.healStats.healCost* (int)Target.unitStats.minDamage);
        }

        var reloadAmount = (int) Reloader.healStats.minHealAmount;
        var ammoDelta = Target.unitStats.ammoAmount - Target.currentAmmo;

       // Debug.Log(reloadAmount + "<rA  aD>" + ammoDelta);

        if(reloadAmount>ammoDelta)
        {
            reloadAmount = ammoDelta;
            Reloader.unitHaveFullAmmo = true;
        }

        Target.ReceiveAmmo(Reloader.transform, reloadAmount);


    }

    
   
    public static void UnitKilled(Unit DeadUnit)
    {

        //Запуск событий связанных со смертью юнита. Счетчики и тд. пока просто запускает посмертный метод у самого юнита можно например запускать респавн юнита.
        DeadUnit.Death();

        if (DeadUnit.unitStats.playerID==2)
        {
            FindObjectOfType<Counters>().IncreaseEnemyCounter();
        }

    }

    public static void UnitConvertionToResouce(RepairStantion Seller, Unit UnitForConversion,ResourceManager Manager)
    {

        var HitPointsRatio = UnitForConversion.currentHealth / UnitForConversion.unitStats.health;
        var Amount = (int)(UnitForConversion.unitStats.productionCost * 0.5 * HitPointsRatio);
        Manager.GetResources(UnitForConversion.unitStats.playerID, Amount);
        Seller.ClearZone();
        UnitForConversion.Sell(Amount);

    }

        


    public static void UnitProduced(Unit NewUnit, UnitManager Manager)
    {

        Manager.AllUnits.Add(NewUnit);
        //Здесь можно проверять условия выполнения задачи по постройке количества определенных юнитов.
    }

    //Запись объект в список активных для отслеживания и обработки списка целей и удаления из этих списков погибших юнитов.
    public static void UnitHaveActivity(Unit ActiveUnit,UnitManager Manager)
    {
        if(!Manager.ActiveUnitsList.Contains(ActiveUnit))
        {
            Manager.ActiveUnitsList.Add(ActiveUnit);
        }
        

    }

    public static void StartUnitProduction(GameObject Product,BuildingsManager Manager)
    {
        Manager.SelectedBuilding.ProduceUnit(Product);
    }

    public static void StartBlueprintProduction(GameObject Product,BuildingsManager Manager,bool ConsumeResources)
    {
        //Manager.SelectedBuilding.ProduceBlueprint(Product);
    }

    public static void StartConstructionSite(GameObject BlueprintForSite, BuildingsManager Manager)
    {
        //Сделать вызов из блупринта установки призрака для рабочего

    }


    public static void UnitBuildingConstruction(Unit Builder,int currentPlayerID, int currentBuidlingCost, ResourceManager Manager)
    {
        Builder.AddFloatingText("Working",Builder.transform.position, Vector3.right * 2, Color.gray);
        if(Builder.unitStats.unitName!="MCV")
        {
            Manager.ConsumeResources(currentPlayerID, currentBuidlingCost);
        }
        
    }




    #endregion

    #region BuildingsEvents

    public static void SiteTakeWork(Unit Builder,BlueprintScript Target)
    {
        var amount = Random.Range(1, Builder.unitStats.workPower);

        Target.TakeWork(Builder, amount);

    }


    public static void BuildingTakeDamage(Unit Attacker, BuildingScript Target)
    {
        UnitHaveActivity(Attacker, FindObjectOfType<UnitManager>());
        BuildingHaveActivity(Target, FindObjectOfType<BuildingsManager>());

        var damage = Random.Range(Attacker.unitStats.minDamage, Attacker.unitStats.maxDamage) - Target.buildingStats.armor;

        Target.TakeDamage(Attacker, damage);

    }

    public static void BuildingDestroyed(BuildingScript DestroyedBuilding)
    {
        DestroyedBuilding.Destroyed();
    }

    public static void BuildingHaveActivity(BuildingScript ActiveBuilding, BuildingsManager Manager)
    {
        if (!Manager.ActiveBuildingsList.Contains(ActiveBuilding))
        {
            Manager.ActiveBuildingsList.Add(ActiveBuilding);
        }


    }

    public static void BuildingConstruction(int currentPlayerID, int currentBuidlingCost, ResourceManager Manager)
    {
        Manager.ConsumeResources(currentPlayerID, currentBuidlingCost);
    }

    public static void BlueprintProduction(int currentPlayerID, int currentBluepringCost, ResourceManager Manager)
    {
        Manager.ConsumeResources(currentPlayerID, currentBluepringCost);
    }

    public static void UnitProduction(int currentPlayerID, int currentUnitCost, ResourceManager Manager)
    {
        Manager.ConsumeResources(currentPlayerID, currentUnitCost);
    }


    public static void BlueprintPlaced(BlueprintScript blueprint, BuildingScript ProductionSource)
    {
        //Nothing to do now
    }



    public static void BlueprintProduced(BlueprintScript blueprint, BuildingScript ProductionSource)
    {
        //Nothing to do now

    }
    #endregion

    #region Loading/Unloading units

    public static void LoadFromVeinToUnit(Unit LoadingUnit,VeinScript targetVein)
    {
        if (targetVein.currentResourceAmmount >= LoadingUnit.unitStats.carryAmount)
        {

            var loadingAmount = targetVein.LoadingAmmount(LoadingUnit); //LoadingUnit.unitStats.carryAmount; //Если отнимать от этого значения то можно реализовать эффекты воздействия бафоф на добычу.

            targetVein.TakeGathering(LoadingUnit, loadingAmount);

        }
        else
        {
            LoadingUnit.Stop();
            Debug.Log("Vein is empty");
        }

        
    }

    public static void LoadFromCentralBuilding(Unit LoadingUnit, BuildingScript LoadingPlace)
    {
        if(BuildingsManager.Instance.ResourecesForConsumingCheck(LoadingUnit.unitStats.playerID,LoadingUnit.CurrentCarryAmount))
        {
            var LoadingAmount = LoadingUnit.currentSite.LoadingAmmount(LoadingUnit);  //LoadingUnit.CurrentCarryAmount;

            LoadingPlace.GainResource(LoadingUnit, LoadingAmount);
        }
        

        
    }
    
    //метод проходит через здание и вовзращается в менеджер после изменения статуса юнита на разгружен для увеличения ресурса.
    public static void UnloadUnitToCentralBuilding(Unit UnloadingUnit,BuildingScript UnloadingPlace)
    {
        var unloadingAmount = UnloadingUnit.CurrentCarryAmount;

        
        UnloadingPlace.CollectResource(UnloadingUnit, unloadingAmount);
    }
    
    //метод проходит через здание и вовзращается в менеджер после изменения статуса юнита на загружен для уменьшения ресурса.
    public static void UnloadUnitToBuildngSite(Unit UnloadingUnit, BlueprintScript UnloadingPlace)
    {
        var unloadingAmount = UnloadingPlace.StoragingAmountCheck(UnloadingUnit, UnloadingUnit.CurrentCarryAmount);

        UnloadingPlace.CollectResource(UnloadingUnit, unloadingAmount);

    }  


    //вернувшийся метод после изменения статуса разгружающегося юнита в корутине разгрузчика. 
    public static void GetResoucesFromUnloading(Unit UnloadingUnit,int Amount,ResourceManager Manager)
    {
        UnloadingUnit.AddFloatingText(Amount, UnloadingUnit.transform.position, Vector3.up, Color.yellow);
        Manager.GetResources(UnloadingUnit.unitStats.playerID, Amount);
    }
    //вернувшийся метод после изменения статуса разгружающегося юнита в корутине зaгрузчика. 
    public static void ConsumeResourcesForLoading(Unit LoadingUnit, int Amount, ResourceManager Manager)
    {

        LoadingUnit.AddFloatingText(Amount, LoadingUnit.transform.position, Vector3.up, Color.gray);
        Manager.ConsumeResources(LoadingUnit.unitStats.playerID, Amount);

    }


    public static void StorageResourcesFromUnloadingToSite(Unit UnloadingUnit,BlueprintScript StoragingSite ,int Amount)
    {
        
        UnloadingUnit.AddFloatingText(Amount, StoragingSite.transform.position, Vector3.up, Color.green);

        StoragingSite.StorageResource(UnloadingUnit, Amount);

    }

    #endregion

    #region Tech Events

    public static void ConsumeResourcesForTechResearching(int currentPlayerID, int currentTechCost, ResourceManager Manager)
    {
        Manager.ConsumeResources(currentPlayerID, currentTechCost);
        
    }

    public static void StartTechResearching(TechStats currentTech, BuildingsManager Manager)
    {
        
            //Manager.SelectedBuilding.StartTechResearch(currentTech);
        
    }


    #endregion

    #region World Objects Events


    public static GameObject ObjectRandomCheckSpawn(GameObject Object,int SpawnValue,Vector3 InstPos,Quaternion Qatr,Transform Parent)
    {
        var RndCheck = Random.Range(0, 100);


        if (RndCheck < 50)
        {
           var  Cross = Instantiate(Object, InstPos, Qatr, Parent);
           return Cross;
        }
        else
        {
           return null;
        }
            
       
    }


    //Пример того как можно вызывать увеличение счетчика при событии на карте.
    public static void CrossClicked()
    {
        FindObjectOfType<Counters>().IncreaseCrossCounter();
    }

    #endregion
}
