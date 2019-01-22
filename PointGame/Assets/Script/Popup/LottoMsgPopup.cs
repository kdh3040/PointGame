using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LottoMsgPopup : Popup
{
    public Text Msg;
    public Button OkButton;
    private Action OkEndAction;
    public Button CancelButton;

    public class LottoMsgPopupData : PopupData
    {
        public string Msg;
        public Action OkEndAction;

        public LottoMsgPopupData(string msg, Action okEndAction = null)
        {
            PopupType = POPUP_TYPE.LOTTO_MSG;
            Msg = msg;
            OkEndAction = okEndAction;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
        CancelButton.onClick.AddListener(OnClickCancel);
    }

    public override void SetData(PopupData data)
    {
        LottoMsgPopupData popupData = data as LottoMsgPopupData;
        if (popupData == null)
            return;

        OkEndAction = popupData.OkEndAction;
        SetMsg(popupData.Msg);
    }

    private void SetMsg(string msg)
    {
        Msg.text = msg;
    }

    public void OnClickOk()
    {
        CloseAction();
        if (OkEndAction != null)
            OkEndAction();
    }

    public void OnClickCancel()
    {
        CloseAction();
    }
}