using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject player;
    public GUISkin skin;
    public Texture2D menuScreenTexture;

    public static PlayerController playerController;

    public static Island island;

	// Use this for initialization
	void Start () {
        playerController = player.GetComponent<PlayerController>();
        island = GameObject.FindGameObjectWithTag("island").GetComponent<Island>();
        
        SaveGame.LoadGame();

		GameData.PopupateResources();
        GameData.PopulateBuildings();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private Vector2 scrollPosition = Vector2.zero;
    private void OnGUI() {
        GUI.skin = skin;
        float w = Screen.width, h = Screen.height;

        if(menuScreenTexture != null)
            GUI.DrawTexture(new Rect(0, 0, w, h), menuScreenTexture);

        GUI.BeginGroup(new Rect(w - 460, h - 250, 440, 230));
        
        GUI.Box(new Rect(0, 0, 440, 300), "");
        GUI.BeginGroup(new Rect(0, 0, 440, 45));
            GUI.Box(new Rect(0, 0, 500, 45), "", "darkblue");
            GUI.Button(new Rect(440 - 16 - 15, 15, 16, 16), "", "minimize");
        GUI.EndGroup();

        int buildingCount = GameData.buildings.buildings.Count;
        scrollPosition = GUI.BeginScrollView(new Rect(5, 50, 430, 175), scrollPosition, new Rect(0, 0, 400, buildingCount * 30));
            for (int i = 0; i < buildingCount; i++)
            {
                Buildable building = GameData.buildings.buildings[i];
                if(GUI.Button(new Rect(0, 30 * i, 440, 27), building.name)){
                    playerController.SetBuilding(building);
                }
            }

        GUI.EndScrollView();

        GUI.EndGroup();
    }

}
