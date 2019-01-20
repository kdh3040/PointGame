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
    public Text EmptyText;

    private int CurrViewGiftconIndex = 0;
    
    private string Url;
    private int Index;
    private List<KeyValuePair<int, string>> UrlList = new List<KeyValuePair<int, string>>();
    private bool MyGiftconList = false;
    private int CurrViewUrlListIndex = 0;
    

    public class GiftconPopupData : PopupData
    {
        public string Url;
        public int Index;
        public bool DeleteEnable = true;
        public bool MyGiftconList = false;

        public GiftconPopupData(int index, string url, bool deleteEnable = true)
        {
            PopupType = POPUP_TYPE.GIFT_CON;
            Index = index;
            Url = url;
            DeleteEnable = deleteEnable;
            MyGiftconList = false;
        }
        public GiftconPopupData()
        {
            PopupType = POPUP_TYPE.GIFT_CON;
            MyGiftconList = true;
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

        MyGiftconList = popupData.MyGiftconList;
        CurrViewUrlListIndex = 0;

        DeleteButton.gameObject.SetActive(popupData.DeleteEnable);

        RefreshUI();
    }

    public void RefreshUI()
    {
        UrlList = TKManager.Instance.MyData.GiftconURLList;
        NextButton.gameObject.SetActive(false);
        PrevButton.gameObject.SetActive(false);
        EmptyText.gameObject.SetActive(false);
        GiftconImage.gameObject.SetActive(false);

        if (MyGiftconList == false)
        {
            GiftconImage.gameObject.SetActive(true);
            CurrViewGiftconIndex = Index;
            ImageCache.Instance.SetImage(Url, GiftconImage);
        }
        else
        {
            if(UrlList.Count == 0)
            {
                GiftconImage.gameObject.SetActive(false);
                EmptyText.gameObject.SetActive(true);
                DeleteButton.gameObject.SetActive(false);
            }
            else
            {
                GiftconImage.gameObject.SetActive(true);
                if (UrlList.Count > CurrViewUrlListIndex + 1)
                    NextButton.gameObject.SetActive(true);
                if (CurrViewUrlListIndex - 1 >= 0)
                    PrevButton.gameObject.SetActive(true);

                CurrViewGiftconIndex = UrlList[CurrViewUrlListIndex].Key;
                ImageCache.Instance.SetImage(UrlList[CurrViewUrlListIndex].Value, GiftconImage);
            }
            
        }
    }

    public void OnClickOk()
    {
        CloseAction();
    }
    public void OnClickDelete()
    {
        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("기프티콘을 삭제 하시겠습니까?",
            () =>
            {
                CurrViewUrlListIndex -= 1;
                if (CurrViewUrlListIndex < 0)
                    CurrViewUrlListIndex = 0;

                TKManager.Instance.MyData.DeleteGiftconData(CurrViewGiftconIndex);
                FirebaseManager.Instance.DelGiftImage(CurrViewGiftconIndex);
                RefreshUI();
            },
            MsgPopup.MSGPOPUP_TYPE.TWO));
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
