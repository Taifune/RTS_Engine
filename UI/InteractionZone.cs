using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class InteractionZone : MonoBehaviour, IPointerEnterHandler
{

    //private UnitManager unitManager;
    //private BuildingsManager buildingsManager;
    private InputDetectManager inputDetectManager;


    private void Start()
    {
        inputDetectManager = FindObjectOfType<InputDetectManager>();

        
        //Флаг отправлялся в оба клааса до объеденения и создания инпут менеджера
        //unitManager = FindObjectOfType<UnitManager>();
        //buildingsManager = FindObjectOfType<BuildingsManager>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inputDetectManager.isInIteractionZone = true;
        //unitManager.isInIteractionZone = true ;
        //buildingsManager.isInIteractionZone = true ;
    }




}

