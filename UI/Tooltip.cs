using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private static Tooltip Instance;
           
    
    public Vector2 Offset;
    

    private Text tooltipText;
    private Text tooltipTextShadow;
    private RectTransform backgroundRectTransform;
          
     


    private void Awake()
    {
        Instance = this;

       

        backgroundRectTransform = transform.Find("TooltipBackground").GetComponent<RectTransform>();
        tooltipText = transform.Find("TooltipText").GetComponent<Text>();
        tooltipTextShadow = transform.Find("TooltipTextShadow").GetComponent<Text>();
        //ShowToolTip("Random tooltip text");

        gameObject.SetActive(false);
    }


    private void Update()
    {
        //ToolTipFollowMouse();
    }


    public void ToolTipFollowMouse()
    {
        

        Vector2 localPoint = Input.mousePosition;

        transform.position = localPoint+Offset;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);

                                    
    }

    


    public void ShowToolTip(string tooltipString)
    {
        ToolTipFollowMouse();
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        tooltipTextShadow.text = tooltipString;
        float TextPaddingSize = 5f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + TextPaddingSize*2,tooltipText.preferredHeight + TextPaddingSize * 2);
        backgroundRectTransform.sizeDelta = backgroundSize;
     

    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }


    public static void ShowTooltipStatic(string tooltipString)
    {
        Instance.ShowToolTip(tooltipString);
    }

    public static void HideTooltipStatic()
    {
        Instance.HideToolTip();
    }

}
