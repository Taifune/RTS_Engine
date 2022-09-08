using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeinScript : MonoBehaviour
{
    public GameObject MyModel;
    public GameObject MySelector;
    public WorldObjectHUDScript MyHUD;
    

    public WorldObjectStats worldObjectStats;

    public float currentHealth;
    public float currentResourceAmmount;
    public float regenerationTimer;
    public bool isDestroyed = false;

    public bool isFlashing = false;


    public GameObject OriginPoint;
    public GameObject WayPoint;
    public Vector3 WayPointPos;

    

    public bool Selected;

    public List<Unit> PersonalDammazKron = new List<Unit>(); //DammazKron - Great Book of Grudges - Великая Книга Обид

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(worldObjectStats.isDestructble)
        {
            currentHealth = worldObjectStats.health;

            MyHUD.healthBarScript.SetMaxHealth(worldObjectStats.health);

        }
        else
        {
            MyHUD.healthBarScript.gameObject.SetActive(false);
        }


        currentResourceAmmount = worldObjectStats.ResourceAmmount;

        MyHUD.resourceBarScript.SetMaxAmmount(worldObjectStats.ResourceAmmount);

        

        WayPointPos = WayPoint.transform.position;
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

    // Update is called once per frame
    void Update()
    {
        //Regeneration Resource timer
        if (regenerationTimer <= worldObjectStats.ResourceRegeneration)
        {
            regenerationTimer += Time.deltaTime;
        }
        else
        {
            RegenerateResource();
        }

        //Heatlh check for destroy
        if (worldObjectStats.isDestructble)
        {
            if (currentHealth <= 0)
            {
                Destructing();
            }
        }
      

    

        //Attack if building can attack like tower or fort 
    }

    public void  RegenerateResource()
    {


        if (currentResourceAmmount<=worldObjectStats.ResourceAmmount)
        {
            var Rnd = Random.Range(1, 15);



            IncreaseAmount(Rnd);
        }

        regenerationTimer = 0;

    }

    public void ProduceUnit(GameObject Unit)
    {
        var newUnit = Instantiate(Unit, OriginPoint.transform.position, Quaternion.identity);

        //newUnit.gameObject.SetActive(false);

        //Debug.Log(WayPointPos);
        //WayPointPos.y = 0;
        //Debug.Log(WayPointPos);
        newUnit.GetComponent<Unit>().MoveUnit(WayPointPos);

    }

    public void SetWayPoint(Vector3 pos)
    {
        WayPoint.transform.position = pos;
        WayPointPos = pos;
    }

    //Метод получения урона при вызове из RTSGameManager.
    public void TakeDamage(Unit Attacker, float damage)
    {
        //В корутину отправляется цвет модели
        if (!isFlashing)
        {
            StartCoroutine(Flasher(MyModel.GetComponent<MeshRenderer>().material.color));
        }

        //Debug.Log(damage);

        currentHealth -= damage;
        FloatingTextUI.Instance.AddTextFromFloat((int)damage, transform.position, Vector3.up, Color.red);
        MyHUD.healthBarScript.SetHealth(currentHealth);
        //Debug.Log(CurrentHealth);             

    }

    public int LoadingAmmount(Unit LoadingUnit)
    {
        if (currentResourceAmmount>0)
        {
            if(currentResourceAmmount>=LoadingUnit.unitStats.carryAmount)
            {
                return LoadingUnit.unitStats.carryAmount;
            }
            else
            {
                return (int) currentResourceAmmount;
            }
                        
        }
        else
        {
            return 0;
        }

    }

    public void TakeGathering(Unit Gatherer, float amount)
    {
        
        //Возможно стартовать здесь корутину с изчезвением юнита на время загрузки.
        
        
        //менять текущее значение ресурса
        

        StartCoroutine(GathererEnterVein(Gatherer,amount));


        //обновлять HUD

    }

    public void ChangeAmount(float amount)
    {
        currentResourceAmmount -= amount;
        FloatingTextUI.Instance.AddTextFromFloat((int)amount, transform.position, Vector3.up, Color.gray);

        MyHUD.resourceBarScript.SetCurrentAmmount(currentResourceAmmount);
    }

    public void IncreaseAmount(float amount)
    {
        currentResourceAmmount += amount;
        FloatingTextUI.Instance.AddTextFromFloat((int)amount, transform.position, Vector3.up, Color.yellow);
        MyHUD.resourceBarScript.SetCurrentAmmount(currentResourceAmmount);
    }

    public void Destructing()
    {
       // RTSGameManager.BuildingDestroyed(this);
    }

    public void Destroyed()
    {
        isDestroyed = true;


        transform.gameObject.SetActive(false);

    }

    public void Select()
    {
        
        MySelector.SetActive(true);
        //WayPoint.SetActive(true);
        Selected = true;
        //Debug.Log("SelectedVein");
        
    }

    public void Deselect()
    {
        MySelector.SetActive(false);
        Selected = false;
    }
   
    //Анимация вспышки через корутину
    IEnumerator Flasher(Color defaultColor)
    {
        isFlashing = true;
        //Debug.Log(transform.name + "is Flashing!");
        var renderer = MyModel.GetComponent<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = Color.gray;
            yield return new WaitForSeconds(.05f);
            renderer.material.color = defaultColor;
            yield return new WaitForSeconds(.05f);
        }

        isFlashing = false;
    }
    //Корутина погрузки юнита
    IEnumerator GathererEnterVein(Unit Gatherer,float Amount)
    {
        if(Gatherer.Selected)
        {
            FindObjectOfType<UnitManager>().UnselectUnit(Gatherer);
        }
        
        Gatherer.gameObject.SetActive(false);

        
        yield return new WaitForSeconds(1.0f);

        //Debug.Log("Almost done!");
        ChangeAmount(Amount);
        Gatherer.CurrentCarryAmount = (int) Amount;

        yield return new WaitForSeconds(1.0f);
        Gatherer.ResourceModel.SetActive(true);
        Gatherer.gameObject.SetActive(true);
        
        Gatherer.isLoaded = true;
    }
}
