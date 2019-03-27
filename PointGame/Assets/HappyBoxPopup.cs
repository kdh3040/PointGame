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
        if (FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
        {
            // 이미 참가 완료
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청이 완료되었습니다"));
        }
        else
        {
            if (TKManager.Instance.IsRPSEnterTime())
            {
                if (FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
                {
                    // 이미 참가 완료
                    ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청이 완료되었습니다"));
                }
                else
                {
                    // 가위바위보 참여하쉴?
                    ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청을 하시겠습니까?", () =>
                    {

                        FirebaseManager.Instance.GetPoint(() =>
                        {
                            if (TKManager.Instance.MyData.Point < CommonData.RPSCost)
                            {
                                ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("포인트가 부족합니다"));
                            }
                            else
                            {
                                // 광고 보여주기
                                AdsManager.Instance.ShowSkipRewardedAd(() =>
                                {
                                    ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청이 완료 되었습니다"));
                                    FirebaseManager.Instance.FirebaseRPSGameEnterEnable = true;
                                    FirebaseManager.Instance.EnterRPSGame();
                                    TKManager.Instance.MyData.RemovePoint(CommonData.RPSCost);
                                });
                            }
                        });
                    }, MsgPopup.MSGPOPUP_TYPE.TWO));
                }
            }
            else
            {
                // TODO 가위바위보에 참가 불가능 또는 가위바위보 진행중
                ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("현재 참여 할 수 없습니다"));
            }
        }  
    }
}
