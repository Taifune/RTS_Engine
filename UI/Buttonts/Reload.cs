using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reload : MonoBehaviour
{
    public BuildingsManager buildingsManager;
    public Button button;
    public string MyTooltip;
    public bool State;
    private RepairStantion RepStantion;

    // Start is called before the first frame update
    void Start()
    {
        buildingsManager = FindObjectOfType<BuildingsManager>();
        button = transform.GetComponent<Button>();
    }


    public void ReloadSwitchButton()
    {
        RepStantion = buildingsManager.SelectedBuilding.GetComponent<RepairStantion>();

        RepStantion.ReloadSwitchButton();

        State = RepStantion.isReloadProcess;

        Tooltip.HideTooltipStatic();    

        HoverOver();

    }

    public void HoverOver()
    {
        RepStantion = buildingsManager.SelectedBuilding.GetComponent<RepairStantion>();

        State = RepStantion.isReloadProcess;

        if (RepStantion.ZoneOcupied)
        {
            button.interactable = true;

            if (!State)
            {

                Tooltip.ShowTooltipStatic
                (
                "Start " + MyTooltip + " " + RepStantion.unitInRepairZone.unitStats.unitName +
                "\n" + "Cost per tick:" + RepStantion.healStats.healCost * RepStantion.unitInRepairZone.unitStats.minDamage + " Resource." +
                "\n" + "Tick Time" + RepStantion.healStats.healSpeed + " Seconds."
                );

            }
            else
            {
                Tooltip.ShowTooltipStatic
                (
                "Stop " + MyTooltip + " " + RepStantion.unitInRepairZone.unitStats.unitName
                );
            }
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
