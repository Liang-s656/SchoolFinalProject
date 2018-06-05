/*
Moves from a to b
*/
using UnityEngine;
using System.Collections.Generic;

public class AgentMovement : MonoBehaviour {
    public static float speed = 1.5f;
    private List<Vector3> movementBatch;
    private Animator animator;
    private AgentController agentController;

    private Tile myTile;

    private void Start() {
        animator = GetComponent<Animator>();
        agentController = GetComponent<AgentController>();
        UpdateSelf();
    }

    void FixedUpdate() {
        if(CanMove()){
            Move();
        }
    }

    private bool CanMove() {
        return movementBatch != null && movementBatch.Count > 0;
    }

    private void Move() {
        Vector3 from = transform.position;
        Vector3 to = movementBatch[0];
        transform.position = Vector3.MoveTowards(from, to, Time.deltaTime * speed);
        if( Vector3.Distance(from, to) < 0.01f){
            movementBatch.RemoveAt(0);
            transform.position = to;
            UpdateSelf();
        }
    }

    private void UpdateSelf() {

        if(myTile != null && myTile.GetBuilding() == "agent"){
            myTile.building = null;
        }

        myTile = GameController.island.GetTileByWorldCoords(transform.position.x, transform.position.z);
        myTile.building = gameObject;

        if(CanMove()){
            Vector3 from = transform.position;
            Vector3 to = movementBatch[0];    
            if(from.y == to.y){
                transform.LookAt(to);          
            }

            animator.SetBool("wallking", from.y == to.y);
            animator.SetBool("climbing", from.y != to.y);
            animator.SetBool("action", false);
            
        } else {
            animator.SetBool("wallking", false);
            animator.SetBool("climbing", false);

            agentController.walking = false;
            // Start task/action
        }
    }

    public void AddPointToPath(Vector3 point){
        if(movementBatch == null)
            movementBatch = new List<Vector3>();
        movementBatch.Add(point);
    }

    public void AddPath(List<Vector3> path, bool keepLast = true, bool clear = true){
        if(clear) ClearPath();
        int to = keepLast ? path.Count : path.Count - 1;
        for (int i = 0; i < to; i++) {
            AddPointToPath(path[i]);
        }
    }

    public void ClearPath(){
        if(movementBatch == null)
            movementBatch = new List<Vector3>();
        else
            movementBatch.Clear();
    }

}
