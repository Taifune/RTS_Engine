using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{

    //Объявление для объекта используемого для селектора
    public GameObject MySelector;
    public GameObject MyModel;
    public GameObject ResourceModel;
    public Transform TargetingTransform;
    public Transform ProjectileOriginPoint;
    public GameObject TempObject = null;
    //Объявление для объекта используемого для интерфейса что бы связаться с хелсбаром
    public UnitHUDScript MyHUD;

    //Флаг для мультивыбора
    public bool Selected = false;
    public int SelectedListIndex;
    public Transform currentTarget;
    public Vector3 lastKnownTargetPos;

    public Unit currentAttacker;
    public List<Unit> PersonalDammazKron = new List<Unit>(); //DammazKron - Great Book of Grudges - Великая Книга Обид


    public bool AttackComand = true;
    public bool AnswerFlag = true;
    public bool GuardFlag = false;
    public float attackColdDownTimer;
    public bool isTargeting;

    public bool isDead;

    //public UnitManager MyUnitManager;
    public NavMeshAgent navAgent;
    public UnitStats unitStats;
    public float currentHealth;
    public int currentAmmo;
    


    public bool isGatheringInMode;
    public bool isLoaded;
    public int CurrentCarryAmount;
    public bool movingForUnload = false;
    public bool movingForLoad = false;
    public VeinScript currentVein = null;
    public BuildingScript currentHome = null;
    

    public bool isWorkingOnSite;
    public BlueprintScript currentSite;
    public int buildingsCompleted = 0;
    public float workColdDownTimer;

    public bool ReadyForPatrool;
    public bool isPatrooling;
    public bool isMovingToPos2;
    public bool isMovingToPoint;
    public Vector3 PatroolPos1;
    public Vector3 PatroolPos2;

    public bool ForceAttackFlag;
    public bool ForcedMoveButtonFlag;
    public bool GatherButtonFlag;

    public bool isFlashing = false;
    public bool isAnimate = false;
    public bool MouseOverBypass = false;

    public UnitManager unitManager;

    public bool ScanMode;
    //public float ScanSectorValue = 60;
    //public float ScanAngleSpeed = 0.5f;

    [SerializeField] bool minusangel = false;
    [SerializeField] private bool zero = false;
    [SerializeField] private bool zeroplus = false;

    public Plane Plane
    {
        private set;
        get;
    }


    private void Start()
    {

        unitManager = FindObjectOfType<UnitManager>();


        //Обнуление таймера для первой атаки из статов юнита
        attackColdDownTimer = unitStats.attackSpeed;

        if (unitStats.canConstructBuilding)
        {
            workColdDownTimer = unitStats.workSpeed;
        }
        
        //Установка текущего значения параметра здоровья из связанного назначеннго ScriptableObject
        currentHealth = unitStats.health;
        currentAmmo = unitStats.ammoAmount;

        //Установка дистанции остановки 
        NavAgentNewStopingDistance(unitStats.moveStopRange);

        //Отправка начального значения в интефейс полоски здоровья
        MyHUD.healthBarScript.SetMaxHealth(unitStats.health);


    }


    private void Update()
    {
        //Update timer обновляем таймер для юнита после атаки. Во время атаки таймер сбрасывается до 0. Зависит от скорости атаки из статов.
        Timers();


        //Првоерка здоровья на запуск события умирания
        HealthCheck();

        PatroolProcess();

        ScanProcess();

        TargetProcess();

    }


    #region UpdateMetods

    public void Timers()
    {

        //Update timer обновляем таймер для юнита после атаки. Во время атаки таймер сбрасывается до 0. Зависит от скорости атаки из статов.
        if (attackColdDownTimer <= unitStats.attackSpeed)
        {
            attackColdDownTimer += Time.deltaTime;
        }

        if(unitStats.canConstructBuilding)
        {
            if (workColdDownTimer <= unitStats.workSpeed)
            {
                workColdDownTimer += Time.deltaTime;
            }

        }


    }

    public void HealthCheck()
    {
        if (currentHealth <= 0)
        {

            Dying();

        }
    }

    public void PatroolProcess()
    {

        #region Patrool;

        if (isPatrooling)
        {
            if (isMovingToPos2)
            {
                var distance = (transform.position - PatroolPos2).magnitude;

                if (distance >= unitStats.moveStopRange)
                {
                    if (!isMovingToPoint)
                    {
                        MoveToPatroolPos(PatroolPos2);
                    }

                }
                else if (distance <= unitStats.moveStopRange)
                {
                    isMovingToPos2 = false;
                    isMovingToPoint = false;
                }
            }
            else
            {
                var distance = (transform.position - PatroolPos1).magnitude;

                if (distance >= unitStats.moveStopRange)
                {
                    MoveToPatroolPos(PatroolPos1);
                }
                else if (distance <= unitStats.moveStopRange)
                {
                    isMovingToPos2 = true;
                    isMovingToPoint = false;
                }
            }

        }


        #endregion

    }

    public void ScanProcess()
    {
        if (ScanMode)
        {
            ScanRaycasting();
        }

    }

    public void TargetProcess()
    {


        #region Target Check

        if (currentTarget != null)
        {
            if (isGatheringInMode)
            {

                #region Gathering

              

                if (!isLoaded)
                {

                    if (!movingForLoad)
                    {
                        if (currentVein != null)
                        {
                            TakeWaytoVein();
                        }

                    }
                    else
                    {
                        var distance = (transform.position - currentVein.OriginPoint.transform.position).magnitude;

                        //Debug.Log(distance);

                        if (distance <= 2)
                        {
                            LoadResource(currentVein);
                        }
                        else if (distance >= 2)
                        {
                            // UpdatePosition();//цели не несет в контексте сбора
                        }

                    }




                }
                else
                {
                    if (!movingForUnload)
                    {
                        if (currentHome == null)
                        {
                            var Finded = FindTheWayToCB();

                            if (Finded)
                            {
                                TakeWaytoHomeForUnload();
                            }
                            else
                            {
                                Stop();
                            }


                        }
                        else
                        {
                            TakeWaytoHomeForUnload();
                        }



                    }
                    else
                    {

                        var distancetohome = (transform.position - currentHome.OriginPoint.transform.position).magnitude;
                        if (distancetohome <= 2)
                        {
                            UnloadResource(currentHome);

                        }
                        else if (distancetohome >= 2)
                        {
                            //TakeWaytoHome();
                            // Debug.Log("Moving, distance to go "+distancetohome);
                        }


                    }

                }


                #endregion

            }
            else if (isWorkingOnSite)
            {

                #region Working On Site                            
                
                if (currentSite.isNeedForResource)
                {
                    if (isLoaded)
                    {
                        //Двигаться на разгрузку к площадке
                        if (!movingForUnload)
                        {
                            TakeWaytoSite();
                        }
                        else
                        {
                            //проверка дистанции
                            var distance = (transform.position - currentSite.transform.position).magnitude;

                            //Debug.Log(distance);

                            if (distance <= currentSite.buildingPrefab.buildingStats.buildingSize)
                            {
                                UnloadResource(currentSite);
                            }
                            else if (distance >= currentSite.buildingPrefab.buildingStats.buildingSize)
                            {
                                if (navAgent.destination != currentSite.transform.position)
                                {
                                    TakeWaytoSite();
                                }
                            }

                        }




                    }
                    else
                    {
                        //Двигаться на загрузку к ближайшему месту где загружают ресурс.
                        if (currentHome != null)
                        {
                            if(!movingForLoad)
                            {
                                TakeWaytoHomeForLoad();
                            }
                            else
                            {
                                DistanceToHomeForLoadingCheck();
                            }
                        }
                        else
                        {
                            if(!movingForLoad)
                            {

                                var Finded = FindTheWayToCB();

                                if (Finded)
                                {
                                    TakeWaytoHomeForLoad();
                                }
                                else
                                {
                                    Stop();
                                }

                            }
                            else
                            {
                                DistanceToHomeForLoadingCheck();

                            }
                        }
                    }
                }
                else
                {

                    if (currentSite.currentWorkProgress != 100)
                    {
                        //Двигаться к площадке и производить работу увеличивая хп здания. 

                        //проверка дистанции
                        var distance = (transform.position - currentSite.transform.position).magnitude;

                        //Debug.Log(distance);

                        if (distance <= currentSite.buildingPrefab.buildingStats.buildingSize)
                        {
                            //оставновка на месте.
                            NavAgenNewDestination(transform.position);

                            //работаем увеличиваем хп здания на площадке на силу работы
                            Work();

                        }
                        else if (distance >= currentSite.buildingPrefab.buildingStats.buildingSize)
                        {
                            NavAgenNewDestination(currentSite.transform.position);
                        }



                    }
                    else
                    {

                        ClearWorkingFlags();
                        currentSite = null;


                        //Работа завершена снимаем флаги обнуляем переменные связанные с работой на площадке
                    }


                }




                #endregion

            }
            else
            {

                if (currentTarget.CompareTag("Building"))//Переделать на ID 
                {
                    #region Building

                    if (currentTarget.GetComponent<BuildingScript>().isDestroyed)
                    {
                        NextTarget();
                    }
                    else
                    {
                        if (AttackComand)
                        {

                            if (currentTarget != null)
                            {


                                NavAgenNewDestination(lastKnownTargetPos);
                                NavAgentNewStopingDistance(unitStats.attackRange * 0.9f);


                                //Проверка дальности до цели в чистом значении magnitude
                                var distance = (transform.position - currentTarget.position).magnitude;

                                if (distance <= unitStats.attackRange)
                                {
                                    Attack();
                                }
                                else if (distance >= unitStats.attackRange)
                                {
                                    UpdatePosition();
                                }

                            }

                        }

                    }

                    #endregion
                }
                else
                {

                    #region Unit
                    //При поднятом флаге цели isDead происходит смена цели в методе NextTarget()
                    if (currentTarget.GetComponent<Unit>().isDead)
                    {
                        NextTarget();

                    }
                    else
                    {


                        if (AttackComand)
                        {


                            if (currentTarget != null)
                            {
                                // Запуск события активности через RTSGameManager. Активные юниты попадают в лист.

                                // RTSGameManager.UnitHaveActivity(this, FindObjectOfType<UnitManager>()); Лучше запускать активность в самом RTSGameManager



                                //Назначение цели для движения из кеша позиции последнего места значения объекта
                                NavAgenNewDestination(lastKnownTargetPos);

                                //Обновление дистанции остановки с подгрузкой из привязанного ScriptableObject
                                NavAgentNewStopingDistance(unitStats.attackRange);

                                //Проверка дальности до цели в чистом значении magnitude
                                var distance = (transform.position - currentTarget.position).magnitude;

                                if (distance <= unitStats.attackRange)
                                {

                                    Attack();

                                }
                                else if (distance >= unitStats.attackRange)
                                {
                                    UpdatePosition();
                                }
                            }


                        }
                    }
                    #endregion

                }

            }

        }
        else
        {
            ClearWorkingFlags();
        }
        #endregion


    }

    public void DistanceToHomeForLoadingCheck()
    {
        var distancetohome = (transform.position - currentHome.OriginPoint.transform.position).magnitude;

        if (distancetohome <= 2)
        {
            //CurrentCarryAmount = currentSite.LoadingAmmount(this);
            //Debug.Log(CurrentCarryAmount);

            LoadResource(currentHome);

        }
        else if (distancetohome >= 2)
        {


            if (navAgent.destination != currentHome.OriginPoint.transform.position)
            {
                TakeWaytoHomeForLoad();
            }
        }
    }


    #endregion


    #region Hover Metods

    private void OnMouseEnter()
    {
        if (!Selected)
        {
            MySelector.SetActive(true);
        }

    }

    private void OnMouseExit()
    {
        if (!Selected)
        {
            if (!MouseOverBypass)
            {
                MySelector.SetActive(false);
            }


        }

    }

    #endregion


    public void ClearWorkingFlags()
    {
        isWorkingOnSite = false;
        isGatheringInMode = false;

    }

    //Метод перебирающий цели из листа обидчиков
    public void NextTarget()
    {
        if (PersonalDammazKron.Count > 0)
        {
            currentTarget = PersonalDammazKron[0].transform;
            //  Debug.Log(this.transform.name + " is now targeting to " + attackersList[0].transform.name);
        }
        else
        {
            AnswerFlag = true;
            ScanMode = true;
            //Debug.Log("Next target work");
            currentTarget = null;

            //Debug.Log(this.transform.name + " is lost target");

        }

    }

    //Вызов события об убийстве 
    private void Dying()
    {

        RTSGameManager.UnitKilled(this);

    }

    public void RemoveUnitFromLists()
    {

        //Обновления листа обидчиков у всех целей атаковавших эту через список активных юнитов

        foreach (var ActiveUnit in FindObjectOfType<UnitManager>().ActiveUnitsList)
        {
            if (ActiveUnit.PersonalDammazKron.Contains(this))
            {
                ActiveUnit.PersonalDammazKron.Remove(this);
                // Debug.Log(this.transform.name + "Removed from list of " + ActiveUnit.transform.name);
            }
        }

        //Старый способ удаления погибшей цели у атаковавших. Возможно его стоит убрать для уменьшения колва обработок.
        for (int i = 0; i < PersonalDammazKron.Count; i++)
        {

            PersonalDammazKron[i].PersonalDammazKron.Remove(this);
            //Цель зависает если кто то другой убивает цель находяющуюся в листе. UPD Пофикшено листом активностей.
            //Возможное решение общий список обидчиков для всех юнитов. Может сработать как груповая механика.

            //attackersList[i].nextTar   get ; //Нужно не обнулять а отправлять к следующей цели UPD Отправка к следующей цели сделана при проверке назначение текущей цели.

        }

        unitManager.ActiveUnitsList.Remove(this);
    }

    //Обновление флагов и списков при смерте юнита.
    public void Death()
    {
        AnswerFlag = false;
        isDead = true;

        AddFloatingText("RIP", transform.position, (Vector3.right * 2) + Vector3.down, Color.black);

        Stop();
        //Debug.Log(transform.name + " is dead");

        //Можно сделать лист мертвых юнитов что бы уничтожать потом объекты.

        RemoveUnitFromLists();

        //Пока просто выключаем объект. Возможно необходимо добавить удаление. Через запуск таймера. Или добавление респавна в зависимости от механики
        transform.gameObject.SetActive(false);

        Instantiate(unitManager.ExplosionPrefab, transform.position, Quaternion.identity, unitManager.UnitsFolder.transform);


        // Despawn();
    }

    public void Sell(int Amount)
    {
        AnswerFlag = false;
        isDead = true;

        AddFloatingText(Amount, transform.position, (Vector3.right * 2) + Vector3.down, Color.yellow);

        Stop();

        RemoveUnitFromLists();

        //можно оставлять после объекта шарик ресурса и его или кликать для сбора или рабочего подзывать 


        transform.gameObject.SetActive(false);
    }

    public void Disassemble()
    {
        AnswerFlag = false;
        isDead = true;
        navAgent.enabled = false;
        Stop();
        RemoveUnitFromLists();
        Destroy(this.gameObject);

    }

    public void AddFloatingText(float amount, Vector3 objectPosition, Vector3 textStartPoint, Color color)
    {
        FloatingTextUI.Instance.AddTextFromFloat(amount, objectPosition, textStartPoint, color);
    }

    public void AddFloatingText(string text, Vector3 objectPosition, Vector3 textStartPoint, Color color)
    {
        FloatingTextUI.Instance.AddText(text, objectPosition, textStartPoint, color);
    }

    public void ScanRaycasting()
    {
        #region ScanRaycasting

        if (currentTarget != null)
        {


        }

        //var Angle = 0.5f;  // Vector3.Angle(ProjectileOriginPoint.forward, (currentTarget.position - transform.position));

        //Debug.Log(Angle);

        if (TempObject == null)
        {
            TempObject = Instantiate(ProjectileOriginPoint.gameObject, ProjectileOriginPoint);
        }



        RaycastHit ScanHit;

        Ray ScanRay = new Ray(TempObject.transform.position, TempObject.transform.forward);
        if (Physics.Raycast(ScanRay, out ScanHit, unitStats.viewRange))
        {
            if (ScanHit.collider.gameObject.activeSelf)
            {
                if (ScanHit.collider.gameObject != transform.gameObject)
                {
                    if (!ScanHit.collider.transform.CompareTag("Ground"))
                    {

                        var ID = ScanHit.collider.gameObject.GetComponent<ID>().playerID;

                        if (ID != unitStats.playerID)
                        {

                            if (ID != -1 && ID != 0)
                            {
                                //Debug.Log(ID);
                                //Debug.Log("I found " + ScanHit.collider.gameObject.name);
                                currentTarget = ScanHit.collider.transform;
                                ScanMode = false;

                                ResetScanFlags();
                                Destroy(TempObject);

                            }

                        }
                    }


                }



                //currentTarget = ScanHit.collider.transform;
            }

        }

        Debug.DrawRay(ProjectileOriginPoint.position, ProjectileOriginPoint.forward * unitStats.viewRange);

        RotationOfScanner(unitStats.viewSpeed, unitStats.viewAngle);


        Debug.DrawRay(ProjectileOriginPoint.position, TempObject.transform.forward * unitStats.viewRange, Color.green);

        #endregion
    }

    public void RotationOfScanner(float Angle, float SectorValue)
    {

        #region Rotate



        var SectorAngle = (Vector3.Angle(ProjectileOriginPoint.forward, TempObject.transform.forward));
        var SectorBorder = SectorValue;

        //Debug.DrawLine(ProjectileOriginPoint.forward * unitStats.attackRange, TempObject.transform.forward * unitStats.attackRange);
        //Debug.Log(SectorAngle);

        //Debug.Log(TempObject.transform.rotation.y);

        var ZeroLine = TempObject.transform.localRotation.y;


        if (!minusangel)
        {
            if (SectorAngle <= SectorBorder)
            {
                TempObject.transform.RotateAround(ProjectileOriginPoint.position, ProjectileOriginPoint.up, Angle);

            }
            else
            {
                minusangel = true;
                // Debug.Log("Minusangle true");

            }
        }
        else
        {
            if (!zero)
            {


                if (ZeroLine <= 0)
                {

                    TempObject.transform.RotateAround(ProjectileOriginPoint.position, ProjectileOriginPoint.up, -Angle);

                    zero = true;
                    //Debug.Log("Zero true");


                }
                else
                {
                    // Debug.Log(SectorAngle);

                    TempObject.transform.RotateAround(ProjectileOriginPoint.position, ProjectileOriginPoint.up, -Angle);
                }

            }
            else
            {
                if (!zeroplus)
                {
                    if (SectorAngle <= SectorBorder)
                    {
                        TempObject.transform.RotateAround(ProjectileOriginPoint.position, ProjectileOriginPoint.up, -Angle);
                    }
                    else
                    {
                        zeroplus = true;
                        //Debug.Log("Zeroplus true");
                    }
                }
                else
                {
                    if (ZeroLine >= 0)
                    {

                        // Debug.Log("All false");

                        TempObject.transform.RotateAround(ProjectileOriginPoint.position, ProjectileOriginPoint.up, Angle);

                        ResetScanFlags();

                    }
                    else
                    {
                        TempObject.transform.RotateAround(ProjectileOriginPoint.position, ProjectileOriginPoint.up, Angle);
                    }

                }
            }
        }

        #endregion

    }

    public void ResetScanFlags()
    {
        minusangel = false;
        zero = false;
        zeroplus = false;
    }

    private void Work()
    {
        if(workColdDownTimer >= unitStats.workSpeed)
        {
            RTSGameManager.SiteTakeWork(this, currentSite);
            
            if(!isAnimate)
            {
                StartCoroutine(AnimationWork(ResourceModel.transform.position, currentSite.transform.position, unitStats.workSpeed));
            }

            workColdDownTimer = 0;

        }

    }

    //Метод атаки вызывающий событие из статик метода RTSGameManager который хранит в себе способы обработки событий. 
    //События можно как в нем так и на самом юните просчитывать в зависимости от дизайна
    private void Attack()
    {
        AnswerFlag = false;

        if (attackColdDownTimer >= unitStats.attackSpeed)
        {
            //Debug.Log("Attacking!");


            UpdatePosition();

            //Сделать вылетающий проджектл сферой.

            //Вычисляем направление прицеливания. Что бы юнит был повернут лицом к цели в момент выстрела.

            //var Angle = Vector3.Angle(ProjectileOriginPoint.forward, (currentTarget.position - transform.position));
            //var Angle = Vector3.Angle(transform.forward, (currentTarget.position - transform.position));


            //var NormalizedAngle = Vector3.Angle(Vector3.ProjectOnPlane(ProjectileOriginPoint.forward,new Vector3(0,0,0)), Vector3.ProjectOnPlane((currentTarget.position - transform.position), new Vector3(0, 0, 0)));

            // + " Normalized Angle " + NormalizedAngle);

            //Plane = new Plane(transform.up,transform.position);

            //Debug.DrawRay(Plane.ClosestPointOnPlane(transform.position), Plane.normal,Color.black);


            var PointOfCollision = Vector3.ProjectOnPlane(currentTarget.position, transform.up);
            var TransformOnPlane = Vector3.ProjectOnPlane(transform.position, transform.up);

            var PointOfOriginOnPlane = Vector3.ProjectOnPlane(ProjectileOriginPoint.position, transform.up);
            var PointOfOriginForwardOnPlane = Vector3.ProjectOnPlane(ProjectileOriginPoint.forward, transform.up);


            var ClearAngle = Vector3.Angle(PointOfOriginForwardOnPlane, (PointOfCollision - TransformOnPlane));

            #region debug drawings

            //Debug.Log("RAW angle " + Angle+" Clear Angle " + ClearAngle);

            //Debug.Log("TPOS " + transform.position + " CTPOS " + currentTarget.position + " PoC " + PointOfCollision);



            //Debug.DrawRay(transform.position, PointOfCollision-transform.position,Color.green);

            //Debug.DrawRay(currentTarget.position, PointOfCollision-currentTarget.position, Color.blue);

            //Debug.DrawLine(currentTarget.position, PointOfCollision, Color.red);


            //Debug.DrawRay(ProjectileOriginPoint.position, ProjectileOriginPoint.forward * unitStats.attackRange,Color.cyan);
            //Debug.DrawRay(PointOfOriginOnPlane, PointOfOriginForwardOnPlane * unitStats.attackRange, Color.cyan);
            //Debug.DrawRay(transform.position, (currentTarget.position - transform.position), Color.green);
            //Debug.DrawRay(transform.position, (currentTarget.position -  ProjectileOriginPoint.position), Color.magenta);

            //Debug.DrawLine(transform.position, currentTarget.transform.position, Color.yellow);
            //Debug.DrawLine(PointOfCollision, TransformOnPlane, Color.white);
            //Debug.DrawLine(PointOfCollision, PointOfOriginOnPlane, Color.magenta);
            #endregion


            if (ClearAngle <= 1.1f)
            {
                if (ReadyForShoot(this))
                {
                    Shoot();
                }
                else
                {
                    Debug.Log("Out of Ammo!");
                    //Убегать для нпц.
                    Stop();
                }


            }
            else
            {
                if (!isTargeting)
                {

                    StartCoroutine(Targeting(TargetingTransform));

                }

            }

        }

    }

    #region Targeting courutine

    //Прицеливание через корутину.
    IEnumerator Targeting(Transform transform)
    {
        //TargetCorutine    
        isTargeting = true;

        if (currentTarget != null)
        {
            //var RawAngle = Vector3.Angle(ProjectileOriginPoint.forward, (currentTarget.position - transform.position));

            var PointOfCollision = Vector3.ProjectOnPlane(currentTarget.position, transform.up);
            var TransformOnPlane = Vector3.ProjectOnPlane(transform.position, transform.up);
            var PointOfOriginOnPlane = Vector3.ProjectOnPlane(ProjectileOriginPoint.position, transform.up);
            //var PointOfOriginForwardOnPlane = Vector3.ProjectOnPlane(ProjectileOriginPoint.forward, transform.up);

            var ClearAngle = Vector3.Angle(transform.forward, (PointOfCollision - TransformOnPlane));



            //Debug.Log(Angle + "Before");

            while (ClearAngle > 0.08f)
            {
                float newAngle;


                bool Processing;

                if (currentTarget != null)
                {
                    //Debug.Log("Processing");
                    Processing = true;
                }
                else
                {
                    //Debug.Log("Processing aborted");
                    Processing = false;
                }

                if (Processing)
                {




                    //Debug.Log("TargetLoop for " + transform.name + " Target POS " + currentTarget.position + "  Angle  " + Angle);

                    var direction = (PointOfCollision - TransformOnPlane);
                    var distance = direction.magnitude;
                    var normalizedDirection = direction / distance;
                    var PointOfOriginForwardOnPlane = Vector3.ProjectOnPlane(ProjectileOriginPoint.forward, transform.up);

                    newAngle = Vector3.Angle(PointOfOriginForwardOnPlane, (PointOfCollision - TransformOnPlane));

                    // Debug.Log("DIR: " + direction + " DIST: " + distance + " NDIR: " + normalizedDirection + " ANGLE: " + newAngle);

                    var SignedAngle = Vector3.SignedAngle(PointOfOriginForwardOnPlane, normalizedDirection, transform.up);

                    transform.Rotate(transform.up, SignedAngle * 0.05f);

                    yield return new WaitForSeconds(.01f);

                    ClearAngle = newAngle;
                }
                else
                {

                    ClearAngle = 0;
                }


            }

            //Shoot();

            isTargeting = false;

        }


    }

    #endregion

    public void ForcedAttack()
    {
        ForceAttackFlag = true;
    }

    public void ForcedMove()
    {
        ForcedMoveButtonFlag = true;
    }

    private bool ReadyForShoot(Unit unit)
    {

        if (unit.currentAmmo > 0)
        {
            return true;
        }
        else
        {

            return false;
        }

    }

    private void ConsumeAmmo()
    {
        currentAmmo -= 1;
    }

    public void ReceiveAmmo(Transform Loader, int ammoAmount)
    {

        AddFloatingText((int)ammoAmount, transform.position, Vector3.right * 2, Color.blue);

        currentAmmo += ammoAmount;

        if (!isFlashing)
        {
            StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color, Color.blue, .5f));
        }
    }

    private void Shoot()
    {
        attackColdDownTimer = 0;
        ConsumeAmmo();

        var projectile = Instantiate(unitManager.ProjectilePrefab, ProjectileOriginPoint.transform.position, Quaternion.identity, transform);

        StartCoroutine(UpdateProjectle(projectile, currentTarget.gameObject, unitStats.projectileSpeed));


        //projectile.transform.position = Vector3.Lerp(transform.position, currentTarget.position,0.1f);


    }

    //Проблема корутин висящих на объекте в том что они изчеззают вместе с объектом. В идеале они дожны висеть на мире т.к каждый выстрел постути это объект мира и уже не принадлежит объекту как таковому
    //и должен завершатся не зависимо от жизни самого объекта. Еще одна проблема что во время движения снаряд прилетает совсем уж мимо. Нужно либо вводить промахи либо делать супер точное прицеливание с предсказанием точки.

    #region Corutine UpdateProjectle 


    IEnumerator UpdateProjectle(GameObject Projectle, GameObject Target, float Speed)
    {



        //определение позиции вылета снаряда в момент выстрела
        var ProjectleOriginPos = ProjectileOriginPoint.position;

        var ProjectleTargetPos = ProjectleOriginPos;
        var ProjecleTargetLocalPos = ProjectleTargetPos;


        #region Unit

        if (Target.CompareTag("Unit"))
        {
            if (!Target.GetComponent<Unit>().isDead)
            {
                ProjectleTargetPos = Target.transform.position;
                ProjecleTargetLocalPos = Target.transform.localPosition;
            }
            else
            {
                ProjectleTargetPos = lastKnownTargetPos;
            }

            /*
            //Количество единиц в цикле = количество промежутков в анимации
            for (int i = 0; i < 100; i++)
            {
                //Смещение от позиции начала выстреа до последней известной позиции противника
                Projectle.transform.position = Vector3.Lerp(ProjectleOriginPos, ProjecleTargetPos, 0.01f * i);
                //Projectle.transform.position = Vector3.MoveTowards(ProjectleOriginPos, ProjecleTargetPos, 0.1f* i);


                yield return new WaitForSeconds(.005f);

            }
            */


            var dist = (ProjectleTargetPos - Projectle.transform.position).magnitude;


            while (dist > 0.1f)
            {
                dist = (ProjectleTargetPos - Projectle.transform.position).magnitude;
                //Debug.Log(dist);
                Projectle.transform.position += (ProjecleTargetLocalPos - Projectle.transform.position).normalized * Speed * Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }


            //Урон после завершения полета снаряда

            if (!Target.GetComponent<Unit>().isDead)
            {
                RTSGameManager.UnitTakeDamage(this, Target.GetComponent<Unit>());
            }

        }


        #endregion

        if (Target.CompareTag("Building"))
        {
            if (!Target.GetComponent<BuildingScript>().isDestroyed)
            {
                ProjectleTargetPos = Target.transform.position;
                ProjecleTargetLocalPos = Target.transform.localPosition;
            }
            else
            {
                ProjectleTargetPos = lastKnownTargetPos;
            }

            /*
            //Количество единиц в цикле = количество промежутков в анимации
            for (int i = 0; i < 100; i++)
            {
                //Смещение от позиции начала выстреа до последней известной позиции противника
                Projectle.transform.position = Vector3.Lerp(ProjectleOriginPos, ProjectleTargetPos, 0.01f * i);

                yield return new WaitForSeconds(.01f);

            }
            */

            var dist = (ProjectleTargetPos - Projectle.transform.position).magnitude;

            while (dist > 0.1f)
            {
                dist = (ProjectleTargetPos - Projectle.transform.position).magnitude;

                //Debug.Log(dist);

                Projectle.transform.position += (ProjecleTargetLocalPos - Projectle.transform.position).normalized * Speed * Time.deltaTime;

                yield return new WaitForSeconds(Time.deltaTime);
            }


            //Урон после завершения полета снаряда

            //Можно добавить в снаряд короткий луч перед собой и делать рейкаст для оценки попадания. Если попадания не было урон не наносить

            if (!Target.GetComponent<BuildingScript>().isDestroyed)
            {
                RTSGameManager.BuildingTakeDamage(this, Target.GetComponent<BuildingScript>());
            }
        }

        Destroy(Projectle);
    }

    #endregion


    //Вызываемый метод отстановки и сброса цели.
    public void Stop()
    {
        isGatheringInMode = false;
        isWorkingOnSite = false;
        AttackComand = false;
        isPatrooling = false;
        AnswerFlag = false;
        ScanMode = false;
        NavAgenNewDestination(transform.position);


        //attackersList.Clear();
        currentTarget = null;

    }

    //Альтернатива полной остановки когда юниты будут готовы ответить если их атакуют.
    public void Hold()
    {
        isWorkingOnSite = false;
        isGatheringInMode = false;
        AttackComand = false;
        AnswerFlag = false;
        isPatrooling = false;
        NavAgenNewDestination(transform.position);
        currentTarget = null;
    }

    public void Guard()
    {
        GuardFlag = true;
        isWorkingOnSite = false;
        isGatheringInMode = false;
        AttackComand = true;
        isPatrooling = false;
        AnswerFlag = true;
        ScanMode = true;

        NavAgenNewDestination(transform.position);

        //attackersList.Clear();
        //currentTarget = null;
    }

    #region NavAgent

    public void NavAgenNewDestination(Vector3 Destination)
    {
        if (navAgent != null)
        {

            if (navAgent.enabled)
            {
                navAgent.destination = Destination;
            }

        }


    }

    public void NavAgentNewStopingDistance(float Distance)
    {
        if (navAgent != null)
        {
            if (navAgent.enabled)
            {
                navAgent.stoppingDistance = Distance;
            }
        }


    }
    #endregion

    public void Hide()
    {
        //Убрать юнит из всех листов 

    }

    public void TurnOnReadyForPatrool()
    {
        ReadyForPatrool = true;
    }

    public void StartPatroolingMove(Vector3 destination)
    {
        ReadyForPatrool = false;
        isMovingToPoint = false;
        NavAgentNewStopingDistance(unitStats.moveStopRange);

        //Debug.Log("Start patrool work");
        ScanMode = true;
        AttackComand = true;
        AnswerFlag = true;
        PersonalDammazKron.Clear();
        if (!isDead)
        {
            PatroolPos1 = transform.position;
            PatroolPos2 = destination;

            NavAgenNewDestination(PatroolPos2);

        }

        isPatrooling = true;

    }

    public void MoveToPatroolPos(Vector3 pos)
    {



        // Debug.Log("Move to patrool pos");
        ScanMode = true;
        AttackComand = true;
        AnswerFlag = true;

        if (!isDead)
        {
            NavAgenNewDestination(pos);

        }
        isMovingToPoint = true;

    }

    //Движение юнита к целе полученной из UnitManager через NavAgent
    public void MoveUnit(Vector3 destination)
    {
        NavAgentNewStopingDistance(unitStats.moveStopRange);

        //На всермя движения делаем юнит пассивным и забываем все цели
        ScanMode = false;
        AttackComand = false;
        AnswerFlag = false;
        PersonalDammazKron.Clear();

        if (!isDead)
        {
            currentTarget = null;
            //Debug.Log(destination);

            NavAgenNewDestination(destination);


        }

    }

    public void SetSelected(bool isSelected)
    {
        MySelector.SetActive(isSelected);
        Selected = isSelected;
    }

    //Получение цели из UnitManager 
    public void SetNewTarget(Transform Target)
    {
        ScanMode = false;
        isPatrooling = false;
        AnswerFlag = false;
        currentTarget = Target;
        UpdatePosition();

    }


    //Обновление информации о том где находится цель посути кэширование местоположения
    public void UpdatePosition()
    {
        if (currentTarget != null)
        {
            lastKnownTargetPos = currentTarget.transform.position;
            // Debug.Log("Pos updated: " + lastKnownTargetPos);
        }
    }

    //Метод который можно вызвать для того что бы пополнить список обидчиков. Вызываем его из UnitManager с зажатым шифтом.
    public void AddToAttackerList(Unit Unit)
    {

        if (!PersonalDammazKron.Contains(Unit))
        {
            PersonalDammazKron.Add(Unit);
        }
        else
        {
            // 
        }

        if (PersonalDammazKron.Count > 1)
        {
            if (PersonalDammazKron.Contains(Unit))
            {
                PersonalDammazKron.Remove(Unit);
                PersonalDammazKron.Insert(0, Unit);
            }
        }



        if (currentAttacker != null)
        {
            if (currentAttacker != Unit)
            {
                Debug.DrawLine(currentAttacker.transform.position, Unit.transform.position, Color.red);

            }
        }

    }

    //Составляем книгу обидчиков нужно еще добавить удаление из листа при смерти одного из обидчиков.
    public void AnswerAttack(Unit Attacker)
    {

        //Здесь пока берется первый из списка. Можно потом подумать над увеличением приоритетов.
        if (PersonalDammazKron.Count > 0)
        {
            AttackComand = true;
            SetNewTarget(PersonalDammazKron[0].transform);

        }        

    }

    //Метод получения урона при вызове из RTSGameManager.
    public void TakeDamage(Unit Attacker, float damage)
    {
        //Вызов корутины с передачей изначального цвета материала. Корутины работают хреново. Куча юнитов при атаке запускает кучу корутин. Надо фиксить. UPD С флагом isFlashing вроде стало получше
        if (!isFlashing)
        {
            StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color, Color.gray, .05f));
        }

        //Debug.Log(damage);
        currentHealth -= damage;

        AddFloatingText((int)damage, transform.position, Vector3.right * 2, Color.red);

        MyHUD.healthBarScript.SetHealth(currentHealth);
        //Debug.Log(CurrentHealth);


        AddToAttackerList(Attacker);

        currentAttacker = Attacker;

        if (AnswerFlag)
        {
            AnswerAttack(Attacker);
        }

    }

    public void Healing(Transform Healer, float HealAmount)
    {
        //Debug.Log(Healer.name + " " + HealAmount+"  "+currentHealth);

        currentHealth += HealAmount;

        AddFloatingText((int)HealAmount, transform.position, Vector3.right * 2, Color.green);

        MyHUD.healthBarScript.SetHealth(currentHealth);

        if (!isFlashing)
        {
            StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color, Color.green, .5f));
        }
    }

    public void TurnOnGatheringMode()
    {
        if (currentVein != null)
        {
            
            currentTarget = currentVein.transform;
            isGatheringInMode = true;

            if (!isLoaded)
            {


                if (currentVein != null)
                {
                    TakeWaytoVein();
                }

            }
            else
            {
                if (currentHome == null)
                {

                    var Finded = FindTheWayToCB();
                    if (Finded)
                    {
                        TakeWaytoHomeForUnload();
                    }
                    else
                    {
                        Stop();
                    }
                    
                }
                else
                {
                    movingForUnload = true;
                    TakeWaytoHomeForUnload();
                }
            }

        }



    }

    public void StartBuildingSiteWorking(BlueprintScript targetSite)
    {
        //Debug.Log("working");

        if (unitStats.canConstructBuilding)
        {
            isWorkingOnSite = true;

            currentSite = targetSite;

            currentTarget = targetSite.transform;

           // NavAgenNewDestination(targetSite.transform.position);
            
        }
    }

    public void StartGathering(VeinScript targetVein)
    {
        if (unitStats.canGatherResources)
        {
            currentVein = targetVein;

            currentTarget = targetVein.transform;

            // NavAgenNewDestination(targetVein.OriginPoint.transform.position);

            isGatheringInMode = true;
          
        }


    }    

    public bool FindTheWayToCB()
    {
        //Debug.Log("Im going home!");

        var NearestCB = NavigationScript.FindNearestCB(this.transform, FindObjectOfType<BuildingsManager>());

        if (NearestCB != null)
        {
            currentHome = NearestCB;

            return true;
                        
        }
        else
        {
            return false;
        }

    }

    public void TakeWaytoHomeForUnload()
    {
        NavAgenNewDestination(currentHome.OriginPoint.transform.position);
        movingForUnload = true;
    }

    public void TakeWaytoHomeForLoad()
    {

        NavAgenNewDestination(currentHome.OriginPoint.transform.position);
        movingForLoad = true;

    }

    public void TakeWaytoVein()
    {
        NavAgenNewDestination(currentVein.OriginPoint.transform.position);        
        movingForLoad = true;

    }

    public void TakeWaytoSite()
    {
        NavAgenNewDestination(currentSite.transform.position);
        movingForUnload = true;
    }

    #region Resourse Loading/Unloading

    public void UnloadResource(BuildingScript currentHome)
    {

        movingForUnload = false;
        RTSGameManager.UnloadUnitToCentralBuilding(this, currentHome);

    }

    public void UnloadResource(BlueprintScript currentSite)
    {

        movingForUnload = false;
        RTSGameManager.UnloadUnitToBuildngSite(this, currentSite);

    }

    public void LoadResource(VeinScript currentVein)
    {

        movingForLoad = false;        
        RTSGameManager.LoadFromVeinToUnit(this, currentVein);

    }

    public void LoadResource(BuildingScript currentHome)
    {

        movingForLoad = false;
        RTSGameManager.LoadFromCentralBuilding(this, currentHome);

    }

    public void DropResource()
    {
        //Метод для спавна объекта ресурса на карте.
    }

    #endregion

    //Анимация вспышки через корутину
    IEnumerator Flasher(Color defaultColor,Color flashColor,float flashDelta)
    {
        isFlashing = true;
        //Debug.Log(transform.name + "is Flashing!");
        var renderer = MyModel.GetComponent<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDelta);
            renderer.material.color = defaultColor;
            yield return new WaitForSeconds(flashDelta);
        }

        isFlashing = false;
    }

    IEnumerator AnimationWork(Vector3 Start, Vector3 Stop, float delay)
    {
        isAnimate = true;
        
        var State = ResourceModel.activeSelf;
        var Pos = ResourceModel.transform.localPosition;

        ResourceModel.SetActive(true);

        

        for (int i = 0; i < 100; i++)
        {
            
            ResourceModel.transform.position = Vector3.Lerp(Start, Stop, 0.01f*i);

            var color = new Color(ResourceModel.GetComponent<MeshRenderer>().material.color.r, ResourceModel.GetComponent<MeshRenderer>().material.color.g, ResourceModel.GetComponent<MeshRenderer>().material.color.b, 1 - 0.01f * i);

            ResourceModel.GetComponent<MeshRenderer>().material.color = color;

            yield return new WaitForSeconds(delay/200);
        }

        isAnimate = false;

        ResourceModel.SetActive(false);

        ResourceModel.transform.localPosition = Pos;

        var defaultColor = new Color(ResourceModel.GetComponent<MeshRenderer>().material.color.r, ResourceModel.GetComponent<MeshRenderer>().material.color.g, ResourceModel.GetComponent<MeshRenderer>().material.color.b, 1);

        ResourceModel.GetComponent<MeshRenderer>().material.color = defaultColor;

        ResourceModel.SetActive(State);
        
    }


    /*
     * Иногда вылазит такая хрень
     * 
     ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
System.ThrowHelper.ThrowArgumentOutOfRangeException (System.ExceptionArgument argument, System.ExceptionResource resource) (at <7d97106330684add86d080ecf65bfe69>:0)
System.ThrowHelper.ThrowArgumentOutOfRangeException () (at <7d97106330684add86d080ecf65bfe69>:0)
System.Collections.Generic.List`1[T].get_Item (System.Int32 index) (at <7d97106330684add86d080ecf65bfe69>:0)
Unit.AnswerAttack (Unit Attacker) (at Assets/Scripts/GameObjects/Unit/Unit.cs:674)
Unit.TakeDamage (Unit Attacker, System.Single damage) (at Assets/Scripts/GameObjects/Unit/Unit.cs:694)
RTSGameManager.UnitTakeDamage (Unit Attacker, Unit Target) (at Assets/Scripts/Game Machanics/RTSGameManager.cs:28)
Unit+<UpdateProjectle>d__44.MoveNext () (at Assets/Scripts/GameObjects/Unit/Unit.cs:485)
UnityEngine.SetupCoroutine.InvokeMoveNext (System.Collections.IEnumerator enumerator, System.IntPtr returnValueAddress) (at C:/buildslave/unity/build/Runtime/Export/Scripting/Coroutines.cs:17)
     * 
     */

}
