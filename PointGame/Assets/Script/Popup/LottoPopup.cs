using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LottoPopup : Popup
{
    public Button OkButton;
    public List<LottoSlotUI> LottoSlotList = new List<LottoSlotUI>();
    public CountImgFont AllPoint;
    public Text Info;

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
            if (TKManager.Instance.LottoSeriesCountMin < 0)
                LottoSlotList[i].SetData(i);
            else
                LottoSlotList[i].SetData(TKManager.Instance.LottoSeriesCountMin + i);

            LottoSlotList[i].ParentPopup = ParentPopup;
        }

        if (FirebaseManager.Instance.AdsMode > 0)
        {
            Info.text = "- 1000포인트를 사용하여 행운번호를 뽑아보세요.\n- 하루 최대 4명의 행운의 당첨자가 뽑힙니다.";
        }
        else
            Info.text = "- 1000포인트를 사용하여 행운번호를 뽑아보세요.\n- 하루 최대 4명의 행운의 당첨자가 뽑힙니다.\n- 2000포인트의 주인공이 되어보세요~\n** 해피박스는 애플과의 관계가 일절 없습니다";
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
