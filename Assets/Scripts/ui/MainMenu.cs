using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class MainMenu : MonoBehaviour {

	private SortedList<string, string>[] menuItems;
    private int selectedItem = 0;
    private float clickTimer = 0;
    private int visibleMenu = 0;
    private bool isTiping = false;

    public GUISkin skin;

    private void Start() {
        menuItems = new SortedList<string, string>[3];
        menuItems[0] = new SortedList<string, string>();
        menuItems[0].Add("login", "Login");
        menuItems[0].Add("play_offline", "Play");    
        menuItems[0].Add("exit", "Exit");       

        menuItems[1] = new SortedList<string, string>();
        menuItems[1].Add("username:", "EnterUsername");
        menuItems[1].Add("password:", "EnterPassword");    
        menuItems[1].Add("login", "Login");    
        menuItems[1].Add("back", "ReturnToMain"); 
    }

    public void Exit() {
        Application.Quit();
    }

    public void Login() {
        ChangeMenuTo(1);
    }

    public void ReturnToMain(){
        ChangeMenuTo(0);
    }

    private void ChangeMenuTo(int menuID){
        visibleMenu = menuID;
        selectedItem = 0;
    }

	void Update () {
        if(!isTiping) {
            if(Input.GetAxis("Vertical") != 0){
                clickTimer -= Time.deltaTime;
                if(clickTimer < 0){            
                    UpdateSelectedItem(-Input.GetAxis("Vertical"));
                    clickTimer = 10f;
                }
            }else{
                clickTimer = 0;
            }

            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("enter")){
                MethodInfo actionMethod = this.GetType().GetMethod(menuItems[visibleMenu].ElementAt(selectedItem).Value);
                if(actionMethod != null){
                    actionMethod.Invoke(this, null);
                }
            }
        }
	}

    void UpdateSelectedItem(float dirr){
        selectedItem += dirr < 0 ? -1 : 1;
        if(selectedItem < 0) selectedItem = menuItems[visibleMenu].Count - 1;
        else if(selectedItem >= menuItems[visibleMenu].Count) selectedItem = 0;
    }

    void OnGUI(){
        GUI.skin = skin;
        int h = Screen.height;

        int i = 0;
        int offsetY = (h - 60 * menuItems[visibleMenu].Count) / 2;

        foreach(KeyValuePair<string, string> value in menuItems[visibleMenu]){
            string output = value.Key;
            if(i == selectedItem) output = ">" + output;
            GUI.Label(new Rect(20, 60 * i + offsetY, 500, 55), output);
            i++;
        }
    }
}
