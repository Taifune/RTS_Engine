using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    //GUI Instructions 
    //1. Add a public GameObject
    //2. Add this GameObject in unity editor in GUIManager inspector
    //3. Add this GameObject to list of metod where it belongs.
    //4. Add activation of this GameObject to the manager call metod or GameObject list.


    public static GUIManager Instance;

    public GameObject ActiveGUIElement;


    [Space(10)]
    [Header("topPanel")]
    [Space(10)]

    public GameObject topPanel;
    [Space(10)]

    public GameObject Resource;
    public GameObject ResourceName;
    public Image resourceIcon;
    public GameObject resourceValue;

    [Space(10)]
    [Header("buttonsPanel")]
    [Space(10)]

    public GameObject buttonsPanel;

    [Space(10)]

    public ToggleScript toggleScript;


    [Space(10)]
    [Header("bottomPanel")]
    [Space(10)]

    public GameObject bottomPanel;
    public List<GameObject> bottomPanelList;


    [Space(10)]
    [Header("InfoPanel")]
    [Space(10)]

    public UnitInfoPanel infoPanel;


    [Space(10)]
    [Header("cancelWindow")]
    [Space(10)]

    public GameObject cancelWindow;

    [Space(10)]

    public GameObject cancelButton;

    [Space(10)]
    [Header("unitControls")]
    [Space(10)]

    public GameObject unitControls;

    [Space(10)]

    public GameObject moveButton;
    public GameObject stopButton;
    public GameObject holdButton;
    public GameObject attackButton;

    public GameObject patroolButton;
    public GameObject repairButton;
    public GameObject formationButton;
    public GameObject gatherButton;

    public GameObject buildMenuButton;
    public GameObject destroyButton;
    public GameObject tempButton;
    public GameObject cancelSelectionButton;

    [Space(10)]
    public List<GameObject> unitControlsList;
    [Space(10)]
    [Header("buildingControls")]
    [Space(10)]

    public GameObject buildingControls;

    [Space(10)]

    public GameObject placeBuilding;
    public GameObject buildingMenuButton;
    public GameObject produceMenuButton;
    public GameObject researchMenuButton;
    public GameObject repairMenuButton;

    [Space(10)]
    public List<GameObject> buildingControlsList;
    [Space(10)]
    [Header("buildMenu")]
    [Space(10)]

    public GameObject buildMenu;

    [Space(10)]

    public GameObject backFromBuildMenuButtonToUnitControls;
    public GameObject backFromBuildMenuButtonToBuildingControls;
    public GameObject buildingOneButton;
    public GameObject buildingTwoButton;
    public GameObject buildingThreeButton;
    public GameObject buildingFourButton;
    public GameObject buildingFiveButton;


    [Space(10)]
    public List<GameObject> buildMenuList;
    [Space(10)]
    [Header("produceMenu")]
    [Space(10)]

    public GameObject produceMenu;

    [Space(10)]

    public GameObject backFromProduceMenuButton;
    public GameObject produceUnitButton;
    public GameObject produceWorkerButton;
    public GameObject produceUnitTank;

    [Space(10)]
    public List<GameObject> produceMenuList;
    [Space(10)]
    [Header("researchMenu")]
    [Space(10)]

    public GameObject researchMenu;

    [Space(10)]

    public GameObject backFromResearchMenuButton;
    public GameObject ResearchRCButton;
    public GameObject ResearchSpeedButton;
    public GameObject ResearchDamageButton;
    public GameObject ResearchArmorButton;
    public GameObject ResearchUnitTankButton;
    public GameObject ResearchTurretButton;
    public GameObject ResearchTechsButton;
    public GameObject ResearchRSButton;
    public GameObject ResearchSellTechButton;

    [Space(10)]
    public List<GameObject> researchMenuList;
    [Space(10)]
    [Header("repairMenu")]
    [Space(10)]

    public GameObject repairhMenu;

    [Space(10)]

    public GameObject backFromRepairMenuButton;
    public GameObject StartRepairButton;
    public GameObject StartReloadButton;
    public GameObject DisassembleUnitButton;
    public GameObject ReleaseUnitButton;




    [Space(10)]
    public List<GameObject> repairMenuList;
    [Space(10)]
    [Header("buttonsPositionsList")]
    [Space(10)]
    [HideInInspector]    public List<Vector2> buttonsPositionsList;
    [Space(10)]
    [HideInInspector]    public Vector2 L0P00;
    [HideInInspector]    public Vector2 L1P01;
    [HideInInspector]    public Vector2 L1P02;
    [HideInInspector]    public Vector2 L1P03;
    [HideInInspector]    public Vector2 L1P04;
    [HideInInspector]    public Vector2 L2P05;
    [HideInInspector]    public Vector2 L2P06;
    [HideInInspector]    public Vector2 L2P07;
    [HideInInspector]    public Vector2 L2P08;
    [HideInInspector]    public Vector2 L3P09;
    [HideInInspector]    public Vector2 L3P10;
    [HideInInspector]    public Vector2 L3P11;
    [HideInInspector]    public Vector2 L3P12;


    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        

        InitializeGUILists();
    }

    public void InitializeGUILists()
    {

        InitButtonsPostitionsList();
        InitBottomPanelList();
        InitUnitControlsList();
        InitBuildingControlsList();
        InitBuildMenuList();
        InitProduceMenuList();
        InitResearchMenuList();
        InitRepairMenuList();


    }

    public void InitButtonsPostitionsList()
    {
        // Line 1 [01][02][03][04]

        // Line 2 [05][06][07][08]

        // Line 3 [09][10][11][12]

        var X1 = 30;
        var X2 = 85;
        var X3 = 140;
        var X4 = 195;
        var Y1 = -30;
        var Y2 = -85;
        var Y3 = -140;

        L0P00 = new Vector2(0, 0);
        L1P01 = new Vector2(X1, Y1);
        L1P02 = new Vector2(X2, Y1);
        L1P03 = new Vector2(X3, Y1);
        L1P04 = new Vector2(X4, Y1);
        L2P05 = new Vector2(X1, Y2);
        L2P06 = new Vector2(X2, Y2);
        L2P07 = new Vector2(X3, Y2);
        L2P08 = new Vector2(X4, Y2);
        L3P09 = new Vector2(X1, Y3);
        L3P10 = new Vector2(X2, Y3);
        L3P11 = new Vector2(X3, Y3);
        L3P12 = new Vector2(X4, Y3);

        buttonsPositionsList.Add(L0P00);
        buttonsPositionsList.Add(L1P01);
        buttonsPositionsList.Add(L1P02);
        buttonsPositionsList.Add(L1P03);
        buttonsPositionsList.Add(L1P04);
        buttonsPositionsList.Add(L2P05);
        buttonsPositionsList.Add(L2P06);
        buttonsPositionsList.Add(L2P07);
        buttonsPositionsList.Add(L2P08);
        buttonsPositionsList.Add(L3P09);
        buttonsPositionsList.Add(L3P10);
        buttonsPositionsList.Add(L3P11);
        buttonsPositionsList.Add(L3P12);

    }

    public void InitBottomPanelList()
    {

        bottomPanelList.Add(unitControls);
        bottomPanelList.Add(buildingControls);
        bottomPanelList.Add(buildMenu);
        bottomPanelList.Add(produceMenu);
        bottomPanelList.Add(researchMenu);

    }


    public void InitUnitControlsList()
    {

        unitControlsList.Add(moveButton);       // place [1][-][-][-]
        unitControlsList.Add(stopButton);       // place [-][2][-][-]
        unitControlsList.Add(holdButton);       // place [-][-][3][-]
        unitControlsList.Add(attackButton);     // place [-][-][-][4]

        unitControlsList.Add(patroolButton);    // place [5][-][-][-]
        unitControlsList.Add(repairButton);     // place [-][6][-][-]
        unitControlsList.Add(formationButton);  // place [-][-][7][-]
        unitControlsList.Add(gatherButton);     // place [-][-][-][8] 

        unitControlsList.Add(buildMenuButton);  // place [9][-][-][-]
        unitControlsList.Add(destroyButton);    // place [-][10][-][-]
        unitControlsList.Add(tempButton);       // place [-][-][11][-]
        unitControlsList.Add(cancelSelectionButton);     // place [-][-][-][12]


    }

    public void InitBuildingControlsList()
    {
        //  buildingControlsList.Add();
        //  buildingControlsList.Add();
        //  buildingControlsList.Add();
        buildingControlsList.Add(placeBuilding);           // place [-][-][-][4]

        //  buildingControlsList.Add();
        //  buildingControlsList.Add();
        //  buildingControlsList.Add();
        //  buildingControlsList.Add();

        buildingControlsList.Add(buildingMenuButton);      // place [9][-][-][-]
        buildingControlsList.Add(produceMenuButton);       // place [-][10][-][-]
        buildingControlsList.Add(researchMenuButton);      // place [-][-][11][-]
        buildingControlsList.Add(repairMenuButton);        // place [-][-][-][12]

    }

    public void InitBuildMenuList()
    {
        buildMenuList.Add(backFromBuildMenuButtonToUnitControls);        // place [1][-][-][-]
        buildMenuList.Add(backFromBuildMenuButtonToBuildingControls);    // place [1][-][-][-]
        // buildMenuList.Add();
        // buildMenuList.Add();
        // buildMenuList.Add();

        // buildMenuList.Add();
        // buildMenuList.Add();
        // buildMenuList.Add();
        buildMenuList.Add(buildingFiveButton);       // place [-][-][-][8]

        buildMenuList.Add(buildingOneButton);        // place [9][-][-][-]
        buildMenuList.Add(buildingTwoButton);        // place [-][10][-][-]
        buildMenuList.Add(buildingThreeButton);      // place [-][-][11][-]
        buildMenuList.Add(buildingFourButton);       // place [-][-][-][12]
    }

    public void InitProduceMenuList()
    {
        produceMenuList.Add(backFromProduceMenuButton);    // place [1][-][-][-]
        // produceMenuList.Add();
        // produceMenuList.Add();
        // produceMenuList.Add();

        produceMenuList.Add(produceUnitTank);              // place [5][-][-][-]
        // produceMenuList.Add();
        // produceMenuList.Add();
        // produceMenuList.Add();

        produceMenuList.Add(produceUnitButton);            // place [9][-][-][-]
        produceMenuList.Add(produceWorkerButton);          // place [-][10][-][-]
        // produceMenuList.Add();
        // produceMenuList.Add();
    }

    public void InitResearchMenuList()
    {
        researchMenuList.Add(backFromResearchMenuButton);  // place [1][-][-][-]
        researchMenuList.Add(ResearchRCButton);            // place [-][2][-][-]
        researchMenuList.Add(ResearchTechsButton);         // place [-][2][-][-]
        //researchMenuList.Add();
        // researchMenuList.Add();

        researchMenuList.Add(ResearchSpeedButton);         // place [5][-][-][-]
        researchMenuList.Add(ResearchDamageButton);        // place [-][6][-][-]
        researchMenuList.Add(ResearchArmorButton);         // place [-][-][7][-]
        researchMenuList.Add(ResearchUnitTankButton);      // place [-][-][-][8] 

        researchMenuList.Add(ResearchTurretButton);        // place [9][-][-][-]
        researchMenuList.Add(ResearchRSButton);            // place [-][10][-][-]
        researchMenuList.Add(ResearchSellTechButton);            // place [-][-][11][-]
                                                                 // researchMenuList.Add();


    }

    public void InitRepairMenuList()
    {
        repairMenuList.Add(backFromRepairMenuButton);      // place [1][-][-][-]
        // repairMenuList.Add();
        // repairMenuList.Add();
        repairMenuList.Add(ReleaseUnitButton);             // place [-][-][-][4]


        repairMenuList.Add(StartRepairButton);             // place [5][-][-][-]
        repairMenuList.Add(StartReloadButton);             // place [-][6][-][-]
        repairMenuList.Add(DisassembleUnitButton);         // place [-][-][7][-]
        // repairMenuList.Add();

        // repairMenuList.Add();
        // repairMenuList.Add();
        // repairMenuList.Add();
        // repairMenuList.Add();
    }

    public void SetButtonPosition(GameObject Button, Vector2 Position)
    {
        //Debug.Log(Button.GetComponent<RectTransform>().anchoredPosition);
        //Debug.Log(Position);

        Button.GetComponent<RectTransform>().anchoredPosition = Position;

    }



    public void UpdateResource(int value)
    {
        string newtext = value.ToString();

        resourceValue.GetComponent<TMPro.TextMeshProUGUI>().text = newtext; 
    }


    public static void ToggleBuildButton()
    {
        

        
    }

    public static void DeactivateList(List<GameObject> List)
    {
        for (int i=0; i<List.Count;i++)
        {
            Tooltip.HideTooltipStatic();
            DeactivateGUIElement(List[i]);

        }

    }



    public static void ActivateList(List<GameObject> List)
    {

        for(int i=0;i<List.Count;i++)
        {
            Tooltip.HideTooltipStatic();
            ActivateGUIElement(List[i]);

        }


    }

    public static void EnableInteractableForList(List<GameObject> List)
    {
        for(int i=0;i<List.Count;i++)
        {
            Tooltip.HideTooltipStatic();
            EnableInteractableForButton(List[i]);
        }
    }

    public static void DisableInteractableForList(List<GameObject> List)
    {
        for (int i = 0; i < List.Count; i++)
        {
            Tooltip.HideTooltipStatic();
            DisableInteractableForButton(List[i]);
        }
    }


    public static void EnableInteractableForButton(GameObject ObjectWithButton)
    {
        ObjectWithButton.GetComponent<Button>().interactable = true;
    }

    public static void DisableInteractableForButton(GameObject ObjectWithButton)
    {
        ObjectWithButton.GetComponent<Button>().interactable = false;
    }



    public static void ActivateGUIElement(GameObject GUIElement)
    {
        if (GUIElement != null)
        {

            GUIElement.SetActive(true);
            
        }
            
    }

    public static void ActivateGUIElement(GameObject GUIElement,bool MakeActive)
    {
        

        if (GUIElement != null)
        {
            if (Instance.ActiveGUIElement != null)
            {
                Instance.ActiveGUIElement.SetActive(false);
            }

            GUIElement.SetActive(true);

        }

        Instance.ActiveGUIElement = GUIElement;
    }

    public static void DeactivateGUIElement(GameObject GUIElement)
    {
        if(GUIElement!=null)
        {
            Tooltip.HideTooltipStatic();
            GUIElement.SetActive(false);
        }
        

    }

    public static void DeactivateGUIElement(GameObject GUIElement,bool ClearActive)
    {
        if (GUIElement != null)
        {
            Tooltip.HideTooltipStatic();
            GUIElement.SetActive(false);

            Instance.ActiveGUIElement = null;
        }


    }

    public static void ToggleGUIElement(GameObject GUIElement)
    {
        if (GUIElement != null)
        {

            bool state = GUIElement.activeSelf;

            GUIElement.SetActive(!state);

        }
                  
        
    }

    public static void SimulateOnClick(GameObject ObjectWithButton)
    {
        ObjectWithButton.GetComponent<Button>().onClick.Invoke();
    }

    public static void AdaptiveBackButton(GameObject PreviousMenu, GameObject CurrentMenu)
    {
        ActivateGUIElement(PreviousMenu);
        Tooltip.HideTooltipStatic();
        DeactivateGUIElement(CurrentMenu);
    }

    public void BackFrombuildMenuToUnitControlsButton()
    {
        ActivateGUIElement(unitControls);
        Tooltip.HideTooltipStatic();
        DeactivateGUIElement(buildMenu);

    }
    public void BackFrombuildMenuToBuildControlsButton()
    {
        ActivateGUIElement(buildingControls);
        Tooltip.HideTooltipStatic();
        DeactivateGUIElement(buildMenu);
    }


    public void StartFlashButton(GameObject ButtonForFlash,Color flashColor, float flashTime)
    {

        ButtonForFlash.GetComponent<State>().Flashing = true;

        var defaultColor = ButtonForFlash.GetComponent<Button>().image.color;

        StartCoroutine(ButtonFlasher(ButtonForFlash, defaultColor, flashColor, flashTime));

    }

    public void StopFlashButton(GameObject ButtonForStopFlash)
    {
        ButtonForStopFlash.GetComponent<State>().Flashing = false;
    }


    IEnumerator ButtonFlasher(GameObject ButtonForFlash,Color defaultColor, Color flashColor, float flashTime)
    {
       
        //Debug.Log(transform.name + "is Flashing!");
        var Button = ButtonForFlash.GetComponent<Button>();

        var State = ButtonForFlash.GetComponent<State>().Flashing;
        
        while(State!=false)
        {

            for (int i = 0; i < 2; i++)
            {
                Button.image.color = flashColor;
                yield return new WaitForSeconds(flashTime);
                Button.image.color = defaultColor;
                yield return new WaitForSeconds(flashTime);
            }

            State = ButtonForFlash.GetComponent<State>().Flashing;

        }

    }



}
