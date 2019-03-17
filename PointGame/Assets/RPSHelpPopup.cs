using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPSHelpPopup : Popup
{
    public Button OkButton;

    public class RPSHelpPopupData : PopupData
    {
        public RPSHelpPopupData()
        {
            PopupType = POPUP_TYPE.RPS_HELP;
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
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
