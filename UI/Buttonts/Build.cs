using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    public GameObject Blueprint;
    public string MyTooltip;

    private BlueprintScript blueprintScript;
    private BuildingsManager buildingsManager;

    public bool Placing = false;
    

    public void Start()
    {
        buildingsManager = FindObjectOfType<BuildingsManager>();
        blueprintScript = Blueprint.GetComponent<BlueprintScript>();
    }



    public void SpawnBlueprint()
    {
        if(buildingsManager.SelectedBuilding!=null)
        {
            if (!buildingsManager.SelectedBuilding.isBusy)
            {
                if (!buildingsManager.isPlacing)
                {
                    Instantiate(Blueprint, new Vector3(0, 1, 0), Quaternion.identity, buildingsManager.BuildingsFolder);
                }
            }
            else
            {
                Debug.Log(buildingsManager.SelectedBuilding.buildingStats.Name + " is busy!");
            }
        }
        else
        {

            Instantiate(Blueprint, new Vector3(0, 1, 0), Quaternion.identity, buildingsManager.BuildingsFolder);

        }
       


    }


    public void StartBlueprintProduction()
    {
       
        if (!buildingsManager.SelectedBuilding.isReadyForPlacing)
        {

            if (!buildingsManager.SelectedBuilding.isBusy)
            {


                if (buildingsManager.ResourecesForConsumingCheck(blueprintScript.buildingPrefab.buildingStats.playerID,blueprintScript.buildingPrefab.buildingStats.buildingCost))
                {

                    RTSGameManager.StartBlueprintProduction(Blueprint, buildingsManager,true);

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
        else
        {
            buildingsManager.PlaceBuildingButton();

            Debug.Log("Place building!"); //Сюда поидее не должно попадать.

        }


    }




    public void MakeConstructionSite()
    {
        if (!buildingsManager.isPlacing)
        {
           var BlueprintForSite = Instantiate(Blueprint, new Vector3(0, 1, 0), Quaternion.identity, buildingsManager.BuildingsFolder);

            BlueprintForSite.GetComponent<BlueprintScript>().SiteConstruction = true;
        }
    }




    public void HoverOver()
    {
        if (buildingsManager.SelectedBuilding != null)
        {
            if (!buildingsManager.SelectedBuilding.isReadyForPlacing)
            {
                Tooltip.ShowTooltipStatic
                           (
                           MyTooltip + "\n" +
                           blueprintScript.buildingPrefab.buildingStats.Name + "\n" +
                           "Cost: " + blueprintScript.buildingPrefab.buildingStats.buildingCost + " Resource. \n" +
                           "Building time: " + blueprintScript.buildingPrefab.buildingStats.buildingSpeed + " Seconds."
                           );
            }
            else
            {
                Tooltip.ShowTooltipStatic("Place " + blueprintScript.buildingPrefab.buildingStats.Name);

            }
        }
        else
        {
            Tooltip.ShowTooltipStatic
                           (
                           MyTooltip + "\n" +
                           blueprintScript.buildingPrefab.buildingStats.Name + "\n" +
                           "Cost: " + blueprintScript.buildingPrefab.buildingStats.buildingCost + " Resource. \n" +
                           "Building time: " + blueprintScript.buildingPrefab.buildingStats.buildingSpeed + " Seconds."
                           );
        }
       


    }

    public void HoverOut()
    {
        Tooltip.HideTooltipStatic();
    }




}
