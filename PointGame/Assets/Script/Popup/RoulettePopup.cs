using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoulettePopup : Popup
{
    public Button StartButton;
    public Button OkButton;

    public class RoulettePopupData : PopupData
    {
        public RoulettePopupData()
        {
            PopupType = POPUP_TYPE.LOTTO;
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
