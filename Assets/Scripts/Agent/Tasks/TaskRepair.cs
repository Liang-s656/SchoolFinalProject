using UnityEngine;

public class TaskRepair : ITask {
    Repairable target;
    private float timer;
    private string state = "Repairing";

    public TaskRepair(Repairable target = null){
        this.target = target;
    }
    public bool Execute(){
        if(target == null)
            return false;
        if(target.IsFullyRepaired()){
            state = "DONE";
        } else {
            timer -= Time.deltaTime;
            if(timer < 0){
                Repair();
                timer = target.repairCooldown;
            }  
        }
        return true;
    }

    private void Repair() {
        target.AddHealth(Random.Range(1, 8));
    }

    public string GetState(){
        return state;
    }


}
