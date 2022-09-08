using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestroyOnColide : MonoBehaviour
{

    public NavMeshObstacle navMeshObstacle;


    private void OnMouseDown()
    {
        RTSGameManager.CrossClicked();
        Destroy(this.gameObject);
    }


}
