using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgPopup : Popup
{
    public Text Msg;
    public Button OkButton;

    public class MsgPopupData : PopupData
    {
        public string Msg;

        public MsgPopupData(string msg)
        {
            PopupType = POPUP_TYPE.MSG;
            Msg = msg;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    public override void SetData(PopupData data)
    {
        MsgPopupData popupData = data as MsgPopupData;
        if (popupData == null)
            return;

        SetMsg(popupData.Msg);
    }

    private void SetMsg(string msg)
    {
        Msg.text = msg;
    }

    public void OnClickOk()
    {
        CloseAction();
        ParentPopup.ShowPopup(new MsgPopupData("우왕굳222222"));
    }

}
