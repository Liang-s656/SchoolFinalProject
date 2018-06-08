using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeFix : MonoBehaviour {

	void Start () {
        int w = Screen.width;
        int h = Screen.height;

        Camera c = this.GetComponent<Camera>();
        c.pixelRect = new Rect(Screen.width - 140, Screen.height - 140, 120, 120);
	}

}
