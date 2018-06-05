using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour {

	void Start () {
		StartCoroutine(Upload());
	}
	
	IEnumerator Upload () {

        WWWForm form = new WWWForm();
        form.AddField("username", "admin");
        form.AddField("password", "wxqwrw112");
        form.AddField("action", "load_all_games");                        

        WWW www = new WWW("localhost:8080/NotusCore/api", form);
        yield return www;

        if(www.error != null) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Form upload complete!");
            Debug.Log(www.text);
        }


	}
}
