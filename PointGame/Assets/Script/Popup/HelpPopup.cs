using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopup : Popup
{
    public Text HelpDesc;
    public Button OkButton;
    private bool FirstOpen = true;

    public class HelpPopupData : PopupData
    {
        public HelpPopupData()
        {
            PopupType = POPUP_TYPE.HELP;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
        FirstOpen = true;
    }

    public override void SetData(PopupData data)
    {
        //var str2 = HelpDesc.text;
        //str2 = str2.Replace("캐시", "포인트");
        //HelpDesc.text = str2;
        //if (FirebaseManager.Instance.ReviewMode)
        //{
        //    var str = HelpDesc.text;
        //    str = str.Replace("캐시", "포인트");
        //    HelpDesc.text = str;
        //}

        if(FirstOpen)
        {
            var str = HelpDesc.text;
            str += string.Format("\n- 1등에겐 {0}캐시 2등에게는 {1}캐시 지급됩니다", FirebaseManager.Instance.FirebaseRPSWinnerPrizeMoney, FirebaseManager.Instance.FirebaseRPSWinnerSecPrizeMoney);
            HelpDesc.text = str;
            FirstOpen = false;
        }
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
