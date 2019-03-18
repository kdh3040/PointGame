using System;
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

    //public Image LottoWinBg;
    //public Text LottoWinTitle;
    //public Text LottoWinName;
    //public Text LottoWinSeries;

    public CountImgFont AllPoint;
    public Button PointSwap;
    public Button MiniGameButton;
    public Button AllPointButton;
    public GameObject TopRightButtons;

    public Button RecommenderCodeButton;
    public Button PushButton;
    public GameObject PushBoxNotiObj;
    public Button WinnerButton;
    public Text WinnerButtonText;

    public Button LogoButton;
    public Button GamePlayButton;
    public Button LottoButton;
    public GameObject LottoNotiObj;
    public Button LottoHelpButton;
    public Button FreeRoulette;
    public Button RPSButton;
    public Button RPSHelpButton;

    public PopupUI Popup;

    public AudioSource mBGM;
    public AudioClip mClip;

#if UNITY_ANDROID
    AndroidJavaObject Activity;
#endif

    private void Awake()
    {

#if UNITY_ANDROID
        //AndroidJavaClass jc = new AndroidJavaClass("com.justtreasure.treasureone.MyPluginActivity");
        Activity = new AndroidJavaObject("com.justtreasure.treasureone.MyPluginActivity");
        //Activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif

        MiniGameButton.onClick.AddListener(OnClickFreePoint);
        LogoButton.onClick.AddListener(OnClickGamePlay);
        GamePlayButton.onClick.AddListener(OnClickGamePlay);
        PushButton.onClick.AddListener(OnClickPushBox);
        LottoButton.onClick.AddListener(OnClickLotto);
        FreeRoulette.onClick.AddListener(OnClickFreeRoulette);
        PointSwap.onClick.AddListener(OnClickPointSwap);
        AllPointButton.onClick.AddListener(OnClickPointSwap);
        WinnerButton.onClick.AddListener(OnClickWinnerList);
        RecommenderCodeButton.onClick.AddListener(OnClickRecommenderCodeButton);
        RPSButton.onClick.AddListener(OnClickRPSGame);
        LottoHelpButton.onClick.AddListener(OnClickHelp);
        RPSHelpButton.onClick.AddListener(OnClickRPSHelp);
    }

    void Start()
    {
        mBGM.Play();
        TKManager.Instance.MainUIView = true;
        Id.text = string.Format("ID : {0}", TKManager.Instance.MyData.NickName);

        CurrLottoTime = DateTime.Now.Hour;

        if (TKManager.Instance.GameOverRouletteStart)
            StartCoroutine(Co_GameOverRouletteStart());

        if (FirebaseManager.Instance.FirebaseRPSGamePlayTime < DateTime.Now.Ticks)
        {
            FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
            FirebaseManager.Instance.FirebaseRPSGameEnterEnable = false;
        }

        if (FirebaseManager.Instance.ReviewMode || FirebaseManager.Instance.ExamineMode)
        {
            PointSwap.gameObject.SetActive(false);
            MiniGameButton.gameObject.SetActive(false);
            PushButton.gameObject.SetActive(false);
            LottoButton.gameObject.SetActive(false);
            FreeRoulette.gameObject.SetActive(false);
            RPSButton.gameObject.SetActive(false);
            WinnerButtonText.text = "랭킹";
            NextLottoTime.text = "";

            //LottoWinTitle.text = "랭킹";
            //LottoWinBg.gameObject.SetActive(true);
            //LottoWinSeries.gameObject.SetActive(true);
            //LottoWinName.gameObject.SetActive(true);

            //// 다시 들어 올때 랭킹이 바뀔수 있기때문에 처리
            //List<KeyValuePair<string, int>> rankList = new List<KeyValuePair<string, int>>();
            //rankList.AddRange(TKManager.Instance.ReviewRank);
            //for (int i = 0; i < rankList.Count; i++)
            //{
            //    if (rankList[i].Value <= TKManager.Instance.MyData.Point + TKManager.Instance.ReviewRankPlusScore)
            //    {
            //        rankList.Insert(i, new KeyValuePair<string, int>(TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point + TKManager.Instance.ReviewRankPlusScore));
            //        break;
            //    }
            //}

            //StringBuilder winCount = new StringBuilder();
            //int viewRank = 0;
            //for (int i = 0; i < rankList.Count; i++)
            //{
            //    winCount.Append(string.Format("- {0:D2}위", i + 1));
            //    if(i < rankList.Count - 1)
            //        winCount.AppendLine();

            //    viewRank++;

            //    if (viewRank >= 4)
            //        break;
            //}

            //LottoWinSeries.text = winCount.ToString();

            //StringBuilder winUser = new StringBuilder();
            //viewRank = 0;
            //for (int i = 0; i < rankList.Count; i++)
            //{
            //    winUser.Append(string.Format(" : {0} ({1:n0}점)", rankList[i].Key, rankList[i].Value));
            //    if (i < rankList.Count - 1)
            //        winUser.AppendLine();

            //    viewRank++;

            //    if (viewRank >= 4)
            //        break;
            //}

            //LottoWinName.text = winUser.ToString();

            //RectTransform pointSwapRect = PointSwap.GetComponent<RectTransform>();
            //var pos = pointSwapRect.anchoredPosition;
            //pos.y = -141f;
            //pointSwapRect.anchoredPosition = pos;

            //RectTransform topRightButtonsRect = TopRightButtons.GetComponent<RectTransform>();
            //pos = topRightButtonsRect.anchoredPosition;
            //pos.y = -141f;
            //topRightButtonsRect.anchoredPosition = pos;
        }
        else
        {
            WinnerButtonText.text = "당첨자현황";
            //LottoWinTitle.text = "* 당첨자 현황 *";

            //var winList = TKManager.Instance.LottoWinUserList;
            //StringBuilder winCount = new StringBuilder();

            //for (int i = 0; i < winList.Count - 1; i++)
            //{
            //    winCount.Append(string.Format("- {0:D2}회 당첨자", winList[i].Key + 1));
            //    winCount.AppendLine();
            //}

            //LottoWinSeries.text = winCount.ToString();

            //StringBuilder winUser = new StringBuilder();

            //for (int i = 0; i < winList.Count - 1; i++)
            //{
            //    winUser.Append(string.Format(" : {0}", winList[i].Value));
            //    winUser.AppendLine();
            //}

            //LottoWinName.text = winUser.ToString();

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

            if (TKManager.Instance.RecommendUsers.Count > 0)
            {
                StartCoroutine(Co_RecommendUsers());
            }
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
#if UNITY_ANDROID
        Activity.CallStatic("CheckRooted");
#endif
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


    IEnumerator Co_RecommendUsers()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        if (TKManager.Instance.RecommendUsers.Count > 0)
        {
            var cost = CommonData.RecommendGetCost * TKManager.Instance.RecommendUsers.Count;

            Popup.ShowPopup(new MsgPopup.MsgPopupData(string.Format("{0:n0}추천포인트를\n획득했습니다", cost), () =>
            {
                TKManager.Instance.MyData.AddPoint(cost);
            }));
        }

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
        TKManager.Instance.MainUIView = false;
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

    public void OnClickRPSHelp()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new RPSHelpPopup.RPSHelpPopupData());
    }

    public void OnClickWinnerList()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        if (FirebaseManager.Instance.ReviewMode || FirebaseManager.Instance.ExamineMode)
        {
            Popup.ShowPopup(new RankingPopup.RankingPopupData());
        }
        else
        {
            Popup.ShowPopup(new WinnerListPopup.WinnerListPopupData());
        }
    }
    public void OnClickRecommenderCodeButton()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new RecommenderCodePopup.RecommenderCodePopupData());
    }

    public void OnClickRPSGame()
    {
        if (FirebaseManager.Instance.ReviewMode || FirebaseManager.Instance.ExamineMode)
            return;

        if (FirebaseManager.Instance.FirebaseRPSGameEnterTime < DateTime.Now.Ticks &&
            FirebaseManager.Instance.FirebaseRPSGamePlayTime > DateTime.Now.Ticks &&
            FirebaseManager.Instance.FirebaseRPSGamePlayTime != long.MaxValue)
        {
            if (FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
            {
                // 이미 참가 완료
                Popup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청이 완료되었습니다."));
            }
            else
            {
                // 가위바위보 참여하쉴?
                Popup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청을 하시겠습니까?", () =>
                {

                    FirebaseManager.Instance.GetPoint(() =>
                    {
                        if (TKManager.Instance.MyData.Point < CommonData.RPSCost)
                        {
                            Popup.ShowPopup(new MsgPopup.MsgPopupData("포인트가 부족합니다."));
                        }
                        else
                        {
                            // 광고 보여주기
                            AdsManager.Instance.ShowSkipRewardedAd(() =>
                            {
                                Popup.ShowPopup(new MsgPopup.MsgPopupData("참가 신청이 완료 되었습니다."));
                                FirebaseManager.Instance.FirebaseRPSGameEnterEnable = true;
                                FirebaseManager.Instance.EnterRPSGame();
                            });
                        }
                    });
                }, MsgPopup.MSGPOPUP_TYPE.TWO));
            }
        }
        else
        {
            // TODO 가위바위보에 참가 불가능 또는 가위바위보 진행중
            Popup.ShowPopup(new MsgPopup.MsgPopupData("현재 참여 할 수 없습니다."));
        }
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

        if (FirebaseManager.Instance.FirebaseRPSGameSeries > -1 &&
            FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
        {
            if(Popup.CurrPopupType == POPUP_TYPE.NONE)
            {
                FirebaseManager.Instance.FirebaseRPSGameEnterEnable = false;
                FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
                if (FirebaseManager.Instance.ReviewMode || FirebaseManager.Instance.ExamineMode)
                    return;

                Popup.ShowPopup(new RPSPopup.RPSPopupData());
            }
            else
            {
                FirebaseManager.Instance.FirebaseRPSGameEnterEnable = false;
                FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
            }
            
        }

        //if(FirebaseManager.Instance.FirebaseRPSGameStatus == 1 &&
        //    FirebaseManager.Instance.FirebaseRPSGamePlayTime < DateTime.Now.Ticks &&
        //    FirebaseManager.Instance.FirebaseRPSGameMyRoom >= 0)
        //{
        //    FirebaseManager.Instance.FirebaseRPSGameStatus = 0;
        //    // 가위바위보 시작
        //    // 가위바위보 팝업이 떠져 있으면 리턴
        //}



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

    void CheckRooted(string value)
    {
        if(value == "true")
        {
            // 루팅폰
            Popup.ShowPopup(new MsgPopup.MsgPopupData("비정상적인 방법으로 게임을 실행 하였습니다.", () =>
            {
                Application.Quit();
            }));
        }
    }
    
}