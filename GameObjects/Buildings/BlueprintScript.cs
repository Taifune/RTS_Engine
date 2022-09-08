using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintScript : MonoBehaviour
{
    RaycastHit hit;
    RaycastHit hitCheck;
    Vector3 movePoint;
    public ID MyID;
    public BuildingScript buildingPrefab;
    public GameObject myBlueprintPrefab;
    public BoxCollider MyBox;
    public GameObject BuildZone;
    public BlueprintHUDScript blueprintHUDScript;

    public bool ConstructionProcess = false;
    public bool DistanceCheck;

    public bool SiteConstruction;
    public bool isNeedForResource = true;
    public int currentResourcesOnSite;
    public float currentWorkProgress;
    public GameObject building = null;
    public Vector3 ModelStartPos;
    public Vector3 ModelEndPos;

    public List<Unit> buildersList = new List<Unit>();


    public GameObject MySelector = null;
    public bool Selected;

    public bool inBuildingRange;
    public GameObject MyModel;
    public bool haveChilds;
    public LayerMask layerMask;
    public BuildingsManager buildingsManager;
    public InputDetectManager inputDetectManager;
    public bool isFlashing;
    public Bounds boundBox;
    public float ConstructionHeight;
    public Unit Builder;

    public BuildingScript ParentBuilding = null;
    public bool ResourceConsumed = false;

    private MeshRenderer meshRenderer;
    private Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {

        meshRenderer = MyModel.GetComponent<MeshRenderer>();
        defaultColor = meshRenderer.material.color;

        buildingsManager = BuildingsManager.Instance;
        inputDetectManager = InputDetectManager.Instance;

        MyModel.gameObject.SetActive(false);


        buildingsManager.isPlacing = true;
        buildingsManager.ShowBuildZones();


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = new Vector3(hit.point.x, ConstructionHeight, hit.point.z);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistanceChecking();

        if (inputDetectManager.isInIteractionZone)
        {
            if (!ConstructionProcess)
            {

                if (!MyModel.gameObject.activeSelf)
                {
                    MyModel.gameObject.SetActive(true);
                }

            }



            DetectMouse();
        }
        else
        {
            if (!ConstructionProcess)
            {
                MyModel.gameObject.SetActive(false);
            }

            
        }
               
       
    }

    #region Update Distance Checking

    private void UpdateDistanceChecking()
    {


        if (DistanceCheck)
        {
            var distance = (Builder.transform.position - transform.position).magnitude;

            if (Builder.navAgent.destination != new Vector3(transform.position.x, Builder.navAgent.destination.y, transform.position.z))
            {

                DistanceCheck = false;
                CancelBlueprint();

            }

            if (distance <= 2)
            {
                DistanceCheck = false;
                RTSGameManager.UnitBuildingConstruction(Builder, buildingPrefab.buildingStats.playerID, buildingPrefab.buildingStats.buildingCost, buildingsManager.resourceManager);
                StartCoroutine(UnitBuildingConstruction(Builder, buildingPrefab.gameObject, transform.position, transform.rotation, buildingPrefab.buildingStats.buildingSpeed));
            }

            if (distance < Builder.unitStats.viewRange)
            {
                inBuildingRange = true;

                if (!isFlashing)
                {
                    meshRenderer.material.color = defaultColor;
                    UpdateChildColor();
                }

            }
            else
            {
                inBuildingRange = false;
                if (!isFlashing)
                {
                    meshRenderer.material.color = Color.red;
                    UpdateChildColor();
                }

            }

        }



    }


    #endregion

    #region Mouse Update

    private void DetectMouse()
    {

        if (!ConstructionProcess)
        {

            #region Mouse Following with Check

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000f, ~layerMask))
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    transform.position = new Vector3(hit.point.x, ConstructionHeight, hit.point.z);

                    if (!SiteConstruction)
                    {
                        

                        #region Distacne Check
                                               
                        if (buildingsManager.SelectedBuilding != null)
                        {

                            var distance = (buildingsManager.SelectedBuilding.transform.position - transform.position).magnitude;

                            //Debug.Log(distance);

                            if (distance < buildingsManager.SelectedBuilding.buildingStats.buildingRange)
                            {
                                inBuildingRange = true;
                                if (!isFlashing)
                                {
                                    meshRenderer.material.color = defaultColor;
                                    UpdateChildColor();
                                }

                            }
                            else
                            {
                                inBuildingRange = false;
                                if (!isFlashing)
                                {
                                    meshRenderer.material.color = Color.red;
                                    UpdateChildColor();
                                }

                            }

                        }
                        else
                        {
                            if (Builder == null)
                            {
                                if (buildingsManager.unitManager.selectedUnits.Count > 0)
                                {
                                    Builder = buildingsManager.unitManager.selectedUnits[0];
                                }


                            }

                            if (Builder != null)
                            {
                                var distance = (Builder.transform.position - transform.position).magnitude;

                                //Debug.Log(distance);

                                if (distance < Builder.unitStats.viewRange)
                                {
                                    inBuildingRange = true;

                                    if (!isFlashing)
                                    {
                                        meshRenderer.material.color = defaultColor;
                                        UpdateChildColor();
                                    }

                                }
                                else
                                {
                                    inBuildingRange = false;
                                    if (!isFlashing)
                                    {
                                        meshRenderer.material.color = Color.red;
                                        UpdateChildColor();
                                    }

                                }
                            }


                        }

                        #endregion
                        
                    }

                }

            }

            #endregion

            DebugPyramideDraw();
                      

            #region Left Click

            if (Input.GetMouseButtonUp(0))
            {
                //Запуск корутины с анимацией постройки в конце которой делать инстанс здания

                //Надо создать объект хранилище для зданий.

                //Нужна проверка на разрешение постройки отсутствие коллизий
                //Debug.Log("Cost check from blue print = " + buildingPrefab.buildingStats.buildingCost);


                if (!SiteConstruction)
                {
                    
                    #region BuildingConsruction

                    if (buildingsManager.ResourecesForConsumingCheck(buildingPrefab.buildingStats.playerID,buildingPrefab.buildingStats.buildingCost))
                    {

                        bool Collision = PyramideRayCheck();

                        if (Collision)
                        {
                            Debug.Log("Collision!");
                            StartCoroutine(ZoneFlasher(BuildZone.GetComponent<MeshRenderer>().material.color));

                        }
                        else
                        {


                            if (buildingsManager.SelectedBuilding != null)
                            {

                                #region Build From Building

                                if (inBuildingRange)
                                {
                                    if(!ResourceConsumed)
                                    {
                                        RTSGameManager.BuildingConstruction(buildingPrefab.buildingStats.playerID, buildingPrefab.buildingStats.buildingCost, buildingsManager.resourceManager);
                                    }
                                
                                    

                                    MyBox.size = buildingPrefab.MyBox.size;
                                    MyBox.enabled = true;

                                    transform.gameObject.layer = 0;

                                    StartCoroutine(BuildingConstruction(buildingPrefab.gameObject, transform.position, transform.rotation));

                                    ConstructionProcess = true;

                                    buildingsManager.isPlacing = false;
                                    BuildZone.SetActive(false);
                                    buildingsManager.HideBuildZones();
                                }
                                else
                                {
                                    Debug.Log("Too far away from build source");
                                    buildingsManager.SelectedBuilding.Flash(buildingsManager.SelectedBuilding.MyModel.GetComponent<MeshRenderer>().material.color, Color.green);
                                    StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));
                                }

                                #endregion

                            }
                            else
                            {

                                #region Build From Unit

                                if (inBuildingRange)
                                {

                                    Debug.Log("Unit builds");
                                    //unit building

                                    //нужно запускать метод что строит юнит
                                    RTSGameManager.UnitBuildingConstruction(Builder, buildingPrefab.buildingStats.playerID, buildingPrefab.buildingStats.buildingCost, buildingsManager.resourceManager);
                                    StartCoroutine(UnitBuildingConstruction(Builder, buildingPrefab.gameObject, transform.position, transform.rotation, buildingPrefab.buildingStats.buildingSpeed));

                                    MyBox.size = buildingPrefab.MyBox.size;
                                    MyBox.enabled = true;

                                    transform.gameObject.layer = 0;                                    

                                    ConstructionProcess = true;

                                    buildingsManager.isPlacing = false;
                                    BuildZone.SetActive(false);
                                    buildingsManager.HideBuildZones();

                                }
                                else
                                {
                                    Debug.Log("Too far away. Coming");

                                    Builder.MoveUnit(transform.position);
                                    
                                    DistanceCheck = true;

                                    MyBox.size = buildingPrefab.MyBox.size;
                                    MyBox.enabled = true;
                                    transform.gameObject.layer = 0;
                                    ConstructionProcess = true;
                                    buildingsManager.isPlacing = false;
                                    BuildZone.SetActive(false);
                                    buildingsManager.HideBuildZones();

                                }

                                #endregion

                            }
                                                                                

                        }
                        

                    }
                    else
                    {
                        

                        //В корутину отправляется цвет модели
                        if (!isFlashing)
                        {
                            Debug.Log("Need more resources!");
                            StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));
                        }

                    }

                    #endregion


                }
                else
                {

                    #region SitePlacing

                    bool Collision = PyramideRayCheck();

                    if (Collision)
                    {
                        Debug.Log("Collision!");
                        StartCoroutine(ZoneFlasher(BuildZone.GetComponent<MeshRenderer>().material.color));

                    }
                    else
                    {
                        //Запускаем появление строительной площадки
                        ConstructionProcess = true;

                        MyBox.size = buildingPrefab.MyBox.size;
                        MyBox.enabled = true;

                        transform.gameObject.layer = 0;

                        buildingsManager.isPlacing = false;
                        BuildZone.SetActive(false);
                        buildingsManager.HideBuildZones();

                        MySelector = Instantiate(buildingPrefab.MySelector, transform);

                        MySelector.GetComponent<BuildingSelector2DScript>().MyHud.ObjectName.text = buildingPrefab.buildingStats.Name;
                        MySelector.GetComponent<BuildingSelector2DScript>().MyHud.healthBarScript.SetHealth(0);

                        var BPID = buildingPrefab.GetComponent<ID>();

                        MyID.playerID = BPID.playerID;
                        MyID.objectClass = 21;
                        MyID.pesonalID = BPID.pesonalID;
                        MyID.groupID = BPID.pesonalID;
                        MyID.typeID = BPID.typeID;
                    }

                    #endregion

                }


            }

            #endregion

            #region Right Click

            if (Input.GetMouseButton(1))
            {

                CancelBlueprint();

            }

            #endregion 

        }

    }

    public void CancelBlueprint()
    {

        buildingsManager.isPlacing = false;
        BuildZone.SetActive(false);
        buildingsManager.HideBuildZones();

        if (ParentBuilding != null)
        {
            ParentBuilding.isBusy = true;
            ParentBuilding.isReadyForPlacing = true;
            ParentBuilding.ResourceConsumed = ResourceConsumed;

        }

        Destroy(gameObject);

    }

    #endregion

    #region Collisions Raycast Check

    public void DebugPyramideDraw()
    {
        #region Рассчет координат ребер и углов пирамиды


        //Габаритные координаты BoxCollider с учетом искажения от скаллирования
        var X = MyBox.size.x * transform.localScale.x;
        var Y = MyBox.size.y * transform.localScale.y;
        var Z = MyBox.size.z * transform.localScale.z;

        var GroundCollisionCorrection = 0.2f;

        //Кординаты центра BoxCollider полученые с учетом искажения от скаллирования
        var CenterX = MyBox.center.x * transform.localScale.x;
        var CenterY = (MyBox.center.y * transform.localScale.y) + GroundCollisionCorrection;
        var CenterZ = MyBox.center.z * transform.localScale.z;

        //Центр полученный из координат центра
        var Center = transform.position + new Vector3(CenterX, CenterY, CenterZ);

        //Проекция центра на нижнюю плоскость
        var BottomCenter = Center + new Vector3(0, -Y, 0) / 2;


        //Середины ребер вычисляются от половины суммы позиции углов
        var middleFront = new Vector3(X, -Y, Z) / 2 + new Vector3(-X, -Y, Z) / 2;
        var middleBack = new Vector3(X, -Y, -Z) / 2 + new Vector3(-X, -Y, -Z) / 2;
        var middleRight = new Vector3(X, -Y, Z) / 2 + new Vector3(X, -Y, -Z) / 2;
        var middleLeft = new Vector3(-X, -Y, Z) / 2 + new Vector3(-X, -Y, -Z) / 2;

        //Debug.DrawLine(Center + new Vector3( X,-Y, Z) / 2, Center + new Vector3(-X,-Y, Z) / 2, Color.magenta);
        //Debug.DrawLine(Center + new Vector3( X,-Y,-Z) / 2, Center + new Vector3(-X,-Y,-Z) / 2, Color.magenta);
        //Debug.DrawLine(Center + new Vector3( X,-Y, Z) / 2, Center + new Vector3( X,-Y,-Z) / 2, Color.magenta);
        //Debug.DrawLine(Center + new Vector3(-X,-Y, Z) / 2, Center + new Vector3(-X,-Y,-Z) / 2, Color.magenta);

        //Наклонные лучи к углам из общего центра BoxCollider
        Debug.DrawRay(Center, new Vector3(X, -Y, Z) / 2, Color.cyan);
        Debug.DrawRay(Center, new Vector3(-X, -Y, Z) / 2, Color.cyan);
        Debug.DrawRay(Center, new Vector3(-X, -Y, -Z) / 2, Color.cyan);
        Debug.DrawRay(Center, new Vector3(X, -Y, -Z) / 2, Color.cyan);

        //Лучи по ребру из угла в угол 2 луча из правого фронтального
        Debug.DrawRay(Center + new Vector3(X, -Y, Z) / 2, new Vector3(-X, 0, 0), Color.blue);
        Debug.DrawRay(Center + new Vector3(X, -Y, Z) / 2, new Vector3(0, 0, -Z), Color.blue);

        //2 луча из левого заднего
        Debug.DrawRay(Center + new Vector3(-X, -Y, -Z) / 2, new Vector3(X, 0, 0), Color.blue);
        Debug.DrawRay(Center + new Vector3(-X, -Y, -Z) / 2, new Vector3(0, 0, Z), Color.blue);

        


        //Лучи из центра BoxCollider к серединам нижних ребер
        Debug.DrawRay(Center, middleFront / 2, Color.green);
        Debug.DrawRay(Center, middleBack / 2, Color.green);
        Debug.DrawRay(Center, middleRight / 2, Color.green);
        Debug.DrawRay(Center, middleLeft / 2, Color.green);

        //Луч из центра до нижней плоскости
        Debug.DrawRay(Center, new Vector3(0, -Y, 0) / 2, Color.red);

        //Лучи из центра нижней плоскости к углам
        Debug.DrawRay(BottomCenter, new Vector3(X, 0, Z) / 2, Color.cyan);
        Debug.DrawRay(BottomCenter, new Vector3(-X, 0, Z) / 2, Color.cyan);
        Debug.DrawRay(BottomCenter, new Vector3(-X, 0, -Z) / 2, Color.cyan);
        Debug.DrawRay(BottomCenter, new Vector3(X, 0, -Z) / 2, Color.cyan);

        //Лучи из центра нижней плоскости к серединам ребер
        Debug.DrawRay(BottomCenter, (middleFront - new Vector3(0, -Y, 0)) / 2, Color.green);
        Debug.DrawRay(BottomCenter, (middleBack - new Vector3(0, -Y, 0)) / 2, Color.green);
        Debug.DrawRay(BottomCenter, (middleRight - new Vector3(0, -Y, 0)) / 2, Color.green);
        Debug.DrawRay(BottomCenter, (middleLeft - new Vector3(0, -Y, 0)) / 2, Color.green);

        #endregion
    }

    public bool PyramideRayCheck()
    {

        #region Объявление локальных переменных

        //Габаритные координаты BoxCollider с учетом искажения от скаллирования
        var X = MyBox.size.x * transform.localScale.x;
        var Y = MyBox.size.y * transform.localScale.y;
        var Z = MyBox.size.z * transform.localScale.z;

        var GroundCollisionCorrection = 0.2f;
        // Debug.Log(X + "," + Y + "," + Z);

        //Кординаты центра BoxCollider полученые с учетом искажения от скаллирования
        var CenterX = MyBox.center.x * transform.localScale.x;
        var CenterY = (MyBox.center.y * transform.localScale.y)+GroundCollisionCorrection;
        var CenterZ = MyBox.center.z * transform.localScale.z;

        //Центр полученный из координат центра
        var Center = transform.position + new Vector3(CenterX, CenterY, CenterZ);

        //Проекция центра на нижнюю плоскость
        var BottomCenter = Center + new Vector3(0, -Y*.9f, 0) / 2;


        //Середины ребер вычисляются от половины суммы позиции углов
        var middleFront = new Vector3 (X, -Y , Z) / 2 + new Vector3(-X, -Y,  Z) / 2;
        var middleBack  = new Vector3( X, -Y, -Z) / 2 + new Vector3(-X, -Y, -Z) / 2;
        var middleRight  = new Vector3( X, -Y,  Z) / 2 + new Vector3( X, -Y, -Z) / 2;
        var middleLeft = new Vector3(-X, -Y,  Z) / 2 + new Vector3(-X, -Y, -Z) / 2;


        //Длинны сторон
        var FrontSideLength = ((Center + new Vector3(X, -Y, Z) / 2) - (Center + new Vector3(-X, -Y, Z) / 2)).magnitude;
        var RightSideLenght = ((Center + new Vector3(X, -Y, Z) / 2) - (Center + new Vector3(X, -Y, -Z) / 2)).magnitude;
        var BackSideLenght = ((Center + new Vector3(-X, -Y, -Z) / 2) - (Center + new Vector3(X, -Y, -Z) / 2)).magnitude;
        var LeftSideLenght = ((Center + new Vector3(-X, -Y, -Z) / 2) - (Center + new Vector3(-X, -Y, Z) / 2)).magnitude;




        #endregion

        #region Объявления лучей

        //Наклонные лучи к углам из общего центра BoxCollider
        //R-Right F-Front L-Left B-Back
        Ray CenterToRFCorner = new Ray(Center, new Vector3( X, -Y,  Z) / 2);
        Ray CenterToLFCorner = new Ray(Center, new Vector3(-X, -Y,  Z) / 2);
        Ray CenterToLBCorner = new Ray(Center, new Vector3(-X, -Y, -Z) / 2);
        Ray CenterToRBCorner = new Ray(Center, new Vector3( X, -Y, -Z) / 2);



        //Лучи по ребру из угла в угол 2 луча из правого фронтального
        Ray FromRFCornerToLFCorner = new Ray(Center + new Vector3( X, -Y,  Z) / 2, new Vector3(-X,  0,  0));
        Ray FromRFCornerToRBCorner = new Ray(Center + new Vector3( X, -Y , Z) / 2, new Vector3( 0,  0, -Z));

        //2 луча из левого заднего
        Ray FromLBCornerToRBCorner = new Ray(Center + new Vector3(-X, -Y, -Z) / 2, new Vector3( X,  0,  0));
        Ray FromLBCornerToLFCorner = new Ray(Center + new Vector3(-X, -Y, -Z) / 2, new Vector3( 0,  0,  Z));

        //2 луча из левого фронтального
        Ray FromLFCornerToRFCorner = new Ray(Center + new Vector3(-X, -Y,  Z) / 2, new Vector3( X,  0,  0));
        Ray FromLFCornerToLBCorner = new Ray(Center + new Vector3(-X, -Y,  Z) / 2, new Vector3( 0,  0, -Z));

        //2 луча из правого заднего
        Ray FromRBCornerToLBCorner = new Ray(Center + new Vector3( X, -Y, -Z) / 2, new Vector3(-X,  0,  0));
        Ray FromRBCornerToRFCorner = new Ray(Center + new Vector3( X, -Y, -Z) / 2, new Vector3( 0,  0,  Z));


        //Лучи из центра BoxCollider к серединам нижних ребер
        Ray CenterToMiddleFront = new Ray(Center, middleFront / 2);
        Ray CenterToMiddleBack = new Ray(Center, middleBack / 2);
        Ray CenterToMiddleRight = new Ray(Center, middleRight / 2);
        Ray CenterToMiddleLeft = new Ray(Center, middleLeft / 2);


        //Луч из центра до нижней плоскости
        Ray CenterToBottonCenter = new Ray(Center, new Vector3(0, -Y, 0) / 2);

        //Лучи из центра нижней плоскости к углам
        Ray BottomCenterToRFCorner = new Ray(BottomCenter, new Vector3( X, 0,  Z) / 2);
        Ray BottomCenterToLFCorner = new Ray(BottomCenter, new Vector3(-X, 0,  Z) / 2);
        Ray BottomCenterToLBCorner = new Ray(BottomCenter, new Vector3(-X, 0, -Z) / 2);
        Ray BottomCenterToRBCorner = new Ray(BottomCenter, new Vector3( X, 0, -Z) / 2);


        //Лучи из центра нижней плоскости к серединам ребер
        Ray BottomCenterToMiddleFront = new Ray(BottomCenter, (middleFront - new Vector3(0, -Y, 0)) / 2);
        Ray BottomCenterToMiddleBack  = new Ray(BottomCenter, (middleBack  - new Vector3(0, -Y, 0)) / 2);
        Ray BottomCenterToMiddleRight = new Ray(BottomCenter, (middleRight - new Vector3(0, -Y, 0)) / 2);
        Ray BottomCenterToMiddleLeft  = new Ray(BottomCenter, (middleLeft  - new Vector3(0, -Y, 0)) / 2);




        #endregion

        #region Проверка лучами в нижней плоскости из угла в угол.

        // Debug.DrawRay(FromRFCornerToLFCorner.origin, FromRFCornerToLFCorner.direction * FrontSideLength,Color.red);
        // Debug.DrawRay(FromRFCornerToRBCorner.origin, FromRFCornerToRBCorner.direction * RightSideLenght, Color.red);
        // Debug.DrawRay(FromLBCornerToRBCorner.origin, FromLBCornerToRBCorner.direction * BackSideLenght, Color.red);
        // Debug.DrawRay(FromLBCornerToLFCorner.origin, FromLBCornerToLFCorner.direction * LeftSideLenght, Color.red);

        if (Physics.Raycast(FromRFCornerToLFCorner, out hitCheck, FrontSideLength, ~layerMask))
        {

            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                  //  Debug.Log("Front from Right Contact" + hitCheck.collider.name);

                  // StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }


            }


        }
        else if (Physics.Raycast(FromRFCornerToRBCorner, out hitCheck, RightSideLenght, ~layerMask))
        {
            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                  //  Debug.Log("Right from Front Contact" + hitCheck.collider.name);

                  //  StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }


            }

        }
        else if (Physics.Raycast(FromLBCornerToRBCorner, out hitCheck, BackSideLenght, ~layerMask))
        {
            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                  //  Debug.Log("Back from left Contact" + hitCheck.collider.name);

                  //  StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }

            }

        }
        else if (Physics.Raycast(FromLBCornerToLFCorner, out hitCheck, LeftSideLenght, ~layerMask))
        {
            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                  // Debug.Log("Left from Back Contact" + hitCheck.collider.name);

                  //  StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }

            }

        }
        else if (Physics.Raycast(FromLFCornerToRFCorner, out hitCheck, FrontSideLength, ~layerMask))
        {
            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                  //  Debug.Log("Front from left Contact" + hitCheck.collider.name);

                  //  StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }

            }

        }
        else if (Physics.Raycast(FromLFCornerToLBCorner, out hitCheck, LeftSideLenght, ~layerMask))
        {
            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                   // Debug.Log("Left from front Contact" + hitCheck.collider.name);

                   // StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }

            }

        }
        else if (Physics.Raycast(FromRBCornerToRFCorner, out hitCheck, RightSideLenght, ~layerMask))
        {
            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                   // Debug.Log("Right from Back Contact" + hitCheck.collider.name);

                   // StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }


            }

        }
        else if (Physics.Raycast(FromRBCornerToLBCorner, out hitCheck, BackSideLenght, ~layerMask))
        {
            if (hitCheck.collider.gameObject.activeSelf)
            {

                if (!isFlashing)
                {

                   // Debug.Log("Back from right Contact" + hitCheck.collider.name);

                   // StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));

                    return true;

                }

            }

        }
        #endregion

        //Возвращаем ложь когда ничего не пересекли.
        return false;
    }

    #endregion

    #region Construction Site Procesings

    //Написать метод складывания ресурсов на площадку.
    public void StorageResource(Unit UnloadingUnit,int Amount)
    {
        if(isNeedForResource)
        {
            currentResourcesOnSite += Amount;
        }

        if(currentResourcesOnSite >= buildingPrefab.buildingStats.buildingCost)
        {
            isNeedForResource = false;
        }

        UnloadingUnit.transform.Rotate(Vector3.up, 170);

    }

    public int StoragingAmountCheck(Unit UnloadingUnit,int Amount)
    {
        if(UnloadingUnit.CurrentCarryAmount>= buildingPrefab.buildingStats.buildingCost-currentResourcesOnSite)
        {
            return buildingPrefab.buildingStats.buildingCost - currentResourcesOnSite;
        }
        else
        {
            return UnloadingUnit.CurrentCarryAmount;
        }

    }
    
    
    
    //Написать метод получения работы на площадке.
   


    public void TakeWork(Unit Builder,float Amount)
    {
        if(!buildersList.Contains(Builder))
        {
            buildersList.Add(Builder);
        }        

        currentWorkProgress += Amount;

        

        MySelector.GetComponent<BuildingSelector2DScript>().MyHud.healthBarScript.SetHealth(currentWorkProgress); 

     

        Builder.AddFloatingText((int)Amount, transform.position, Vector3.right * 2, Color.white);
        

        if(building==null)
        {
            InitializeBuilding();

            ProgressWork();

        }
        else
        {
            ProgressWork();
            
        }

        if (currentWorkProgress >= buildingPrefab.buildingStats.health)
        {
            WorkComplete();
        }


    }

    public void InitializeBuilding()
    {
        MySelector.GetComponent<BuildingSelector2DScript>().MyHud.healthBarScript.SetMaxHealth(buildingPrefab.buildingStats.health); 

        blueprintHUDScript.gameObject.SetActive(true);

        blueprintHUDScript.progressBarScript.SetMaxAmmount(100);

        building = Instantiate(buildingPrefab.gameObject, transform.position , transform.rotation, buildingsManager.BuildingsFolder);

        ModelEndPos = building.GetComponent<BuildingScript>().MyModel.transform.position;

        var Height = buildingPrefab.GetComponent<BoxCollider>().size.y * buildingPrefab.transform.localScale.y;

        building.GetComponent<BuildingScript>().MyModel.transform.position += new Vector3(0, -Height * 1.2f, 0);

        ModelStartPos = building.GetComponent<BuildingScript>().MyModel.transform.position;

        buildingsManager.AddBuildingToLists(building);

    }

    public void ProgressWork()
    {
        var Progress = (currentWorkProgress / buildingPrefab.buildingStats.health) * 100;

        //Debug.Log(Progress);

        UpdateProgressBar((int)Progress);

        var ModelPos = building.GetComponent<BuildingScript>().MyModel.transform;

        ModelPos.position = Vector3.Lerp(ModelStartPos, ModelEndPos, 0.01f * Progress);

        MyModel.GetComponent<MeshRenderer>().material.color = new Color(0.784f, 0.784f, 0.784f, 0.392f - 0.392f * Progress / 100.0f);

        UpdateChildColor();

    }

    public void WorkComplete()
    {
        building.GetComponent<BoxCollider>().enabled = true;

        foreach (var builder in buildersList)
        {
            builder.buildingsCompleted += 1;
        }

        Destroy(gameObject);
    }

    //Написать метод подсчета необходимого количества ресурса.

    public int LoadingAmmount(Unit LoadingUnit)
    {
        if (isNeedForResource)
        {
            var amount = buildingPrefab.buildingStats.buildingCost - currentResourcesOnSite ;

            if (amount > LoadingUnit.unitStats.carryAmount)
            {
                return LoadingUnit.unitStats.carryAmount;
            }
            else
            {
                return amount;
            }
        }
        else
        {
            return 0;
        }
                      
    }

    public void CollectResource(Unit UnloadingUnit, int Ammount)
    {

        StartCoroutine(UnlodingunitEnterUnloadingPlace(UnloadingUnit, Ammount));

    }


    //Корутина разгузки юнита
    IEnumerator UnlodingunitEnterUnloadingPlace(Unit UnloadingUnit, int Amount)
    {
        if (UnloadingUnit.Selected)
        {
            UnitManager.Instance.UnselectUnit(UnloadingUnit);
        }

        UnloadingUnit.gameObject.SetActive(false);

        //Длительность разгрузки можно сделать напрямую зависимой от количества разгружаемого ресурса.
        yield return new WaitForSeconds(1.0f);
               
        //Тут нужно вызывать метод получения ресурсов от юнита на склад площадки.
        RTSGameManager.StorageResourcesFromUnloadingToSite(UnloadingUnit, this, Amount);
        UnloadingUnit.CurrentCarryAmount -= Amount;

        yield return new WaitForSeconds(1.0f);

        if(UnloadingUnit.CurrentCarryAmount==0)
        {
            UnloadingUnit.ResourceModel.SetActive(false);
            UnloadingUnit.isLoaded = false;
        }
        
        UnloadingUnit.gameObject.SetActive(true);



        
        //UnloadingUnit.navAgent.destination = UnloadingUnit.currentVein.OriginPoint.transform.position;

        //Debug.Log("Unloaded!");


    }



    #endregion

    #region Building Construction Corutine

    IEnumerator BuildingConstruction(GameObject Prefab, Vector3 Position, Quaternion Rotation)
    {
        if (ParentBuilding != null)
        {
            ParentBuilding.ProducedBlueprintForPlacing = null;
        }

        var Building = Instantiate(Prefab, Position, Rotation,buildingsManager.BuildingsFolder);

        ModelEndPos = Building.GetComponent<BuildingScript>().MyModel.transform.position;

        var Height = buildingPrefab.GetComponent<BoxCollider>().size.y * buildingPrefab.transform.localScale.y;

        Building.GetComponent<BuildingScript>().MyModel.transform.position += new Vector3(0, -Height*1.2f, 0);

        ModelStartPos = Building.GetComponent<BuildingScript>().MyModel.transform.position;

        blueprintHUDScript.gameObject.SetActive(true);

        blueprintHUDScript.progressBarScript.SetMaxAmmount(100);

        buildingsManager.AddBuildingToLists(Building);
        
        // Debug.Log("Start color  "+MyModel.GetComponent<MeshRenderer>().material.color);

        //Количество единиц в цикле = количество промежутков в анимации
        for (int i = 0; i < 100; i++)
        {

            blueprintHUDScript.progressBarScript.SetCurrentAmmount(i);
            blueprintHUDScript.progressBarScript.ProgressBarText.text = i + "%";
            //Debug.Log(i);
            //Анимация вылезания здания из под земли
            var ModelTransform = Building.GetComponent<BuildingScript>().MyModel.transform;

            ModelTransform.position = Vector3.Lerp(ModelStartPos, ModelEndPos, 0.01f * i);

            MyModel.GetComponent<MeshRenderer>().material.color = new Color(0.784f, 0.784f, 0.784f, 0.392f - 0.392f * i / 100.0f);
            UpdateChildColor();
            // Debug.Log(MyModel.GetComponent<MeshRenderer>().material.color);

            yield return new WaitForSeconds(.025f);

        }

        Building.GetComponent<BoxCollider>().enabled = true;

        

        Destroy(gameObject);

    }

#endregion

    #region Unit Building Corutine

    IEnumerator UnitBuildingConstruction(Unit Builder ,GameObject Prefab, Vector3 Position, Quaternion Rotation,float buildingSpeed)
    {
        blueprintHUDScript.gameObject.SetActive(true);
        blueprintHUDScript.progressBarScript.SetMaxAmmount(100);

        if (Builder.Selected)
        {

            buildingsManager.unitManager.UnselectUnit(Builder);

        }

        Builder.gameObject.SetActive(false);
        


        var Building = Instantiate(Prefab, Position, Rotation, buildingsManager.BuildingsFolder);

        ModelEndPos = Building.GetComponent<BuildingScript>().MyModel.transform.position;

        var Height = buildingPrefab.GetComponent<BoxCollider>().size.y * buildingPrefab.transform.localScale.y;

        Building.GetComponent<BuildingScript>().MyModel.transform.position += new Vector3(0, -Height*1.2f , 0);

        ModelStartPos = Building.GetComponent<BuildingScript>().MyModel.transform.position;



        buildingsManager.AddBuildingToLists(Building);
        // Debug.Log("Start color  "+MyModel.GetComponent<MeshRenderer>().material.color);

        //Количество единиц в цикле = количество промежутков в анимации
        for (int i = 0; i < 100; i++)
        {
            blueprintHUDScript.progressBarScript.SetCurrentAmmount(i);
            blueprintHUDScript.progressBarScript.ProgressBarText.text = i + "%";

            //Debug.Log(i);
            //Анимация вылезания здания из под земли

            var ModelTransform = Building.GetComponent<BuildingScript>().MyModel.transform;

            ModelTransform.position = Vector3.Lerp(ModelStartPos, ModelEndPos, 0.01f * i);


            MyModel.GetComponent<MeshRenderer>().material.color = new Color(0.784f, 0.784f, 0.784f, 0.392f - 0.392f * i / 100.0f);
            UpdateChildColor();
            // Debug.Log(MyModel.GetComponent<MeshRenderer>().material.color);

            yield return new WaitForSeconds(buildingSpeed/100);

        }

        Building.GetComponent<BoxCollider>().enabled = true;


        if(Builder.unitStats.unitName!="MCV")
        {
            Builder.gameObject.SetActive(true);
            Builder.transform.position = Building.GetComponent<BuildingScript>().OriginPoint.transform.position;
        }
        else
        {
            Builder.Disassemble();
        }


        
        
        Debug.Log("Complete");
        Destroy(gameObject);

    }

    #endregion

    #region Flashers

    IEnumerator Flasher(Color defaultColor)
    {
        isFlashing = true;
        //Debug.Log(transform.name + "is Flashing!");
        var renderer = MyModel.GetComponent<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = Color.red;
            UpdateChildColor();
            yield return new WaitForSeconds(.05f);
            renderer.material.color = defaultColor;
            UpdateChildColor();
            yield return new WaitForSeconds(.05f);
        }

        isFlashing = false;
    }

    IEnumerator ZoneFlasher(Color defaultColor)
    {
        isFlashing = true;
        //Debug.Log(transform.name + "is Flashing!");
        var renderer = BuildZone.GetComponent<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = Color.red;
            UpdateChildColor();
            yield return new WaitForSeconds(.05f);
            renderer.material.color = defaultColor;
            UpdateChildColor();
            yield return new WaitForSeconds(.05f);
        }

        isFlashing = false;
    }

    #endregion

    #region Helping and Other

    public void UpdateProgressBar(int Amount)
    {
        blueprintHUDScript.progressBarScript.SetCurrentAmmount(Amount);
        blueprintHUDScript.progressBarScript.ProgressBarText.text = Amount + "%";
    }


    public void UpdateChildColor()
    {
        if (haveChilds)
        {
            MyModel.GetComponent<ParentObject>().Child.SetColor();
        }
    }


    //Неиспользуемый метод с баундами. Интересно его тоже реализовать.
    public static bool IsInsideBoxCollider(BoxCollider aCol, Vector3 aPoint)
    {
        var b = aCol.bounds;
        var p = aCol.transform.InverseTransformPoint(aPoint);
        return b.Contains(p);
    }


    public void Select()
    {
        SelectionManager.Instance.DeselectAll();

        MySelector.SetActive(true);

        Selected = true;

        SelectionManager.Instance.SelectBlueprint(this);

        ActivateGUI();
    }

    public void Deselect()
    {
        MySelector.SetActive(false);

        Selected = false;

        DeactivateGUI();
    }
    

    private void OnMouseEnter()
    {
        if(MySelector!=null)
        {
            if(!Selected)
            {
                MySelector.SetActive(true);
            }
        
            
            
        }
                
    }


    private void OnMouseDown()
    {
        if(MySelector!=null)
        {
            if(!Selected)
            {
                                

                Select();


            }

            

        }

       

    }

    private void OnMouseExit()
    {
        
        if (MySelector != null)
        {
            if (!Selected)
            {
                MySelector.SetActive(false);
            }
            

        }


    }

    #endregion

    #region GUI

    public void ActivateGUI()
    {
        GUIManager.ActivateGUIElement(GUIManager.Instance.cancelWindow,true);

    }

    public void DeactivateGUI()
    {
        GUIManager.DeactivateGUIElement(GUIManager.Instance.ActiveGUIElement,true);
    }

    public void CancelButton()
    {
        if(buildersList.Count>0)
        {
            foreach (var builder in buildersList)
            {
                if (builder.currentSite == this)
                {
                    
                    builder.currentSite = null;
                    builder.isWorkingOnSite = false;

                    if(builder.currentTarget==this.transform)
                    {
                        Builder.Stop();
                    }
                    
                }

            }
        }
      

        if (SelectionManager.Instance.SelectedBlueprint == this)
        {
            SelectionManager.Instance.DeselectAll();
        }

        if(building!=null)
        {
            building.GetComponent<BuildingScript>().Canceled();
        }


        Destroy(this.gameObject);
    }

    #endregion


}
