using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class API : MonoBehaviour {
    private static string API_URL = "localhost:80/NotusCoreWEB/api";
    public bool requestInProggress = false;
    public WWW requestResult;


    public void Login(string username, string password){
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["username"] = username;
        fields["password"] = password;

        string action = "login";
        StartCoroutine(APICoroutine(action, fields));
    }

    public void GetAllGames(string token){
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["key"] = token;

        string action = "load_all_games";
        StartCoroutine(APICoroutine(action, fields));
    }

    public void GetGame(string token, string gameID){
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["key"] = token;
        fields["game_id"] = gameID;
        
        string action = "load_game";
        StartCoroutine(APICoroutine(action, fields));
    }

    public void SaveGame(string token, int gameID, string content, string title){
        Dictionary<string, string> fields = new Dictionary<string, string>();
        fields["key"] = token;
        fields["save_data"] = content;
        fields["title"] = title;
        if(gameID > 0){
            fields["game_id"] = gameID.ToString();
        }
        
        string action = "save_game";
        StartCoroutine(APICoroutine(action, fields));
    }


    IEnumerator APICoroutine(string action, Dictionary<string,string> fields){
        requestInProggress = true;
        requestResult = null;
        WWWForm form = GetForm(action, fields);

        /*#########################################*/
        /*#*/ WWW www = new WWW(API_URL, form); /*#*/
        /*#*/ yield return www;                 /*#*/
        /*#########################################*/

        requestInProggress = false;
        requestResult = www;
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
