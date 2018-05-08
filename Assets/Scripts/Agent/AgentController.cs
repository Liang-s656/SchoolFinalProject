using UnityEngine;
using System.Collections.Generic;
public class AgentController : MonoBehaviour {
    public float energyPower = 100.0f;
    private List<ITask> taskBatch;

    public bool walking;

    AgentMovement agentMovement;

    private void Start() {
        agentMovement = GetComponent<AgentMovement>();    
    }

    void FixedUpdate() {
        if(!walking) {
            if(taskBatch != null & taskBatch.Count > 0){
                if(taskBatch[0].GetState() == "DONE"){
                    taskBatch.RemoveAt(0);
                }else if(!taskBatch[0].Execute()){
                    taskBatch.Clear();
                    Debug.LogError("Task Execution Error");
                }
            }
        }
    }

    public void TargetToAction(Transform target){
        GeneratePathToTarget(target);
        if(target.GetComponent<Collectable>()){
            taskBatch.Add(new TaskCollectResources(target.GetComponent<Collectable>()));
        } else if(target.name == "Charge Station") {

        }
    }

    public void GeneratePathToTarget(Transform target){
        ///
        ///
        walking = true;
    }

}