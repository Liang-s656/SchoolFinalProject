using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RihaCompiler))]
public class CompilerEditor : Editor {
    private string command = "set   parameter as  number : 99";
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        command = GUILayout.TextArea(command);
        if(GUILayout.Button("Compile")){
            RihaCompiler rc = target as RihaCompiler;
            rc.Execute(command);
        }
    }

}
