using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDetectManager : MonoBehaviour
{
    public static InputDetectManager Instance;


    public UnitManager unitManager;
    public BuildingsManager buildingsManager;
    public ResourceManager resourceManager;
    public SelectionManager selectionManager;



    RaycastHit hit;

    Vector3 mousePosition;

    public bool isInIteractionZone = true;
    public bool isDragging = false;

    public LayerMask FogOfWar;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnGUI()
    {
        if (isDragging)
        {

            var rect = ScreenHelper.GetScreenRect(mousePosition, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, new Color(0, 100, 150, .3f));
            ScreenHelper.DrawScreenRectBorder(rect, 2, new Color(0, 100, 150, .8f));

        }


    }

    private bool CheckSelectionBounds(Transform transform)
    {
        if (!isDragging)
        {
            return false;
        }


        var camera = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(camera, mousePosition, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }


    // Update is called once per frame
    void Update()
    {
        if(isInIteractionZone)
        {
            if(!buildingsManager.isPlacing)
            {
                DetectMouse();
            }
                  

        }
           
    }

    public void DetectMouse()
    {
        // All for LMC
        #region Left Button

        //Detect mouse input
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("MouseDown");

            mousePosition = Input.mousePosition;
            //Create a ray from camera
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            //  m_PointerEventData = new PointerEventData(m_EventSystem);
            //  m_PointerEventData.position = Input.mousePosition;





            //Shoot ray and get hit date
            if (Physics.Raycast(camRay, out hit, 1000f, ~FogOfWar))
            {
                //Data processing
                //Debug.Log(hit.transform.tag);
                //Debug.Log(hit.transform.name);
                if (hit.transform.GetComponent<ID>().playerID == 1 && hit.transform.GetComponent<ID>().objectClass == 1)
                {
                    selectionManager.DeselectAll();

                  

                    //Debug.Log("Select unit");
                    unitManager.SelectUnit(hit.transform, Input.GetKey(KeyCode.LeftShift));

                }
                else if(hit.transform.GetComponent<ID>().playerID == 1 && hit.transform.GetComponent<ID>().objectClass == 2)

                {
                    selectionManager.DeselectAll();
                                        

                    //unitManager.selectedUnits.Clear();
                    //Debug.Log(hit.transform.tag + " " + hit.transform.name);
                    buildingsManager.SelectBuilding(hit.transform);

                }
                else    // if (hit.transform.CompareTag("Ground"))

                {
                    if (hit.transform.CompareTag("Ground") || hit.transform.CompareTag("Building") || hit.transform.CompareTag("Unit"))
                    {

                        isDragging = true;

                    }
                    else if(hit.transform.CompareTag("WorldObject"))
                    {
                        //Debug.Log("Im trying to find vein!");

                        
                        selectionManager.SelectVein(hit.transform);
                    }




                }


            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("MouseUP");

            if (isDragging)
            {
                selectionManager.DeselectAll();

                //unitManager.DeselectUnits();

                foreach (var selectableObject in FindObjectsOfType<Unit>())
                {
                    if (CheckSelectionBounds(selectableObject.transform))
                    {
                        if (selectableObject.CompareTag("Unit"))
                        {
                            if (selectableObject.GetComponent<ID>().playerID == 1)
                            {

                                unitManager.SelectUnit(selectableObject.transform, true);

                            }


                        }


                        // Debug.Log(selectableObject.transform.tag);

                    }
                }
            }




            isDragging = false;
            //Debug.Log("Stoped Dragging");
        }
        #endregion//left button



        #region Right Button

        if (Input.GetMouseButtonDown(1))
        {
            if (buildingsManager.SelectedBuilding != null)
            {
                var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Shoot ray and get hit date
                if (Physics.Raycast(camRay, out hit, 1000f, ~FogOfWar))
                {
                    //Data processing
                    // Debug.Log(hit.transform.tag);
                    if (hit.transform.CompareTag("Ground"))
                    {
                        //Установить точку сбора
                        buildingsManager.SelectedBuilding.SetWayPoint(hit.point);

                    }



                }
            }


            if (unitManager.selectedUnits.Count > 0)
            {
                var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Shoot ray and get hit date


                /*
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Hello!");
                }
                */



                if (Physics.Raycast(camRay, out hit, 1000f, ~FogOfWar))
                {
                    //Data processing
                    // Debug.Log(hit.transform.tag);
                    if (hit.transform.CompareTag("Ground"))
                    {

                        if(unitManager.selectedUnits[0].ReadyForPatrool)
                        {
                            foreach (var selectableObject in unitManager.selectedUnits)
                            {

                                selectableObject.StartPatroolingMove(hit.point);

                            }
                        }
                        else
                        {
                            foreach (var selectableObject in unitManager.selectedUnits)
                            {

                                selectableObject.MoveUnit(hit.point);

                            }
                        }



                       

                    }
                    else if (hit.transform.GetComponent<ID>().playerID == 2) //Здесь нужно написать проверку на враждебность по таблице враждебности исходя из данных playerID пока player ID = 2 это враг;
                    {


                        if (!unitManager.selectedUnits[0].ForcedMoveButtonFlag)
                        {

                            foreach (var selectableObject in unitManager.selectedUnits)
                            {


                                if (Input.GetKey(KeyCode.LeftShift))
                                {

                                    selectableObject.AttackComand = true;
                                    selectableObject.AddToAttackerList(hit.transform.GetComponent<Unit>());
                                    selectableObject.NextTarget();

                                }
                                else
                                {
                                    selectableObject.AttackComand = true;
                                    selectableObject.SetNewTarget(hit.transform);
                                }

                            }
                        }
                        else
                        {
                            foreach (var selectableObject in unitManager.selectedUnits)
                            {

                                selectableObject.MoveUnit(hit.transform.position);
                                selectableObject.ForcedMoveButtonFlag = false;

                            }
                        }




                    }
                    else if (hit.transform.GetComponent<ID>().playerID == 1)
                    {

                        if(unitManager.selectedUnits[0].ForceAttackFlag)
                        {

                            foreach (var selectableObject in unitManager.selectedUnits)
                            {

                                selectableObject.AttackComand = true;
                                selectableObject.ForceAttackFlag = false;
                                selectableObject.SetNewTarget(hit.transform);
                            }

                        }
                        else
                        {

                            if (hit.transform.GetComponent<ID>().typeID == 4)
                            {

                                //Отправлять самого дохлого на починку пока просто одного из списка и убираем с него выделение.
                                var Target = hit.transform.GetComponent<RepairStantion>();


                                unitManager.selectedUnits[0].MoveUnit(hit.transform.position);
                                unitManager.selectedUnits[0].navAgent.stoppingDistance = 0;
                                Target.UnitMovingToZone = true;
                                Target.MovingUnit = unitManager.selectedUnits[0];
                                unitManager.UnselectUnit(unitManager.selectedUnits[0]);



                            }
                            else if(hit.transform.GetComponent<ID>().objectClass == 21)
                            {
                                //Начинаем строить
                                //Debug.Log("Find Blueprint");
                                foreach (var selectableObject in unitManager.selectedUnits)
                                {

                                    selectableObject.MoveUnit(hit.transform.position);
                                    selectableObject.StartBuildingSiteWorking(hit.transform.GetComponent<BlueprintScript>());


                                }
                            }
                            else
                            {

                                foreach (var selectableObject in unitManager.selectedUnits)
                                {

                                    Debug.Log("Moving to " + hit.transform.name);
                                    selectableObject.MoveUnit(hit.transform.position);

                                }


                            }

                        }

                    }
                    else if (hit.transform.GetComponent<ID>().groupID == 3)
                    {
                        foreach (var selectableObject in unitManager.selectedUnits)
                        {

                            selectableObject.MoveUnit(hit.transform.position);
                            selectableObject.StartGathering(hit.transform.GetComponent<VeinScript>());
                            

                        }

                    }
     
                }
            }

        }

        #endregion





    }

    


}
