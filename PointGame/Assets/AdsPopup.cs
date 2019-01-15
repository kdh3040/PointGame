using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsPopup : Popup
{
    public Button OkButton;

    public class AdsPopupData : PopupData
    {
        public AdsPopupData()
        {
            PopupType = POPUP_TYPE.ADS;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    public override void SetData(PopupData data)
    {
    }


    public void OnClickOk()
    {
        CloseAction();
    }


}
