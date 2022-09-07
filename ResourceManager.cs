using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;


    public GUIManager guiManager;
    public MissionProperies CurrentMission;
    public int CurrentPlayers;
    public int Player1Resources;    
    public int Player2Resources;
    public int Player1Capacity;
    public int Player2Capacity;

    public List<PlayerStats> Players = new List<PlayerStats>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        CurrentPlayers = CurrentMission.NumberOfPlayers;

        Player1Resources = CurrentMission.StartingResources;
        Players[1].Resources = CurrentMission.StartingResources;
        Player2Resources = CurrentMission.StartingResources;
        Players[2].Resources = CurrentMission.StartingResources;

        guiManager.UpdateResource(Players[1].Resources);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConsumeResources(int PlayerID, int Amount)
    {
        Players[PlayerID].Resources -= Amount;

        if (PlayerID == 1)
        {           
            guiManager.UpdateResource(Players[1].Resources);
            FloatingTextUI.Instance.AddTextFromFloat((int)Amount, guiManager.resourceValue.transform.position, Vector3.right * 15 + Vector3.down * 25, Color.gray, -2, -2, false);
        }    

    }
   
    public void GetResources(int PlayerID, int Amount)
    {
        Players[PlayerID].Resources += Amount;

        if (PlayerID == 1)
        {
           
            guiManager.UpdateResource(Players[1].Resources);
            FloatingTextUI.Instance.AddTextFromFloat((int)Amount, guiManager.resourceValue.transform.position, Vector3.right * 10 + Vector3.down * 25, Color.yellow, 2, 2, false);

        }
        
    }

    public int CheckCapacity(int PlayerID, int Amount)
    {
        if(PlayerID<=Players.Count)
        {
            if (Amount > Players[PlayerID].Capacity - Players[PlayerID].Resources)
            {
                return Players[PlayerID].Capacity - Players[PlayerID].Resources;
            }
            else
            {
                return Amount;
            }
        }
        else
        {
            return -1;
        }
            

    }

    public void IncreaseCapacity(int PlayerID,int Amount)
    {

        Players[PlayerID].Capacity += Amount;

    }

    public void DecreaseCapacity(int PlayerID,int Amount)
    {

        Players[PlayerID].Capacity -= Amount;

    }

}

