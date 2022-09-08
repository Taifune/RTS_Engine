using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cancel : MonoBehaviour
{
    
    public void CancelButton()
    {
        //Debug.Log("Work");
        SelectionManager.Instance.SelectedBlueprint.CancelButton();
    }


}
