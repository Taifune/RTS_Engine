using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float LifeTimer = 5f;
    public float ExplosionForce = 150f;
    public float ExplosiinRadius = 10f;

    public List<Rigidbody> Particles = new List<Rigidbody>();

    public UnitManager unitManager;
    public GameObject Cross;
    private Vector3 Pos;
    private Vector3 InstPos;
    

    // Start is called before the first frame update
    void Start()
    {

        unitManager = FindObjectOfType<UnitManager>();

        //.AddExplosionForce(ExplosionForce, transform.position, 10f);
       foreach(var Particle in Particles)
        {
            var Rnd = Random.Range(-.9f, .9f);

            Particle.AddExplosionForce(ExplosionForce*Rnd, transform.position, ExplosiinRadius*Rnd);
            StartCoroutine(Flasher(Particle.transform));
           
        }

        Pos = new Vector3( transform.position.x, 0.5f , transform.position.z);

        InstPos = new Vector3(Pos.x,-0.5f,Pos.z);

        var Qatr = Quaternion.identity;        

        Cross = RTSGameManager.ObjectRandomCheckSpawn(unitManager.GravePrefab, 50, InstPos, Qatr,unitManager.UnitsFolder.transform);

    }

    
    void Update()
    {
        
        LifeTimer -= Time.deltaTime;
        if(Cross!=null)
        {
            Cross.transform.position = Vector3.Lerp(Cross.transform.position, Pos, 1f * Time.deltaTime);
        }
        else
        {
            //Destroy(this.gameObject);
        }

        if(LifeTimer<=0)
        {
            
            Destroy(this.gameObject);
            
        }
    }

    IEnumerator Flasher(Transform Particle)
    {
        
        //Debug.Log(transform.name + "is Flashing!");
        var renderer = Particle.GetComponent<MeshRenderer>();

        var defaultColor = renderer.material.color;

        for (int i = 0; i < 4; i++)
        {
            renderer.material.color = Color.black;
            yield return new WaitForSeconds(.05f);
            renderer.material.color = defaultColor;
            yield return new WaitForSeconds(.05f);           
        }       
       
    }

}
