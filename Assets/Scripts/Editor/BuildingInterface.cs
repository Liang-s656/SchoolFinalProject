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
        selected = JsonUtility.FromJson<Buildable>(json);
        Debug.Log(selected.name);
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
        GUILayout.BeginVertical(GUILayout.Width(w - 200));
            if(selected != null){

            }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

    }
}