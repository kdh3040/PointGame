﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {

    private int CurrLottoTime;
    public Text NextLottoTime;
    public Text CurrTime;
    public Text Id;

    public Image LottoWinBg;
    public Text LottoWinName;
    public Text LottoWinSeries;

    public CountImgFont AllPoint;
    public Button PointSwap;
    public Button FreePoint;
    public Button SwapPoint;
    public GameObject TopRightButtons;

    public Button LogoButton;
    public Button GamePlayButton;
    public Button PushButton;
    public GameObject PushBoxNotiObj;
    public Button LottoButton;
    public GameObject LottoNotiObj;
    public Button FreeRoulette;
    public Button HelpButton;

    public PopupUI Popup;

    public AudioSource mBGM;
    public AudioClip mClip;

    private void Awake()
    {
        FreePoint.onClick.AddListener(OnClickFreePoint);
        LogoButton.onClick.AddListener(OnClickGamePlay);
        GamePlayButton.onClick.AddListener(OnClickGamePlay);
        PushButton.onClick.AddListener(OnClickPushBox);
        FreePoint.onClick.AddListener(OnClickFreePoint);
        LottoButton.onClick.AddListener(OnClickLotto);
        FreeRoulette.onClick.AddListener(OnClickFreeRoulette);
        PointSwap.onClick.AddListener(OnClickPointSwap);
        SwapPoint.onClick.AddListener(OnClickPointSwap);
        HelpButton.onClick.AddListener(OnClickHelp);
    }

    void Start()
    {
        mBGM.Play();

        Id.text = string.Format("ID : {0}", TKManager.Instance.MyData.NickName);

        CurrLottoTime = DateTime.Now.Hour;

        if (TKManager.Instance.GameOverRouletteStart)
            StartCoroutine(Co_GameOverRouletteStart());

        if(FirebaseManager.Instance.ReviewMode || FirebaseManager.Instance.ExamineMode)
        {
            SwapPoint.gameObject.SetActive(false);
            FreePoint.gameObject.SetActive(false);
            HelpButton.gameObject.SetActive(false);
            LottoButton.gameObject.SetActive(false);
            FreeRoulette.gameObject.SetActive(false);
            PushButton.gameObject.SetActive(false);

            LottoWinSeries.gameObject.SetActive(false);
            LottoWinName.gameObject.SetActive(false);
            NextLottoTime.text = "";

            LottoWinBg.gameObject.SetActive(false);

            RectTransform pointSwapRect = PointSwap.GetComponent<RectTransform>();
            var pos = pointSwapRect.anchoredPosition;
            pos.y = -141f;
            pointSwapRect.anchoredPosition = pos;

            RectTransform topRightButtonsRect = TopRightButtons.GetComponent<RectTransform>();
            pos = topRightButtonsRect.anchoredPosition;
            pos.y = -141f;
            topRightButtonsRect.anchoredPosition = pos;
        }
        else
        {
            SwapPoint.gameObject.SetActive(true);
            FreePoint.gameObject.SetActive(true);
            HelpButton.gameObject.SetActive(true);
            LottoButton.gameObject.SetActive(true);
            FreeRoulette.gameObject.SetActive(true);
            PushButton.gameObject.SetActive(true);

            var winList = TKManager.Instance.LottoWinUserList;
            StringBuilder winCount = new StringBuilder();

            for (int i = 0; i < winList.Count - 1; i++)
            {
                winCount.Append(string.Format("- {0:D2}회 당첨자", winList[i].Key + 1));
                winCount.AppendLine();
            }

            LottoWinSeries.text = winCount.ToString();

            StringBuilder winUser = new StringBuilder();

            for (int i = 0; i < winList.Count - 1; i++)
            {
                winUser.Append(string.Format(" : {0}", winList[i].Value));
                winUser.AppendLine();
            }

            LottoWinName.text = winUser.ToString();

            bool timeView = false;
            for (int i = 0; i < CommonData.LottoWinTime.Length; i++)
            {
                if (CurrLottoTime < CommonData.LottoWinTime[i])
                {
                    NextLottoTime.text = string.Format("다음 추첨시간 {0}:00시", CommonData.LottoWinTime[i]);
                    timeView = true;
                    break;
                }
            }

            if(timeView == false)
                NextLottoTime.text = string.Format("다음 추첨시간 내일 {0}:00시", CommonData.LottoWinTime[0]);
        }

        StartCoroutine(Co_ReviewMode());
    }

    IEnumerator Co_GameOverRouletteStart()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        Popup.ShowPopup(new RoulettePopup.RoulettePopupData());
    }

    IEnumerator Co_ReviewMode()
    {
        yield return null;
#if (UNITY_ANDROID && !UNITY_EDITOR)
        if (FirebaseManager.Instance.ExamineMode)
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData(FirebaseManager.Instance.ExamineContrext, () =>
            {
                Application.Quit();
            }));
        }
#elif (UNITY_ANDROID && UNITY_EDITOR) || UNITY_IOS
        var desc = string.Format("{0}\n{1}", FirebaseManager.Instance.ExamineContrext, "*포인트를 획득 할 수 없습니다.");
        if (FirebaseManager.Instance.ExamineMode)
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData(desc));
        }
#endif



    }
    public void OnClickFreePoint()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        StartCoroutine(Co_IsPlayableAds());

        //if(AdsManager.Instance.IsPlayableAds())
        //{
        //    Popup.ShowPopup(new MiniGamePopup.MiniGamePopupData());
        //}
        //else
        //{
        //    Popup.ShowPopup(new MsgPopup.MsgPopupData("일일 시청 제한으로 인해 미니게임이 불가합니다"));
        //}        
    }
    private IEnumerator Co_IsPlayableAds()
    {
        yield return AdsManager.Instance.Co_IsPlayableAds();

        if (AdsManager.Instance.AdEnable)
        {
            Popup.ShowPopup(new MiniGamePopup.MiniGamePopupData());
        }
        else
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData("일일 시청 제한으로 인해 미니게임이 불가합니다"));
        }
    }


    public void OnClickGamePlay()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void OnClickPushBox()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new PushBoxPopup.PushBoxPopupData());
    }

    public void OnClickLotto()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new LottoPopup.LottoPopupData());
    }

    public void OnClickFreeRoulette()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        AdsManager.Instance.ShowSkipRewardedAd(() =>
        {
            Popup.ShowPopup(new RoulettePopup.RoulettePopupData());
        });
    }

    public void OnClickPointSwap()
    {
        if (FirebaseManager.Instance.ReviewMode)
            return;

        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new PointCashSwapPopup.PointCashSwapPopupData());
    }
    public void OnClickHelp()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new HelpPopup.HelpPopupData());
    }
    

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown("space"))
        {
            UnityEngine.ScreenCapture.CaptureScreenshot("shot.png");
        }
#endif
        if (FirebaseManager.Instance.ReviewMode)
        {
            PushBoxNotiObj.SetActive(false);
            AllPoint.SetValue(string.Format("{0}p", TKManager.Instance.MyData.Point), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);
        }
        else
        {
            PushBoxNotiObj.SetActive(TKManager.Instance.PushNotiEnable);
            AllPoint.SetValue(string.Format("{0}p / {1}c", TKManager.Instance.MyData.Point, TKManager.Instance.MyData.Cash), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);
        }

        if (FirebaseManager.Instance.ReviewMode)
        {
            CurrTime.text = DateTime.Now.ToString("HH:mm");
        }
        else
        {
            for (int i = 0; i < TKManager.Instance.LottoLuckyNumber.Count; i++)
            {
                int seriesCount = TKManager.Instance.LottoLuckyNumber[i].Key;
                if (TKManager.Instance.MyData.LottoResultShowSeriesList.ContainsKey(seriesCount) == false)
                {
                    LottoNotiObj.SetActive(true);
                    break;
                }
                else
                    LottoNotiObj.SetActive(false);
            }

            CurrTime.text = DateTime.Now.ToString("HH:mm");

            int hourTime = DateTime.Now.Hour;

            if (CurrLottoTime != hourTime)
            {
                CurrLottoTime = hourTime;
                bool view = false;
                for (int i = 0; i < CommonData.LottoWinTime.Length; i++)
                {
                    if (i == 0)
                    {
                        if (CurrLottoTime < CommonData.LottoWinTime[i])
                        {
                            view = true;
                            NextLottoTime.text = string.Format("다음 추첨시간 {0}:00시", CommonData.LottoWinTime[i]);
                            break;
                        }
                    }
                    else
                    {
                        if (CurrLottoTime >= CommonData.LottoWinTime[i - 1] && CurrLottoTime < CommonData.LottoWinTime[i])
                        {
                            view = true;
                            NextLottoTime.text = string.Format("다음 추첨시간 {0}:00시", CommonData.LottoWinTime[i]);
                            break;
                        }
                    }
                }

                if (view == false)
                {
                    NextLottoTime.text = string.Format("다음 추첨시간 내일 {0}:00시", CommonData.LottoWinTime[0]);
                }
            }
        }
            


#if UNITY_EDITOR || UNITY_ANDROID
        if (Popup.IsShowPopup(POPUP_TYPE.MSG) == false && Input.GetKeyUp(KeyCode.Escape))
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData("게임을 종료 하시겠습니까?", () =>
            {
                Application.Quit();
            }, MsgPopup.MSGPOPUP_TYPE.TWO));
        }
#endif
    }


    //   public enum MAIN_UI_TYPE
    //   {
    //       MAIN,
    //       READY,
    //       ROULETTE,
    //       LOTTO,
    //       LOTTO_NUMBER_GET,
    //       LOTTO_RESULT
    //   }

    //   public GameObject MainObj;
    //   public Button Main_StartButton;
    //   public Button Main_GiftBoxButton;
    //   public Button Main_SignUpButton;
    //   public Text Main_LottoWinText;

    //   public GameObject ReadyObj;
    //   public Button Ready_GameStartButton;
    //   public Button Ready_AdViewButton;
    //   public Button Ready_PointRewardButton;
    //   public Button Ready_LottoButton;
    //   public Button Temp_LuckyLottoButton;
    //   public Button Ready_GiftBoxButton;

    //   public GameObject RouletteObj;
    //   public Button Roulette_StartButton;
    //   public Image Roulette_Img;

    //   public GameObject LottoObj;
    //   public Text Lotto_AllPointText;
    //   public Button Lotto_GetButton;
    //   public Text Lotto_GetText;
    //   public Button Lotto_ResultButton;
    //   public Text Lotto_ResultText;

    //   public GameObject LottoNumberGetObj;
    //   public Text LottoNumberGet_MyNumber;
    //   public Text LottoNumberGet_AllPointText;
    //   public Button LottoNumberGet_OkButton;

    //   public GameObject LottoResultObj;
    //   public Text LottoResult_MyNumber;
    //   public Text LottoResult_ResultNumber;
    //   public Button LottoResult_OkButton;
    //   public Button LottoResult_WinButton;

    //   public PopupUI Popup;

    //   private MAIN_UI_TYPE UIType = MAIN_UI_TYPE.MAIN;

    //   private void Awake()
    //   {
    //       Main_StartButton.onClick.AddListener(OnClickMainStart);
    //       Main_GiftBoxButton.onClick.AddListener(OnClickMainGiftBox);
    //       Main_SignUpButton.onClick.AddListener(OnClickMainSignUp);
    //       Ready_GameStartButton.onClick.AddListener(OnClickReadyGameStart);
    //       Ready_AdViewButton.onClick.AddListener(OnClickReadyAdView);
    //       Ready_PointRewardButton.onClick.AddListener(OnClickReadyPointReward);
    //       Ready_LottoButton.onClick.AddListener(OnClickReadyLotto);
    //       Ready_GiftBoxButton.onClick.AddListener(OnClickMainGiftBox);
    //       Roulette_StartButton.onClick.AddListener(OnClickRouletteStart);
    //       Lotto_GetButton.onClick.AddListener(OnClickLottoGet);
    //       Lotto_ResultButton.onClick.AddListener(OnClickLottoResult);
    //       LottoNumberGet_OkButton.onClick.AddListener(OnClickLottoOk);
    //       LottoResult_OkButton.onClick.AddListener(OnClickLottoResultOk);
    //       LottoResult_WinButton.onClick.AddListener(OnClickLottoResultWin);

    //       Temp_LuckyLottoButton.onClick.AddListener(() =>
    //       {
    //           TKManager.Instance.Temp_LuckyLotto();
    //       }
    //       );
    //   }

    //   // Use this for initialization
    //   void Start () {
    //       if (TKManager.Instance.GameOverRouletteStart == false)
    //           UIType = MAIN_UI_TYPE.MAIN;
    //       else
    //       {
    //           StartCoroutine(Co_Test());

    //           UIType = MAIN_UI_TYPE.ROULETTE;
    //       }

    //       UpdateUIType();
    //   }
    //   IEnumerator Co_Test()
    //   {
    //       yield return null;
    //       yield return null;
    //       yield return null;
    //       yield return null;
    //       Popup.ShowPopup(new AdsPopup.AdsPopupData());
    //   }

    //   // Update is called once per frame
    //   void Update () {

    //}

    //   public void UpdateUIType()
    //   {
    //       MainObj.SetActive(false);
    //       ReadyObj.SetActive(false);
    //       RouletteObj.SetActive(false);
    //       LottoObj.SetActive(false);
    //       LottoNumberGetObj.SetActive(false);
    //       LottoResultObj.SetActive(false);

    //       switch (UIType)
    //       {
    //           case MAIN_UI_TYPE.MAIN:
    //               ViewMainObj();
    //               break;
    //           case MAIN_UI_TYPE.READY:
    //               ViewReadyObj();
    //               break;
    //           case MAIN_UI_TYPE.ROULETTE:
    //               ViewRouletteObj();
    //               break;
    //           case MAIN_UI_TYPE.LOTTO:
    //               ViewLottoObj();
    //               break;
    //           case MAIN_UI_TYPE.LOTTO_NUMBER_GET:
    //               ViewLottoNumberGetObj();
    //               break;
    //           case MAIN_UI_TYPE.LOTTO_RESULT:
    //               ViewLottoResultObj();
    //               break;
    //           default:
    //               break;
    //       }
    //   }

    //   public void ViewMainObj()
    //   {
    //       MainObj.SetActive(true);
    //   }

    //   public void OnClickMainStart()
    //   {
    //       UIType = MAIN_UI_TYPE.READY;
    //       UpdateUIType();
    //   }

    //   public void OnClickMainGiftBox()
    //   {
    //       Popup.ShowPopup(new GiftconListPopup.GiftconListPopupData());
    //   }

    //   public void OnClickMainSignUp()
    //   {
    //       Popup.ShowPopup(new SignUpPopup.SignUpPopupData());
    //   }

    //   public void ViewReadyObj()
    //   {
    //       ReadyObj.SetActive(true);
    //   }

    //   public void OnClickReadyGameStart()
    //   {
    //       SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    //   }

    //   public void OnClickReadyAdView()
    //   {
    //       Popup.ShowPopup(new AdsPopup.AdsPopupData());

    //       UIType = MAIN_UI_TYPE.ROULETTE;
    //       UpdateUIType();
    //   }

    //   public void OnClickReadyPointReward()
    //   {
    //       // 광고 보면 100포인트
    //       TKManager.Instance.MyData.AddPoint(CommonData.AdsPointReward);

    //       Popup.ShowPopup(new AdsPopup.AdsPopupData());
    //   }

    //   public void OnClickReadyLotto()
    //   {
    //       UIType = MAIN_UI_TYPE.LOTTO;
    //       UpdateUIType();
    //   }

    //   public void ViewRouletteObj()
    //   {
    //       // 기프티콘 갯수 파베에서 확인
    //       RouletteObj.SetActive(true);
    //   }
    //   public void OnClickRouletteStart()
    //   {
    //       // 룰렛 진행 하고

    //       iTween.RotateTo(Roulette_Img.gameObject, iTween.Hash("z", 1800f, "time", 3.5f,"easetype", iTween.EaseType.linear));

    //       StartCoroutine(Co_Test2());
    //   }

    //   IEnumerator Co_Test2()
    //   {
    //       yield return new WaitForSeconds(4f);

    //       var percentValue = Random.Range(0, 101); // 100으로 하면 99까지만 나옴

    //       var roulettePercent = TKManager.Instance.RoulettePercent;

    //       for (int index = 0; index < roulettePercent.Count; ++index)
    //       {
    //           if (index == 0)
    //           {
    //               if (roulettePercent[index].Value >= percentValue)
    //               {
    //                   Popup.ShowPopup(new RouletteResultPopup.RouletteResultPopupData(TKManager.Instance.RouletteGiftconUrl, RouletteResultClose));
    //                   break;
    //               }
    //           }
    //           else if (roulettePercent[index - 1].Value < percentValue &&
    //               roulettePercent[index].Value >= percentValue)
    //           {
    //               Popup.ShowPopup(new RouletteResultPopup.RouletteResultPopupData(roulettePercent[index].Key, RouletteResultClose));
    //               break;
    //           }
    //       }
    //   }



    //   public void RouletteResultClose()
    //   {
    //       UIType = MAIN_UI_TYPE.READY;
    //       UpdateUIType();
    //   }

    //   public void ViewLottoObj()
    //   {
    //       LottoObj.SetActive(true);

    //       Lotto_AllPointText.text = string.Format("총 포인트 : {0:n0}", TKManager.Instance.MyData.Point);

    //       if (TKManager.Instance.MyData.MyLottoSeriesCount != TKManager.Instance.CurrentLottoSeriesCount)
    //           Lotto_GetText.text = string.Format("{0}회 HAPPY BOX\n번호 받기", TKManager.Instance.CurrentLottoSeriesCount);
    //       else
    //           Lotto_GetText.text = string.Format("{0}회 HAPPY BOX\n번호 확인", TKManager.Instance.CurrentLottoSeriesCount);

    //       Lotto_ResultButton.gameObject.SetActive(TKManager.Instance.ResultLottoSeriesCount != 0 && TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.ResultLottoSeriesCount);
    //       Lotto_ResultText.text = string.Format("{0}회 HAPPY BOX\n결과 확인", TKManager.Instance.ResultLottoSeriesCount);
    //   }
    //   public void OnClickLottoGet()
    //   {
    //       Popup.ShowPopup(new AdsPopup.AdsPopupData());

    //       if (TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.CurrentLottoSeriesCount)
    //       {
    //           UIType = MAIN_UI_TYPE.LOTTO_NUMBER_GET;
    //           UpdateUIType();
    //       }
    //       else
    //       { 
    //           if (CommonData.LottoNumberCost > TKManager.Instance.MyData.Point)
    //           {
    //               Popup.ShowPopup(new MsgPopup.MsgPopupData("포인트가 충분하지 않습니다.", LottoGetMsgClose));
    //           }
    //           else
    //           {
    //               Lotto_ResultButton.gameObject.SetActive(false);
    //               TKManager.Instance.Temp_LottoNumberGet();
    //               UIType = MAIN_UI_TYPE.LOTTO_NUMBER_GET;
    //               UpdateUIType();
    //           }
    //       }   
    //   }

    //   public void OnClickLottoResult()
    //   {
    //       if (TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.CurrentLottoSeriesCount)
    //       {
    //           Popup.ShowPopup(new MsgPopup.MsgPopupData("아직 당첨번호가 생성되지 않았습니다.", LottoNumberGetMsgClose));
    //       }
    //       else
    //       {
    //           Popup.ShowPopup(new AdsPopup.AdsPopupData());
    //           UIType = MAIN_UI_TYPE.LOTTO_RESULT;
    //           UpdateUIType();
    //       }
    //   }

    //   public void LottoGetMsgClose()
    //   {
    //       UIType = MAIN_UI_TYPE.READY;
    //       UpdateUIType();
    //   }

    //   public void ViewLottoNumberGetObj()
    //   {
    //       LottoNumberGet_MyNumber.text = string.Format("{0}회 Happy 번호\n{1}", TKManager.Instance.MyData.MyLottoSeriesCount, TKManager.Instance.MyData.MyLottoNumber);
    //       LottoNumberGetObj.SetActive(true);
    //   }
    //   public void OnClickLottoOk()
    //   {
    //       UIType = MAIN_UI_TYPE.READY;
    //       UpdateUIType();
    //   }

    //   public void LottoNumberGetMsgClose()
    //   {
    //       UIType = MAIN_UI_TYPE.READY;
    //       UpdateUIType();
    //   }

    //   public void ViewLottoResultObj()
    //   {
    //       LottoResult_MyNumber.text = string.Format("{0}회 Happy 번호\n{1}", TKManager.Instance.MyData.MyLottoSeriesCount, TKManager.Instance.MyData.MyLottoNumber);
    //       LottoResult_ResultNumber.text = string.Format("{0}회 Happy 당첨번호\n{1}", TKManager.Instance.ResultLottoSeriesCount, TKManager.Instance.ResultLottoNumber);

    //       if(TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.ResultLottoSeriesCount &&
    //           TKManager.Instance.MyData.MyLottoNumber == TKManager.Instance.ResultLottoNumber)
    //       {
    //           Popup.ShowPopup(new MsgPopup.MsgPopupData("당첨을 축하 드립니다!"));
    //       }
    //       else
    //       {
    //           Popup.ShowPopup(new MsgPopup.MsgPopupData("다음 기회에.."));
    //       }

    //       if (TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.ResultLottoSeriesCount &&
    //           TKManager.Instance.MyData.MyLottoNumber == TKManager.Instance.ResultLottoNumber)
    //       {
    //           LottoResult_WinButton.gameObject.SetActive(true);
    //       }
    //       else
    //           LottoResult_WinButton.gameObject.SetActive(false);

    //       LottoResultObj.SetActive(true);
    //   }
    //   public void OnClickLottoResultOk()
    //   {
    //       UIType = MAIN_UI_TYPE.READY;
    //       UpdateUIType();
    //   }

    //   public void OnClickLottoResultWin()
    //   {
    //       Popup.ShowPopup(new LottoWinPopup.LottoWinPopupData());
    //   }
}