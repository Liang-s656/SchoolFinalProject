using UnityEngine;
using System.Collections.Generic;
public class AgentController : MonoBehaviour {
    public float energyPower = 100.0f;
    public GameObject visual;
    private List<ITask> taskBatch;

    public bool walking;
    public Material evilMaterial;
    bool isEvill;
    float evillTimer = 5;
    public GameObject evillDead;

    private Island island;
    private Animator animator;
    AgentMovement agentMovement;

    private void Start() {
        animator = GetComponent<Animator>();
        agentMovement = GetComponent<AgentMovement>();    
        island = GameObject.FindGameObjectWithTag("island").GetComponent<Island>();
    }

    void FixedUpdate() {
        if(!walking) {
            if(taskBatch != null && taskBatch.Count > 0){
                energyPower -= Time.deltaTime;
                SetSize();
                animator.SetBool("action", true);
                if(taskBatch[0].GetState() == "DONE"){
                    taskBatch.RemoveAt(0);
                    animator.SetBool("action", false);                    
                }else if(!taskBatch[0].Execute()){
                    taskBatch.Clear();
                    animator.SetBool("action", false);                    
                    Debug.LogError("Task Execution Error");
                }
            }
        }else{
            energyPower -= Time.deltaTime / 2;
            SetSize();
        }
        if(isEvill){
            evillTimer -= Time.deltaTime;
        }
    }

    void Update(){
        if(energyPower < 25){                  
            Destroy(gameObject, 1);
        }else if(energyPower > 150){
            if(!isEvill){
                SetEvill();
            }
        }

        if(evillTimer < 0){
            Explode();
        }
    }

    void SetEvill(){
        isEvill = true;
        visual.GetComponent<MeshRenderer>().material = evilMaterial;
        animator.SetBool("evill", true);
    }

    void Explode(){
        for(int x = -2; x <= 2; x++){
            for(int z = -2; z <= 2; z++){      
                Tile tile = island.GetTileByWorldCoords(transform.position.x + x, transform.position.z + z);
                if(tile != null && tile.building != null){
                    Repairable r = tile.building.GetComponent<Repairable>();
                    if(r != null){
                        r.AddHealth(Random.Range(-60, -20));
                    }
                }
            }
        }
        Instantiate(evillDead, transform.position, transform.rotation);
        Destroy(gameObject);            
    }

    public void SetSize(){
        float size = energyPower / 200.0f;
        visual.transform.localScale = Vector3.one * size;
    }

    public void TargetToAction(Transform target){
        animator.SetBool("action", false);       
        taskBatch = null;

        if(GeneratePathToTarget(target.position)){
            if(target.GetComponent<Collectable>()){
                Collectable collectable = target.GetComponent<Collectable>();
                TaskCollectResources taskCollect = new TaskCollectResources(collectable);
                AddTask(taskCollect);
            } else if(target.name == "Charge Station") {

            } else if(target.GetComponent<Repairable>()){
                Repairable repairable = target.GetComponent<Repairable>();
                AddTask(new TaskRepair(repairable));            
            }
        }
    }

    private void AddTask(ITask task){
        if(taskBatch == null)
            taskBatch = new List<ITask>();
        taskBatch.Add(task);
    }

    public bool GeneratePathToTarget(Vector3 position, bool include = false){
        Tile to = island.GetTileByWorldCoords(position.x, position.z);
        return GeneratePathToTarget(to, include);
    }

    public bool GeneratePathToTarget(Tile to, bool include = false){
        Tile from = island.GetTileByWorldCoords(transform.position.x, transform.position.z);
        if(to != null && from != null){
            List<Vector3> path = island.GetPath(to, from);
            if(path != null){
                agentMovement.AddPath(path, include);
                walking = true;
                return true;
            }
        }
        return false;
    }

    public void DrawGUI(){
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(pos.x + 20, Screen.height - pos.y - 22, 100, 20), "", "EnergyEmpty");
        GUI.Box(new Rect(pos.x + 20, Screen.height - pos.y - 22, 1 * energyPower, 20), Mathf.Round(energyPower * 10) / 10.0f + " En", "Energy");
    }

}
