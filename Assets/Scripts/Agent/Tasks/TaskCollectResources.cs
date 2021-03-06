using UnityEngine;

public class TaskCollectResources : ITask {
    
    Collectable target;
    float coolDownTimer = 0, coolDown;

    private string state = "Collecting";

    public TaskCollectResources(Collectable collectable){
        this.target = collectable;
        coolDownTimer = coolDown = collectable.coolDownPeriod;
    }

    public bool Execute() {
        if(target == null)
            return false;

        coolDownTimer -= Time.deltaTime;
        if(coolDownTimer < 0){
            coolDownTimer = coolDown;
            CollectResources();
        }

        return true;
    }

    public string GetState() {
        return state;
    }
    private void CollectResources() {
        Resource collected = target.CollectRandomResources();
        if( collected == null ){
            target.Finish();
            // Dead target
            state = "DONE";
        } else {
            PlayerData.AddResource(collected);
        }
    }

    public void InitTarget(Collectable target){
        this.target = target;
        coolDown = target.coolDownPeriod;
    }

}
