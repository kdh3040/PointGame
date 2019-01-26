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

        TodayAccumulatePoint.text = string.Format(": {0}", TKManager.Instance.MyData.TodayAccumulatePoint);
        AllAccumulatePoint.text = string.Format(": {0}", TKManager.Instance.MyData.AllAccumulatePoint);
        Cash.text = string.Format(": {0}", TKManager.Instance.MyData.Cash);
    }

    public void OnClickCashRefund()
    {
        if(TKManager.Instance.MyData.Cash < CommonData.PointToCashChange)
        {
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData(string.Format("{0} 캐시 부터 교환 가능합니다.", CommonData.PointToCashChange)));
        }
        else
            CashRefundInfoObj.gameObject.SetActive(true);
    }

    public void OnClickCashRefundOK()
    {
        // TODO 캐쉬 교환 ㄱㄱ
        CashRefundInfoObj.gameObject.SetActive(true);
        CloseAction();
    }

    public void OnClickOK()
    {
        CloseAction();
    }
}
