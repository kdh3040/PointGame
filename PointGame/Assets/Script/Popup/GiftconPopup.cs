using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftconPopup : Popup
{
    public Image GiftconImage;
    public Button OkButton;

    public class GiftconPopupData : PopupData
    {
        public string Url;
        public GiftconPopupData(string url)
        {
            PopupType = POPUP_TYPE.GIFT_CON;
            Url = url;
        }
    }

    // Use this for initialization
    void Start ()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SetData(PopupData data)
    {
        var popupData = data as GiftconPopupData;
        if (popupData == null)
            return;

        ImageCache.Instance.SetImage(popupData.Url, GiftconImage);
    }

    public void OnClickOk()
    {
        CloseAction();
    }
}
