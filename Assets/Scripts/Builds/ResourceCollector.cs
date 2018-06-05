using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : MonoBehaviour {
    [Header("Generates Resource:")]
    public string resourceName;
    public int coolDownPeriod;
    public int amount;

    protected float timer;
    private Resource resource;
	public virtual void Start () {
        resource = new Resource();
        resource.resourceID = GameData.GetResourceIdByName(resourceName);
        resource.amount = amount;
        timer = coolDownPeriod;
	}

    protected virtual void FixedUpdate() {
        timer -= Time.deltaTime;
        if(timer < 0){
            FinishTask();
        }
    }

    protected void FinishTask() {
        PlayerData.AddResource(resource);
        timer = coolDownPeriod;
    }
}
