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

    
    private BannerView bannerView;

    private const string android_game_id = "3019112";
    private const string ios_game_id = "3019113";

    private const string rewarded_video_id = "rewardedVideo";
    private const string Skip_rewarded_video_id = "SkipAds";
    private const string Lotto_rewarded_video_id = "LottoAds";

    public bool AdView = false;


    public void Start()
    {
        DontDestroyOnLoad(this);

#if UNITY_ANDROID
        //string appId = "ca-app-pub-3940256099942544~3347511713";
        string appId = "ca-app-pub-7615036525367000~1421003475";
        Advertisement.Initialize(android_game_id);
#elif UNITY_IPHONE
            //string appId = "ca-app-pub-3940256099942544~1458002511";
        string appId = "ca-app-pub-7615036525367000~9065234368"; 
         Advertisement.Initialize(ios_game_id);
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
        //Debug.Log("!@@@@@ Initialize");

        this.RequestBanner();
        //Debug.Log("!@@@@@ RequestBanner");

        this.RequestInterstitial();
        //Debug.Log("!@@@@@ RequestInterstitial");
    }

    AdRequest request;
    public void RequestBanner()
    {
#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        string adUnitId = "ca-app-pub-7615036525367000/7411696753";


#elif UNITY_IPHONE
        //string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        string adUnitId = "ca-app-pub-7615036525367000/9535049797";
        
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom );
        
        
        // Create an empty ad request.
        request = new AdRequest.Builder().Build();

        // Load the banner with the request.

        //this.bannerView.LoadAd(request);

    }

    public void ShowBanner()
    {
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
        //string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        string adUnitId = "ca-app-pub-7615036525367000/3352784828";
#elif UNITY_IPHONE
        //string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        string adUnitId = "ca-app-pub-7615036525367000/5432153254";
        
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
 

    }

    public void ShowInterstitialAds()
    {
        if(FirebaseManager.Instance.ReviewMode)
        {

        }
        else
        {
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


    public void ShowMiniGameRewardAd()
    {
        if(FirebaseManager.Instance.ReviewMode)
        {
            TKManager.Instance.MyData.AddPoint(CommonData.AdsPointReward);
        }
        else
        {
            if (Advertisement.IsReady(rewarded_video_id))
            {
                var options = new ShowOptions { resultCallback = HandleShowRewardVideoResult };
                Advertisement.Show(rewarded_video_id, options);
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

                    TKManager.Instance.MyData.AddPoint(CommonData.AdsPointReward);
                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");

                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");

                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }
    

    public void ShowLottoRewardedAd(Action endAction)
    {
        if (FirebaseManager.Instance.ReviewMode)
        {
            endAction();
        }
        else
        {
            AdView = false;

            if (Advertisement.IsReady(Lotto_rewarded_video_id))
            {
                AdView = true;
                var options = new ShowOptions { resultCallback = HandleShowLottoRewardVideoResult };
                Advertisement.Show(Lotto_rewarded_video_id, options);
            }

            if (AdView && endAction != null)
            {
                StartCoroutine(Co_AdEnd(endAction));
            }
        }
    }
    
    private void HandleShowLottoRewardVideoResult(ShowResult result)
    {
        AdView = false;
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("The ad was successfully shown.");

                    
                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");

                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");

                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }
    

    public void ShowSkipRewardedAd()
    {
        if (FirebaseManager.Instance.ReviewMode)
        {

        }
        else
        {
            if (Advertisement.IsReady(Skip_rewarded_video_id))
            {
                var options = new ShowOptions { resultCallback = HandleShowSkipRewardVideoResult };
                Advertisement.Show(Skip_rewarded_video_id, options);
            }
        }   
    }
    
    private void HandleShowSkipRewardVideoResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("The ad was successfully shown.");

                    //TKManager.Instance.MyData.AddPoint(CommonData.AdsPointReward);
                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");

                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");

                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }
    
    private IEnumerator Co_AdEnd(Action endAction)
    {
        while(true)
        {
            if (AdView == false)
                break;

            yield return null;
        }

        endAction();
    }


}
