using UnityEngine;
using System.Collections.Generic;
public class PlayerData : MonoBehaviour {
    public static List<Resource> myResources;

    public static void AddResource(Resource resource){
        if(myResources == null) 
            myResources = new List<Resource>();

        // Empty list
        if(myResources.Count == 0){
            myResources.Add(resource);
            return;
        } 

        // Not empty && In list
        for (int i = 0; i < myResources.Count; i++)
        {
            if(myResources[i].resourceID == resource.resourceID){
                myResources[i].amount += resource.amount;
                return;
            }
        }

        // Not empty & Not in list
        myResources.Add(resource);
    }
}