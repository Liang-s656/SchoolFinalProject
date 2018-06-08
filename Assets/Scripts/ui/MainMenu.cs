using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using System.IO;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {

    enum MenuItemType {
        input, action, password, gameGlobal,  gameLocal
    }

    class MenuItem {
        public MenuItemType type;
        public string value;

        public string target;

        public MenuItem(MenuItemType type, string value, string target = ""){
            this.type = type;
            this.value = value;
            this.target = target;
        }
    }

    private string saveDir;
	private List<MenuItem>[] menuItems;
    private int selectedItem = 0;
    private float clickTimer = 0;
    private int visibleMenu = 0;

    public Texture2D mainMenuScreen;
    API api;
    Animation anim;
    private MenuItem typeingTarget;
    bool requestedLogin = false, 
         requestedGetGames = false, 
         requestedGetGame = false,
         requestedSaveGame = false;

    public GUISkin skin;
    public float menuLeftOffset = 250;
    public int menuTopOffset = 60;

    public bool isGameMenu = false;
    GameObject player, world;
    private void Start() {
        api = GetComponent<API>();
        saveDir = Application.dataPath + "/games";
        if(isGameMenu) {
            PopulateGameMenu();
            player = GameObject.FindGameObjectWithTag("Player");
            world = GameObject.FindGameObjectWithTag("World");
        } else { 
            PopulateMenu();
            if(PlayerPrefs.HasKey("token")){
                AuthorizedView();
            }
            anim = GetComponent<Animation>();
        }
    }

    public void SetPause(){
        Camera.main.depth = 2;
        Time.timeScale = 0;
        player.GetComponent<CameraMovement>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        world.GetComponent<GameController>().enabled = false;
    }
    public void Resume(){
        Camera.main.depth = 0;
        Time.timeScale = 1;
        player.GetComponent<CameraMovement>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
        world.GetComponent<GameController>().enabled = true;
        GameController.isPause = false;
    }

    void PopulateGameMenu(){
        menuItems = new List<MenuItem>[1];
        menuItems[0] = new List<MenuItem>();
        if(PlayerPrefs.HasKey("token")){
            menuItems[0].Add(new MenuItem(MenuItemType.action, "save_game_global", "SaveGlobalGame"));
        }
        menuItems[0].Add(new MenuItem(MenuItemType.action, "save_game_local", "SaveLocalGame"));
        menuItems[0].Add(new MenuItem(MenuItemType.action, "resume", "Resume"));
        menuItems[0].Add(new MenuItem(MenuItemType.action, "back_to_menu", "BackToMainMenu"));
    }

    public void SaveGlobalGame(){
        string saveData = SaveGame.GetSaveGame();
        int gameID = -1;
        string gameTitle = PlayerPrefs.GetString("GameTitle");
        if(PlayerPrefs.HasKey("GameID")){
            gameID = int.Parse( PlayerPrefs.GetString("GameID") );
        }

        api.SaveGame(PlayerPrefs.GetString("token"), gameID, saveData, gameTitle);
        requestedSaveGame = true;
    }

    public void SaveLocalGame(){
        string saveData = SaveGame.GetSaveGame();
        Debug.Log(saveData);
        string fileDir = saveDir +"/"+ PlayerPrefs.GetString("GameTitle") + ".rih";
        FileStream file = File.Open(fileDir, FileMode.Create);
        file.Dispose();
        File.WriteAllText(fileDir, saveData);
        PlayerPrefs.SetString("GamePath", fileDir);        
    }

    void PopulateMenu() {
        menuItems = new List<MenuItem>[6];
        menuItems[0] = new List<MenuItem>();
        menuItems[0].Add(new MenuItem(MenuItemType.action, "login", "GoToLoginMenu"));
        menuItems[0].Add(new MenuItem(MenuItemType.action, "play_offline", "PlayOffline"));
        menuItems[0].Add(new MenuItem(MenuItemType.action, "exit", "Exit"));

        menuItems[1] = new List<MenuItem>();
        menuItems[1].Add(new MenuItem(MenuItemType.input,"<b>username:</b>"));
        menuItems[1].Add(new MenuItem(MenuItemType.password,"<b>password:</b>"));    
        menuItems[1].Add(new MenuItem(MenuItemType.action, "login", "Login"));    
        menuItems[1].Add(new MenuItem(MenuItemType.action,"back", "GoToMainMenu")); 

        menuItems[2] = new List<MenuItem>();
        menuItems[2].Add(new MenuItem(MenuItemType.action, "global_games", "GetAllGlobalGames"));
        menuItems[2].Add(new MenuItem(MenuItemType.action, "local_games", "GoToLocalGamesMenu"));
        menuItems[2].Add(new MenuItem(MenuItemType.action, "new_game", "GoToNewGamesMenu"));        
        menuItems[2].Add(new MenuItem(MenuItemType.action, "logout", "GoToMainMenu")); 
        menuItems[2].Add(new MenuItem(MenuItemType.action, "exit", "Exit"));

        menuItems[3] = new List<MenuItem>();
        menuItems[3].Add(new MenuItem(MenuItemType.input, "<b>title:</b>"));
        menuItems[3].Add(new MenuItem(MenuItemType.action, "start", "StartNewGame"));        
        menuItems[3].Add(new MenuItem(MenuItemType.action, "back", "GoToGameMenu")); 

    }

    public void BackToMainMenu(){
        Resume();
        SceneManager.LoadScene(0);
    }

    public void Exit() {
        Application.Quit();
    }

    public void Login() {
        api.Login(menuItems[1][0].target, menuItems[1][1].target);
        requestedLogin = true;
    }

    public void StartNewGame(){
        if(menuItems[3][0].target.Length < 4){
            anim.Play();
            return;
        }

        if(PlayerPrefs.HasKey("LoadGame"))
            PlayerPrefs.DeleteKey("LoadGame");
        if(PlayerPrefs.HasKey("GameID"))
            PlayerPrefs.DeleteKey("GameID");
        PlayerPrefs.SetString("GameTitle", menuItems[3][0].target);
        SceneManager.LoadScene(1);
    }

    public void GetAllGlobalGames() {
        if(PlayerPrefs.HasKey("token")){
            api.GetAllGames(PlayerPrefs.GetString("token"));
            requestedGetGames = true;
        }
    }

    public void BuildGlobalGames(string reutrn) {
        menuItems[4] = new List<MenuItem>();
        string[] lines = reutrn.Split('\n');
        if(lines.Length > 1){
            for( int i = 1; i < lines.Length - 1; i++){
                string[] game = lines[i].Split('|');
                if(game.Length > 1){
                    menuItems[4].Add(new MenuItem(MenuItemType.gameGlobal, game[1], game[0])); 
                }
            }
        }
        menuItems[4].Add(new MenuItem(MenuItemType.action, "back", "GoToGameMenu")); 
    }

    public void BuildLocalGames(string[] files){
        menuItems[5] = new List<MenuItem>();
        foreach(string file in files){
            string name = Path.GetFileNameWithoutExtension(file);
            string path = file;
            menuItems[5].Add(new MenuItem(MenuItemType.gameLocal, name, path)); 
        }
        menuItems[5].Add(new MenuItem(MenuItemType.action, "back", "GoToGameMenu")); 
    }

    public void GoToMainMenu(){ ChangeMenuTo(0); }
    public void GoToLoginMenu(){ ChangeMenuTo(1); }
    public void GoToGameMenu(){ ChangeMenuTo(2); }
    public void GoToNewGamesMenu(){ ChangeMenuTo(3); }
    public void GoTGlobalGameMenu(){ ChangeMenuTo(4); }
    public void PlayOffline(){
        PopulateMenu();
        if(PlayerPrefs.HasKey("token"))
            PlayerPrefs.DeleteKey("token");
        menuItems[2][3].value = "back";
        menuItems[2].RemoveAt(0);
        GoToGameMenu();
    }

    private void ChangeMenuTo(int menuID){
        visibleMenu = menuID;
        selectedItem = 0;
    }

    void AuthorizedView(){
PopulateMenu();
                    GoToGameMenu();
    }

	void Update () {
        if(requestedLogin == true){
            if(api.requestResult != null){
                if(api.requestResult.text.Contains("TOKEN: ")){
                    string token = api.requestResult.text.Split(' ')[1];
                    PlayerPrefs.SetString("token", token);
                    AuthorizedView();
                }else{
                    anim.Play();
                }
                requestedLogin = false;
            }
            return;
        }else if(requestedGetGames == true){
            if(api.requestResult != null){
                if(api.requestResult.text.Contains("GAMES: ")){
                    Debug.Log( api.requestResult.text);
                    BuildGlobalGames(api.requestResult.text);
                    GoTGlobalGameMenu();
                }
                requestedGetGames = false;
            }
        }else if (requestedGetGame == true){
            if(api.requestResult != null){
                if(api.requestResult.text.Contains("GAME_SAVE_DATA: ")){
                    string saveData = api.requestResult.text.Substring(16, api.requestResult.text.Length - 16);
                    PlayerPrefs.SetString("LoadGame", saveData);
                    PlayerPrefs.SetString("GameTitle", menuItems[visibleMenu][selectedItem].value);
                    PlayerPrefs.SetString("GameID", menuItems[visibleMenu][selectedItem].target);
                    if(PlayerPrefs.HasKey("GamePath")){
                        PlayerPrefs.DeleteKey("GamePath");
                    }
                    SceneManager.LoadScene(1);
                }
                requestedGetGame = false;
            }
        }else if (requestedSaveGame == true){
            if(api.requestResult != null){
                if(api.requestResult.text.Contains("GAME_ID: ")){
                    string gameID = api.requestResult.text.Substring(9, api.requestResult.text.Length - 9);
                    PlayerPrefs.SetString("GameID", gameID);
                    Debug.Log("saved");
                }
                requestedSaveGame = false;
            }
        }

        if(typeingTarget == null) {
            SelectAction();
        }else{
            InputAction();
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)){
                typeingTarget = null;
            } 
        }
	}

    void SelectAction(){
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
            UpdateSelectedItem(-1);
        }else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
            UpdateSelectedItem(1);
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("enter")){
            MenuItem selectedMenuItem= menuItems[visibleMenu][selectedItem];
            if(selectedMenuItem.type == MenuItemType.action){
                MethodInfo actionMethod = this.GetType().GetMethod(selectedMenuItem.target);
                if(actionMethod != null){
                    actionMethod.Invoke(this, null);
                }
            }else if (selectedMenuItem.type == MenuItemType.input || selectedMenuItem.type == MenuItemType.password){
                typeingTarget = selectedMenuItem;
            }else if (selectedMenuItem.type == MenuItemType.gameGlobal ){
                GetGlobalGame(selectedMenuItem.target);
                requestedGetGame = true;
            }else if(selectedMenuItem.type == MenuItemType.gameLocal ){
                string saveData = File.ReadAllText(selectedMenuItem.target);
                PlayerPrefs.SetString("LoadGame", saveData);
                PlayerPrefs.SetString("GameTitle", selectedMenuItem.value);
                PlayerPrefs.SetString("GamePath", selectedMenuItem.target);
                if(PlayerPrefs.HasKey("GameID")){
                    PlayerPrefs.DeleteKey("GameID");
                }
                SceneManager.LoadScene(1);
            }
        }
    }

    void GetGlobalGame(string id){
        if(PlayerPrefs.HasKey("token")){
            api.GetGame(PlayerPrefs.GetString("token"), id);
            requestedGetGames = true;
        }
    }

    public void GoToLocalGamesMenu(){
        
        string[] myFiles;
        if(Directory.Exists(saveDir)){
            myFiles = Directory.GetFiles(saveDir, "*.rih", SearchOption.AllDirectories);
        }else{
            myFiles = new string[0];
        }
        BuildLocalGames(myFiles);
        ChangeMenuTo(5);
    }

    void InputAction(){
        MenuItem selectedMenuItem= menuItems[visibleMenu][selectedItem];
        foreach (char c in Input.inputString)
        {
            if (c == '\b') { // Delete
                if (selectedMenuItem.target.Length != 0) {
                    selectedMenuItem.target = selectedMenuItem.target.Substring(0, selectedMenuItem.target.Length - 1);
                }
            }
            else if ((c == '\n') || (c == '\r')) // enter/return
            {
                print("User entered their name: " + selectedMenuItem.target);
            }
            else
            {
                selectedMenuItem.target += c;
            }
        }
    }

    void UpdateSelectedItem(float dirr){
        selectedItem += dirr < 0 ? -1 : 1;
        if(selectedItem < 0) selectedItem = menuItems[visibleMenu].Count - 1;
        else if(selectedItem >= menuItems[visibleMenu].Count) selectedItem = 0;
    }

    void OnGUI(){
        if(isGameMenu){
            if(GameController.isPause == false)
                return;
            else
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), mainMenuScreen);
        }

        GUI.skin = skin;
        if(api.requestInProggress){
            LoadingGUI();
        }else{
            MenuGUI();
        }
    }

    float loadingTimer = 1;
    int loadingDots = 0;

    void LoadingGUI(){
        loadingTimer -= Time.deltaTime;
        if(loadingTimer < 0){
            loadingDots ++;
            if(loadingDots > 3 || loadingDots < 0) loadingDots = 0;
            loadingTimer = 1;
        }

        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), "LOADING" + new String('.',loadingDots), "loading");
    }

    void MenuGUI(){
        int h = Screen.height;
        int w = Screen.width;
        
        int i = 0;
        int offsetY = (h - menuTopOffset * menuItems[visibleMenu].Count) / 2 - 70;

        GUI.Label(new Rect(w / 2 - 350 + menuLeftOffset, offsetY, 700, 55), "<color=#000000A3><b>"+((isGameMenu) ? "PAUSE_MENU" : "MAIN_MENU")+":</b></color>");

        foreach(MenuItem value in menuItems[visibleMenu]){
            string output = value.value;
            if(i == selectedItem) output = "> " + output;
            if(value.type == MenuItemType.input || value.type == MenuItemType.password){
                string insertTarger = value.target;
                if(value.type == MenuItemType.password){
                    insertTarger = new String('*', value.target.Length);
                }
                output += " <color=#c2eda3>" + insertTarger + "</color>";

                if(value == typeingTarget){
                    output = " <color=#c2eda3>" + output + "</color>";
                }
            }
            GUI.Label(new Rect(w / 2 - 350 + menuLeftOffset, 60 * (i + 1) + offsetY, 700, 55), output);
            i++;
        }
    }

}
