using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoulettePointResultPopup : Popup
{
    public Text Point;
    public Button OkButton;

    public class RoulettePointResultPopupData : PopupData
    {
        public int Point = 0;

        public RoulettePointResultPopupData(int point)
        {
            PopupType = POPUP_TYPE.ROULETTE_POINT_RESULT;
            Point = point;
        }
    }

    public void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }


    public override void SetData(PopupData data)
    {
        var popupData = data as RoulettePointResultPopupData;

        Point.text = string.Format("{0:n0} Point", popupData.Point);
    }

    public void OnClickOk()
    {
        CloseAction();
    }
}
