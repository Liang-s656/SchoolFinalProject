using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour {

    public Transform body;
    public bool move = false;
    public float speed = 0;

    private bool rotateLeft;

    Animator animator;

    float idleTimer = 0;
    private void Start() {
        idleTimer = Random.Range(0, 4);
        animator = GetComponent<Animator>();
        changeRotationDirection();
        transform.Rotate(0, Random.Range(0, 360), 0);
    }
    private void FixedUpdate() {
        idleTimer-=Time.deltaTime;
        if(idleTimer<0){
            animator.SetBool("move", !animator.GetBool("move"));
            idleTimer = Random.Range(1, 6);
            changeRotationDirection();            
        }
        if(move){
            transform.position += transform.forward * Time.deltaTime * speed;
            transform.Rotate(0, 1.4F * (rotateLeft ? -1 : 1), 0);
        }
    }

    private void changeRotationDirection(){
        rotateLeft = Random.Range(0, 2) == 0;
    }

}
