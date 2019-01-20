using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LottoPopup : Popup
{
    public Button OkButton;
    public List<LottoSlotUI> LottoSlotList = new List<LottoSlotUI>();
    public CountImgFont AllPoint;

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
        AllPoint.SetValue(string.Format("{0:n0}", TKManager.Instance.MyData.Point), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);
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
