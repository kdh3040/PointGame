using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCashSwapPopup : Popup
{
    public Button PointSwapButton;

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
        PointSwapButton.onClick.AddListener(OnClickPointSwap);
        CashRefundButton.onClick.AddListener(OnClickCashRefund);
        CashRefundInfoOKButton.onClick.AddListener(OnClickCashRefundOK);
        OkButton.onClick.AddListener(OnClickOK);
    }

    public override void SetData(PopupData data)
    {
        CashRefundInfoObj.gameObject.SetActive(false);
    }

    public void OnClickPointSwap()
    {
        if (TKManager.Instance.MyData.Point < 5000)
        {
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("5000포인트 부터 교환이 가능합니다."));
        }
        else
        {
            // TODO 포인트 교환
        }

    }

    public void OnClickCashRefund()
    {
        if(TKManager.Instance.MyData.Cash < 5000)
        {
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("5000캐쉬 부터 환급 가능합니다."));
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
