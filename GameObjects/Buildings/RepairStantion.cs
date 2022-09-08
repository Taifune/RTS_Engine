using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStantion : MonoBehaviour
{
    public BuildingScript building;
    public HealStats healStats;
    public Unit unitInRepairZone;
    public bool ZoneOcupied;
    public bool UnitMovingToZone;
    public Unit MovingUnit;

    public bool isRepairProcess = false;
    public bool isReloadProcess = false;

    public bool unitHaveFullHealth = false;
    public bool unitHaveFullAmmo = false;

    public float healColdDownTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        healColdDownTimer = healStats.healSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Timers();

        if (!ZoneOcupied)
        {
            if (UnitMovingToZone)
            {

                MovingUnitStatusCheck();

                RaycastScan();
            }

        }

        if (ZoneOcupied)
        {

            UnitLiveStatusCheck();

            OccupiedAreaCheck();

            UnitSelectorControl();

            CheckFlags();          

        }


    }

    #region UpdateMetods

    public void Timers()
    {
        if (healColdDownTimer <= healStats.healSpeed)
        {
            healColdDownTimer += Time.deltaTime;
        }
    }
    

    public void UnitSelectorControl()
    {

        if (building.MySelector.activeSelf)
        {
            if (!unitInRepairZone.MySelector.activeSelf)
            {

                unitInRepairZone.MySelector.SetActive(true);

            }
        }
        else
        {
            if (!unitInRepairZone.Selected)
            {

                unitInRepairZone.MySelector.SetActive(false);

            }
        }
    }


    public void CheckFlags()
    {
        if (isRepairProcess)
        {
            if(unitInRepairZone.currentHealth<unitInRepairZone.unitStats.health)
            {
                RepairProcess(unitInRepairZone);
            }
            else
            {
                isRepairProcess = false;
            }
            
        }
        if (isReloadProcess)
        {
            if(unitInRepairZone.currentAmmo<unitInRepairZone.unitStats.ammoAmount)
            {
                ReloadProcess(unitInRepairZone);
            }
            else
            {
                isReloadProcess = false;
            }

            
        }
    }

    public void ClearZone()
    {
        isReloadProcess = false;
        isRepairProcess = false;
        unitHaveFullHealth = false;
        unitHaveFullAmmo = false;
        ZoneOcupied = false;
        unitInRepairZone = null;

    }

    public void ClearMovingUnit()
    {
        UnitMovingToZone = false;
        MovingUnit = null;
    }

    public void UnitLiveStatusCheck()
    {
        if(unitInRepairZone.currentHealth== unitInRepairZone.unitStats.health)
        {
            unitHaveFullHealth = true;
        }
        else
        {
            unitHaveFullHealth = false;
        }
        if (unitInRepairZone.currentAmmo == unitInRepairZone.unitStats.ammoAmount)
        {
            unitHaveFullAmmo = true;
        }
        else
        {
            unitHaveFullAmmo = false;
        }



        if (unitInRepairZone.isDead)
        {
            ClearZone();
        }
    }

    public void MovingUnitStatusCheck()
    {

        if (MovingUnit.isDead)
        {
            ClearMovingUnit();

        }

        Vector2 Dest = new Vector2(MovingUnit.navAgent.destination.x, MovingUnit.navAgent.destination.z);
        Vector2 Pos = new Vector2(transform.position.x, transform.position.z);

        //Debug.Log(MovingUnit.navAgent.destination + "  " + transform.position);
        //Debug.Log(Dest + "  " + Pos);

        if (Dest != Pos)
        {


            //Debug.Log("Done");
            ClearMovingUnit();
        }

    }


    #endregion

    #region MainMetods


    public void GrabUnit(Unit currentUnitInZone )
    {
        unitInRepairZone = currentUnitInZone;
        unitInRepairZone.MouseOverBypass = true;

        unitInRepairZone.MySelector.SetActive(true);
        //unitInRepairZone.MySelector. можно окрашивать или делать статичным как у здания
        
        unitInRepairZone.navAgent.enabled = false;

        var pos = new Vector3(building.transform.position.x, unitInRepairZone.transform.position.y, building.transform.position.z);

        unitInRepairZone.transform.position = Vector3.Lerp(unitInRepairZone.transform.position, pos, 1);
        ZoneOcupied = true;
        ClearMovingUnit();
    }

    public void ReleaseUnit()
    {

        unitInRepairZone.MouseOverBypass = false;
        if (!unitInRepairZone.Selected)
        {
            unitInRepairZone.MySelector.SetActive(false);
        }
        
        unitInRepairZone.navAgent.enabled = enabled;

        if(building.isWayPointSet)
        {
            unitInRepairZone.navAgent.destination = building.WayPointPos;
        }
        else
        {
            unitInRepairZone.navAgent.destination = building.OriginPoint.transform.position;
        }

        ClearZone();
    }

    




    public void OccupiedAreaCheck()
    {
        #region RaycastScan

        RaycastHit ScanHit;

        Ray RepairZoneRay = new Ray(building.transform.position + Vector3.up * 2, Vector3.down * 2);
        Debug.DrawRay(building.transform.position + Vector3.up * 2, Vector3.down * 3, Color.red);

        if(Physics.Raycast(RepairZoneRay,out ScanHit,2.0f))
        {
            if (ScanHit.collider.gameObject.activeSelf)
            {
                if (ScanHit.collider.gameObject != transform.gameObject)
                {
                    if (!ScanHit.collider.transform.CompareTag("Ground"))
                    {

                        var ID = ScanHit.collider.gameObject.GetComponent<ID>().playerID;

                        if (ID == building.buildingStats.playerID)
                        {

                            if (ID != -1 && ID != 0)
                            {
                                //Debug.Log(ID);

                                var currentUnitInZone = ScanHit.collider.GetComponent<Unit>();

                                Debug.Log(currentUnitInZone.unitStats.unitName+" on Service");

                                //ReleaseUnit();
                                

                            }

                        }
                    }


                }



                //currentTarget = ScanHit.collider.transform;
            }

        }


        #endregion
    }

    public void RaycastScan()
    {
        #region RaycastScan

        RaycastHit ScanHit;

        Ray RepairZoneRay = new Ray(building.transform.position + Vector3.up * 1.5f, Vector3.down * 2);
        Debug.DrawRay(building.transform.position + Vector3.up * 2, Vector3.down * 2, Color.cyan);

        if (Physics.Raycast(RepairZoneRay, out ScanHit, 2.0f))
        {
            if (ScanHit.collider.gameObject.activeSelf)
            {
                if (ScanHit.collider.gameObject != transform.gameObject)
                {
                       

                    var ID = ScanHit.collider.gameObject.GetComponent<ID>().playerID;

                    if (ID == building.buildingStats.playerID)
                    {

                        if (ID != -1 && ID != 0)
                        {
                            //Debug.Log(ID);
                            

                            var currentUnitInZone = ScanHit.collider.GetComponent<Unit>();

                            //Debug.Log("I found " + ScanHit.collider.gameObject.name + "  " + currentUnitInZone.currentHealth + "/" + currentUnitInZone.unitStats.health + "   |  " + currentUnitInZone.currentAmmo +   "/" + currentUnitInZone.unitStats.ammoAmount);

                            if (currentUnitInZone.currentHealth < currentUnitInZone.unitStats.health || currentUnitInZone.currentAmmo < currentUnitInZone.unitStats.ammoAmount)
                            {
                                //Debug.Log("Grabing unit");

                                var distance = (currentUnitInZone.transform.position - transform.position).magnitude;

                                //Debug.Log(distance);

                                if( distance < 0.67)
                                {
                                    GrabUnit(currentUnitInZone);
                                }

                                
                            }
                            else
                            {

                                //Debug.Log("Grabing unit");

                                var distance = (currentUnitInZone.transform.position - transform.position).magnitude;

                                //Debug.Log(distance);

                                if (distance < 0.67)
                                {
                                    GrabUnit(currentUnitInZone);
                                    unitHaveFullHealth = true;
                                    unitHaveFullAmmo = true;
                                }

                                

                                /*
                                 
                            
                                Debug.Log("Unit have full health");
                                Debug.Log("Unit have full ammo");

                                currentUnitInZone.navAgent.destination = building.OriginPoint.transform.position;
                                UnitMovingToZone = false;
                                
                                */
                            }




                        }

                    }



                }
                else
                {
                    ZoneOcupied = false;
                }



                //currentTarget = ScanHit.collider.transform;
            }

        }


        #endregion
    }


    public void RepairProcess(Unit unit)
    {
        if(healColdDownTimer>=healStats.healSpeed)
        {
            Heal(unit);
        }        

    }

    public void ReloadProcess(Unit unit)
    {
        if (healColdDownTimer >= healStats.healSpeed)
        {
            Reload(unit);

        }


    }

    public void Reload(Unit unit)
    {
        healColdDownTimer = 0;
        RTSGameManager.UnitReciveAmmunitionFromStantion(this, unit,true,FindObjectOfType<ResourceManager>());
    }    

    public void Heal(Unit unit)
    {
        healColdDownTimer = 0;
        RTSGameManager.UnitTakeHealingFromStantion(this, unit, true, FindObjectOfType<ResourceManager>());
    }

    public void ConvertUnitToResouces(Unit unit)
    {

        RTSGameManager.UnitConvertionToResouce(this,unit, FindObjectOfType<ResourceManager>());
        
    }

    #endregion

    #region GUI Buttons

    public void RepairSwitchButton()
    {
        if(!unitHaveFullHealth)
        {
            isRepairProcess = !isRepairProcess;
        }
        
    }


    public void ReloadSwitchButton()
    {
        if(!unitHaveFullAmmo)
        {
            isReloadProcess = !isReloadProcess;
        }
        
    }

    public void SellUnitButton()
    {
        if(unitInRepairZone!=null)
        {
            ConvertUnitToResouces(unitInRepairZone);
        }
    }

    public void ReleaseUnitButton()
    {
        if(ZoneOcupied)
        {
            ReleaseUnit();            
        }
    }

    #endregion









}
