using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LottoWinPopup : Popup
{
    public InputField Name;
    public InputField Bank;
    public InputField AccountNumber;
    public Button OkButton;

    public class LottoWinPopupData : PopupData
    {
        public LottoWinPopupData()
        {
            PopupType = POPUP_TYPE.LOTTO_WIN_INFO;
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
        // TODO 김도형 파베로 데이터 넘기기
        CloseAction();
    }
}
