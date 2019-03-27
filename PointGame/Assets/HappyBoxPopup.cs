using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappyBoxPopup : Popup
{
    public Button LottoButton;
    public GameObject LottoNotiObj;
    public Button RPSButton;
    public Text RPSButtonText;
    
    public Button OkButton;

    public class HappyBoxPopupData : PopupData
    {
        public HappyBoxPopupData()
        {
            PopupType = POPUP_TYPE.HAPPY_BOX;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
        LottoButton.onClick.AddListener(OnClickLotto);
        RPSButton.onClick.AddListener(OnClickRPS);
    }

    public override void SetData(PopupData data)
    {
        RPSButtonText.text = string.Format("{0}회 가위바위보", FirebaseManager.Instance.FirebaseRPSGameCurSeries + 1);
    }

    private void Update()
    {
        // 로또 노티
        for (int i = TKManager.Instance.LottoSeriesCountMin; i < TKManager.Instance.CurrLottoSeriesCount; i++)
        {
            if (TKManager.Instance.MyData.LottoResultShowSeriesList.ContainsKey(i) == false)
            {
                LottoNotiObj.SetActive(true);
                break;
            }
            else
                LottoNotiObj.SetActive(false);
        }
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }

    public void OnClickLotto()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        ParentPopup.ShowPopup(new LottoPopup.LottoPopupData());
    }
    public void OnClickRPS()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        ParentPopup.ShowPopup(new RPGJoinPopup.RPGJoinPopupData());
    }
}
