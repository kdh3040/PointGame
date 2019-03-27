using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPGJoinPopup : Popup
{
    public Button OkButton;
    public Button CancelButton;

    public Text Desc;

    public class RPGJoinPopupData : PopupData
    {
        public RPGJoinPopupData()
        {
            PopupType = POPUP_TYPE.RPS_JOIN;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
        CancelButton.onClick.AddListener(OnClickCancel);
    }

    public override void SetData(PopupData data)
    {
        if (FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
        {
            Desc.text = "참가 신청이 완료되었습니다";
            OkButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(false);
        }
        else
        {
            if (TKManager.Instance.IsRPSEnterTime())
            {
                Desc.text = "참가 신청을 하시겠습니까?";
                OkButton.gameObject.SetActive(true);
                CancelButton.gameObject.SetActive(true);
            }
            else
            {
                Desc.text = "현재 참가 신청을 할 수 없습니다";
                OkButton.gameObject.SetActive(true);
                CancelButton.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickOk()
    {
        if (FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
        {
            SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
            CloseAction();
        }
        else
        {
            if (TKManager.Instance.IsRPSEnterTime())
            {
                ParentPopup.ShowPopup(new MsgPopup.MsgPopupData(string.Format("{0:n0}포인트로 참가 신청을 하시겠습니까?", CommonData.RPSCost), () =>
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
                                CloseAction();
                                ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청이 완료 되었습니다"));
                                FirebaseManager.Instance.FirebaseRPSGameEnterEnable = true;
                                FirebaseManager.Instance.EnterRPSGame();
                                TKManager.Instance.MyData.RemovePoint(CommonData.RPSCost);
                            });
                        }
                    });
                }, MsgPopup.MSGPOPUP_TYPE.TWO));
            }
            else
            {
                SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
                CloseAction();
            }
        }
    }

    public void OnClickCancel()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
