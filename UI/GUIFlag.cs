using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

     
 

public class GUIFlag : MonoBehaviour, IPointerEnterHandler
{

   // private UnitManager unitManager;
   // private BuildingsManager buildingsManager;

    private InputDetectManager inputDetectManager;

    private void Start()
    {
        //unitManager = FindObjectOfType<UnitManager>();
        //buildingsManager = FindObjectOfType<BuildingsManager>();

        inputDetectManager = FindObjectOfType<InputDetectManager>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        inputDetectManager.isInIteractionZone = false;

        //unitManager.isInIteractionZone = false;
        //buildingsManager.isInIteractionZone = false;


    }




}