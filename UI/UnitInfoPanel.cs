using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanel : MonoBehaviour
{

    public UnitManager unitManager;
    public BuildingsManager buildingsManager;
    public GUIManager guiManager;
    public InputDetectManager inputDetectManager;


    public static UnitInfoPanel Instance;

    public Text Name;
    public Image Icon;
    public Text Damage;
    public Text Armor;
    public Text Range;
    public Text View;
    public Text Ammo;
    public GUIHUD guiHUD;

    public bool Active;

    public Unit currentUnit;
    public BuildingScript currentBuilding;
    public VeinScript currentVein;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if(Active)
        {
            if(currentUnit!=null)
            {
                if(!currentUnit.isDead)
                {
                    if(currentUnit.Selected)
                    {
                        UpdateUnitCurrency();
                    }
                    else
                    {
                        DeactivatePanelStatic();
                    }
                }
                else
                {
                    
                    DeactivatePanelStatic();
                }
            }
            
        }
    }



    public void UpdatePanelForUnit(Unit unit)
    {
        Active = true;
        gameObject.SetActive(true);

        currentUnit = unit;
        UpdateUnitCurrency();

        var Stats = unit.unitStats;

        Name.text = Stats.unitName;
        Icon.overrideSprite = Stats.Icon;
        Damage.text = Stats.minDamage + " - " + unit.unitStats.maxDamage;
        Armor.text = Stats.armor.ToString();
        Range.text = Stats.attackRange.ToString();
        View.text = Stats.viewRange.ToString();
        Ammo.text = currentUnit.currentAmmo.ToString();
        guiHUD.healthBarScript.SetMaxHealth(Stats.health);
        
    }

    public void UpdateUnitCurrency()
    {
        //MyHUD.healthBarScript.SetHealth(CurrentHealth);
        guiHUD.healthBarScript.SetHealth(currentUnit.currentHealth);
        Ammo.text = currentUnit.currentAmmo.ToString();

    }

    public static void UpdatePanelUnitHealthBarStatic()
    {
        Instance.UpdateUnitCurrency();
    }


    public static void UpdatePanelForUnitStatic(Unit unit)
    {
        Instance.UpdatePanelForUnit(unit);
    }

    public static void DeactivatePanelStatic()
    {
        Instance.Active = false;
        Instance.gameObject.SetActive(false);
    }
}
