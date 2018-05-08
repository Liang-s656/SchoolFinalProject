using UnityEngine;
using System.Collections.Generic;
public class AgentController : MonoBehaviour {
    public float energyPower = 100.0f; // Energy Power
    private ITask task;

    void FixedUpdate() {
        if(CanDoTask()){
            task.Execute();
        }
    }

    public bool CanDoTask() {
        return task != null && task.GetState() != "DONE";
    }

}