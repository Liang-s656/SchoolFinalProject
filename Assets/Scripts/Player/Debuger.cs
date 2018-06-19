using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuger : MonoBehaviour {
    public GUISkin skin;
    RaycastHit hit;
	void OnGUI () {
        GUI.skin = skin;
    
        Vector2 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (GameController.playerController.island.Raycast(ray, out hit, 100)) {
            if(hit.normal.y == 1){
                Rect pos = new Rect(mousePos.x + 20, Screen.height - mousePos.y - 9, 60, 24);
                //int r = + GameController.island.islandDiameter / 2;
                int x = Mathf.RoundToInt(hit.point.x) ;//+ r;
                int z = Mathf.RoundToInt(hit.point.z) ;//+ r;
                GUI.Box(pos, x + ":" + z);
            }
        }

        GameObject selectedObject = GameController.playerController.selectedGO;
        if(selectedObject != null){
            //Collectable c = selectedObject.GetComponent<Collectable>();
             
            GUI.BeginGroup(new Rect(mousePos.x + 20, Screen.height - mousePos.y + 19, 210, 400));

                int r = + GameController.island.islandDiameter / 2;
                int x = Mathf.RoundToInt(selectedObject.transform.position.x) + r;
                int z = Mathf.RoundToInt(selectedObject.transform.position.z) + r;

                GUI.Box(new Rect(0, 0, 210, 24), "name: <b>" + selectedObject.name + "</b>", "statistics");
                GUI.Box(new Rect(0, 29, 210, 24), "tile: <b>[" + x + ", " + z + "]</b>", "statistics");
                GUI.Box(new Rect(0, 58, 210, 24), "position: <b>" + selectedObject.transform.position + "</b>", "statistics");
                //if(c){
               // }
            
            GUI.EndGroup();
        }
    }
}
