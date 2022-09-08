using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Research : MonoBehaviour
{
    public TechStats Tech;
    public TechManager techManager;
    public bool Researching;
    public string MyTooltip;
    
    // Start is called before the first frame update
    void Start()
    {
        techManager = FindObjectOfType<TechManager>();
    }

    public void Button_StartResearch()
    {
        var currentResearchCenter = techManager.buildingsManager.SelectedBuilding;
        
        if(!currentResearchCenter.isBusy)
        {
            if (!Researching)
            {
                if (!techManager.ResearchedTechs.Contains(Tech))
                {
                    if (techManager.ResourecesForResearchCheck(BuildingsManager.Instance.SelectedBuilding.buildingStats.playerID,Tech.techCost))
                    {
                        Researching = true;

                        var ID = techManager.buildingsManager.SelectedBuilding.buildingStats.playerID;
                        var COST = Tech.techCost;
                        var RMANAGER = techManager.resourceManager;

                        RTSGameManager.ConsumeResourcesForTechResearching(ID, COST, RMANAGER);



                        currentResearchCenter.StartTechResearch(Tech, this);

                    }
                    else
                    {
                        Debug.Log("Not enough of resource!");
                    }
                }
                else
                {
                    Debug.Log("Tech is aready reserched!");
                }
            }
            else
            {
                Debug.Log("Reserching in progress");
            }
        }
        else
        {
            Debug.Log("This Research center is busy");
        }
       
  
    }

    public void HoverOver()
    {

        Tooltip.ShowTooltipStatic(MyTooltip + "\n" + Tech.techName + "\n" + "Cost: " + Tech.techCost + " Resource. \n" + "Production time: " + Tech.techTimeForResearch + " Seconds.");

    }

    public void HoverOut()
    {
        Tooltip.HideTooltipStatic();
    }


}
