using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

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

    public void Start()
    {
        DontDestroyOnLoad(this);

#if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
        Debug.Log("!@@@@@ Initialize");

        this.RequestBanner();
        Debug.Log("!@@@@@ RequestBanner");

        this.RequestInterstitial();
        Debug.Log("!@@@@@ RequestInterstitial");
    }

    AdRequest request;
    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom );
        
        
        // Create an empty ad request.
        request = new AdRequest.Builder().Build();

        // Load the banner with the request.

        this.bannerView.LoadAd(request);

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
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
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
        this.interstitial.Show();
    }

    
}
