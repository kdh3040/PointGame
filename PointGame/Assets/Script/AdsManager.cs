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


    /// ////////////////////////////////////////////////
    /// 벙글  셋팅
    private string Vungle_android_AppID = "5c5e8f366eceb929941bc24d";
    private string Vungle_iOS_AppID = "5c5e8f72330d082982b75ebe";

    private string Vungle_android_AdsID = "95192-2304521";
    private string Vungle_iOS_AdsID = "90578-2153733";

    private string Vungle_adsID;
    /// ////////////////////////////////////////////////



    /// ////////////////////////////////////////////////
    /// 애드콜로니  셋팅
    private const string android_appId = "appa4e0780d1d51492680";
    private const string android_zoneId = "vzd3b1fcd91ef346dab9";

    private const string ios_appId = "app904eea111f4847c58e";
    private const string ios_zoneId = "vz172fde3f76734b6f85";

    private string appId = string.Empty;
    private string zoneId = string.Empty;

    private AdColony.InterstitialAd adColony = null;

    /// ////////////////////////////////////////////////
    /// 애드몹
    /// 
    AdRequest request;



    // 광고 종료후 콜백 함수
    private Action AdEndCallFunc = null;

    public void Start()
    {
        DontDestroyOnLoad(this);


#if UNITY_ANDROID
        this.appId = android_appId;
        this.zoneId = android_zoneId;
        Vungle_adsID = Vungle_android_AdsID;
        string appId = "ca-app-pub-7615036525367000~1421003475";
        Advertisement.Initialize(android_game_id);
#elif UNITY_IPHONE
        Vungle_adsID = Vungle_iOS_AdsID;
        this.appId = ios_appId;
        this.zoneId = ios_zoneId;
        string appId = "ca-app-pub-7615036525367000~9065234368"; 
        Advertisement.Initialize(ios_game_id);
#else
        	Vungle_adsID = null;
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
        this.RequestBanner();
        this.RequestInterstitial();
        this.RequestAdmobVideo();
#endif


        ////////////////////////////////////////////////////
        //// 벙글 광고 초기화

        this.RequestVungleAds();


        ////////////////////////////////////////////////////
        //// 애드콜로니 광고 초기화
        
        AdColony.Ads.OnConfigurationCompleted += (List<AdColony.Zone> zones_) =>
        {
            Debug.Log("AdColony.Ads.OnConfigurationCompleted called");
            AdView = false;
            if (zones_ == null || zones_.Count <= 0)
            {
                Debug.Log("Configure Failed");              
            }
            else
            {
                Debug.Log("Configure Succeeded.");
            }
        };

        AdColony.Ads.OnRequestInterstitial += (AdColony.InterstitialAd ad_) =>
        {
            Debug.Log("AdColony.Ads.OnRequestInterstitial called");

            adColony = ad_;
            AdView = false;
            //ShowColony();
        };

        AdColony.Ads.OnRequestInterstitialFailed += () =>
        {
            Debug.Log("AdColony.Ads.OnRequestInterstitialFailed called");
            AdView = false;

            // to do ...
            // 광고 요청에 실패했을 때 처리

        };

        AdColony.Ads.OnOpened += (AdColony.InterstitialAd ad_) =>
        {
            AdView = false;
            Debug.Log("AdColony.Ads.OnOpened called");
        };

        AdColony.Ads.OnClosed += (AdColony.InterstitialAd ad_) =>
        {
            Debug.Log("AdColony.Ads.OnClosed called, expired: " + ad_.Expired);
            AdView = false;
        };

        AdColony.Ads.OnExpiring += (AdColony.InterstitialAd ad_) =>
        {
            AdView = false;
            Debug.Log("AdColony.Ads.OnExpiring called");
        };

        AdColony.Ads.OnRewardGranted += (string zoneId, bool success, string name, int amount) =>
        {
            Debug.Log(string.Format("AdColony.Ads.OnRewardGranted called\n\tzoneId: "
                + "{0}\n\tsuccess: {1}\n\tname: {2}\n\tamount: {3}",
                zoneId, success, name, amount));

            AdView = false;

            if (success)
            {
                // to do ...
                // 광고 시청이 완료되었을 때 처리
                // 광고 시청에 대한 보상 지급 등 ...
            
            }
        };

        AdColony.AppOptions appOptions = new AdColony.AppOptions();
        appOptions.AdOrientation = AdColony.AdOrientationType.AdColonyOrientationAll;

        AdColony.Ads.Configure(this.appId, appOptions, this.zoneId);

        RequestAdColonyAds();
        //

    }

    private void RequestVungleAds()
    {
        string appID;

#if UNITY_IPHONE
		appID = Vungle_iOS_AppID;
#elif UNITY_ANDROID
        appID = Vungle_android_AppID;
#elif UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
		appID = null;
#endif

        

        Vungle.init(appID);
        Vungle.loadAd(Vungle_adsID);

        Vungle.onAdStartedEvent += (placementID) => {
            AdView = false;
            Debug.Log("!!!!!@ Ad " + placementID + " is starting!  Pause your game  animation or sound here.");
        };

        Vungle.onAdFinishedEvent += (placementID, args) => {
            Debug.Log("!!!!!@ Ad finished - placementID " + placementID + ", was call to action clicked:" + args.WasCallToActionClicked + ", is completed view:"
                + args.IsCompletedView);
            AdView = false;
        };

        Vungle.adPlayableEvent += (placementID, adPlayable) => {
            Debug.Log("!!!!!@ Ad's playable state has been changed! placementID " + placementID + ". Now: " + adPlayable);
            AdView = false;
        };

        Vungle.onInitializeEvent += () => {
            Debug.Log("!!!!!@ SDK initialized");
            AdView = false;
        };
        
        
    }

 


    private void RequestAdmobVideo()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7615036525367000/9414508134";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-7615036525367000/6977591648";
#else
            string adUnitId = "unexpected_platform";
#endif
        request = new AdRequest.Builder().Build();
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
    }


    public void RequestAdColonyAds()
    {

        Debug.Log("**** Request Ad ****");

        AdColony.AdOptions adOptions = new AdColony.AdOptions();
        adOptions.ShowPrePopup = false;
        adOptions.ShowPostPopup = false;

        AdColony.Ads.RequestInterstitialAd(this.zoneId, adOptions);

    }

    
    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7615036525367000/7411696753";


#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-7615036525367000/9535049797";
        
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom );
        
        
        // Create an empty ad request.
        request = new AdRequest.Builder().Build();

        // Load the banner with the request.

       // this.bannerView.LoadAd(request);

    }

    public void ShowBanner()
    {
#if UNITY_EDITOR
        return;
#endif
        if (this.bannerView == null)
            return;

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
        string adUnitId = "ca-app-pub-7615036525367000/5432153254";
        
#else
        string adUnitId = "unexpected_platform";
#endif

        this.interstitial = new InterstitialAd(adUnitId);

        request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);
 

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

        }
        else
        {
            AdView = false;
            if (this.interstitial == null)
                return;

            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
            }
            else
            {
                this.RequestInterstitial();
                this.interstitial.Show();
            }
            
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
        AdView = false;
        return;
#endif
        if (rewardAdmobVideo.IsLoaded())
        {
            rewardAdmobVideo.Show();
        }
        else
        {
            ShowInterstitialAds();
        }
        
    }

    //벙글 리워드 비디오 (스킵 불가능)
    void ShowVungleAds()
    {

        if (Vungle.isAdvertAvailable(Vungle_adsID))
        {
            Vungle.playAd(Vungle_adsID);
        }
        else
            AdView = false;
        
    }

    // 애드콜로니 리워드 비디오 (스킵 불가)
    private void ShowColonyAds()
    {

        Debug.Log("**** Show Ad ****");

        if (this.adColony != null)
        {
            AdColony.Ads.ShowAd(this.adColony);
        }

    }


    // 유니티애즈 미니게임 리워드 비디오 스킵 불가
    public void ShowMiniGameRewardAd(Action endAction)
    {
        //ShowAdColonyAds();
        //return;

        if (FirebaseManager.Instance.ReviewMode)
        {
            SetAdEndCallFunc(endAction);
            AdView = false;
        }
        else
        {
            SetAdEndCallFunc(endAction);
            if (Advertisement.IsReady(rewarded_video_id))
            {
                var options = new ShowOptions { resultCallback = HandleShowRewardVideoResult };
                Advertisement.Show(rewarded_video_id, options);
            }
            else if (Vungle.isAdvertAvailable(Vungle_adsID))
            {
                ShowVungleAds();
            }
            else
            {
                AdColony.Zone adcolonyZone = AdColony.Ads.GetZone(this.zoneId);
                if (adcolonyZone != null ||  adcolonyZone.Enabled)
                {
                    ShowColonyAds();
                }
                else
                    AdView = false;
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

        CallAdEndCallFunc();
        TKManager.Instance.HideHUD();
    }


    public bool IsPlayableAds()
    {
        bool rtValue = false;

        if (Advertisement.IsReady(rewarded_video_id))
        {
            return true;
        }
        else if (Vungle.isAdvertAvailable(Vungle_adsID))
        {
            return true;
        }
        else
        {
            AdColony.Zone adcolonyZone =  AdColony.Ads.GetZone(this.zoneId);
            if (adcolonyZone != null)
                return adcolonyZone.Enabled;
            else
                return false;
        }

        return rtValue;
    }


}
