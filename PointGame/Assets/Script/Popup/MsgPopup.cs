using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgPopup : Popup
{
    public enum MSGPOPUP_TYPE
    {
        ONE,
        TWO,
    }

    public Text Msg;
    public Button OkButton;
    private Action OkEndAction;
    public Button CancelButton;
    private MSGPOPUP_TYPE MsgPopupType = MSGPOPUP_TYPE.ONE;

    public class MsgPopupData : PopupData
    {
        public string Msg;
        public Action OkEndAction;
        public MSGPOPUP_TYPE MsgPopupType = MSGPOPUP_TYPE.ONE;

        public MsgPopupData(string msg, Action okEndAction = null, MSGPOPUP_TYPE type = MSGPOPUP_TYPE.ONE)
        {
            PopupType = POPUP_TYPE.MSG;
            Msg = msg;
            OkEndAction = okEndAction;
            MsgPopupType = type;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
        CancelButton.onClick.AddListener(OnClickCancel);
    }

    public override void SetData(PopupData data)
    {
        MsgPopupData popupData = data as MsgPopupData;
        if (popupData == null)
            return;

        MsgPopupType = popupData.MsgPopupType;

        OkButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);

        switch (MsgPopupType)
        {
            case MSGPOPUP_TYPE.ONE:
                OkButton.gameObject.SetActive(true);
                break;
            case MSGPOPUP_TYPE.TWO:
                OkButton.gameObject.SetActive(true);
                CancelButton.gameObject.SetActive(true);
                break;
            default:
                break;
        }

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
