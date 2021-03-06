﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LottoSlotUI : MonoBehaviour {

    [System.NonSerialized]
    public PopupUI ParentPopup;
    public Text LottoSeriesCount;
    public GameObject LottoReadyObj;
    public GameObject LottoReady_MyNumber;
    public Text LottoReady_MyNumberText;
    public Button LottoReady_Button;
    public Text LottoReady_InfoText;

    public GameObject LottoResultObj;
    public GameObject LottoResult_MyNumber;
    public Text LottoResult_MyNumberText;
    public GameObject LottoResult_WinNumber;
    public Text LottoResult_WinNumberText;
    public Button LottoResult_WinButton;
    public Text LottoResult_WinButtonText;
    public Text LottoResult_Continue;

    public GameObject LottoInfo;

    private int SeriesCount = 0;

    private void Awake()
    {
        LottoReady_Button.onClick.AddListener(OnClickNumberPick);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(int seriesCount)
    {
        SeriesCount = seriesCount;
        RefreshUI();
    }

    public void RefreshUI()
    {
        LottoSeriesCount.text = string.Format("{0:n0}회", SeriesCount + 1);

        int lottoWinNumber = 0;
        if (TKManager.Instance.IsLottoLuckyNumber(SeriesCount))
            lottoWinNumber = TKManager.Instance.GetLottoLuckyNumber(SeriesCount).Value;

        int lottoMyNumber = 0;
        if (TKManager.Instance.MyData.LottoList.ContainsKey(SeriesCount))
            lottoMyNumber = TKManager.Instance.MyData.LottoList[SeriesCount];

        LottoReadyObj.gameObject.SetActive(false);
        LottoResultObj.gameObject.SetActive(false);
        LottoInfo.gameObject.SetActive(false);

        if (SeriesCount > TKManager.Instance.CurrLottoSeriesCount)
        {
            // 준비중
            LottoInfo.gameObject.SetActive(true);
        }
        else if (SeriesCount < TKManager.Instance.CurrLottoSeriesCount)
        {
            // 결과 확인
            LottoResultObj.gameObject.SetActive(true);

            LottoResult_MyNumber.gameObject.SetActive(false);
            //LottoResult_MyNumber.gameObject.transform.localPosition = new Vector3(0, 80f, 0);
            LottoResult_WinNumber.gameObject.SetActive(false);
            //LottoResult_WinNumber.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            LottoResult_WinButton.gameObject.SetActive(false);
            LottoResult_WinButton.onClick.RemoveAllListeners();
            LottoResult_WinButtonText.text = "결과 확인";
            LottoResult_Continue.gameObject.SetActive(false);

            if (TKManager.Instance.MyData.LottoResultShowSeriesList.ContainsKey(SeriesCount) == false)
            {
                if (lottoMyNumber == 0)
                {
                    LottoResult_MyNumber.gameObject.SetActive(true);
                    LottoResult_MyNumberText.text = "내번호 : -";
                    LottoResult_WinNumber.gameObject.SetActive(true);
                    //LottoResult_WinNumber.gameObject.transform.localPosition = new Vector3(0, 40, 0);
                    LottoResult_WinNumberText.text = "당첨번호를 확인하세요";
                    LottoResult_WinButton.gameObject.SetActive(true);
                }
                else
                {
                    LottoResult_MyNumber.gameObject.SetActive(true);
                    LottoResult_MyNumberText.text = string.Format("내번호 : {0:D6}", lottoMyNumber);
                    LottoResult_WinNumber.gameObject.SetActive(true);
                    LottoResult_WinNumberText.text = "당첨번호를 확인하세요";
                    LottoResult_WinButton.gameObject.SetActive(true);
                }

                LottoResult_WinButton.onClick.AddListener(OnClickLottoResult);
            }
            else
            {
                if (lottoMyNumber == 0)
                {
                    LottoResult_MyNumber.gameObject.SetActive(true);
                    LottoResult_MyNumberText.text = "내번호 : -";
                    LottoResult_WinNumber.gameObject.SetActive(true);
                    //LottoResult_WinNumber.gameObject.transform.localPosition = new Vector3(0, 40, 0);
                    LottoResult_WinNumberText.text = string.Format("당첨번호 : {0:D6}", lottoWinNumber);
                }
                else
                {
                    LottoResult_MyNumber.gameObject.SetActive(true);
                    LottoResult_MyNumberText.text = string.Format("내번호 : {0:D6}", lottoMyNumber);
                    LottoResult_WinNumber.gameObject.SetActive(true);
                    LottoResult_WinNumberText.text = string.Format("당첨번호 : {0:D6}", lottoWinNumber);
                }

                if(lottoWinNumber != 0 && lottoMyNumber == lottoWinNumber)
                {
                    if(TKManager.Instance.MyData.LottoWinSeriesList.ContainsKey(SeriesCount))
                    {
                        LottoResult_Continue.gameObject.SetActive(true);
                        LottoResult_Continue.text = "당첨금을 수령 하였습니다";
                    }
                    else
                    {
                        LottoResult_WinButton.gameObject.SetActive(true);
                        LottoResult_WinButtonText.text = "당첨금 수령";
                        LottoResult_WinButton.onClick.AddListener(OnClickLottoWin);
                    }
                }
                else
                {
                    LottoResult_Continue.gameObject.SetActive(true);
                    LottoResult_Continue.text = "다음 기회에";
                }
            }
        }
        else if (SeriesCount == TKManager.Instance.CurrLottoSeriesCount)
        {
            // 뽑기 가능
            LottoReadyObj.gameObject.SetActive(true);

            if (lottoMyNumber > 0)
            {
                LottoReady_MyNumberText.text = string.Format("내번호 : {0:D6}", lottoMyNumber);
                LottoReady_Button.gameObject.SetActive(false);
                LottoReady_InfoText.gameObject.SetActive(true);
            }
            else
            {
                LottoReady_MyNumberText.text = "번호를 뽑아주세요";
                LottoReady_Button.gameObject.SetActive(true);
                LottoReady_InfoText.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickNumberPick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        ParentPopup.ShowPopup(new LottoMsgPopup.LottoMsgPopupData(string.Format("{0:n0}포인트로 번호를 뽑으시겠습니까?", CommonData.LottoNumberCost),
            () =>
            {
                FirebaseManager.Instance.GetPoint(() =>
                {
                    if (TKManager.Instance.MyData.Point < CommonData.LottoNumberCost)
                    {
                        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("포인트가 부족합니다"));
                    }
                    else
                    {
                        AdsManager.Instance.ShowSkipRewardedAd(() =>
                        {
                            FirebaseManager.Instance.SetLottoNumber(() =>
                            {
                                TKManager.Instance.MyData.RemovePoint(CommonData.LottoNumberCost);
                                RefreshUI();
                            }, ParentPopup);
                        });
                    }
                });
            }));
        
    }

    /*
     * StartCoroutine(Co_GameOverRouletteStart());
    }

    IEnumerator Co_GameOverRouletteStart()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        Popup.ShowPopup(new RoulettePopup.RoulettePopupData());
    }
     */

    public void OnClickLottoResult()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        AdsManager.Instance.ShowLottoRewardedAd(LottoResultView);
    }
    public void LottoResultView()
    {
        TKManager.Instance.MyData.LottoResultShowSeriesList.Add(SeriesCount, true);
        TKManager.Instance.SaveFile();
        RefreshUI();
    }
    public void OnClickLottoWin()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        if (FirebaseManager.Instance.ReviewMode)
        {
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("당첨을 축하드립니다!\n2000포인트 획득!", () =>
            {
                TKManager.Instance.MyData.AddPoint(2000);
                TKManager.Instance.MyData.LottoWinSeriesList.Add(SeriesCount, true);
                RefreshUI();
            }));
        }
        else
        {
            ParentPopup.ShowPopup(new MsgPopup.MsgPopupData(string.Format("당첨을 축하드립니다!\n{0:n0}P 획득!", CommonData.LottoWinBonus), () =>
            {
                TKManager.Instance.MyData.AddPoint(CommonData.LottoWinBonus);
                TKManager.Instance.MyData.LottoWinSeriesList.Add(SeriesCount, true);
                RefreshUI();
            }));

            //ParentPopup.ShowPopup(new LottoWinPopup.LottoWinPopupData(SeriesCount, () =>
            //{
            //    RefreshUI();
            //}));
        }
        
    }
}
