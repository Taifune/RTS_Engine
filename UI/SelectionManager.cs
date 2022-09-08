using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;


    public UnitManager unitManager;
    public BuildingsManager buildingsManager;
    public InputDetectManager inputDetectManager;

    public BlueprintScript SelectedBlueprint;

    public bool isObjectSelected = false;
    public bool isBlueprintSelected = false;

    public VeinScript currentSelectedVein   = null;
    public BuildingScript currentedSelectedBuilding = null;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
        

    public void SelectBuilding(Transform SelectedObject)
    {
        buildingsManager.SelectBuilding(SelectedObject);
    }

    public void SelectVein(Transform SelectedObject)
    {
        DeselectAll();

        var currentVein = SelectedObject.GetComponent<VeinScript>();

        currentVein.Select();
        currentSelectedVein = currentVein;
        isObjectSelected = true;

    }

    public void SelectBlueprint(BlueprintScript blueprint)
    {
        SelectedBlueprint = blueprint;
        isBlueprintSelected = true;
    }
        
    public static void SelectBluePrintStatic(BlueprintScript blueprint)
    {
        Instance.SelectBlueprint(blueprint);
    }




    public void DeselectBuilding()
    {
        if (buildingsManager.SelectedBuilding != null)
        {
            buildingsManager.DeselectBuilding();
            isObjectSelected = false;
        }
    }

    public void DeselectUnits()
    {
        if (unitManager.selectedUnits.Count > 0)
        {
            unitManager.DeselectUnits();
            isObjectSelected = false;
        }

    }

    public void DeselectVein()
    {
        if (currentSelectedVein != null)
        {
            currentSelectedVein.Deselect();
            isObjectSelected = false;
        }
    }

    public void DeselectBlueprint()
    {
        if (SelectedBlueprint != null)
        {
            SelectedBlueprint.Deselect();
            SelectedBlueprint = null;
            isBlueprintSelected = false;
        }
    }


    public void DeselectAll()
    {

        DeselectUnits();
        DeselectBuilding();
        DeselectVein();
        DeselectBlueprint();

    }

    
}



