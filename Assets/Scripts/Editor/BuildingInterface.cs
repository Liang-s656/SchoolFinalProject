using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
public class BuildingInterface : EditorWindow {

    [System.Serializable]
    class Buildings{
        public List<Buildable> buildings = new List<Buildable>();
    }


    [MenuItem("Custom/Building Interface")]
    private static void ShowWindow() {
        GetWindow<BuildingInterface>().Show();
    }

    Vector2 scroll = Vector2.zero;
    Buildable selected;
    Buildings buildings = new Buildings();

    private void LoadData() {
    }

    private void TestSave() {
        Buildable building = new Buildable();
        building.name = "Stair";
        building.description = "...";
        building.type = "navigation";
        building.prefabPath = "/path/to/prefab";
        Resource requiredResource = new Resource();
        requiredResource.resourceID = 0;
        requiredResource.amount = 3;
        building.requiredResources = new Resource[]{
            requiredResource
        };


        buildings.buildings.Add(building);
        string json = JsonUtility.ToJson(buildings);
        Debug.Log(json);
    }

    private void TestLoad() {
        string path = Application.dataPath + "/Scripts/Editor/stairs.json";
        string json = File.ReadAllText(path);
        buildings = JsonUtility.FromJson<Buildings>(json);
        selected = buildings.buildings[0];
        Debug.Log(buildings.buildings.Count);
    }

    private void OnGUI() {
        float w = Screen.width;

        GUILayout.BeginHorizontal();

        // Side panel
        GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("Buildings");

            scroll = GUILayout.BeginScrollView(scroll);

            GUILayout.EndScrollView();

            if(GUILayout.Button("Add new")){
                TestSave();
            };
            if(GUILayout.Button("Load")){
                TestLoad();
            };
        GUILayout.EndVertical();

        // Main panel
        GUILayout.BeginVertical(GUILayout.Width(300));
            if(selected != null){
                GUILayout.Label("Name:");
                selected.name = GUILayout.TextField(selected.name);
                GUILayout.Label("Description:");                
                selected.description = GUILayout.TextArea(selected.description, GUILayout.MinHeight(80));
                GUILayout.Label("Type:");                
                selected.type = GUILayout.TextField(selected.type);
                GUILayout.Label("Prefab Path:");                
                selected.prefabPath = GUILayout.TextField(selected.prefabPath);                
                GUILayout.BeginHorizontal();
                    selected.isRotatable = GUILayout.Toggle(selected.isRotatable, "Rotatable");
                    selected.requiresBlueprint = GUILayout.Toggle(selected.requiresBlueprint, "Blueprint");
                GUILayout.EndHorizontal();
                
                if(selected.requiresBlueprint){
                    GUILayout.Label("Blueprint Name:");                
                    GUILayout.TextField("Blueprint name");
                }   

                GUILayout.Button("Save");       
                
            }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

    }
}