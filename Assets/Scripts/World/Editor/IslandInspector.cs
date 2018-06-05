using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Island))]
public class IslandInspector : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if(GUILayout.Button("Regenerate")){
            Island island = target as Island;
            island.Start();
        }
    }

}
