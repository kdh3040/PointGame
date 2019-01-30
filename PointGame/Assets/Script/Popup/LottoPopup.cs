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
        AllPoint.SetValue(string.Format("{0}p", TKManager.Instance.MyData.Point), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);
    }

    public override void SetData(PopupData data)
    {
        for (int i = 0; i < LottoSlotList.Count; i++)
        {
            if(TKManager.Instance.LottoSeriesCountMin < 0)
                LottoSlotList[i].SetData(i);
            else
                LottoSlotList[i].SetData(TKManager.Instance.LottoSeriesCountMin + i);

            LottoSlotList[i].ParentPopup = ParentPopup;
        }
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
