using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class API : MonoBehaviour {
    private static string API_URL = "localhost:80/NotusCoreWEB/api";
    private static string TOKEN = "";
	void Start () {
        Login("admin", "1234567890");
	}

    void LoadToken(){

    }

    void SaveToken(){
        
    }

    void Login(string username, string password){
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["username"] = username;
        fields["password"] = password;

        string action = "login";
        WWWForm form = GetForm(action, fields);
        StartCoroutine(LoginCoroutine(form));
    }

    IEnumerator LoginCoroutine(WWWForm form){
        /*#########################################*/
        /*#*/ WWW www = new WWW(API_URL, form); /*#*/
        /*#*/ yield return www;                 /*#*/
        /*#########################################*/

        //Do login stuff here
        if(www.error != null) {
            Debug.Log(www.error);
        }
        else {
            if(www.text.Contains("ERROR:")){

            }else{
                TOKEN = www.text;
                Debug.Log(TOKEN);
            }
        }
    }

    void GetGames(string token){
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["key"] = token;
        string action = "get_games";
        WWWForm form = GetForm(action, fields);
        StartCoroutine(GetGamesCoroutine(form));
    }

    IEnumerator GetGamesCoroutine(WWWForm form){
        /*#########################################*/
        /*#*/ WWW www = new WWW(API_URL, form); /*#*/
        /*#*/ yield return www;                 /*#*/
        /*#########################################*/

        //Do login stuff here
        if(www.error != null) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Form upload complete!");
            Debug.Log(www.text);
        }
    }

    void GetGame(string token, int gameID){
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["key"] = token;
        fields["game_id"] = gameID.ToString();
        string action = "get_game";
        WWWForm form = GetForm(action, fields);
        StartCoroutine(GetGameCoroutine(form));
    }

    IEnumerator GetGameCoroutine(WWWForm form){
        /*#########################################*/
        /*#*/ WWW www = new WWW(API_URL, form); /*#*/
        /*#*/ yield return www;                 /*#*/
        /*#########################################*/

        //Do login stuff here
        if(www.error != null) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Form upload complete!");
            Debug.Log(www.text);
        }
    }

	
	WWWForm GetForm (string action, Dictionary<string, string> fields) {
        // Set ups form
        WWWForm form = new WWWForm();
        form.AddField("action", action);                        
        foreach(KeyValuePair<string, string> field in fields){
            form.AddField(field.Key, field.Value);
        }
        return form;

	}
}