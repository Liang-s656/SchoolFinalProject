using System.Collections.Generic;
using UnityEngine;
public class Collectable : MonoBehaviour {
    public List<Resource> resources;
    public float coolDownPeriod = 5;
    public GameObject destroyObject;


    public Resource CollectRandomResources() {
        if(resources == null || resources.Count == 0){
            return null;
        }

        int randomResource = Random.Range(0, resources.Count);
        int randomResourceCount = Random.Range(1, resources[randomResource].amount + 1);
        resources[randomResource].amount -= randomResourceCount;

        Resource collected = new Resource();
        collected.resourceID = resources[randomResource].resourceID;
        collected.amount = randomResourceCount;

        if(resources[randomResource].amount <= 0){
            resources.RemoveAt(randomResource);
        }

        return collected;
    }

    public void Finish(){
        if(destroyObject != null){
            GameObject d = (GameObject) Instantiate(destroyObject, transform.position, transform.rotation);
        }
        Tile tile = GameController.island.GetTileByWorldCoords(transform.position.x, transform.position.z);
        GameController.island.DestroyObject(tile);
        //Destroy(gameObject);
    }

     public void DrawGUI(){
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        float mid = Screen.height - pos.y - 22 - (resources.Count * 21) / 2;
        for(int r = 0; r < resources.Count; r++){
            Resource resource = resources[r];
            string name = GameData.GetResourceName(resource.resourceID);
            GUI.Box(new Rect(pos.x + 20, mid + 21 * r, 100, 20), name + ": <b>" + resource.amount + "</b>");
        }
    }

}
