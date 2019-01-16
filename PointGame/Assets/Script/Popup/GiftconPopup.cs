using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftconPopup : Popup
{
    public Image GiftconImage;
    public Button OkButton;
    public Button DeleteButton;
    public Button NextButton;
    public Button PrevButton;

    private int CurrViewGiftconIndex = 0;
    
    private string Url;
    private int Index;
    private List<KeyValuePair<int, string>> UrlList = new List<KeyValuePair<int, string>>();
    private int CurrViewUrlListIndex = 0;

    public class GiftconPopupData : PopupData
    {
        public string Url;
        public int Index;
        public List<KeyValuePair<int, string>> UrlList = new List<KeyValuePair<int, string>>();
        public bool DeleteEnable = true;

        public GiftconPopupData(int index, string url, bool deleteEnable = true)
        {
            PopupType = POPUP_TYPE.GIFT_CON;
            Index = index;
            Url = url;
            DeleteEnable = deleteEnable;
        }
        public GiftconPopupData(List<KeyValuePair<int, string>> urlList)
        {
            PopupType = POPUP_TYPE.GIFT_CON;
            UrlList = urlList;
        }
    }

    // Use this for initialization
    void Start ()
    {
        OkButton.onClick.AddListener(OnClickOk);
        DeleteButton.onClick.AddListener(OnClickDelete);
        NextButton.onClick.AddListener(OnClickNext);
        PrevButton.onClick.AddListener(OnClickPrev);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SetData(PopupData data)
    {
        var popupData = data as GiftconPopupData;
        if (popupData == null)
            return;

        Index = popupData.Index;
        Url = popupData.Url;
        UrlList = popupData.UrlList;

        CurrViewUrlListIndex = 0;

        DeleteButton.gameObject.SetActive(popupData.DeleteEnable);

        RefreshUI();
    }

    public void RefreshUI()
    {
        NextButton.gameObject.SetActive(false);
        PrevButton.gameObject.SetActive(false);

        if (UrlList.Count == 0)
        {
            CurrViewGiftconIndex = Index;
            ImageCache.Instance.SetImage(Url, GiftconImage);
        }
        else
        {
            if(UrlList.Count > CurrViewUrlListIndex + 1)
                NextButton.gameObject.SetActive(true);
            if(CurrViewUrlListIndex - 1 >= 0)
                PrevButton.gameObject.SetActive(true);

            CurrViewGiftconIndex = UrlList[CurrViewUrlListIndex].Key;
            ImageCache.Instance.SetImage(UrlList[CurrViewUrlListIndex].Value, GiftconImage);
        }
    }

    public void OnClickOk()
    {
        CloseAction();
    }
    public void OnClickDelete()
    {
        RefreshUI();
        TKManager.Instance.MyData.DeleteGiftconData(CurrViewGiftconIndex);
    }

    public void OnClickNext()
    {
        if (CurrViewUrlListIndex >= UrlList.Count)
            return;

        CurrViewUrlListIndex++;
        RefreshUI();
    }

    public void OnClickPrev()
    {
        if (CurrViewUrlListIndex <= 0)
            return;

        CurrViewUrlListIndex--;
        RefreshUI();
    }

    
}
