using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechManager : MonoBehaviour
{
    public static TechManager Instance;

    public UnitManager unitManager;
    public BuildingsManager buildingsManager;
    public ResourceManager resourceManager;
    public SelectionManager selectionManager;
    public InputDetectManager inputDetectManager;
    public GUIManager guiManager;



    public TechStats techSpeedUP;     //1
    public TechStats techArmorUP;     //2
    public TechStats techDamageUP;    //3
    public TechStats techUnitTank;    //4
    public TechStats techRC;          //5
    public TechStats techRS;          //6
    public TechStats techTurret;      //7
    public TechStats techT2Techs;     //8
    public TechStats SellServiceTech; //9
    


    //Надо сделать метод активации бонусов по наличию технологии в списке изученных.
    public List<TechStats> ResearchedTechs = new List<TechStats>();


    public int SpeedBonus = 0;
    public int ArmorBonus = 0;
    public int DamageBonus = 0;

    public bool TankResearched = false;
    public bool ResearchCenterResearched = false;
    public bool RepairStantionResearched = false;
    public bool TurretResearched = false;
    public bool Techs2Researched = false;
    public bool SellServiceResearched = false;


    private void Awake()
    {
        Instance = this;
    }
    
   

    public void ResearchTech(TechStats tech)
    {
        
        ResearchedTechs.Add(tech);

        ActivateTechBonus(tech.techType);

    }


    public void ActivateTechBonus(int TechType)
    {
        if (TechType == 1)
        {

            SpeedBonus += techSpeedUP.techEffect;

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchSpeedButton,3);

        }
        else if (TechType == 2)
        {

            ArmorBonus += techArmorUP.techEffect;

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchArmorButton, 3);

        }
        else if (TechType == 3)
        {

            DamageBonus += techDamageUP.techEffect;

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchDamageButton, 3);

        }
        else if (TechType == 4)
        {
            TankResearched = true;

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchUnitTankButton, 3);

            /*
            buildingsManager.ReserchCenterTechsList.Remove(guiManager.ResearchUnitTankButton);

            if (buildingsManager.SelectedBuilding.GetComponent<ID>().groupID == 3)
            {
                
                GUIManager.DeactivateGUIElement(guiManager.ResearchUnitTankButton);

                // Выключение пустого меню.
                if (buildingsManager.ReserchCenterTechsList.Count == 0)
                {
                    GUIManager.DeactivateGUIElement(guiManager.researchMenuButton);
                    buildingsManager.BackToBuildingControlsButton();

                }

            }

            */

            buildingsManager.AddObjectToList(buildingsManager.BuildingProductionList, guiManager.produceUnitTank, 2);
         
            /*
             * 
            buildingsManager.BuildingProductionList.Add(guiManager.produceUnitTank);

            if (buildingsManager.SelectedBuilding.GetComponent<ID>().groupID==2)
            {
                //Можно просто сделать рефреш окна поидее.но вообще нужно кнопки отображать заблокированными и не активыными и просто их включать.
                GUIManager.ActivateGUIElement(guiManager.produceUnitTank);
            }  
            
            */

            
        }
        else if (TechType == 5)
        {
            ResearchCenterResearched = true;

            buildingsManager.RemoveObjectFromList(buildingsManager.CentralBuildingTechList, guiManager.ResearchRCButton, 1);
            buildingsManager.AddObjectToList(buildingsManager.CentralBuildingBuidingsList, guiManager.buildingThreeButton, 1);

            if (buildingsManager.CentralBuildingTechList.Count == 0)
            {
                GUIManager.DisableInteractableForButton(guiManager.researchMenuButton);
                buildingsManager.BackToBuildingControlsButton();

            }

            /*
             * 
            //Убираем исследованную технологию из списка доступных для места исследования. И из других списков если можно было изучать в нескольких местах.
            buildingsManager.CentralBuildingTechList.Remove(guiManager.ResearchRCButton);
            //Добавляем кнопку строителсьтва исследованного здания в соотвествующие списки где она должна быть доступна.
            buildingsManager.CentralBuildingBuidingsList.Add(guiManager.buildingThreeButton);

            //Проверяем активное окно интефейса и если активно окно где есть изменившиеся позиции перестраиваем его под обновленную конфигурацию.
            if (buildingsManager.SelectedBuilding.GetComponent<ID>().groupID==1)
            {
                //Отключаем кнопку исследованной технологии.                
                GUIManager.DeactivateGUIElement(guiManager.ResearchRCButton);

                GUIManager.ActivateGUIElement(guiManager.buildingThreeButton);
                
                
                // Выключение пустого меню.
                if (buildingsManager.CentralBuildingTechList.Count == 0)
                {
                    GUIManager.DisableInteractableForButton(guiManager.researchMenuButton);
                    buildingsManager.BackToBuildingControlsButton();

                }                               
                
            }           

            
            */

        }
        else if (TechType == 6)
        {
            RepairStantionResearched = true;

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchRSButton, 3);
            buildingsManager.AddObjectToList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchSellTechButton, 3);
            buildingsManager.AddObjectToList(buildingsManager.CentralBuildingBuidingsList, guiManager.buildingFourButton, 1);

            /*

            buildingsManager.ReserchCenterTechsList.Remove(guiManager.ResearchRSButton);

            buildingsManager.ReserchCenterTechsList.Add(guiManager.ResearchSellTechButton);

            buildingsManager.CentralBuildingBuidingsList.Add(guiManager.buildingFourButton);


            if (buildingsManager.SelectedBuilding.GetComponent<ID>().groupID == 3)
            {
                GUIManager.ActivateGUIElement(guiManager.ResearchSellTechButton);
                GUIManager.DeactivateGUIElement(guiManager.ResearchRSButton);

            }

            if (buildingsManager.SelectedBuilding.GetComponent<ID>().groupID == 1)
            {
                GUIManager.ActivateGUIElement(guiManager.buildingFourButton);
            }

            */


        }
        else if (TechType == 7)
        {
            TurretResearched = true;

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchTurretButton, 3);
            buildingsManager.AddObjectToList(buildingsManager.CentralBuildingBuidingsList, guiManager.buildingFiveButton, 1);
                      
            
        }
        else if (TechType == 8)
        {
            Techs2Researched = true;          

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchTechsButton, 3);

            buildingsManager.AddObjectToList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchUnitTankButton, 3);
            buildingsManager.AddObjectToList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchTurretButton, 3);
            buildingsManager.AddObjectToList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchRSButton, 3);
            
        }
        else if(TechType==9)
        {
            SellServiceResearched = true;

            buildingsManager.RemoveObjectFromList(buildingsManager.ReserchCenterTechsList, guiManager.ResearchSellTechButton, 3);
            buildingsManager.AddObjectToList(buildingsManager.RepairStantionsServicesList, guiManager.ResearchRSButton, 4);

            
        }

    }



    public bool ResourecesForResearchCheck(int playerID, int researchCost)
    {
        //Debug.Log(buildingCost);

        //Debug.Log(resourceManager.Player1Resources);


        if (researchCost <= resourceManager.Players[playerID].Resources)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
