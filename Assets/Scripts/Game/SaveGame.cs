using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveGame {

    public static bool newGame = false;
    
    public struct SaveData {
        public string key;
        public string value;
    }

    public static List<SaveData> saveFiles;

    public static void PopulateSaveData(string fileLocation = null){
        saveFiles = new List<SaveData>();
        //var 
        string[] lines;
        if(fileLocation == null){
            lines = PlayerPrefs.GetString("LoadGame").Split('\n');
        }else{
            lines = File.ReadAllLines(fileLocation);
        }
        foreach (string line in lines){
            string[] parts = line.Split(':');
            if(parts.Length != 2) continue; 
            saveFiles.Add(new SaveData(){
                key = parts[0],
                value = parts[1]
            });
        }
    }

    public static string GetSaveData(string parameter){
        foreach(SaveData saveFile in saveFiles){
            if(saveFile.key == parameter)
                return saveFile.value;
        }
        return "-1";
    }

    public static float GetSaveDataFlaot(string parameter){
        return float.Parse(GetSaveData(parameter));
    }

    public static void LoadGame(bool newGame){
        if(!newGame){
            PopulateSaveData();
            LoadIsland();
            LoadPlayer();
            LoadPlayerResources();
            LoadIslandsTrees();
            LoadGameBuildings();
        }else{
            GameController.island.GenerateSeed();
            AddStartResources();
        }
        GameController.island.enabled = true;
        GameController.playerController.gameObject.SetActive(true);
        GameController.playerController.GetComponent<CameraMovement>().enabled = true;
    }

    public static string GetSaveGame(){
        string output = "";
        Dictionary<string, string> saveData = new Dictionary<string, string>();
        saveData.Add("seed_x", GameController.island.seed.x.ToString());
        saveData.Add("seed_y", GameController.island.seed.y.ToString());
        saveData.Add("seed_z", GameController.island.seed.z.ToString());
        
        saveData.Add("seed_tree_x", GameController.island.treeSeed.x.ToString());
        saveData.Add("seed_tree_y", GameController.island.treeSeed.y.ToString());
        saveData.Add("seed_tree_z", GameController.island.treeSeed.z.ToString());
        
        Vector3 playerPosition = GameController.playerController.transform.position;
        saveData.Add("player_position_x", playerPosition.x.ToString());
        saveData.Add("player_position_z", playerPosition.z.ToString());
        saveData.Add("player_rotation_y", GameController.playerController.transform.localEulerAngles.y.ToString());

        saveData.Add("camera_zoom", Camera.main.orthographicSize.ToString());

        foreach(Resource resource in PlayerData.myResources){
            saveData.Add("resource_" + resource.resourceID, resource.amount.ToString());
        }
        
        foreach(KeyValuePair<string, string> saveRow in saveData){
            output += saveRow.Key + ":" + saveRow.Value + "\n";
        }

        output += GameController.island.GetSaveData();

        return output;
    }   

    private static void LoadIsland() {
        GameController.island.seed = new Vector3(
            GetSaveDataFlaot("seed_x"),
            GetSaveDataFlaot("seed_y"),
            GetSaveDataFlaot("seed_z")                
        );

        GameController.island.treeSeed = new Vector3(
            GetSaveDataFlaot("seed_tree_x"),
            GetSaveDataFlaot("seed_tree_y"),
            GetSaveDataFlaot("seed_tree_z") 
        );
    }

    private static void LoadIslandsTrees(){
        string trees = GetSaveData("TREE");
        if(trees != "-1"){
            GameController.island.trees = new List<Vector3>();
            string[] treePositions = trees.Split('|');
            
            foreach(string pos in treePositions){
                string[] vPos = pos.Split(',');
                if(vPos.Length == 3){
                    GameController.island.trees.Add( new Vector3(
                        float.Parse(vPos[0]),
                        float.Parse(vPos[1]),
                        float.Parse(vPos[2])
                    ));
                }
            }
        }
    }

    private static void LoadGameBuildings(){
        string buildings = GetSaveData("BUILDINGS");
        if(buildings != "-1"){
            string[] buildsData = buildings.Split('|');
            foreach(string bd in buildsData){
                string[] data = bd.Split('*');
                if(data.Length == 2){
                    Buildable building = GameData.GetBuilding(data[0]);
                    if(building != null){
                        string[] vPos = data[1].Split(',');
                        if(vPos.Length == 3){
                            Vector3 pos = new Vector3(
                                float.Parse(vPos[0]),
                                float.Parse(vPos[1]),
                                float.Parse(vPos[2])
                            );
                            GameController.island.AddToBuildingBatch(building, pos);
                        }
                    }
                }
            }
        }
    }

    private static void AddStartResources(){
        PlayerData.AddResource(new Resource(){ 
            resourceID = 2,
            amount = 200
        });
        PlayerData.AddResource(new Resource(){ 
            resourceID = 0,
            amount = 10
        });
        PlayerData.AddResource(new Resource(){ 
            resourceID = 1,
            amount = 10
        });
    }
    private static void LoadPlayerResources(){
        for (int i = 0; i < GameData.buildings.buildings.Count; i++)
        {
            int amount = (int)GetSaveDataFlaot("resource_" + i);
            if(amount > 0){
                PlayerData.AddResource(new Resource(){
                    resourceID = i,
                    amount = amount
                });
            }
        }
    }

    private static void LoadPlayer(){
        GameController.playerController.transform.position = new Vector3(
            GetSaveDataFlaot("player_position_x"),
            GameController.playerController.transform.position.y,
            GetSaveDataFlaot("player_position_z")  
        );
        GameController.playerController.transform.localEulerAngles = new Vector3(
            0, GetSaveDataFlaot("player_rotation_y") ,0
        );
        Camera.main.orthographicSize = GetSaveDataFlaot("camera_zoom");
        GameController.playerController.GetComponent<CameraMovement>().enabled = true;
    }
}
