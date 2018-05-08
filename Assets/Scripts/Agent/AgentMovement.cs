/*
Moves from a to b
*/
using UnityEngine;
using System.Collections.Generic;

public class AgentMovement : MonoBehaviour {
    public static float speed;
    private List<Vector3> movementBatch;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
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
        Vector3.Lerp(from, to, Time.deltaTime * 10 * speed);
        if( Vector3.Distance(from, to) < 0.01f){
            movementBatch.RemoveAt(0);
            transform.position = to;
            UpdateSelf();
        }
    }

    private void UpdateSelf() {
        if(CanMove()){
            Vector3 from = transform.position;
            Vector3 to = movementBatch[0];    
            transform.LookAt(to);          

            animator.SetBool("wallking", from.y == to.y);
            animator.SetBool("climbing", from.y != to.y);
        } else {
            animator.SetBool("wallking", false);
            animator.SetBool("climbing", false);
            // Start task/action
        }
    }

    public void AddPointToPath(Vector3 point){
        if(movementBatch == null)
            movementBatch = new List<Vector3>();
        movementBatch.Add(point);
    }

    public void AddPath(List<Vector3> path, bool clear = true){
        if(clear) ClearPath();
        for (int i = 0; i < path.Count; i++) {
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