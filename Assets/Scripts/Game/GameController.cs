using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject player;
    public GUISkin skin;

    public static PlayerController playerController;
    public static RihaCompiler compiler;
    public static bool isPause = false;

    public static Island island;

    bool[] buildingInfo;
	// Use this for initialization
	void Start () {
        GameData.PopupateResources();
        GameData.PopulateBuildings();


        playerController = player.GetComponent<PlayerController>();
        compiler = GetComponent<RihaCompiler>();
        isPause = false;
        
        island = GameObject.FindGameObjectWithTag("island").GetComponent<Island>();
        
        SaveGame.LoadGame(!PlayerPrefs.HasKey("LoadGame"));



        buildingInfo = new bool[GameData.buildings.buildings.Count];
	}

	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)){
            isPause = true;
            GetComponent<MainMenu>().SetPause();
        }
	}
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 scrollPosition2 = Vector2.zero;
    
    private bool showBuildings = false, showRespurces = false;
    int topOffset = 0;
    
    private void OnGUI() {
      //  if(compiler.showMenu){
      //      return;
      //  }

        GUI.skin = skin;
        float w = Screen.width, h = Screen.height;

        float he = 45;
        if(showBuildings){
            he = 230;
        }
        GUI.BeginGroup(new Rect(w - 460, h - he - 20, 440, he));
        
        GUI.Box(new Rect(0, 0, 440, 300), "");
        GUI.BeginGroup(new Rect(0, 0, 440, 45f));
            GUI.Box(new Rect(0, 0, 500, 45f), "Buildings", "darkblue");
            if(GUI.Button(new Rect(440 - 8 - 15f, 15f, 16, 16), "", "minimize")){
                showBuildings = !showBuildings;
            }
        GUI.EndGroup();

        if(showBuildings){
            int buildingCount = GameData.buildings.buildings.Count;
            scrollPosition = GUI.BeginScrollView(new Rect(5f, 50, 430, 175), scrollPosition, new Rect(0, 0, 400, topOffset));
            topOffset = 0;
                for (int i = 0; i < buildingCount; i++)
                {
                    Buildable building = GameData.buildings.buildings[i];
                    string btnStyle = "enough";
                    bool canBuild = PlayerData.HasEnoughResources(building.requiredResources);
                    string btnText = building.name;
                    if(!canBuild){
                        btnStyle = "not_enough";
                        btnText = "<b>ERROR: </b>" + btnText + "<i>[NOT_ENOUGH_RESOURCES]</i>";
                    }

                        
                    if(GUI.Button(new Rect(0, topOffset, 363, 27f), btnText, btnStyle) && canBuild){
                        playerController.SetBuilding(building);
                    }
                    if(GUI.Button(new Rect(368, topOffset, 403, 27f), "i")){
                        buildingInfo[i] = !buildingInfo[i];
                    }
                    if(buildingInfo[i]){
                        GUI.Box(new Rect(0, topOffset + 27, 363, 120), building.GetDescription(), "required");
                        topOffset+= 120;
                    }

                    topOffset += 30;
                }
            GUI.EndScrollView();
        }

        GUI.EndGroup();

        PlayerResourceGUI();
    }

    void PlayerResourceGUI(){
        if(PlayerData.myResources == null )
            return;
        float w = Screen.width, h = Screen.height;

        float he = 45;
        float top = 55;
        if(showBuildings){
            top = 240;
        }
        if(showRespurces){
            he = 230;
        }

        GUI.BeginGroup(new Rect(w - 460, h - he - 20 - top, 440, he));
        
            GUI.Box(new Rect(0, 0, 440, 300), "");
            GUI.BeginGroup(new Rect(0, 0, 440, 45f));
                GUI.Box(new Rect(0, 0, 500, 45f), "Resources", "darkblue");
                if(GUI.Button(new Rect(440 - 8 - 15f, 15f, 16, 16), "", "minimize")){
                    showRespurces = !showRespurces;
                }
            GUI.EndGroup();

            if(showRespurces){
                scrollPosition2 = GUI.BeginScrollView(new Rect(5f, 50, 430, 175), scrollPosition2, new Rect(0, 0, 400, PlayerData.myResources.Count * 30));
                    for (int i = 0; i < PlayerData.myResources.Count; i++)
                    {
                        Resource resource = PlayerData.myResources[i];
                        string name = GameData.GetResourceName(resource.resourceID);
                        GUI.Label(new Rect(0, 30 * i, 210, 27f), "<b>" + name + ":</b> ", "resource");
                        GUI.Label(new Rect(220, 30 * i, 210, 27f), resource.amount.ToString(), "resource_count");
                    }

                GUI.EndScrollView();
            }
        GUI.EndGroup();
    }
}
