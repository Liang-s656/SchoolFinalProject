using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceConverter : ResourceCollector {

    [Header("Required Resource")]
    public string requiredResourceName;
    public int requiredAmount;

    private Resource requiredResource;    
    private bool canWork;    

    private Animator animator;

	// Use this for initialization
	public override void Start () {
		base.Start();
        requiredResource = new Resource();
        requiredResource.resourceID = GameData.GetResourceIdByName(requiredResourceName);
        requiredResource.amount = requiredAmount;
        animator = GetComponent<Animator>();
        CatchResourcesFromPlayer();
    }

    private void CatchResourcesFromPlayer() {
        canWork = PlayerData.RemoveResource(requiredResource);
        animator.SetBool("work", canWork);
    }
	
	// Update is called once per frame
	protected override void FixedUpdate() {
        if(!work) return;
        
        if(canWork){
            timer -= Time.deltaTime;
            if(timer < 0){
                FinishTask();
                CatchResourcesFromPlayer();
            }
        }
    }

    public void DrawGUI(){
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        if(GUI.Button(new Rect(pos.x + 20, Screen.height - pos.y, 80, 20), "refill" , "BuildingBtn")){
            CatchResourcesFromPlayer();
        }
    }

}
