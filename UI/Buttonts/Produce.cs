using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Produce : MonoBehaviour
{
    public Unit unit;
    public BuildingsManager buildingsManager;
    public string MyTooltip;

    public void Start()
    {
        buildingsManager = FindObjectOfType<BuildingsManager>();
    }

    public void StartProduction()
    {
        if(!buildingsManager.SelectedBuilding.isBusy)
        {
            if (buildingsManager.ResourecesForConsumingCheck(unit.unitStats.playerID,unit.unitStats.productionCost))
            {
                RTSGameManager.UnitProduction(unit.unitStats.playerID, unit.unitStats.productionCost, buildingsManager.resourceManager);
                RTSGameManager.StartUnitProduction(unit.gameObject, buildingsManager);
            }
            else
            {
                Debug.Log("Not enough of resource!");
            }
        }
        else
        {
            Debug.Log(buildingsManager.SelectedBuilding.buildingStats.Name + " is busy!");
        }
        
    }


    public void HoverOver()
    {
        
        Tooltip.ShowTooltipStatic(MyTooltip + "\n" + unit.unitStats.unitName + "\n" + "Cost: " + unit.unitStats.productionCost + " Resource. \n" + "Production time: "+ unit.unitStats.productionTime+" Seconds.");
                     
    }

    public void HoverOut()
    {
        Tooltip.HideTooltipStatic();
    }

}
