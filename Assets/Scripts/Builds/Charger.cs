using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour {

    public GameObject energyBall;
    public Resource requires;

    private AgentController target;

    private bool canCharge = false;
    private LineRenderer lineRenderer;

    private float energy;


    private void Start() {
        lineRenderer = energyBall.GetComponent<LineRenderer>();    
    }

	void FixedUpdate () {
        SetLineRenderer();        
        if(canCharge && target != null){
            SetEnergySize();
            if(energy > 0){
                target.energyPower += Time.deltaTime * 2;
                energy -= Time.deltaTime * 2;
                target.SetSize();
                if(target.energyPower > 150){
                    target = null;
                }
            }else{
                GetEnergy();
            }
        }
	}

    private void SetLineRenderer(){
        if(target != null && canCharge){
            lineRenderer.SetPosition(0, energyBall.transform.position);            
            lineRenderer.SetPosition(1, target.transform.position);
        }
        else{
            lineRenderer.enabled = false;
        }
    }

    public void SetTarget(AgentController agent){
        target = agent;
        lineRenderer.enabled = true;    
        if(energy <= 0)    
            GetEnergy();
    }

    void GetEnergy(){
        canCharge = PlayerData.RemoveResource(requires);
        if( canCharge ){
            energy = requires.amount;
        }
    }

    void SetEnergySize(){
        float size = energy / 30.0f;
        energyBall.transform.localScale = Vector3.one * size;
    }
}
