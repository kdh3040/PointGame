using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingHUD : MonoBehaviour {

    public Canvas HUDCanvas;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        HUDCanvas.sortingOrder = 10;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
