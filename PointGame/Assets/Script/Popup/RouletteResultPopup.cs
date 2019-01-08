using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteResultPopup : Popup
{
    public Image GiftconImg;
    public CountImgFont PointImg;
    public Button OkButton;
    private Action CloseEndAction;

    public class RouletteResultPopupData : PopupData
    {
        public Action CloseEndAction;
        public string GiftconUrl = "";
        public int Point = 0;

        public RouletteResultPopupData(int point, Action closeEndAction)
        {
            PopupType = POPUP_TYPE.ROULETTE_RESULT;
            Point = point;
            CloseEndAction = closeEndAction;
        }

        public RouletteResultPopupData(string giftconUrl, Action closeEndAction)
        {
            PopupType = POPUP_TYPE.ROULETTE_RESULT;
            GiftconUrl = giftconUrl;
            CloseEndAction = closeEndAction;
        }
    }

    public void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }


    public override void SetData(PopupData data)
    {
        var popupData = data as RouletteResultPopupData;

        GiftconImg.gameObject.SetActive(false);
        PointImg.gameObject.SetActive(false);

        if(popupData.Point > 0)
        {
            PointImg.gameObject.SetActive(true);
            PointImg.SetValue(popupData.Point.ToString(), CountImgFont.IMG_RANGE.CENTER, CountImgFont.IMG_TYPE.YELLOW);
        }
        else
        {
            GiftconImg.gameObject.SetActive(true);
            ImageCache.Instance.SetImage(popupData.GiftconUrl, GiftconImg);
        }

        CloseEndAction = popupData.CloseEndAction;
    }

    public void OnClickOk()
    {
        CloseAction();
        CloseEndAction();

    }
}
