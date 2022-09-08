using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScript : MonoBehaviour
{
    public GameObject toggleObject;

    public void Toggle()
    {
       
        GUIManager.ToggleGUIElement(toggleObject);

    }

    
}
