using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour {

    public GameObject agentVisual;
    public float speed = 10;

    private GameObject agentPrefab;

    private float timer = 0, destroyTimer = 1;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        agentPrefab = (GameObject)Resources.Load("Prefabs/World/Agent");
    }

    void FixedUpdate() {
        timer += Time.deltaTime / (speed * 2);
        if(timer >= 0.5f){
            destroyTimer -= Time.deltaTime;
            GetComponent<Animator>().SetBool("done", true);
            if(destroyTimer < 0){
                GameObject agent = (GameObject)Instantiate(agentPrefab, transform.position, transform.rotation);
                Tile tile = GameController.island.GetTileByWorldCoords(transform.position.x, transform.position.z);
                GameController.island.DestroyObject(tile);
            }

        }else{
            agentVisual.transform.localScale = Vector3.one * timer;
        }
    }


}
