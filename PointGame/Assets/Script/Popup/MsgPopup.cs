using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgPopup : Popup
{
    public Text Msg;
    public Button OkButton;
    private Action CloseEndAction;

    public class MsgPopupData : PopupData
    {
        public string Msg;
        public Action CloseEndAction;

        public MsgPopupData(string msg, Action closeEndAction = null)
        {
            PopupType = POPUP_TYPE.MSG;
            Msg = msg;
            CloseEndAction = closeEndAction;
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

        CloseEndAction = popupData.CloseEndAction;
        SetMsg(popupData.Msg);
    }

    private void SetMsg(string msg)
    {
        Msg.text = msg;
    }

    public void OnClickOk()
    {
        CloseAction();
        if (CloseEndAction != null)
            CloseEndAction();
    }

}
