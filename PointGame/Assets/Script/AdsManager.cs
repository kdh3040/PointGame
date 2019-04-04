using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour {

    public static AdsManager _instance = null;
    public static AdsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AdsManager>() as AdsManager;
            }
            return _instance;
        }
    }

    public bool AdView = false;
    public bool AdEnable = false;
    public bool AdComplete = false;


    /// ////////////////////////////////////////////////
    /// 유니티 애즈 셋팅
    private BannerView bannerView;

    private const string android_game_id = "3019112";
    private const string ios_game_id = "3019113";

    private const string rewarded_video_id = "rewardedVideo";
    private const string Skip_rewarded_video_id = "SkipAds";
    private const string Lotto_rewarded_video_id = "LottoAds";

    private RewardBasedVideoAd rewardAdmobVideo;
    
    
    /// ////////////////////////////////////////////////
    /// 애드몹
    /// 




    // 광고 종료후 콜백 함수
    private Action AdEndCallFunc = null;

    public void Start()
    {
        DontDestroyOnLoad(this);


#if UNITY_ANDROID
        string appId = "ca-app-pub-7615036525367000~1421003475";
        Advertisement.Initialize(android_game_id);
#elif UNITY_IPHONE
        string appId = "ca-app-pub-7615036525367000~8107038549"; 
        //  string appId = "ca-app-pub-3940256099942544~1458002511";

        Advertisement.Initialize(ios_game_id);
#else
        string appId = "unexpected_platform";
#endif

        ////////////////////////////////////////////////////
        //// 구글 애드몹  광고 초기화
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
        
        this.rewardAdmobVideo = RewardBasedVideoAd.Instance;
        rewardAdmobVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        rewardAdmobVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        rewardAdmobVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        rewardAdmobVideo.OnAdClosed += HandleRewardBasedVideoClosed;        

#if !UNITY_EDITOR
        //this.RequestBanner();
        this.RequestInterstitial();
        this.RequestAdmobVideo();
#endif

    }



    private void RequestAdmobVideo()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7615036525367000/9414508134";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-7615036525367000/2447872970";
            //    string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardAdmobVideo.LoadAd(request, adUnitId);
        
    }

    
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        Debug.Log("!!!!!@ Ad HandleRewardBasedVideoLoaded event received");
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        AdView = false;
        Debug.Log("!!!!!@ Ad HandleRewardBasedVideoFailedToLoad");
        this.RequestAdmobVideo();
       
    }
    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        AdView = false;
        Debug.Log("!!!!!@ Ad " + "HandleRewardBasedVideoClosed");
        this.RequestAdmobVideo();        
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        Debug.Log("!!!!!@ Ad " + "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);

        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);

        AdView = false;
        AdComplete = true;
    }

    private int ConvertPixelsToDP(float pixels)
    {
        return (int)(pixels / (Screen.dpi / 160));
    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7615036525367000/7411696753";


#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-7615036525367000/5129876636";
        //string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        
#else
            string adUnitId = "unexpected_platform";
#endif        
        
        float tempSize = Screen.height / 1280;

#if UNITY_IPHONE
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
#else
        if (tempSize > 0)
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, 0, ConvertPixelsToDP(Screen.height - (tempSize * AdSize.Banner.Height * Screen.dpi / 160)));
        else
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, 0, ConvertPixelsToDP(Screen.height - (AdSize.Banner.Height * Screen.dpi / 160)));        
#endif


        // Create an empty ad request.

        // Load the banner with the request.

        // this.bannerView.LoadAd(request);


        if (FirebaseManager.Instance.ReviewMode == false && FirebaseManager.Instance.ExamineMode == false)
            AdsManager.Instance.ShowBanner();

    }

    public void ShowBanner()
    {
#if UNITY_EDITOR
        return;
#endif
        if (this.bannerView == null)
            return;

        AdRequest request = new AdRequest.Builder().Build();
        this.bannerView.LoadAd(request);
        this.bannerView.Show();
    }

    public void HideBanner()
    {
        this.bannerView.Hide();
    }

    private InterstitialAd interstitial;

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7615036525367000/3352784828";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-7615036525367000/4030038646";
        //   string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        
#else
        string adUnitId = "unexpected_platform";
#endif

        this.interstitial = new InterstitialAd(adUnitId);

        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);
        this.interstitial.OnAdClosed += HandleOnInterstitialAdClosed;
    }

    public void HandleOnInterstitialAdClosed(object sender, EventArgs args)
    {
        print("HandleOnInterstitialAdClosed event received.");

        this.interstitial.Destroy();

        RequestInterstitial();
    }

    // 스테이지  구글 애드몹 전면 광고
    public void ShowInterstitialAds()
    {
#if UNITY_EDITOR
        AdView = false;
        return;
#endif
        if (FirebaseManager.Instance.ReviewMode)
        {
            AdView = false;
        }
        else
        {
            
            if (this.interstitial == null)
            {
                AdComplete = true;
                AdView = false;
                return;
            }
                

            if (!this.interstitial.IsLoaded())
            {
                RequestInterstitial();
                return;
            }
            AdComplete = true;
            AdView = false;
            this.interstitial.Show();            
        }        
    }

    // 구글 애드몹 리워드 비디오 (스킵 가능)
    // 표시되는 부분
    // 1. 본 게임 3스테이지마다
    // 2. 본 게임 재시작 할때
    // 3. 룰렛 진입시
    private void ShowAdmobVideo()
    {
#if UNITY_EDITOR
        AdComplete = true;
        AdView = false;
        return;
#endif

        if (this.rewardAdmobVideo == null)
        {
            AdComplete = true;
            AdView = false;
            return;
        }

        if (rewardAdmobVideo.IsLoaded())
        {
            rewardAdmobVideo.Show();
        }
        else
        {
            ShowInterstitialAds();
        }

        //RequestAdmobVideo();
    }


    // 유니티애즈 미니게임 리워드 비디오 스킵 불가
    public void ShowMiniGameRewardAd(Action endAction)
    {
        //ShowAdColonyAds();
        //return;

        if (FirebaseManager.Instance.ReviewMode)
        {
            AdComplete = true;
            SetAdEndCallFunc(endAction);
            AdView = false;
        }

        else
        {
            AdComplete = false;
            SetAdEndCallFunc(endAction);

            if (Advertisement.IsReady(rewarded_video_id))
            {
                var options = new ShowOptions { resultCallback = HandleShowRewardVideoResult };
                Advertisement.Show(rewarded_video_id, options);
            }
            else if (rewardAdmobVideo.IsLoaded())
            {
                ShowAdmobVideo();
            }
            else
            {
                AdComplete = true;
                AdView = false; 
                ShowInterstitialAds();
            }
        }
    }

    
    private void HandleShowRewardVideoResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("The ad was successfully shown.");
                    AdView = false;
                    AdComplete = true;
                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");
                    AdView = false;
                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");
                    AdView = false;
                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }
    
    // 유니티애즈 로또복권 확인 리워드 광고 스킵불가
    public void ShowLottoRewardedAd(Action endAction)
    {
        if (FirebaseManager.Instance.ReviewMode)
        {
            SetAdEndCallFunc(endAction);
            AdView = false;
        }
        else
        {
            SetAdEndCallFunc(endAction);
            if (rewardAdmobVideo.IsLoaded())
            {
                this.ShowAdmobVideo();
            }
            else
            {
                AdView = false;
                ShowInterstitialAds();
            }

            //SetAdEndCallFunc(endAction);
            //if (Advertisement.IsReady(Lotto_rewarded_video_id))
            //{
            //    var options = new ShowOptions { resultCallback = HandleShowLottoRewardVideoResult };
            //    Advertisement.Show(Lotto_rewarded_video_id, options);
            //}
            //else if(Vungle.isAdvertAvailable(Vungle_adsID))
            //{
            //    ShowVungleAds();
            //}
            //else
            //{
            //    AdColony.Zone adcolonyZone = AdColony.Ads.GetZone(this.zoneId);
            //    if (adcolonyZone != null || adcolonyZone.Enabled)
            //    {
            //        ShowColonyAds();
            //    }
            //    else
            //        AdView = false;

            //}
        }
    }
    
    private void HandleShowLottoRewardVideoResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("The ad was successfully shown.");
                    AdView = false;


                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");
                    AdView = false;
                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");
                    AdView = false;
                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }

    // 구글애드몹 리워드 광고 스킵가능 
    public void ShowSkipRewardedAd(Action endAction)
    {
        if (FirebaseManager.Instance.ReviewMode)
        {
            SetAdEndCallFunc(endAction);
            AdView = false;
        }
        else
        {
            SetAdEndCallFunc(endAction);
            if (rewardAdmobVideo.IsLoaded())
            {
                this.ShowAdmobVideo();
            }
            else
            {
                AdView = false;
                ShowInterstitialAds();
            }

            /*
            else if (Advertisement.IsReady(Skip_rewarded_video_id))
            {
                var options = new ShowOptions { resultCallback = HandleShowSkipRewardVideoResult };
                Advertisement.Show(Skip_rewarded_video_id, options);
            }
            else if (Vungle.isAdvertAvailable(Vungle_adsID))
            {
                ShowVungleAds();
            }
            else
            {
                AdColony.Zone adcolonyZone = AdColony.Ads.GetZone(this.zoneId);
                if(adcolonyZone.Enabled)
                {
                    ShowColonyAds();
                }                
            }
            */
        }
    }
    
    private void HandleShowSkipRewardVideoResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("The ad was successfully shown.");
                    AdView = false;
                    //TKManager.Instance.MyData.AddPoint(CommonData.AdsPointReward);
                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");
                    AdView = false;
                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");
                    AdView = false;
                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }

    public void SetAdEndCallFunc(Action func)
    {
        AdView = true;
        AdEndCallFunc = func;
        StartCoroutine(Co_AdEndCall());
    }

    public void CallAdEndCallFunc()
    {
        if (AdEndCallFunc != null)
            AdEndCallFunc();
    }

    private IEnumerator Co_AdEndCall()
    {
        yield return null;
        TKManager.Instance.ShowHUD();
        while (true)
        {
            if (AdView == false)
                break;
            yield return null;
        }

        TKManager.Instance.HideHUD();

        CallAdEndCallFunc();
        
    }


    //public bool IsPlayableAds()
    //{
    //    bool rtValue = false;

    //    if (Advertisement.IsReady(rewarded_video_id))
    //    {
    //        return true;
    //    }
    //    else if (Vungle.isAdvertAvailable(Vungle_adsID))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        AdColony.Zone adcolonyZone =  AdColony.Ads.GetZone(this.zoneId);
    //        if (adcolonyZone != null)
    //            return adcolonyZone.Enabled;
    //        else
    //            return false;
    //    }

    //    return rtValue;
    //}

    public IEnumerator Co_IsPlayableAds()
    {
        AdEnable = false;
        yield return null;
        
        if (Advertisement.IsReady(rewarded_video_id))
        {
            AdEnable = true;
            yield break;
        }
        else if (rewardAdmobVideo.IsLoaded())
        {
            AdEnable = true;
            yield break;
        }
        else
        {
            AdEnable = true;
            TKManager.Instance.HideHUD();
            yield break;
        }
    }
}
