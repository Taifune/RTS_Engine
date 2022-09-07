using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingsManager : MonoBehaviour
{
    public static BuildingsManager Instance;

    public Transform BuildingsFolder;

    public BuildingScript SelectedBuilding = null;
    public GameObject activeGUIElement;
    public GameObject activeMenuButton;
    public ResourceManager resourceManager;
    public UnitManager unitManager;
    public GUIManager guiManager;
    public SelectionManager selectionManager;

    //public Vector3 mousePosition;

    //public LayerMask FogOfWar;
    //private RaycastHit hit;
    

    public bool isPlacing = false;

    //public bool isInIteractionZone = true;

    public List<BuildingScript> AllBuildingsList = new List<BuildingScript>();
    public List<BuildingScript> ActiveBuildingsList = new List<BuildingScript>();
    public List<BuildingScript> CentralBuildingsOnMap = new List<BuildingScript>();

    public List<WorldObject> WorldObjectsList = new List<WorldObject>();


    #region Activities Lists 

    //Листы достпупных и недоступных функций для каждого здания.
    public List<GameObject> CentralBuildingActiveFunctions = new List<GameObject>();
    public List<GameObject> CentralBuildingInactiveFunctions = new List<GameObject>();

    public List<GameObject> BuildingActiveFunctions = new List<GameObject>();
    public List<GameObject> BuildingInactiveFunctions = new List<GameObject>();

    public List<GameObject> ReserchCenterActiveFunctions = new List<GameObject>();
    public List<GameObject> ReserchCenterInactiveFunctions = new List<GameObject>();

    public List<GameObject> RepairStantionsActiveFunctions = new List<GameObject>();
    public List<GameObject> RepairStantionsInactiveFunctions = new List<GameObject>();



    public List<GameObject> BuildingProductionList = new List<GameObject>();
    public List<GameObject> CentralBuildingProductionList = new List<GameObject>();
    public List<GameObject> CentralBuildingBuidingsList = new List<GameObject>();
    public List<GameObject> CentralBuildingTechList = new List<GameObject>();
    public List<GameObject> ReserchCenterTechsList = new List<GameObject>();
    public List<GameObject> RepairStantionsServicesList = new List<GameObject>();

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    #region Build Collider Zones

    public void ShowBuildZones()
    {
        foreach (var Building in AllBuildingsList)
        {
            Building.BuildZone.SetActive(true);
        }
        foreach (var Object in WorldObjectsList)
        {
            Object.BuildZone.SetActive(true);
        }
    }

    public void HideBuildZones()
    {
        foreach (var Building in AllBuildingsList)
        {
            Building.BuildZone.SetActive(false);
        }
        foreach (var Object in WorldObjectsList)
        {
            Object.BuildZone.SetActive(false);
        }
    }

    #endregion

    #region Selecting Metods

    public void DeselectBuilding()
    {
        if(SelectedBuilding!=null)
        {
            SelectedBuilding.MySelector.SetActive(false);
            SelectedBuilding.WayPoint.SetActive(false);
            SelectedBuilding.Selected = false;
            SelectedBuilding = null;

            CloseActiveGUIElement();
            ClearActiveGUIElement();
            
            
        }

        
        
    }

    public void CloseActiveGUIElement()
    {
        GUIManager.DeactivateGUIElement(activeGUIElement);
        
    }

    public void OpenActiveGUIElement()
    {
        GUIManager.ActivateGUIElement(activeGUIElement);
    }

    public void ClearActiveGUIElement()
    {
        activeGUIElement = null;
        activeMenuButton = null;
    }

    public void SelectBuilding(Transform transform)
    {
        DeselectBuilding();

        var nowSelectedBuilding = transform.GetComponent<BuildingScript>();

        nowSelectedBuilding.MySelector.SetActive(true);
        nowSelectedBuilding.Selected = true;
        nowSelectedBuilding.WayPoint.SetActive(true);

        SelectedBuilding = nowSelectedBuilding;
        selectionManager.isObjectSelected = true;
        UpdateBuildingControlsGUI();
    }

    #endregion

    #region Resource Checks

    public bool ResourecesForConsumingCheck(int playerID,int amountForCheck)
    {
        //Debug.Log(buildingCost);
       
        //Debug.Log(resourceManager.Player1Resources);


        if (amountForCheck <= resourceManager.Players[playerID].Resources)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    
    #endregion

    #region List Metods

    public void AddBuildingToList(GameObject Building,List<BuildingScript> List)
    {
        var buildingScript = Building.GetComponent<BuildingScript>();

        List.Add(buildingScript);
    }

    public void AddBuildingToLists(GameObject Building)
    {
        var buildingScript = Building.GetComponent<BuildingScript>();

        AllBuildingsList.Add(buildingScript);

        if(buildingScript.buildingStats.canReciveResources)
        {
            CentralBuildingsOnMap.Add(buildingScript);
        }

    }

  

    public void AddObjectToList(List<GameObject> List,GameObject ObjectForAdd,int buildingIDforGUIRefresh)
    {
       
        List.Add(ObjectForAdd);

        RefreshActiveGUIElement();

    }

    public void RemoveObjectFromList(List<GameObject> List, GameObject ObjetcForRemove, int buildingIDforGUIRefresh )
    {
        
        List.Remove(ObjetcForRemove);

        RefreshActiveGUIElement();

    }



    #endregion

    
    public void MoveObjectFromListToList(GameObject Object,List<GameObject> FromList,List<GameObject> ToList)
    {
        FromList.Remove(Object);
        ToList.Add(Object);
    }


    public void RefreshActiveGUIElement()
    {

        if (activeGUIElement != null)
        {
            if (activeMenuButton != null)
            {
                GUIManager.SimulateOnClick(activeMenuButton);
                //Debug.Log("SimulatedClick!");
            }
            else
            {
                UpdateBuildingControlsGUI();
            }

        }
    }

    public void ButtonMenuActivityCheck(GameObject MenuButton,List<GameObject> ListForCountCheck, List<GameObject> ActiveFunctionsList, List<GameObject> InactiveFunctionList)
    {
        if (ListForCountCheck.Count != 0)
        {

            if (InactiveFunctionList.Contains(MenuButton))
            {
                MoveObjectFromListToList(MenuButton, InactiveFunctionList, ActiveFunctionsList);

            }

        }
        else
        {
            if (ActiveFunctionsList.Contains(MenuButton))
            {
                MoveObjectFromListToList(MenuButton, ActiveFunctionsList, InactiveFunctionList);
            }
        }
    }

    
    public void UpdateBuildingControlsGUI()
    {
        if(SelectedBuilding!=null)
        {
            activeMenuButton = null;


            if (SelectedBuilding.GetComponent<ID>().groupID == 1)
            {
                //Refresh
                GUIManager.DeactivateList(guiManager.buildingControlsList);

                //Update dynamic elements status.
                ButtonMenuActivityCheck(guiManager.researchMenuButton, CentralBuildingTechList, CentralBuildingActiveFunctions, CentralBuildingInactiveFunctions);

                if (SelectedBuilding.isReadyForPlacing)
                {
                    CentralBuildingActiveFunctions.Add(guiManager.placeBuilding);
                    guiManager.StartFlashButton(guiManager.placeBuilding, Color.green, 0.5f);
                }

                //Window
                GUIManager.ActivateGUIElement(guiManager.buildingControls);
                //Static Elements            


                /*
                  
                 
                if (CentralBuildingTechList.Count != 0)
                {

                    if (CentralBuildingInactiveFunctions.Contains(guiManager.researchMenuButton))
                    {
                        MoveObjectFromListToList(guiManager.researchMenuButton, CentralBuildingInactiveFunctions, CentralBuildingActiveFunctions);

                    }

                }
                else
                {
                   if(CentralBuildingActiveFunctions.Contains(guiManager.researchMenuButton))
                    {
                        MoveObjectFromListToList(guiManager.researchMenuButton, CentralBuildingActiveFunctions, CentralBuildingInactiveFunctions);
                    }
                }
                */
                GUIManager.EnableInteractableForList(CentralBuildingActiveFunctions);
                GUIManager.ActivateList(CentralBuildingActiveFunctions);

                GUIManager.DisableInteractableForList(CentralBuildingInactiveFunctions);
                GUIManager.ActivateList(CentralBuildingInactiveFunctions);
                

                //GUIManager.ActivateGUIElement(guiManager.buildingMenuButton);
                //GUIManager.ActivateGUIElement(guiManager.produceMenuButton);
                //Dynamic Elements
                

                //Set Active Window
                activeGUIElement = guiManager.buildingControls;

            }
            if (SelectedBuilding.GetComponent<ID>().groupID == 2)
            {
                //Refresh
                GUIManager.DeactivateList(guiManager.buildingControlsList);
                //Window
                GUIManager.ActivateGUIElement(guiManager.buildingControls);
                //Static Elements
                GUIManager.ActivateList(BuildingActiveFunctions);
                GUIManager.EnableInteractableForList(BuildingActiveFunctions);

                GUIManager.ActivateList(BuildingInactiveFunctions);
                GUIManager.DisableInteractableForList(BuildingInactiveFunctions);


                //Set Active Window
                activeGUIElement = guiManager.buildingControls;

            }
            if (SelectedBuilding.GetComponent<ID>().groupID == 3)
            {
                //Refresh
                GUIManager.DeactivateList(guiManager.buildingControlsList);
                //Window
                GUIManager.ActivateGUIElement(guiManager.buildingControls);
                //Static Elements

                ButtonMenuActivityCheck(guiManager.researchMenuButton, ReserchCenterTechsList, ReserchCenterActiveFunctions, ReserchCenterInactiveFunctions);

                GUIManager.EnableInteractableForList(ReserchCenterActiveFunctions);
                GUIManager.ActivateList(ReserchCenterActiveFunctions);

                GUIManager.DisableInteractableForList(ReserchCenterInactiveFunctions);
                GUIManager.ActivateList(ReserchCenterInactiveFunctions);
                

                //Set Active Window
                activeGUIElement = guiManager.buildingControls;

            }
            if (SelectedBuilding.GetComponent<ID>().groupID == 4)
            {
                //Refresh
                GUIManager.DeactivateList(guiManager.buildingControlsList);
                //Window
                GUIManager.ActivateGUIElement(guiManager.buildingControls);
                //Static Elements
                GUIManager.ActivateList(RepairStantionsActiveFunctions);
                GUIManager.EnableInteractableForList(RepairStantionsActiveFunctions);

                guiManager.SetButtonPosition(RepairStantionsActiveFunctions[0], guiManager.L3P09);

                GUIManager.ActivateList(RepairStantionsInactiveFunctions);
                GUIManager.DisableInteractableForList(RepairStantionsInactiveFunctions);
                //Set Active Window
                activeGUIElement = guiManager.buildingControls;

            }

        }




    }








    //Написать кнопочки кнопули UPD. Написанны :D
    #region GUI Buttons

    public void BuildMenuButton()
    {
        //Deactivate previous window
        GUIManager.DeactivateGUIElement(activeGUIElement);
        //Refresh
        GUIManager.DeactivateList(guiManager.buildMenuList);
        //Window
        GUIManager.ActivateGUIElement(guiManager.buildMenu);
        //Static Elements
        GUIManager.ActivateGUIElement(guiManager.backFromBuildMenuButtonToBuildingControls);
        //Dynamic Elements
        if (SelectedBuilding.GetComponent<ID>().groupID == 1)
        {
            GUIManager.ActivateList(CentralBuildingBuidingsList);
        }

        //Set Active Window
        activeGUIElement = guiManager.buildMenu;
        activeMenuButton = guiManager.buildMenuButton;
    }

    public void ProduceMenuButton()
    {
        //Deactivate previous window
        GUIManager.DeactivateGUIElement(activeGUIElement);
        //Refresh
        GUIManager.DeactivateList(guiManager.produceMenuList);
        //Window
        GUIManager.ActivateGUIElement(guiManager.produceMenu);
        //Static Elements
        GUIManager.ActivateGUIElement(guiManager.backFromProduceMenuButton);
        //Dynamic Elements
        if (SelectedBuilding.GetComponent<ID>().groupID == 1)
        {
            GUIManager.ActivateList(CentralBuildingProductionList);
        }
        if (SelectedBuilding.GetComponent<ID>().groupID == 2)
        {
            GUIManager.ActivateList(BuildingProductionList);
        }
        //Set Active Window
        activeGUIElement = guiManager.produceMenu;
        activeMenuButton = guiManager.produceMenuButton;

    }

    public void ResearchMenuButton()
    {
        //Deactivate previous window
        GUIManager.DeactivateGUIElement(activeGUIElement);
        //Refresh
        GUIManager.DeactivateList(guiManager.researchMenuList);
        //Window
        GUIManager.ActivateGUIElement(guiManager.researchMenu);
        //Static Elements
        GUIManager.ActivateGUIElement(guiManager.backFromResearchMenuButton);
        //Dynamic Elements
        if(SelectedBuilding.GetComponent<ID>().groupID == 1)
        {
            GUIManager.ActivateList(CentralBuildingTechList);
        }

        if (SelectedBuilding.GetComponent<ID>().groupID == 3)
        {
            GUIManager.ActivateList(ReserchCenterTechsList);
        }
        //Set Active Window
        activeGUIElement = guiManager.researchMenu;
        activeMenuButton = guiManager.researchMenuButton;

    }

    public void RepairMenuButton()
    {
        //Deactivate previous window
        GUIManager.DeactivateGUIElement(activeGUIElement);
        //Refresh
        GUIManager.DeactivateList(guiManager.repairMenuList);
        //Window
        GUIManager.ActivateGUIElement(guiManager.repairhMenu);
        //Static Elements
        GUIManager.ActivateGUIElement(guiManager.backFromRepairMenuButton);
        GUIManager.ActivateGUIElement(guiManager.ReleaseUnitButton);
        //Dynamic Elements
        if (SelectedBuilding.GetComponent<ID>().groupID == 4)
        {
            GUIManager.ActivateList(RepairStantionsServicesList);
        }
        //Set Active Window
        activeGUIElement = guiManager.repairhMenu;
        activeMenuButton = guiManager.repairMenuButton;

    }

    public void PlaceBuildingButton()
    {
        SelectedBuilding.PlaceBuilding(SelectedBuilding.ProducedBlueprintForPlacing,SelectedBuilding.ResourceConsumed);
        SelectedBuilding.isBusy = false;
        SelectedBuilding.isReadyForPlacing = false;
        SelectedBuilding.ResourceConsumed = false;
        //SelectedBuilding.ProducedBlueprintForPlacing = null;

        guiManager.StopFlashButton(guiManager.placeBuilding);
        CentralBuildingActiveFunctions.Remove(guiManager.placeBuilding);

        if(activeMenuButton==null)
        {
            UpdateBuildingControlsGUI();
        }       

        CloseActiveGUIElement();
        //Спрятать кнопку.

    }

    public void BackToBuildingControlsButton()
    {
        GUIManager.DeactivateGUIElement(activeGUIElement);
        activeGUIElement = guiManager.buildingControls;
        UpdateBuildingControlsGUI();
        GUIManager.ActivateGUIElement(activeGUIElement);
        
    }

#endregion

}
