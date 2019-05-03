using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        FirebaseManager.Instance.LottoPopupRefresh = RefrehUI;
        RefrehUI();
        if (FirebaseManager.Instance.ReviewMode)
            Info.text = "- 1,000P를 사용하여 행운 번호를 뽑아보세요\n- 100명 참여시 행운의 당첨자가 뽑힙니다\n- 2,000포인트의 주인공이 되어보세요\n** 랜덤 번호는 애플과의 관계가 일절 없습니다";
        else
            Info.text = string.Format("- 1,000P를 사용하여 행운 번호를 뽑아보세요\n- 100명 참여시 추첨을 진행합니다\n- 당첨시 {0:n0}P가 지급됩니다", CommonData.LottoWinBonus);
        // 1000P를 사용하여 행운 번호를 뽑아보세요. 100명 참여시 자동 추첨 진행 , 당첨금액은 2000캐시
        //    Info.text = "- 1000포인트를 사용하여 행운번호를 뽑아보세요\n- 하루 최대 4명의 행운의 당첨자가 뽑힙니다\n- 2000포인트의 주인공이 되어보세요\n** 랜덤 번호는 애플과의 관계가 일절 없습니다";
        //else
        //    Info.text = "- 1000포인트를 사용하여 행운번호를 뽑아보세요\n- 일일 9시, 12시, 15시, 18시에 추점을 진행합니다\n- 당첨금액은 20000원 입니다";
    }

    public void OnClickOk()
    {
        FirebaseManager.Instance.LottoPopupRefresh = null;
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }

    public void RefrehUI()
    {
        for (int i = 0; i < LottoSlotList.Count; i++)
        {
            if (TKManager.Instance.LottoSeriesCountMin < 0)
                LottoSlotList[i].SetData(i);
            else
                LottoSlotList[i].SetData(TKManager.Instance.LottoSeriesCountMin + i);

            LottoSlotList[i].ParentPopup = ParentPopup;
        }
    }
}
