using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameObject MyModel;
    public bool HaveChilds;
    public GameObject MySelector;
    public BuildingHUDScript MyHUD;
    public GameObject BuildZone;
    public BoxCollider MyBox;

    public BuildingStats buildingStats;
  
    public float currentHealth;
    public bool isDestroyed = false;

    public bool isFlashing = false;


    public GameObject OriginPoint;
    public GameObject WayPoint;
    public Vector3 WayPointPos;
    public bool isWayPointSet = false;
    

    public bool Selected;

    public bool isBusy;
    public bool isReadyForPlacing;
    public GameObject ProducedBlueprintForPlacing = null;

    public float ProductionProgress;
    public float ResearchProgress;

    public bool ResourceConsumed;

    public List<Unit> PersonalDammazKron = new List<Unit>(); //DammazKron - Great Book of Grudges - Великая Книга Обид

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = buildingStats.health;

        MyHUD.ObjectName.text = buildingStats.Name;

        MyHUD.healthBarScript.SetMaxHealth(buildingStats.health);

        //WayPointPos = WayPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Producing unit timer

        //Heatlh check for destroy
        if (currentHealth <= 0)
        {
            Destructing();
        }

        if (isReadyForPlacing)
        {
            if (!isFlashing)
            {
                Flash(MyModel.GetComponent<MeshRenderer>().material.color, Color.green, 0.5f);
            }

        }

        //Attack if building can attack like tower or fort 
    }

    private void OnMouseEnter()
    {
        if(!Selected)
        {
            MySelector.SetActive(true);
        }
        

    }

    private void OnMouseExit()
    {
        if (!Selected)
        {

            MySelector.SetActive(false);

        }

            
    }

    #region Production

    //Производство юнита
    public void ProduceUnit(GameObject Unit)
    {
        if(!isBusy)
        {
            StartCoroutine(UnitProduction(Unit.GetComponent<Unit>()));
        }
        else
        {
            //Можно написать добавление в очередь.
            Debug.Log(buildingStats.Name + " is busy!");

        }
     
    }

    //Производство чертежа для постройки
    public void ProduceBlueprint(GameObject Blueprint,bool ConsumeResources)
    {
        if(!isBusy)
        {
            StartCoroutine(BlueprintProduction(Blueprint, ConsumeResources));
        }
        else
        {
            //Можно написать добавление в очередь.
            Debug.Log(buildingStats.Name + " is busy!");
        }
    }

    public void StartTechResearch(TechStats Tech,Research ButtonForFlag)
    {
        
        if(!isBusy)
        {
            StartCoroutine(Research(Tech,ButtonForFlag));
        }
        else
        {
            //Можно написать добавление в очередь.
            Debug.Log(buildingStats.Name + " is busy!");

        }



    }

    public void AddBonusesFromTech(Unit unit)
    {
        unit.navAgent.speed += FindObjectOfType<TechManager>().SpeedBonus;
    }

    public void BlueprintProduced()
    {

        // ProducedBlueprintForPlacing = Blueprint;

        isReadyForPlacing = true;

        RTSGameManager.BlueprintProduced(ProducedBlueprintForPlacing.GetComponent<BlueprintScript>(), this);

    }

    public void PlaceBuilding(GameObject Blueprint, bool ResourcesConsumed)
    {
        if (isReadyForPlacing)
        {
            var newBlueprintGameObject = Instantiate(Blueprint, Input.mousePosition, Quaternion.identity, FindObjectOfType<BuildingsManager>().BuildingsFolder);

            var newBlueprint = newBlueprintGameObject.GetComponent<BlueprintScript>();

            newBlueprint.ParentBuilding = this;
            newBlueprint.ResourceConsumed = ResourcesConsumed;

            RTSGameManager.BlueprintPlaced(newBlueprint, this);
        }
    }

    public void CollectResource(Unit UnloadingUnit, int Amount)
    {

        StartCoroutine(UnlodingunitEnterUnloadingPlace(UnloadingUnit, Amount));

    }

    public void GainResource(Unit LoadingUnit, int Amount)
    {
        StartCoroutine(LoadingunitEnterLoadingPlace(LoadingUnit, Amount));
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

        //FloatingTextUI.Instance.AddTextFromFloat(Amount, UnloadingUnit.transform.position, Vector3.up, Color.yellow); 
        //Нужно подумать где лучше вызывать текста. Возможно весь вызов текста убрать в RTSGameManager.

        RTSGameManager.GetResoucesFromUnloading(UnloadingUnit, Amount, FindObjectOfType<ResourceManager>());
        UnloadingUnit.CurrentCarryAmount = 0;

        yield return new WaitForSeconds(1.0f);

        UnloadingUnit.ResourceModel.SetActive(false);
        UnloadingUnit.gameObject.SetActive(true);



        UnloadingUnit.isLoaded = false;
        //UnloadingUnit.navAgent.destination = UnloadingUnit.currentVein.OriginPoint.transform.position;

        //Debug.Log("Unloaded!");


    }

    //Корутина погрузки юнита
    IEnumerator LoadingunitEnterLoadingPlace(Unit LoadingUnit, int Amount)
    {
        if (LoadingUnit.Selected)
        {
            UnitManager.Instance.UnselectUnit(LoadingUnit);
        }

        LoadingUnit.gameObject.SetActive(false);


        yield return new WaitForSeconds(1.0f);

        //Debug.Log("Almost done!");
        //ChangeAmount(Amount);

        //Метод потребления ресурсов.
        LoadingUnit.CurrentCarryAmount = Amount;
        RTSGameManager.ConsumeResourcesForLoading(LoadingUnit, Amount, ResourceManager.Instance);


        yield return new WaitForSeconds(1.0f);
        LoadingUnit.ResourceModel.SetActive(true);
        LoadingUnit.gameObject.SetActive(true);

        LoadingUnit.isLoaded = true;
    }

    //Корутина процесса производства юнита
    IEnumerator UnitProduction(Unit unit)
    {

        isBusy = true;
        ProductionProgress = 0;
        MyHUD.progressBarScript.SetMaxAmmount(100);
        MyHUD.progressBarScript.SetCurrentAmmount(ProductionProgress);
        MyHUD.progressBarScript.gameObject.SetActive(true);


        var time = unit.unitStats.productionTime;




        for (int i = 0; i < 100; i++)
        {

            yield return new WaitForSeconds(time / 100);

            ProductionProgress += 1;
            MyHUD.progressBarScript.SetCurrentAmmount(ProductionProgress);
            //Debug.Log(ProductionProgress);

            //Update ProgressBar in INFO panel or GUI.
        }

        //UnitProduced
        var RNDPOINT = OriginPoint.transform.position + new Vector3(Random.Range(-.05f, .05f), 0, Random.Range(-.05f, .05f));

        var newUnitGameObject = Instantiate(unit.gameObject, RNDPOINT, Quaternion.identity, FindObjectOfType<UnitManager>().UnitsFolder);

        var newUnit = newUnitGameObject.GetComponent<Unit>();


        AddBonusesFromTech(newUnit);

        RTSGameManager.UnitProduced(newUnit, FindObjectOfType<UnitManager>());

        if (isWayPointSet)
        {
            newUnit.MoveUnit(WayPointPos);
        }
        else
        {
            newUnit.MoveUnit(RNDPOINT + new Vector3(Random.Range(-.05f, .05f), 0, Random.Range(-.05f, .05f)));
        }
        MyHUD.progressBarScript.gameObject.SetActive(false);
        isBusy = false;
    }

    //Корутина процесса производства блупринта
    IEnumerator BlueprintProduction(GameObject Blueprint, bool ConsumeResource)
    {

        isBusy = true;
        ProductionProgress = 0;
        MyHUD.progressBarScript.SetMaxAmmount(100);
        MyHUD.progressBarScript.SetCurrentAmmount(ProductionProgress);
        MyHUD.progressBarScript.gameObject.SetActive(true);

        var BlueprintScript = Blueprint.GetComponent<BlueprintScript>();

        var time = BlueprintScript.buildingPrefab.buildingStats.buildingSpeed;

        if (ConsumeResource)
        {
            var BuildingStats = Blueprint.GetComponent<BlueprintScript>().buildingPrefab.buildingStats;

            RTSGameManager.BlueprintProduction(BuildingStats.playerID, BuildingStats.buildingCost, ResourceManager.Instance);

            ResourceConsumed = true;
        }


        for (int i = 0; i < 100; i++)
        {
            //Debug.Log("Tick");
            yield return new WaitForSeconds(time / 100);

            ProductionProgress += 1;
            MyHUD.progressBarScript.SetCurrentAmmount(ProductionProgress);
            //Debug.Log(ProductionProgress);

            //Update ProgressBar in INFO panel or GUI.
        }



        //AddBonusesFromTech(newUnit);



        MyHUD.progressBarScript.gameObject.SetActive(false);

        ProducedBlueprintForPlacing = Blueprint;

        BlueprintProduced();


        //isBusy = false;
    }

    //Корутина процесса исследования
    IEnumerator Research(TechStats tech, Research ButtonForFlag)
    {

        isBusy = true;
        MyHUD.progressBarScript.SetMaxAmmount(100);
        MyHUD.progressBarScript.SetCurrentAmmount(ResearchProgress);
        MyHUD.progressBarScript.gameObject.SetActive(true);


        var time = tech.techTimeForResearch;


        ResearchProgress = 0;

        for (int i = 0; i < 100; i++)
        {
            ResearchProgress += 1;
            MyHUD.progressBarScript.SetCurrentAmmount(ResearchProgress);
            //Debug.Log(ResearchProgress);
            yield return new WaitForSeconds(time / 100);

            //Update ProgressBar in INFO panel or GUI.
        }
        //Tech researched
        FindObjectOfType<TechManager>().ResearchTech(tech);

        ButtonForFlag.Researching = false;
        MyHUD.progressBarScript.gameObject.SetActive(false);
        isBusy = false;
    }

    #endregion

    #region World Interaction
    //Определение координат для точки выхода
    public void SetWayPoint(Vector3 pos)
    {
        WayPoint.transform.position = pos;
        WayPointPos = pos;
        isWayPointSet = true;
    }

    //Метод получения урона при вызове из RTSGameManager.
    public void TakeDamage(Unit Attacker, float damage)
    {
        //В корутину отправляется цвет модели
        if (!isFlashing)
        {
            Flash(MyModel.GetComponent<MeshRenderer>().material.color,Color.gray);            
        }

        //Debug.Log(damage);
        currentHealth -= damage;
        FloatingTextUI.Instance.AddTextFromFloat((int)damage, transform.position, Vector3.up, Color.red);
        MyHUD.healthBarScript.SetHealth(currentHealth);
        //Debug.Log(CurrentHealth);
             

    }

    public void Destructing()
    {
        RTSGameManager.BuildingDestroyed(this);
    }

    public void Destroyed()
    {
        isDestroyed = true;


        transform.gameObject.SetActive(false);

    }

    public void Canceled()
    {
        isDestroyed = true;

        if(BuildingsManager.Instance.AllBuildingsList.Contains(this))
        {
            BuildingsManager.Instance.AllBuildingsList.Remove(this);
        }

        Destroy(this.gameObject);
    }

    public void Select()
    {
        
        MySelector.SetActive(true);
        WayPoint.SetActive(true);
        Selected = true;

    }

    #endregion

    #region Animation

    public void Flash(Color defaultColor, Color FlashColor)
    {
        StartCoroutine(Flasher(defaultColor, FlashColor));
    }

    public void Flash(Color defaultColor, Color FlashColor,float flashTime)
    {
        StartCoroutine(Flasher(defaultColor, FlashColor,flashTime));
    }

    //Анимация вспышки через корутину
    IEnumerator Flasher(Color defaultColor,Color flashColor)
    {
        isFlashing = true;
        //Debug.Log(transform.name + "is Flashing!");
        var renderer = MyModel.GetComponent<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = flashColor;
            UpdateChildColor();
            yield return new WaitForSeconds(.05f);
            renderer.material.color = defaultColor;
            UpdateChildColor();
            yield return new WaitForSeconds(.05f);
        }

        isFlashing = false;
    }

    IEnumerator Flasher(Color defaultColor, Color flashColor,float flashTime)
    {
        isFlashing = true;
        //Debug.Log(transform.name + "is Flashing!");
        var renderer = MyModel.GetComponent<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = flashColor;
            UpdateChildColor();
            yield return new WaitForSeconds(flashTime);
            renderer.material.color = defaultColor;
            UpdateChildColor();
            yield return new WaitForSeconds(flashTime);
        }

        isFlashing = false;
    }

    public void UpdateChildColor()
    {
        if (HaveChilds)
        {
            MyModel.GetComponent<ParentObject>().Child.SetColor();
        }
    }

    #endregion

 

   

}
