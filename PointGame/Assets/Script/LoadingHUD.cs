using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHUD : MonoBehaviour {

    public Canvas HUDCanvas;
    public Image ButonImg;
    public Text Desc;
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

    public void ShowHUD(bool alpha = false)
    {
        //var tempColor = ButonImg.color;
        //tempColor.a = alpha ? 0f : 0.16f;
        //ButonImg.color = tempColor;

        //if (alpha)
        //    Desc.text = "";
        //else
        //    Desc.text = "로딩중..";
    }
}
