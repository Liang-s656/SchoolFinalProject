using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSibling : MonoBehaviour {
    private static float SPEED = 0.1f;

    void FixedUpdate() {
        if(transform.localScale.y < 1){
            transform.localScale += Vector3.one * Time.deltaTime * SPEED;
        }else{
            gameObject.tag = "tree";
            Tile tile = GameController.island.GetTileByWorldCoords(transform.position.x, transform.position.z);
            tile.building = null;
            GameController.island.PlaceObject(tile, gameObject, true);
        }
    }
	
}
