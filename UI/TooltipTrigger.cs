using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour
{

    public string MyTooltip;
    


    public void HoverOver()
    {        
        Tooltip.ShowTooltipStatic(MyTooltip);
               
    }

    public void HoverOut()
    {
        Tooltip.HideTooltipStatic();
    }

}
