using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftconPopup : Popup
{
    public Image GiftconImage;
    public Button OkButton;
    public Button DeleteButton;

    private int GiftconIndex = 0;

    public class GiftconPopupData : PopupData
    {
        public string Url;
        public int Index;
        public GiftconPopupData(int index, string url)
        {
            PopupType = POPUP_TYPE.GIFT_CON;
            Index = index;
            Url = url;
        }
    }

    // Use this for initialization
    void Start ()
    {
        OkButton.onClick.AddListener(OnClickOk);
        DeleteButton.onClick.AddListener(OnClickDelete);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SetData(PopupData data)
    {
        var popupData = data as GiftconPopupData;
        if (popupData == null)
            return;
        GiftconIndex = popupData.Index;
        ImageCache.Instance.SetImage(popupData.Url, GiftconImage);
    }

    public void OnClickOk()
    {
        CloseAction();
    }
    public void OnClickDelete()
    {
        CloseAction();
        TKManager.Instance.MyData.DeleteGiftconData(GiftconIndex);

        ParentPopup.GiftconListPopupObj.RefreshUI();
    }
}
