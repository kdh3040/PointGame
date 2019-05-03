using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PointCashSwapPopup : Popup
{
    public Text TodayAccumulatePoint;
    public Text AllAccumulatePoint;
    public Text Cash;

    public Button CashRefundButton;
    public GameObject CashRefundInfoObj;
    public InputField Name;
    public InputField Bank;
    public InputField AccountNumber;
    public Button CashRefundInfoOKButton;
    public Button CashRefundInfoCancelButton;

    public Button OkButton;

    public class PointCashSwapPopupData : PopupData
    {
        public PointCashSwapPopupData()
        {
            PopupType = POPUP_TYPE.POINT_CASH_SWAP;
        }
    }

    public void Awake()
    {
        CashRefundButton.onClick.AddListener(OnClickCashRefund);
        CashRefundInfoOKButton.onClick.AddListener(OnClickCashRefundOK);
        CashRefundInfoCancelButton.onClick.AddListener(OnClickCashRefundCencel);
        OkButton.onClick.AddListener(OnClickOK);
    }

    public override void SetData(PopupData data)
    {
        CashRefundInfoObj.gameObject.SetActive(false);

        TodayAccumulatePoint.text = string.Format(": {0}P", TKManager.Instance.MyData.TodayAccumulatePoint);
        AllAccumulatePoint.text = string.Format(": {0}P", TKManager.Instance.MyData.AllAccumulatePoint);
        Cash.text = string.Format(": {0}C", TKManager.Instance.MyData.Cash);
    }

    public void OnClickCashRefund()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        if (TKManager.Instance.MyData.Cash < CommonData.MinCashChange)
        {
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData(string.Format("{0} 캐시 부터 {1} 캐시 단위로 교환 가능합니다", CommonData.MinCashChange, CommonData.MinCashChangeUnit)));
        }
        else
            CashRefundInfoObj.gameObject.SetActive(true);
    }

    public void OnClickCashRefundOK()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);

        if(Name.text.ToString() == "" ||
            Bank.text.ToString() == "" ||
            AccountNumber.text.ToString() == "")
        {
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("교환 정보에\n빈칸이 있습니다"));
            return;
        }
        StringBuilder msg = new StringBuilder();
        msg.AppendLine(string.Format("이름 : {0}", Name.text.ToString()));
        msg.AppendLine(string.Format("은행 : {0}", Bank.text.ToString()));
        msg.AppendLine(string.Format("계좌번호 : {0}", AccountNumber.text.ToString()));
        msg.AppendLine("위 정보로 교환금을 수령 하시겠습니까?");
        msg.Append("* 교환 정보는 1회만 입력 가능합니다");

        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData(msg.ToString(), () =>
        {
            FirebaseManager.Instance.GetCash(() =>
            {
                //FirebaseManager.Instance.SetCashInfo(Name.text.ToString(), Bank.text.ToString(), AccountNumber.text.ToString(), TKManager.Instance.MyData.Cash);

                int tempCash = TKManager.Instance.MyData.Cash;
                int minChangeValue = (tempCash - CommonData.MinCashChange) / CommonData.MinCashChangeUnit;
                int refundCash = CommonData.MinCashChange + minChangeValue * CommonData.MinCashChangeUnit;
                if(refundCash < CommonData.MinCashChange)
                {
                    CloseAction();
                    ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("캐시를 교환 할 수 없습니다"));
                    return;
                }

                FirebaseManager.Instance.SetCashInfo(Name.text.ToString(), Bank.text.ToString(), AccountNumber.text.ToString(), refundCash);

                int tempPoint = TKManager.Instance.MyData.AllAccumulatePoint;
                tempPoint -= (refundCash / CommonData.PointToCashChangeValue) * CommonData.PointToCashChange;

                TKManager.Instance.MyData.SetAllAccumulatePoint(tempPoint);
                FirebaseManager.Instance.SetTotalAccumPoint(TKManager.Instance.MyData.AllAccumulatePoint);
                TKManager.Instance.MyData.RemoveCash(refundCash);
                CashRefundInfoObj.gameObject.SetActive(false);
                CloseAction();
            });

        }, MsgPopup.MSGPOPUP_TYPE.TWO, TextAnchor.MiddleLeft));
    }

    public void OnClickCashRefundCencel()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CashRefundInfoObj.gameObject.SetActive(false);
    }

    public void OnClickOK()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
