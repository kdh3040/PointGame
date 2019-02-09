using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LottoWinPopup : Popup
{
    public InputField Name;
    public InputField Bank;
    public InputField AccountNumber;
    public Button OkButton;
    public Button CancelButton;

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
        CancelButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
            CloseAction();
        });
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

        StringBuilder msg = new StringBuilder();
        msg.AppendLine(string.Format("이름 : {0}", Name.text.ToString()));
        msg.AppendLine(string.Format("은행 : {0}", Bank.text.ToString()));
        msg.AppendLine(string.Format("계좌번호 : {0}", AccountNumber.text.ToString()));
        msg.AppendLine("위 정보로 당첨금을 수령 하시겠습니까?");
        msg.Append("* 지급 정보는 1회만 입력 가능합니다.");

        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData(msg.ToString(), () =>
        {
            FirebaseManager.Instance.SetLottoWinUserData(LottoSeriesCount, Name.text.ToString(), Bank.text.ToString(), AccountNumber.text.ToString());

            // TODO 김도형 파베로 데이터 넘기기
            if (EndAction != null)
                EndAction();

            CloseAction();

            TKManager.Instance.MyData.LottoWinSeriesList.Add(LottoSeriesCount, true);
        }, MsgPopup.MSGPOPUP_TYPE.TWO, TextAnchor.MiddleLeft));
    }
}
