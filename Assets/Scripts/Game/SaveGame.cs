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

    public static void PopulateSaveData(string fileLocation = "C:/Users/rihards-pc/Documents/SchoolFinal/Assets/savefile.rih.txt"){
        saveFiles = new List<SaveData>();
        var lines = File.ReadAllLines(fileLocation);
        foreach (var line in lines){
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
        return "1";
    }

    public static float GetSaveDataFlaot(string parameter){
        return float.Parse(GetSaveData(parameter));
    }

    public static void LoadGame(){
        if(!newGame){
            PopulateSaveData();
            LoadIsland();
            LoadPlayer();
        }
        GameController.island.enabled = true;
        GameController.playerController.gameObject.SetActive(true);
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
