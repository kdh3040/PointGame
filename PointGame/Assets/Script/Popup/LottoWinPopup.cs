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

    private int LottoSeriesCount = 0;
    private Action EndAction = null;

    public class LottoWinPopupData : PopupData
    {
        public Action EndAction = null;
        public int LottoSeriesCount = 0;

        public LottoWinPopupData(int lottoSeriesCount, Action endAction)
        {
            PopupType = POPUP_TYPE.LOTTO_WIN_INFO;
            EndAction = endAction;
            LottoSeriesCount = lottoSeriesCount;
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
        LottoSeriesCount = popupData.LottoSeriesCount;
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        FirebaseManager.Instance.SetLottoWinUserData(LottoSeriesCount, Name.text.ToString(), Bank.text.ToString(), AccountNumber.text.ToString());

        // TODO 김도형 파베로 데이터 넘기기
        if (EndAction != null)
            EndAction();

        CloseAction();
    }
}
