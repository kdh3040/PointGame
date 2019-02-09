using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushMsgPopup : Popup
{
    public Text Title;
    public Text Msg;
    public Button OkButton;

    public class PushMsgPopupData : PopupData
    {
        public string TitleStr;
        public string MsgStr;

        public PushMsgPopupData(string title, string msg)
        {
            PopupType = POPUP_TYPE.PUSH_MSG;
            TitleStr = title;
            MsgStr = msg;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    public override void SetData(PopupData data)
    {
        var popupData = data as PushMsgPopupData;
        Title.text = popupData.TitleStr;
        Msg.text = popupData.MsgStr;
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
