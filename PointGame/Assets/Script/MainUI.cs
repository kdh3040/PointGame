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

    public GameObject LottoWinnerObj;
    public Text LottoWinnerCount;
    public Text LottoWinnerName;
    public GameObject RPSWinnerObj;
    public Text RPSWinnerCount;
    public Text RPSWinnerName;
    public GameObject RankingObj;
    public Text RankingCount;
    public Text RankingName;

    public CountImgFont AllPoint;
    public Button PointSwap;

    public GridLayoutGroup LeftTopGrid;
    public Button HelpButton;
    public Button RecommenderCodeButton;
    public Button PushButton;
    public GameObject PushBoxNotiObj;

    public Button LogoButton;
    public Button GamePlayButton;

    public Button FreeRoulette;
    public Button HappyBoxButton;
    public GameObject HappyBoxNotiObj;
    public Button MiniGameButton;

    public Button SoundButton;
    public Image SoundImg;
    public Text SoundText;

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
        PointSwap.onClick.AddListener(OnClickPointSwap);
        HelpButton.onClick.AddListener(OnClickHelp);
        RecommenderCodeButton.onClick.AddListener(OnClickRecommenderCodeButton);
        PushButton.onClick.AddListener(OnClickPushBox);
        LogoButton.onClick.AddListener(OnClickGamePlay);
        GamePlayButton.onClick.AddListener(OnClickGamePlay);
        FreeRoulette.onClick.AddListener(OnClickFreeRoulette);
        HappyBoxButton.onClick.AddListener(OnClickHappyBox);
        MiniGameButton.onClick.AddListener(OnClickMiniGame);
        SoundButton.onClick.AddListener(OnClickSoundChange);
    }

    void Start()
    {
        TKManager.Instance.MainUIView = true;
        Id.text = string.Format("ID : {0}", TKManager.Instance.MyData.NickName);

        CurrLottoTime = 0;
        RefreshSound();

        if (TKManager.Instance.GameOverRouletteStart)
            StartCoroutine(Co_GameOverRouletteStart());

        if (FirebaseManager.Instance.ReviewMode || FirebaseManager.Instance.ExamineMode)
        {
            PointSwap.gameObject.SetActive(false);
            HelpButton.gameObject.SetActive(false);
            PushButton.gameObject.SetActive(false);
            FreeRoulette.gameObject.SetActive(false);
            MiniGameButton.gameObject.SetActive(false);
            LeftTopGrid.childAlignment = TextAnchor.MiddleLeft;

            LottoWinnerObj.gameObject.SetActive(false);
            RPSWinnerObj.gameObject.SetActive(false);
            RankingObj.gameObject.SetActive(true);
            RefreshRankWinnerList();

            NextLottoTime.text = "";
            
#if UNITY_IOS
            HappyBoxButton.gameObject.SetActive(false);
#endif
        }
        else
        {
            LottoWinnerObj.gameObject.SetActive(true);
            RPSWinnerObj.gameObject.SetActive(true);
            RankingObj.gameObject.SetActive(false);

            RefreshLottoWinnerList();
            RefreshRPSWinnerList();

            StartCoroutine(Co_RecommendUsers());
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
        var desc = string.Format("{0}\n{1}", FirebaseManager.Instance.ExamineContrext, "*포인트를 획득 할 수 없습니다");
        if (FirebaseManager.Instance.ExamineMode)
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData(desc));
        }
#endif

    }

    IEnumerator Co_RecommendUsers()
    {
        if (TKManager.Instance.RecommendUsers.Count <= 0)
            yield break;

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


    public void OnClickMiniGame()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        StartCoroutine(Co_IsPlayableAds());     
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

    public void OnClickHappyBox()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new HappyBoxPopup.HappyBoxPopupData());
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
    public void OnClickRecommenderCodeButton()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Popup.ShowPopup(new RecommenderCodePopup.RecommenderCodePopupData());
    }

    public void OnClickSoundChange()
    {
        TKManager.Instance.SetSoundMute(!TKManager.Instance.SoundMute);
        RefreshSound();
    }

    public void RefreshSound()
    {
        if (TKManager.Instance.SoundMute)
        {
            SoundImg.sprite = (Sprite)Resources.Load("icon_sound_off", typeof(Sprite));
            SoundText.text = "소리끔";
            mBGM.Stop();
        }
        else
        {
            SoundImg.sprite = (Sprite)Resources.Load("icon_sound_on", typeof(Sprite));
            SoundText.text = "소리켬";
            mBGM.Play();
        }
    }

    public void RefreshLottoWinnerList()
    {
        var winList = new List<KeyValuePair<int, string>>();
        winList.AddRange(TKManager.Instance.LottoWinUserList);
        StringBuilder winCount = new StringBuilder();

        for (int i = winList.Count - 5; i <= winList.Count - 2; i++)
        {
            if (i < 0 || winList.Count <= i)
                continue;

            winCount.Append(string.Format("- {0:D2}회 당첨자", winList[i].Key + 1));
            if(i < winList.Count - 2)
                winCount.AppendLine();
        }

        LottoWinnerCount.text = winCount.ToString();

        StringBuilder winUser = new StringBuilder();

        for (int i = winList.Count - 5; i <= winList.Count - 2; i++)
        {
            if (i < 0 || winList.Count <= i)
                continue;

            winUser.Append(string.Format(" : {0}", winList[i].Value));
            if (i < winList.Count - 2)
                winUser.AppendLine();
        }

        LottoWinnerName.text = winUser.ToString();
    }

    public void RefreshRPSWinnerList()
    {
        var RPSwinList = TKManager.Instance.RPSWinUserList;
        StringBuilder RPSwinCount = new StringBuilder();
        StringBuilder RPSwinUser = new StringBuilder();
        for (int i = RPSwinList.Count - 2; i <= RPSwinList.Count; i++)
        {
            if (i < 0 || RPSwinList.Count <= i)
                continue;

            RPSwinCount.Append(string.Format("- {0:D2}회 1등", RPSwinList[i].Count + 1));
            RPSwinCount.Append(string.Format("- {0:D2}회 2등", RPSwinList[i].Count + 1));
            if(i < RPSwinList.Count)
                RPSwinCount.AppendLine();

            RPSwinUser.Append(string.Format(" : {0}", RPSwinList[i].FirstName));
            RPSwinUser.Append(string.Format(" : {0}", RPSwinList[i].SecondName));
            if (i < RPSwinList.Count)
                RPSwinUser.AppendLine();
        }

        RPSWinnerCount.text = RPSwinCount.ToString();
        RPSWinnerName.text = RPSwinUser.ToString();
    }

    public void RefreshRankWinnerList()
    {
        List<KeyValuePair<string, int>> rankList = new List<KeyValuePair<string, int>>();
        rankList.AddRange(TKManager.Instance.ReviewRank);
        for (int i = 0; i < rankList.Count; i++)
        {
            if (rankList[i].Value <= TKManager.Instance.MyData.Point + TKManager.Instance.ReviewRankPlusScore)
            {
                rankList.Insert(i, new KeyValuePair<string, int>(TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point + TKManager.Instance.ReviewRankPlusScore));
                break;
            }
        }

        StringBuilder winCount = new StringBuilder();
        int viewRank = 0;
        for (int i = 0; i < rankList.Count; i++)
        {
            winCount.Append(string.Format("- {0:D2}위", i + 1));
            if (i < rankList.Count - 1)
                winCount.AppendLine();

            viewRank++;

            if (viewRank >= 4)
                break;
        }

        RankingCount.text = winCount.ToString();

        StringBuilder winUser = new StringBuilder();
        viewRank = 0;
        for (int i = 0; i < rankList.Count; i++)
        {
            winUser.Append(string.Format(" : {0} ({1:n0}점)", rankList[i].Key, rankList[i].Value));
            if (i < rankList.Count - 1)
                winUser.AppendLine();

            viewRank++;

            if (viewRank >= 4)
                break;
        }

        RankingName.text = winUser.ToString();
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
            AllPoint.SetValue(string.Format("{0}p", TKManager.Instance.MyData.Point), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);
        else
            AllPoint.SetValue(string.Format("{0}p / {1}c", TKManager.Instance.MyData.Point, TKManager.Instance.MyData.Cash), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);

        UpdateCurrTime();
        UpdateNoti();
        UpdateNextLottoTime();
        UpdateRPS();

        RefreshLottoWinnerList();
        RefreshRPSWinnerList();


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

    public void UpdateNextLottoTime()
    {
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
    public void UpdateCurrTime()
    {
        CurrTime.text = DateTime.Now.ToString("HH:mm");
    }

    public void UpdateNoti()
    {
        // 로또 노티
        for (int i = TKManager.Instance.LottoSeriesCountMin; i < TKManager.Instance.CurrLottoSeriesCount; i++)
        {
            if (TKManager.Instance.MyData.LottoResultShowSeriesList.ContainsKey(i) == false)
            {
                HappyBoxNotiObj.SetActive(true);
                break;
            }
            else
                HappyBoxNotiObj.SetActive(false);
        }

        PushBoxNotiObj.SetActive(TKManager.Instance.PushNotiEnable);
    }

    public void UpdateRPS()
    {
        if (FirebaseManager.Instance.FirebaseRPSGameSeries > -1 &&
                FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
        {
            if (Popup.CurrPopupType == POPUP_TYPE.NONE)
            {
                FirebaseManager.Instance.FirebaseRPSGameEnterEnable = false;
                if (FirebaseManager.Instance.ReviewMode || FirebaseManager.Instance.ExamineMode)
                    return;

                Popup.ShowPopup(new RPSPopup.RPSPopupData());
            }
            else
            {
                FirebaseManager.Instance.FirebaseRPSGameEnterEnable = false;
            }

        }
    }


    void CheckRooted(string value)
    {
        if(value == "true")
        {
            // 루팅폰
            Popup.ShowPopup(new MsgPopup.MsgPopupData("비정상적인 방법으로 게임을 실행 하였습니다", () =>
            {
                Application.Quit();
            }));
        }
    }
    
}