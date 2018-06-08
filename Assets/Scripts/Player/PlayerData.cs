using UnityEngine;
using System.Collections.Generic;
public class PlayerData : MonoBehaviour {
    public static List<Resource> myResources = new List<Resource>();

    public static void AddResource(Resource resource){
        if(myResources == null) 
            myResources = new List<Resource>();

        // Not empty && In list
        for (int i = 0; i < myResources.Count; i++)
        {
            if(myResources[i].resourceID == resource.resourceID){
                int amount = resource.amount;
                myResources[i].amount += resource.amount;
                return;
            }
        }

        // Not empty & Not in list
        myResources.Add(new Resource(){ 
            resourceID = resource.resourceID, 
            amount = resource.amount 
        });
    }

    public static bool RemoveResource(Resource resource){
        if(myResources == null)
            return false;
        
        for (int i = 0; i < myResources.Count; i++)
        {
            if(myResources[i].resourceID == resource.resourceID){
                if(myResources[i].amount >= resource.amount){
                    myResources[i].amount -= resource.amount;
                    if(myResources[i].amount == 0){
                        myResources.RemoveAt(i);
                    }
                    return true;
                }
                return false;        
            }
        }
        return false;        
    }

    public static void RemoveResources(Resource[] resources){
        foreach (Resource resource in resources){
            RemoveResource(resource);
        }
    }

    public static bool HasEnoughResources(Resource[] resources){
        foreach (Resource resource in resources){
            if(!HasEnoughResource(resource)){
                return false;
            }
        }
        return true;
    }

    public static bool HasEnoughResource(Resource resource){
        Resource myResource = GetResource(resource.resourceID);
        return myResource != null && resource.amount <= myResource.amount;
    }

    public static Resource GetResource(int id){
        for (int i = 0; i < myResources.Count; i++){
            if(myResources[i].resourceID == id){
                return myResources[i];
            }
        }
        return null;
    }

    
}
