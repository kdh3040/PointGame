using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoulettePointResultPopup : Popup
{
    public enum POINT_TYPE
    {
        CASH,
        POINT,
    }
    public Text Point;
    public Button OkButton;

    public class RoulettePointResultPopupData : PopupData
    {
        public int Point = 0;
        public POINT_TYPE Type = POINT_TYPE.POINT;

        public RoulettePointResultPopupData(int point, POINT_TYPE type)
        {
            PopupType = POPUP_TYPE.ROULETTE_POINT_RESULT;
            Point = point;
            Type = type;
        }
    }

    public void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }


    public override void SetData(PopupData data)
    {
        var popupData = data as RoulettePointResultPopupData;

        if(popupData.Type == POINT_TYPE.POINT)
        {
            Point.text = string.Format("{0:n0} 포인트\n획득!", popupData.Point);
            TKManager.Instance.MyData.AddPoint(popupData.Point);
        }
        else
        {
            Point.text = string.Format("{0:n0} 캐시\n획득!", popupData.Point);
            TKManager.Instance.MyData.AddCash(popupData.Point);
        }
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
