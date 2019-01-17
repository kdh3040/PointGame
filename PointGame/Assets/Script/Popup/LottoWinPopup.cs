using System;
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

    private Action EndAction = null;

    public class LottoWinPopupData : PopupData
    {
        public Action EndAction = null;

        public LottoWinPopupData(Action endAction)
        {
            PopupType = POPUP_TYPE.LOTTO_WIN_INFO;
            EndAction = endAction;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    public override void SetData(PopupData data)
    {
        var popupData = data as LottoWinPopupData;
        if (popupData == null)
            return;

        EndAction = popupData.EndAction;
    }

    public void OnClickOk()
    {
        // TODO 김도형 파베로 데이터 넘기기
        if (EndAction != null)
            EndAction();

        CloseAction();
    }
}
