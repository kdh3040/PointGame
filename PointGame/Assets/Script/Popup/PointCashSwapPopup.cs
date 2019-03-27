using System.Collections;
using System.Collections.Generic;
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
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData(string.Format("{0} 캐시 부터 교환 가능합니다", CommonData.MinCashChange)));
        }
        else
            CashRefundInfoObj.gameObject.SetActive(true);
    }

    public void OnClickCashRefundOK()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        FirebaseManager.Instance.GetCash(() =>
        {
            FirebaseManager.Instance.SetCashInfo(Name.text.ToString(), Bank.text.ToString(), AccountNumber.text.ToString(), TKManager.Instance.MyData.Cash);

            int tempCash = TKManager.Instance.MyData.Cash;
            int tempPoint = TKManager.Instance.MyData.AllAccumulatePoint;
            tempPoint -= (tempCash / CommonData.PointToCashChangeValue) * CommonData.PointToCashChange;

            TKManager.Instance.MyData.SetAllAccumulatePoint(tempPoint);
            FirebaseManager.Instance.SetTotalAccumPoint(TKManager.Instance.MyData.AllAccumulatePoint);
            TKManager.Instance.MyData.RemoveCash(tempCash);
            CashRefundInfoObj.gameObject.SetActive(true);
            CloseAction();
        });
    }

    public void OnClickOK()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
