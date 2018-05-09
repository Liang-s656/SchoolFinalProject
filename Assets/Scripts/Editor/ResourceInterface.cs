using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
public class ResourceInterface : EditorWindow {
    class ResourceData{
        public int resourceID;
        public string title;
        public ResourceData(int id = 0, string title = "EMPTY TITLE"){
            this.resourceID = id;
            this.title = title;
        }
    }

    static List<ResourceData> resources = new List<ResourceData>();
    Vector2 scrollPosition = Vector2.zero;    

    static FileInfo sourceFile;
    static StreamReader reader;
    //static StreamWriter writer;
    static string path;

    [MenuItem("Custom/Resource Interface")]
    private static void ShowWindow() {
        GetWindow<ResourceInterface>().Show();
        LoadData();
    }

    private static void LoadData() {
        resources = new List<ResourceData>();        
        path = Application.dataPath + "/Scripts/Editor/Resources.txt";
        sourceFile = new FileInfo(path);
        reader = sourceFile.OpenText();

        string text = reader.ReadLine();
        while(text != null){
            Debug.Log(text);
            string[] tokens = text.Split(':');
            try{
                resources.Add(new ResourceData(int.Parse(tokens[0]), tokens[1]));
            }finally{
                text = reader.ReadLine();
            }
        }

        reader.Close();
    }

    private void SaveData() {
        path = Application.dataPath + "/Scripts/Editor/Resources.txt";        
        List<string> lines = new List<string>();
        foreach (ResourceData resources in resources)
        {
            lines.Add(resources.resourceID + ":" + resources.title);
            Debug.Log(lines[lines.Count - 1]);        
        }
        File.WriteAllLines(path, lines.ToArray());
    }

    private void OnGUI() {
        float width = Screen.width;
        float halfWidth = width / 2 - /*GAP*/ 7 - /*DELETE BTN*/ 35;

        GUILayout.BeginHorizontal();
            GUILayout.Label("Title", GUILayout.Width(halfWidth));
            GUILayout.Label("Resource ID", GUILayout.Width(halfWidth));
        GUILayout.EndHorizontal();          
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (ResourceData resource in resources)
        {
            GUILayout.BeginHorizontal();
            resource.title = GUILayout.TextField(resource.title, GUILayout.Width(halfWidth));
            string newId = GUILayout.TextField("" + resource.resourceID, GUILayout.Width(halfWidth));  
            try{
                resource.resourceID = int.Parse(newId);
            } catch {}

            if(GUILayout.Button("Remove", GUILayout.Width(65))){
                resources.Remove(resource);
                return;
            }
            GUILayout.EndHorizontal();          
        }
        GUILayout.EndScrollView();

        if(GUILayout.Button("New Resource")){
            resources.Add(new ResourceData(resources.Count));
        }
        GUILayout.BeginHorizontal();
            if(GUILayout.Button("Save Changes")){
                SaveData();
            }
            if(GUILayout.Button("Undo Changes")){
                LoadData();
            }
        GUILayout.EndHorizontal();          
                          
    }
}