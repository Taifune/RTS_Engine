using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform turret;
    public Transform body;
   // public Transform Target;
    public Vector3 direction;

    public Unit MyUnit;
    public bool isTurretTracking;
    public bool isTurretIdling;
    public bool isIdleIniting;
    public Transform Target;

   
    // Update is called once per frame
    void Update()
    {
        if(MyUnit.currentTarget!=null)
        {
            Target = MyUnit.currentTarget;
            TurretTracking();
        }
        else
        {
            if(!isTurretIdling)
            {
               TurretIdle();
            }
            
        }
    }

    public void TurretTracking()
    {
        StopAllCoroutines();
        isTurretIdling = false;
        isTurretTracking = true;

        //Вычисление направление производится путем вычитания от вектора позиации цели вектора своей позиции 
        direction = Target.position - body.position;
        //взяв магнитуду от вектора направления получаем значение дистанции
        var distance = direction.magnitude;
        //разделив направление на дистанцию получаем нормализованное направление.
        var normalizeDirection = direction / distance;

        /*
        Debug.DrawRay(turret.position, turret.forward*4, Color.red);
        Debug.DrawRay(body.position, body.forward * 4, Color.red);
        Debug.DrawLine(body.position, Target.position);
        Debug.Log("Angle from body forward and target: "+Vector3.AngleBetween(body.forward, normalizeDirection));
        Debug.Log("Angle between body forward and target: " + Vector3.AngleBetween(body.forward, normalizeDirection));
        Debug.Log("Angle between turret and target: " + Vector3.AngleBetween(turret.forward, normalizeDirection));
        Debug.Log("SignedAngle between turret and target: " + Vector3.SignedAngle(turret.forward, normalizeDirection, turret.up));
        */


        //Вращаем башню вокруг оси направленной вверх через метод ПодписанныйУгол который возвращает положительные и отрицательные углы относительно указанной оси между векторами вычисления.

        turret.Rotate(turret.up, Vector3.SignedAngle(turret.forward, normalizeDirection, turret.up)*0.05f);  // .Angle(turret.forward, direction)); Vector3.Angle(turret.forward, normalizeDirection)*0.1f

        //turret.Rotate(direction);
        // Debug.Log()
    }

    public void TurretIdle()
    {
        Target = null;
       
        isTurretTracking = false;
       

        


        turret.Rotate(turret.up, Vector3.SignedAngle(turret.forward, body.forward, turret.up) * 0.05f);

        if(Vector3.Angle(turret.forward,body.forward)<=0.1f)
        {
            isTurretIdling = true;
            //Debug.Log("Turret idling");
        }
        //В состоянии простоя выравнивем башню относительно корпуса.
       
        

    }

    //Анимация вспышки через корутину
    IEnumerator Idler()
    {
        isIdleIniting = true;
        //Debug.Log(transform.name + "is Flashing!");
        
       

            for (int i = 0; i < 100; i++)
            {
                turret.Rotate(turret.up, Vector3.SignedAngle(turret.forward, body.forward, turret.up) * 0.1f);
                yield return new WaitForSeconds(.05f);
            }


            
            yield return new WaitForSeconds(.05f);
        

        isIdleIniting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }

}
