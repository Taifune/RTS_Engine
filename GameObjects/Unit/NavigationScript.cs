using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationScript : MonoBehaviour
{
    //public UnitManager unitManager;
    
         
    public static BuildingScript FindNearestCB(Transform Unit,BuildingsManager Manager)
    {
       if(Manager.CentralBuildingsOnMap.Count>0)
        {

            #region Main

            List<float> Distances = new List<float>();

            foreach (var building in Manager.CentralBuildingsOnMap)
            {
                var distance = (Unit.position - building.OriginPoint.transform.position).magnitude;
                Distances.Add(distance);

            }

            float[,] indexesForDistances = new float[2, Distances.Count];

            for (int i = 0; i < Distances.Count; i++)
            {
                indexesForDistances[0, i] = Distances[i];
                indexesForDistances[1, i] = i;
                // Debug.Log("Dist:" + indexesForDistances[0, i] + " Index: " + indexesForDistances[1, i]);           
            }


            foreach (var distance in Distances)
            {
                //Debug.Log("before sort: " + distance);
            }

            Distances.Sort();


            foreach (var distance in Distances)
            {
                // Debug.Log("After sort: " + distance);
            }


            //Debug.Log("Lowest distance is " + Distances[0]);

            var index = (int)FindIndex(Distances[0], Distances.Count, indexesForDistances);

            if (index == -1)
            {
                Debug.Log("Error in Finding Idexed of Navigation");
            }

            //Debug.Log(Manager.CentralBuildingsOnMap[index].OriginPoint.transform.position + " with distance " + (Unit.position - Manager.CentralBuildingsOnMap[index].OriginPoint.transform.position).magnitude + "is the best!");

            return Manager.CentralBuildingsOnMap[index];

            #endregion

        }
       else
        {
            //Debug.Log("Cant find any CB!");
            return null;
        }

    }


    private static float FindIndex(float SearchableValue,int Lenght, float[,] massiveForSearch)
    {
        float returnVallue = -1;

        for (int i = 0; i < Lenght; ++i)
        {
            //Debug.Log("line i " + i + " Checking " + massiveForSearch[0,i]+ "for == with" + SearchableValue );

            if (massiveForSearch[0,i]==SearchableValue)
            {
                //Debug.Log("Match!");
                returnVallue = massiveForSearch[1, i];
                return returnVallue;
            }
            else
            {
                //Debug.Log("NotMatch");
            }
        }


        return returnVallue;
    }
    
}
