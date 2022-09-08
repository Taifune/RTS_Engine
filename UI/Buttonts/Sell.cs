using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sell : MonoBehaviour
{
    public BuildingsManager buildingsManager;
    public Button button;
    public RepairStantion RepStantion;
    public string MyTooltip;
    //public bool State;

    // Start is called before the first frame update
    void Start()
    {
        buildingsManager = FindObjectOfType<BuildingsManager>();
        button = transform.GetComponent<Button>();
    }


    public void SellButton()
    {
        RepStantion = buildingsManager.SelectedBuilding.GetComponent<RepairStantion>();

        RepStantion.SellUnitButton();

        //State = RepStantion.isReloadProcess;

    }

    public void HoverOver()
    {
        RepStantion = buildingsManager.SelectedBuilding.GetComponent<RepairStantion>();

        if(RepStantion.ZoneOcupied)
        {
            button.interactable = true;
            var HitPointsRatio = RepStantion.unitInRepairZone.currentHealth / RepStantion.unitInRepairZone.unitStats.health;

            Tooltip.ShowTooltipStatic
                (
                MyTooltip + " " + RepStantion.unitInRepairZone.unitStats.unitName +
                "\n" + "For " + RepStantion.unitInRepairZone.unitStats.productionCost * 0.5 * HitPointsRatio + " Resource."
                );

        }
        else
        {
            button.interactable = false;
            Tooltip.ShowTooltipStatic
                (
                MyTooltip + " button"
                );
        }

        
       
    }

    public void HoverOut()
    {
        button.interactable = true;
        Tooltip.HideTooltipStatic();
    }

}
