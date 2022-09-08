using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    //GraphicRaycaster m_Raycaster;
    //PointerEventData m_PointerEventData;
    //EventSystem m_EventSystem;

    //RaycastHit hit;
    //RaycastHit canvasHit;
    //RaycastHit2D hit2D;
    public Transform UnitsFolder;

    public List<Unit> selectedUnits = new List<Unit>();
    public List<Unit> ActiveUnitsList = new List<Unit>();


    public List<Unit> AllUnits = new List<Unit>();
    //public bool isDragging = false;


    public GameObject ProjectilePrefab;
    public GameObject ExplosionPrefab;
    public GameObject GravePrefab;
    
    //public LayerMask FogOfWar;

    public BuildingsManager buildingsManager;
    public GUIManager guiManager;
    public ResourceManager resourceManager;
    public SelectionManager selectionManager;
    public GameObject activeGUIElement;
    public bool isUnitGUIActive;

    //public bool isInIteractionZone = true;

    //Vector3 mousePosition;

    private void Awake()
    {
        Instance = this;
    }

    #region Selection


    public void SelectUnit(Transform Unit, bool isMultiselection)
    {
        buildingsManager.DeselectBuilding();

        if (!isMultiselection)
        {
            DeselectUnits();
            //Debug.Log("Deselect because singleselect");
        }

        var UnitScript = Unit.transform.GetComponent<Unit>();


        if (UnitScript.Selected==false)
        {
            selectedUnits.Add(UnitScript);

            UnitScript.SetSelected(true);

            UnitScript.SelectedListIndex = selectedUnits.Count - 1;

            ActivateGUI(Unit.GetComponent<ID>());
            selectionManager.isObjectSelected = true;

        }
        else
        {

            UnselectUnit(UnitScript);
        }


        
    }
   
    public void UnselectUnit(Unit unit)
    {
        if(unit.Selected)
        {
            unit.SetSelected(false);

            int index = unit.SelectedListIndex;
            selectedUnits.RemoveAt(unit.SelectedListIndex);
            UpdateIndex(index);
        }

        if (selectedUnits.Count==0)
        {

            DeactivateUnitGUI();
        }


    }
    
    private void UpdateIndex(int index)
    {
        for (int i = index; i < selectedUnits.Count; i++)
        {

            selectedUnits[i].SelectedListIndex-=1;
            
        }
    }

    public void DeselectUnits()
    {
        
        DeactivateUnitGUI();
        
        //buildingsManager.DeselectBuilding();

        for (int i = 0; i< selectedUnits.Count;i++)
        {

            selectedUnits[i].MySelector.SetActive(false);
            selectedUnits[i].Selected = false;
            

        }

        selectedUnits.Clear();
    }
    
    public void ActivateGUI(ID id)
    {
        if(id.groupID == 1)
        {
            activeGUIElement = guiManager.buildMenuButton;
            GUIManager.ActivateGUIElement(guiManager.gatherButton);
        }

        if(activeGUIElement!=null)
        {
            GUIManager.ActivateGUIElement(activeGUIElement);
        }

        if(selectedUnits.Count>0)
        {
            if(selectedUnits.Count==1)
            {
                UnitInfoPanel.UpdatePanelForUnitStatic(selectedUnits[0]);
            }
            else
            {
                UnitInfoPanel.DeactivatePanelStatic();
            }

            GUIManager.ActivateGUIElement(guiManager.unitControls);
            isUnitGUIActive = true;
        }

    }

    public void DeactivateUnitGUI()
    {
        GUIManager.DeactivateGUIElement(guiManager.unitControls);
        GUIManager.DeactivateGUIElement(guiManager.buildMenu);
        GUIManager.DeactivateGUIElement(guiManager.gatherButton);

        if (activeGUIElement!=null)
        {
            GUIManager.DeactivateGUIElement(activeGUIElement);
            activeGUIElement = null;
        }

        isUnitGUIActive = false;
    }


    #endregion

    //UI Panel
    //Начал писать GUI метод который подключен к кнопке проходит по листу выделенных объектов посылая им команду Stop() 
    #region Interaction zone

    public static bool OutOfInterationZone(bool State)
    {
        return State;
    }

    public static bool PressedGUI()
    {
        Debug.Log("PressedGUI");
        return true;
    }

    #endregion

    #region GUI buttons

    public void MoveButton()
    {
        //сделать что через эту клавишу как следование за выбранным юнитом ? или просто на лкм движение?? вообще странная команда..
        
    }
       
    public void StopButton()
    {
        foreach (var selectableObject in selectedUnits)
        {

            selectableObject.Stop();

        }
    }

    public void HoldButton()
    {
        foreach(var selectableObject in selectedUnits)
        {

            selectableObject.Hold();

        }        

    }

    public void GuardButton()
    {
        foreach (var selectableObject in selectedUnits)
        {

            selectableObject.Guard();

        }

    }
       
    public void AttackButton()
    {

        foreach (var selectableObject in selectedUnits)
        {

            selectableObject.ForcedAttack();
        }

        //Debug.Log("Attack");
        //Нужно написать включение атаки на обе клавиши мыши и игнорирование флага союзник

    }

    public void PatroolButton()
    {
        foreach (var selectableObject in selectedUnits)
        {
            selectableObject.TurnOnReadyForPatrool();
        
            //нужно менять курсор для понятного отображения
        }
        
        
        
        // Поднять патруль флаг и 2 координаты nav1 и nav2 записать со след кликов nav 1 текущая позиция nav 2 клик

    }

    public void RepairButton()
    {
        //Починка доступна только для спец юнитов 
    }

    public void FormatButton()
    {
        //вызывает вложенное окно построения. Варианты построения юнитов. Линия. Квадрат. Круг. Треугольник.

    }

    public void GatherButton()
    {

        foreach (var selectableObject in selectedUnits)
        {
            if(selectableObject.unitStats.canGatherResources)
            {
                if(selectableObject.currentVein!=null)
                {
                    //selectableObject.currentTarget = selectableObject.currentVein.transform;
                    selectableObject.TurnOnGatheringMode();
                }
                
            }
           

            //нужно менять курсор для понятного отображения
        }
        //переключение в режим добычи. Работает толькое если юнит добывал до этого. Нужно сделать если значения нуль для целей добычи и дома то в интерфейсе отображается серой и не активной.
    }

    public void BuildButton()
    {
        GUIManager.DeactivateList(guiManager.buildMenuList);
        
        //доступна для спец юнита. вызвает вложенное окно постройки с кнопкой назад.
        GUIManager.ToggleGUIElement(guiManager.unitControls);
        GUIManager.ToggleGUIElement(guiManager.buildMenu);
        GUIManager.ActivateGUIElement(guiManager.backFromBuildMenuButtonToUnitControls);
        GUIManager.ActivateGUIElement(guiManager.buildingOneButton);

    
    }

    public void DestroyButton()
    {
        //Самоуничтожение или сделать демонтаж для спец юнитов.
    }

    public void Temp()
    {
        //Запасная кнопка
    }

    public void CancelButton()
    {
        //Снять выделение 
        DeselectUnits();
    }

    public void BackFromBuildMenuButton()
    {
        GUIManager.AdaptiveBackButton(guiManager.unitControls, guiManager.buildMenu);
    }

    #endregion
}
