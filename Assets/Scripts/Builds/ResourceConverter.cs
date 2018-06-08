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
        animator.enabled = canWork;
    }
	
	// Update is called once per frame
	protected override void FixedUpdate() {
        if(canWork){
            timer -= Time.deltaTime;
            if(timer < 0){
                FinishTask();
                CatchResourcesFromPlayer();
            }
        }
    }

    public void DrawGUI(){
        if(GUI.Button(new Rect(Screen.width - 140, 140, 120, 30), "Refill")){
            CatchResourcesFromPlayer();
        }
    }

}
