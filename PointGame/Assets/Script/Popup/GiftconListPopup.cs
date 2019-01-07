using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftconListPopup : Popup
{
    public Button OkButton;

    public class GiftconListPopupData : PopupData
    {
        public GiftconListPopupData()
        {
            PopupType = POPUP_TYPE.GIFT_CON_LIST;
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
