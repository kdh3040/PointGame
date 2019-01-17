using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LottoPopup : Popup
{
    public Button OkButton;
    public List<LottoSlotUI> LottoSlotList = new List<LottoSlotUI>();
    public Text AllPoint;

    public class LottoPopupData : PopupData
    {
        public LottoPopupData()
        {
            PopupType = POPUP_TYPE.LOTTO;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    private void Update()
    {
        AllPoint.text = string.Format("총 포인트 : {0:n0}", TKManager.Instance.MyData.Point);
    }

    public override void SetData(PopupData data)
    {
        for (int i = 0; i < LottoSlotList.Count; i++)
        {
            LottoSlotList[i].SetData(TKManager.Instance.LottoSeriesCountMin + i);
            LottoSlotList[i].ParentPopup = ParentPopup;
        }
    }

    public void OnClickOk()
    {
        CloseAction();
    }
}
