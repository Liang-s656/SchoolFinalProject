using UnityEngine;
using System.Collections.Generic;
using System.IO;
[System.Serializable]
public class GameData{
    [System.Serializable]
    public class Buildings{
        public List<Buildable> buildings = new List<Buildable>();
    }

    public static Buildings buildings;
    public static List<ResourceData> resources;

    private static string pathToFiles = Application.dataPath + "/Resources/World/";
    public static string GetResourceTitleFromID(){
        return "";
    }
    public static void PopulateBuildings(){
        string path = pathToFiles + "buildings.json";
        string json = File.ReadAllText(path);
        buildings = JsonUtility.FromJson<Buildings>(json);
    }
    public static void PopupateResources(){
        resources = new List<ResourceData>();        
        string path = pathToFiles + "Resources.txt";
        FileInfo sourceFile = new FileInfo(path);
        StreamReader reader = sourceFile.OpenText();

        string text = reader.ReadLine();
        while(text != null){
            string[] tokens = text.Split(':');
            try{
                resources.Add(new ResourceData(int.Parse(tokens[0]), tokens[1]));
            }finally{
                text = reader.ReadLine();
            }
        }

        reader.Close();
    }

    public static int GetResourceIdByName(string name){
        foreach (ResourceData resource in resources)
        {
            if(resource.title.ToLower() == name.ToLower()){
                return resource.resourceID;
            }
        }
        return -1;
    }
}
