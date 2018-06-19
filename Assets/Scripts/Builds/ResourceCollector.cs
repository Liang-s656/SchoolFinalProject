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
    protected bool work = true;
	public virtual void Start () {
        resource = new Resource();
        resource.resourceID = GameData.GetResourceIdByName(resourceName);
        resource.amount = amount;
        timer = coolDownPeriod;
	}

    protected virtual void FixedUpdate() {
        if(!work) return;
        timer -= Time.deltaTime;
        if(timer < 0){
            FinishTask();
        }
    }

    protected void FinishTask() {
        PlayerData.AddResource(resource);
        timer = coolDownPeriod;
    }

    public void DrawGUI(){
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        if(GUI.Button(new Rect(pos.x + 20, Screen.height - pos.y - 21, 90, 20), "work: <b>" + work + "</b>", "BuildingBtn")){
            work = !work;
        }
    }
}
