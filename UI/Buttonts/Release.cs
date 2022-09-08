using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Release : MonoBehaviour
{
    public BuildingsManager buildingsManager;
    public Button button;
    public string MyTooltip;
    private RepairStantion RepStantion;
    //public bool State;

    // Start is called before the first frame update
    void Start()
    {
        buildingsManager = FindObjectOfType<BuildingsManager>();
        button = transform.GetComponent<Button>();
    }


    public void ReleaseButton()
    {
        RepStantion = buildingsManager.SelectedBuilding.GetComponent<RepairStantion>();

        RepStantion.ReleaseUnitButton();

        //State = RepStantion.isReloadProcess;

    }

    public void HoverOver()
    {
        RepStantion = buildingsManager.SelectedBuilding.GetComponent<RepairStantion>();

        if(RepStantion.ZoneOcupied)
        {
            button.interactable = true;
            Tooltip.ShowTooltipStatic(MyTooltip + " " + RepStantion.unitInRepairZone.unitStats.unitName);
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
