using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPopup : Popup
{
    public InputField NickName;
    public Button OkButton;

    public class SignUpPopupData : PopupData
    {
        public SignUpPopupData()
        {
            PopupType = POPUP_TYPE.SIGN_UP;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOkButton);
    }

    public override void SetData(PopupData data)
    {
    }

    public void OnClickOkButton()
    {
    
        CloseAction();
    }
}
