using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteResultPopup : Popup
{
    public Button OkButton;

    public class RouletteResultPopupData : PopupData
    {
        public RouletteResultPopupData()
        {
            PopupType = POPUP_TYPE.ROULETTE_RESULT;
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SetData(PopupData data)
    {
    }
}
