﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class FirebaseManager : MonoBehaviour
{

    public static FirebaseManager _instance = null;
    public static FirebaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FirebaseManager>() as FirebaseManager;
            }
            return _instance;
        }
    }

    public Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;


    private string googleIdToken;
    private string googleAccessToken;
    public DatabaseReference mDatabaseRef;
    int LottoRefNnumber = 0;
    int LottoCurSeries = 0;
    int LottoTodaySeries = 0;

    public List<KeyValuePair<string, string>> PushList = new List<KeyValuePair<string, string>>();
    public int PushLastIndex = 0;

    public bool FirstLoadingComplete = false;
    public int LoadingCount = 0;

    private int ReviewVersion = 5;
    public bool ReviewMode = true;
    public bool ExamineMode = true;

    public string ExamineContrext;

    public bool FirebaseProgress = false;
    public Action FirebaseProgressEndCallFunc = null;

    public bool NickNameExist = false;
    public bool AcquirePointMode = false;


    // 가위바위보 시작 플래그
    // 입장시간 10분 후 에 값 변경됨
    public int FirebaseRPSGameStatus = 0;

    // 가위바위보 현재 차전
    public int FirebaseRPSGameSeries = -1;

    // 가위바위보 내 방번호
    public int FirebaseRPSGameMyRoom = -1;

    // 가위바위보 참가?
    public bool FirebaseRPSGameEnterEnable = false;

    // 가위바위보 내 자리번호
    public int FirebaseRPSGameMyPosition = -1;

    // 가위바위보 참가 가능시간
    public long FirebaseRPSGameEnterTime = 0;    

    // 가위바위보 게임 시작 시간
    public long FirebaseRPSGamePlayTime = 0;    

    // 가위바위보 게임 생존자
    public long FirebaseRPSGameUserCount = 0;

    // 가위바위보 상대방 정보
    public string FirebaseRPSGame_EnemyNick = "";
    public int FirebaseRPSGame_EnemyValue = 0;
    public int FirebaseRPSGame_EnemyIndex = 0;

    // 가위바위보에 참가 신청을 했는지?
    public bool FirebaseRPSGame_Enter = false;

    private EventHandler<ValueChangedEventArgs> RPSGameRoomChangedHandle = null;
    private string RPSGameRoomChangedHandle_SaveSeries = "";
    private string RPSGameRoomChangedHandle_SaveMyRoom = "";

    // 가위바위보 우승 상금
    public int FirebaseRPSWinnerPrizeMoney = 0;

    // 가위바위보 준우승 상금
    public int FirebaseRPSWinnerSecPrizeMoney = 0;

    // 가위바위보 현재 회차
    public int FirebaseRPSGameCurSeries = 0;

    public Action LottoPopupRefresh = null;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);

        LoadingCount = 0;
        FirstLoadingComplete = false;

        //SetUserCode();
    }


    // 구글로그인 
    public void GoogleLogin()
    {
        GooglePlayServiceInitialize();

#if UNITY_ANDROID

        PlayGamesPlatform.Instance.Authenticate((bool success) =>
        {
            if (success)
            {
               // StartCoroutine(co_GoogleLogin());
            }
            else
            {
                // to do ...
                // 구글 플레이 게임 서비스 로그인 실패 처리
            }
        });

#elif UNITY_IOS
 
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("@@@@@@@ Social.localUser.Authenticate success");
                // to do ...
                // 애플 게임 센터 로그인 성공 처리
            }
            else
            {
                Debug.Log("@@@@@@@ Social.localUser.Authenticate fail");
                // to do ...
                // 애플 게임 센터 로그인 실패 처리
            }
        }); 
#endif

    }
#if UNITY_ANDROID
    IEnumerator co_GoogleLogin()
    {
        while (System.String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        string accessToken = null;

        Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        //auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
        //    if (task.IsCanceled)
        //    {
        //        Debug.Log("!!!!!! SignInWithCredentialAsync canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.Log("!!!!!! SignInWithCredentialAsync Fault : " + task.Exception);
        //        return;
        //    }

        //    FirebaseUser newUser = task.Result;
        //    Debug.Log("!!!!!! User signed successfully : " + newUser.DisplayName +" ID : " +  newUser.UserId);

        //});
    }

#endif
    void GooglePlayServiceInitialize()
    {

#if UNITY_ANDROID

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

#elif UNITY_IOS
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true); 
#endif

    }

    public void Init()
    {

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://treasureone-4472e.firebaseio.com/");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

      

        FirebaseDatabase.DefaultInstance
       .GetReference("RPSGameSeries")
       .ValueChanged += HandleRPSGameSeriesChanged;

        FirebaseDatabase.DefaultInstance
      .GetReference("RPSUserCount")
      .ValueChanged += HandleRPSGameUserCountChanged;
        

        FirebaseDatabase.DefaultInstance
        .GetReference("LottoCurSeries")
        .ValueChanged += HandleChangedLottoCurSeries;

        FirebaseDatabase.DefaultInstance
        .GetReference("LottoLuckyGroup").OrderByKey().LimitToLast(5)
        .ChildAdded += HandleChildAddedLottoLuckyGroup;

        FirebaseDatabase.DefaultInstance
        .GetReference("LottoLuckyNumber").OrderByKey().LimitToLast(5)
        .ChildAdded += HandleChildAddedLottoLuckyNumber;

        FirebaseDatabase.DefaultInstance
        .GetReference("RPSGameWinnerGroup").OrderByKey().LimitToLast(5)
        .ChildAdded += HandleChildAddedRPSGameWinnerGroup;
    }

    void HandleRPSGameSeriesChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if(args.Snapshot.Value != null)
        {
            FirebaseRPSGameSeries = Convert.ToInt32(args.Snapshot.Value);
            // 데이터가 변경되면 실제로 게임이 시작된다.
            AddHandler();
        }
        Debug.Log("@@@@@@@ FirebaseRPSGameSeries " + FirebaseRPSGameSeries);
    }

    void HandleRPSGameUserCountChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if(args.Snapshot.Value != null)
        {
            FirebaseRPSGameUserCount = Convert.ToInt32(args.Snapshot.Value);
        }
        
        // 데이터가 변경되면 실제로 게임이 시작된다.
        Debug.Log("@@@@@@@ FirebaseRPSGameUserCount " + FirebaseRPSGameUserCount);
    }



    void HandleRPSGameRoomNumberChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if(args.Snapshot.Value != null)
        {
            FirebaseRPSGameMyRoom = Convert.ToInt32(args.Snapshot.Value);
            // 데이터가 변경되면 실제로 게임이 시작된다.
        }

        Debug.Log("@@@@@@@ RPSGameRoomNumber " + TKManager.Instance.MyData.RPSGameRoomNumber);

    }

    void HandleRPSGamePositionNumberChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Value != null)
        {
            FirebaseRPSGameMyPosition = Convert.ToInt32(args.Snapshot.Value);
            // 데이터가 변경되면 실제로 게임이 시작된다.
        }

        Debug.Log("@@@@@@@ HandleRPSGamePositionNumberChanged " + TKManager.Instance.MyData.RPSGameRoomNumber);

    }

    // 로또 회차 변경 모니터링
    void HandleChangedLottoCurSeries(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Value != null)
        {
            LottoCurSeries = Convert.ToInt32(args.Snapshot.Value);
            TKManager.Instance.SetCurrentLottoSeriesCount(LottoCurSeries);
        }

        Debug.Log("@@@@@@@ LottoCurSeries " + LottoCurSeries);
    }

    // 로또 당첨 인원 모니터링
    void HandleChildAddedLottoLuckyGroup(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Value != null)
        {

            String tempIndex = args.Snapshot.Key;
            tempIndex = tempIndex.Substring(0, tempIndex.IndexOf("_"));
            int tempIndexToInt = Convert.ToInt32(tempIndex);
            tempIndexToInt -= CommonData.LottoRefSeries;

            String tempSrc = args.Snapshot.Value.ToString();


            TKManager.Instance.SetLottoWinUserData(tempIndexToInt, tempSrc);

            Debug.Log("@@@@@@@ HandleChildAddedLottoLuckyGroup " + tempIndexToInt);
        }

        // Do something with the data in args.Snapshot
    }


    // 로또 당첨번호 모니터링
    void HandleChildAddedLottoLuckyNumber(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Value != null)
        {

            String tempSeries = args.Snapshot.Key;
            tempSeries = tempSeries.Substring(0, tempSeries.IndexOf("_"));
            int tempSeriesToInt = Convert.ToInt32(tempSeries);
            tempSeriesToInt -= CommonData.LottoRefSeries;

            int tempNumber = Convert.ToInt32(args.Snapshot.Value.ToString());
            TKManager.Instance.SetLottoLuckyNumber(tempSeriesToInt, tempNumber);
            if (LottoPopupRefresh != null)
                LottoPopupRefresh();
            Debug.Log("@@@@@@@ HandleChildAddedLottoLuckyNumber " + tempSeriesToInt);
        }

        // Do something with the data in args.Snapshot
    }

    // 가위바위보 우승자 모니터링
    void HandleChildAddedRPSGameWinnerGroup(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Value != null)
        {
            var tempData = args.Snapshot.Value as Dictionary<string, object>;
            var tempFirstNickName = tempData["first"].ToString();
            var tempSecondNickName = tempData["second"].ToString();
            int tempCount = Convert.ToInt32(args.Snapshot.Key);

            bool plusEnable = true;
            for (int i = 0; i < TKManager.Instance.RPSWinUserList.Count; i++)
            {
                if (TKManager.Instance.RPSWinUserList[i].Count == tempCount)
                {
                    plusEnable = false;
                    break;
                }
                    
            }
            if(plusEnable)
            {
                TKManager.RPSGameWinnerData tempWinnerData = new TKManager.RPSGameWinnerData();
                tempWinnerData.FirstName = tempFirstNickName;
                tempWinnerData.SecondName = tempSecondNickName;
                tempWinnerData.Count = tempCount;
                TKManager.Instance.RPSWinUserList.Add(tempWinnerData);

                Debug.Log("##### RPSGameWinnerGroup " + tempCount);
            }
        }

        // Do something with the data in args.Snapshot
    }


    public void TokenRefresh(Firebase.Auth.FirebaseUser user)
    {
        //user.TokenAsync(true).ContinueWith(task => {
        //    if (task.IsCanceled)
        //    {
        //        Debug.Log("!!!!! TokenAsync was canceled.");
        //        return;
        //    }

        //    if (task.IsFaulted)
        //    {
        //        Debug.Log("!!!!! TokenAsync encountered an error: " + task.Exception);
        //        return;
        //    }

        //    string idToken = task.Result;

        //    Debug.Log("!!!!! Token: " + idToken);
        //});

    }


    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }

    public void GetData()
    {        
        GetUserData();

        GetReviewVersion();
        GetExamineMode();
        GetLottoRefNumber();
        //GetLottoTodaySeries();
        GetLottoCurSeries();
        GetLottoLuckyNumber();
        GetLottoLuckGroup();
        GetPushAlarm();
        GetGiftProb();
        GetReviewRank();
        GetRecommendUser();
        GetUpdatePopup();

        GetRPSGamePlayTime();
        GetRPSGameEnterTime();
        GetRPSGameEnterStatus();

        GetRPSGameWinnerGroup();
        GetRPSWinnerPrizeMoney();
        GetRPSWinnerSecPrizeMoney();

        GetRPSGameCurSeries();
        IsAcquirePointMode();
    }


    private void AddFirstLoadingComplete()
    {
        if (FirstLoadingComplete == false)
            LoadingCount++;

        if (LoadingCount == 20)
            FirstLoadingComplete = true;
    }

    public bool SingedInFirebase()
    {

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            user = auth.CurrentUser;
            TokenRefresh(user);
            return true;
        }

        return false;
    }

    // 사용자 정보 파이어베이스에 세팅
    public void SetUserData()
    {
        mDatabaseRef.Child("UserNameList").Child(TKManager.Instance.MyData.NickName).SetValueAsync(TKManager.Instance.MyData.Index);

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Index").SetValueAsync(TKManager.Instance.MyData.Index);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("NickName").SetValueAsync(TKManager.Instance.MyData.NickName);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Point").SetValueAsync(TKManager.Instance.MyData.Point);

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TotalAccumPoint").SetValueAsync(0);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TodayAccumPoint").Child(GetToday()).SetValueAsync(0);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Cash").SetValueAsync(0);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyRoom").SetValueAsync(-1);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyPosition").SetValueAsync(-1);

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("NewUser").SetValueAsync(true);
    }
    // 사용자 정보 파이어베이스에서 로드
    public void GetUserData()
    {
        string userIdx = TKManager.Instance.MyData.Index;

        mDatabaseRef.Child("Users").Child(userIdx).GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    var tempData = snapshot.Value as Dictionary<string, object>;
                    //int tempPoint = Convert.ToInt32(tempData["Point"]);
                    TKManager.Instance.MyData.SetData(tempData["Index"].ToString(), tempData["NickName"].ToString());

                    if (tempData.ContainsKey("NewUser") == false)
                        TKManager.Instance.MyData.NewUser = false;
                    else
                        TKManager.Instance.MyData.NewUser = true;

                    mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("NewUser").SetValueAsync(true);

                    if (tempData.ContainsKey("NewCalUser") == false)
                        TKManager.Instance.MyData.NewCalUser = false;
                    else
                        TKManager.Instance.MyData.NewCalUser = true;

                    mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("NewCalUser").SetValueAsync(true);

                    string tempNick = TKManager.Instance.MyData.NickName;

                    tempNick = tempNick.Replace(".", "");
                    tempNick = tempNick.Replace("#", "");
                    tempNick = tempNick.Replace("$", "");
                    tempNick = tempNick.Replace("[", "");
                    tempNick = tempNick.Replace("]", "");

                    if(tempNick.Equals(TKManager.Instance.MyData.NickName) == false)
                    {
                        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("NickName").SetValueAsync(tempNick);
                        TKManager.Instance.MyData.NickName = tempNick;
                    }
                        

                    if (tempData.ContainsKey("Lotto"))
                    {
                        var LottoInfo = tempData["Lotto"] as Dictionary<string, object>;
                        foreach (var pair in LottoInfo)
                        {
                            string tempLottoSeries = pair.Key.Substring(0, pair.Key.IndexOf("_"));
                            int tempLottoSeriesToInt = Convert.ToInt32(tempLottoSeries);
                            tempLottoSeriesToInt -= CommonData.LottoRefSeries;

                            int tempLottoNumber = Convert.ToInt32(pair.Value);

                            TKManager.Instance.MyData.SetLottoData(tempLottoSeriesToInt, tempLottoNumber);
                            Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
                        }
                    }


                    if (tempData.ContainsKey("LottoWin"))
                    {
                        var LottoWinInfo = tempData["LottoWin"] as Dictionary<string, object>;
                        foreach (var pair in LottoWinInfo)
                        {
                            string tempLottoSeries = pair.Key.Substring(0, pair.Key.IndexOf("_"));
                            int tempLottoSeriesToInt = Convert.ToInt32(tempLottoSeries);
                            tempLottoSeriesToInt -= CommonData.LottoRefSeries;

                            TKManager.Instance.MyData.SetLottoWinSeriesData(tempLottoSeriesToInt);
                            Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
                        }
                    }

                    if (tempData.ContainsKey("Gift"))
                    {
                        var GiftInfo = tempData["Gift"] as Dictionary<string, object>;
                        foreach (var pair in GiftInfo)
                        {
                            string tempIndex = pair.Key.Substring(0, pair.Key.IndexOf("_"));
                            TKManager.Instance.MyData.SetGiftconData(Convert.ToInt32(tempIndex), pair.Value.ToString());
                            //  Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
                        }
                    }

                    if (tempData.ContainsKey("Cash"))
                    {
                        int tempCash = Convert.ToInt32(tempData["Cash"]);
                        TKManager.Instance.MyData.SetCash(tempCash);
                    }
                    else
                    {
                        TKManager.Instance.MyData.SetCash(0);
                    }

                    if (tempData.ContainsKey("Point"))
                    {
                        int tempPoint = Convert.ToInt32(tempData["Point"]);
                        TKManager.Instance.MyData.SetPoint(tempPoint);
                    }
                    else
                    {
                        TKManager.Instance.MyData.SetPoint(0);
                    }

                    if (tempData.ContainsKey("TotalAccumPoint"))
                    {
                        int tempTotalAccumPoint = Convert.ToInt32(tempData["TotalAccumPoint"]);
                        TKManager.Instance.MyData.SetAllAccumulatePoint(tempTotalAccumPoint);
                    }
                    else
                    {
                        TKManager.Instance.MyData.SetAllAccumulatePoint(0);
                    }

                    if (tempData.ContainsKey("RecommenderCode"))
                    {
                        var tempCode =tempData["RecommenderCode"].ToString();
                        TKManager.Instance.MyData.SetRecommenderCode(tempCode);
                    }
                    else
                    {
                        FirebaseManager.Instance.SetRecommenderCode();
                        //TKManager.Instance.MyData.SetRecommenderCode(TKManager.Instance.MyData.Index);
                    }

                    if (tempData.ContainsKey("TodayAccumPoint"))
                    {
                        var tempTodayAccumPointInfo = tempData["TodayAccumPoint"] as Dictionary<string, object>;
                        foreach (var pair in tempTodayAccumPointInfo)
                        {
                            string tempDate = pair.Key.ToString();
                            if (tempDate.Equals(GetToday()))
                            {
                                TKManager.Instance.MyData.SetTodayAccumulatePoint(Convert.ToInt32(pair.Value));
                            }
                            else
                            {
                                mDatabaseRef.Child("Users").Child(userIdx).Child("TodayAccumPoint").RemoveValueAsync();
                            }
                        }
                    }
                    else
                    {
                        TKManager.Instance.MyData.SetTodayAccumulatePoint(0);
                    }

                    if (tempData.ContainsKey("AdsViewCount"))
                    {
                        int tempAdsViewCount = Convert.ToInt32(tempData["AdsViewCount"]);
                        TKManager.Instance.MyData.SetAdsCount(tempAdsViewCount);
                    }
                    else
                    {
                        TKManager.Instance.MyData.SetAdsCount(0);
                    }                    


                    FirebaseRPSGameMyRoom = -1;

                    FirebaseDatabase.DefaultInstance
                    .GetReference("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyRoom")
                    .ValueChanged += HandleRPSGameRoomNumberChanged;

                    FirebaseRPSGameMyPosition = -1;

                    FirebaseDatabase.DefaultInstance
                    .GetReference("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyPosition")
                    .ValueChanged += HandleRPSGamePositionNumberChanged;

                }

                AddFirstLoadingComplete();

                //  Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
            }
        });

    }


    // 사용자 보유 포인트 파이어베이스에 셋팅
    public void SetPoint(int point)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Point").SetValueAsync(point);
    }

    // 사용자 보유 포인트 파이어베이스에서 로드
    public void GetPoint(Action endAction)
    {
        SetEndCallFunc(endAction);
        int rtPoint = 0;

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Point").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                    rtPoint = Convert.ToInt32(snapshot.Value);
                else
                    rtPoint = 0;

                TKManager.Instance.MyData.SetPoint(rtPoint);
                FirebaseProgress = false;
            }
        });

    }

    public void SetTotalAccumPoint(int point)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TotalAccumPoint").SetValueAsync(point);
    }
    public void GetTotalAccumPoint(Action endAction)
    {
        SetEndCallFunc(endAction);
        int rtPoint = 0;

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TotalAccumPoint").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                FirebaseProgress = false;
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                    rtPoint = Convert.ToInt32(snapshot.Value);
                else
                    rtPoint = 0;
            }
        });

    }

    public void SetTodayAccumPoint(int point)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TodayAccumPoint").Child(GetToday()).SetValueAsync(point);
    }
    public void GetTodayAccumPoint(Action endAction)
    {
        SetEndCallFunc(endAction);
        int rtPoint = 0;

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TodaytAccumPoint").Child(GetToday()).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                FirebaseProgress = false;
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                    rtPoint = Convert.ToInt32(snapshot.Value);
                else
                    rtPoint = 0;

                TKManager.Instance.MyData.SetTodayAccumulatePoint(rtPoint);
                FirebaseProgress = false;
            }
        });

    }

    public void SetCash(int Cash)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Cash").SetValueAsync(Cash);
    }
    public void GetCash(Action endAction)
    {
        SetEndCallFunc(endAction);
        int rtPoint = 0;

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Cash").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                    rtPoint = Convert.ToInt32(snapshot.Value);
                else
                    rtPoint = 0;

                TKManager.Instance.MyData.SetCash(rtPoint);
                FirebaseProgress = false;
            }
        });

    }
    public void SetCashInfo(String Name, String BankName, String Account, int CachBack)
    {
        var strToday = GetToday();

        mDatabaseRef.Child("CashBack").Child(strToday).Child(TKManager.Instance.MyData.Index).Child("NickName").SetValueAsync(TKManager.Instance.MyData.NickName);
        mDatabaseRef.Child("CashBack").Child(strToday).Child(TKManager.Instance.MyData.Index).Child("Name").SetValueAsync(Name);
        mDatabaseRef.Child("CashBack").Child(strToday).Child(TKManager.Instance.MyData.Index).Child("BankName").SetValueAsync(BankName);
        mDatabaseRef.Child("CashBack").Child(strToday).Child(TKManager.Instance.MyData.Index).Child("Account").SetValueAsync(Account);
        mDatabaseRef.Child("CashBack").Child(strToday).Child(TKManager.Instance.MyData.Index).Child("CachBack").SetValueAsync(CachBack);
        mDatabaseRef.Child("CashBack").Child(strToday).Child(TKManager.Instance.MyData.Index).Child("AdsViewCount").SetValueAsync(TKManager.Instance.MyData.GetAdsCount());

    }

    // 상품권 걸릴 확률
    public void GetGiftProb()
    {

        mDatabaseRef.Child("GiftProb").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.Exists)
                {
                    foreach (var tempChild in snapshot.Children)
                    {
                        var tempData = tempChild.Value as Dictionary<string, object>;
                        String tempProb = tempData["Prob"].ToString();
                        String tempName = tempData["Name"].ToString();

                        if (TKManager.Instance.RoulettePercent.Count <= 0)
                            TKManager.Instance.RoulettePercent.Add(new KeyValuePair<int, int>(Convert.ToInt32(tempName), Convert.ToInt32(tempProb)));
                        else
                        {
                            var tempValue = TKManager.Instance.RoulettePercent[TKManager.Instance.RoulettePercent.Count - 1].Value;
                            TKManager.Instance.RoulettePercent.Add(new KeyValuePair<int, int>(Convert.ToInt32(tempName), tempValue + Convert.ToInt32(tempProb)));
                        }


                        //  Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);

                    }
                }

                AddFirstLoadingComplete();

            }
        });

    }

    public void DelGiftImage(int Index)
    {
        String tempIndex = Index.ToString() + "_G";
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Gift").Child(tempIndex).RemoveValueAsync();
    }

    // 상품권 이미지 주소
    public void GetGiftImage(Action<int> endAction)
    {
        TKManager.Instance.ShowHUD();
        string GiftImageSrc = null;



        FirebaseDatabase.DefaultInstance.GetReference("Gift").OrderByKey().LimitToFirst(1)
       .GetValueAsync().ContinueWith(task =>
       {
           if (task.IsFaulted)
           {
               // Handle the error...
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               if (snapshot != null && snapshot.Exists)
               {
                   var giftUrlList = new List<string>();
                   String tempIndex = "";
                   String tempSrc = "";
                   foreach (var tempChild in snapshot.Children)
                   {
                       tempIndex = tempChild.Key;
                       tempSrc = tempChild.Value.ToString();
                       giftUrlList.Add(tempSrc);

                       mDatabaseRef.Child("Gift").Child(tempIndex).RemoveValueAsync();

                       mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Gift").Child(tempIndex).SetValueAsync(tempSrc);
                       tempIndex = tempIndex.Substring(0, tempIndex.IndexOf("_"));
                       TKManager.Instance.MyData.SetGiftconData(Convert.ToInt32(tempIndex), tempSrc);
                   }

                   StartCoroutine(GetGiftconTexture(giftUrlList, Convert.ToInt32(tempIndex), endAction));
               }

           }
       });


    }

    IEnumerator GetGiftconTexture(List<string> urlList, int giftconIndex, Action<int> endAction)
    {
        yield return ImageCache.Instance.GetTexture(urlList);

        TKManager.Instance.HideHUD();

        endAction(giftconIndex);
    }

    // 로또 번호 파이어베이스에서 로드
    public void SetLottoNumber(Action endAction, PopupUI popup)
    {
        SetEndCallFunc(endAction);
        mDatabaseRef.Child("LottoRefNumber").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                popup.ShowPopup(new MsgPopup.MsgPopupData("인터넷 연결이 불안정 합니다"));
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    LottoRefNnumber = Convert.ToInt32(snapshot.Value);

                    mDatabaseRef.Child("LottoCurSeries").GetValueAsync().ContinueWith(CurSeriestask =>
                    {
                        if (CurSeriestask.IsFaulted)
                        {
                            // Handle the error...
                            popup.ShowPopup(new MsgPopup.MsgPopupData("인터넷 연결이 불안정 합니다"));
                        }
                        else if (CurSeriestask.IsCompleted)
                        {
                            snapshot = CurSeriestask.Result;
                            if (snapshot != null && snapshot.Exists)
                            {
                                LottoCurSeries = Convert.ToInt32(snapshot.Value);
                                TKManager.Instance.SetCurrentLottoSeriesCount(LottoCurSeries);
                                mDatabaseRef.Child("LottoCount").RunTransaction(mutableData =>
                                {
                                    int tempCount = Convert.ToInt32(mutableData.Value);
                                    if (tempCount == 0)
                                    {
                                        tempCount = 1;
                                    }
                                    else
                                    {
                                        int tempSeries = LottoCurSeries;
                                        tempSeries += CommonData.LottoRefSeries;

                                        mDatabaseRef.Child("Lotto").Child(tempSeries + "_L").Child(TKManager.Instance.MyData.NickName).SetValueAsync(LottoRefNnumber * tempCount);
                                        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Lotto").Child(tempSeries + "_L").SetValueAsync(LottoRefNnumber * tempCount);
                                        tempSeries -= CommonData.LottoRefSeries;
                                        TKManager.Instance.MyData.SetLottoData(tempSeries, LottoRefNnumber * tempCount);
                                        mutableData.Value = tempCount + 1;
                                        //   mDatabaseRef.Child("TestLottoCount").SetValueAsync(mutableData.Value);
                                        FirebaseProgress = false;
                                    }

                                    return TransactionResult.Success(mutableData);

                                });
                            }
                        }
                    });
                }
                else
                {
                    LottoRefNnumber = 0;
                    Debug.Log("***&& SetLottoNumber 28 ");
                }
                    
            }
        });
    }

    public void GetLottoLuckGroup()
    {

        FirebaseDatabase.DefaultInstance.GetReference("LottoLuckyGroup").OrderByKey().LimitToLast(5)
       .GetValueAsync().ContinueWith(task =>
       {
           if (task.IsFaulted)
           {
               // Handle the error...
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               if (snapshot != null && snapshot.Exists)
               {
                   foreach (var tempChild in snapshot.Children)
                   {
                       String tempIndex = tempChild.Key;
                       String tempSrc = tempChild.Value.ToString();

                       tempIndex = tempIndex.Substring(0, tempIndex.IndexOf("_"));
                       int tempIndexToInt = Convert.ToInt32(tempIndex);
                       tempIndexToInt -= CommonData.LottoRefSeries;

                       TKManager.Instance.SetLottoWinUserData(tempIndexToInt, tempSrc);
                       Debug.Log("##### GetLottoLuckGroup " + tempIndexToInt);
                   }
               }


               AddFirstLoadingComplete();

           }
       });


    }
    //public void GetLottoTodaySeries()
    //{
    //    mDatabaseRef.Child("LottoTodaySeries").GetValueAsync().ContinueWith(task =>
    //    {
    //        if (task.IsFaulted)
    //        {
    //            // Handle the error...
    //        }
    //        else if (task.IsCompleted)
    //        {
    //            DataSnapshot snapshot = task.Result;
    //            LottoTodaySeries = Convert.ToInt32(snapshot.Value);
    //            //TKManager.Instance.SetTodayLottoSeriesMinCount(LottoTodaySeries);

    //            AddFirstLoadingComplete();
    //        }
    //    }
    // );
    //}

    public void GetLottoCurSeries()
    {

        mDatabaseRef.Child("LottoCurSeries").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    LottoCurSeries = Convert.ToInt32(snapshot.Value);
                    TKManager.Instance.SetCurrentLottoSeriesCount(LottoCurSeries);
                }

                AddFirstLoadingComplete();

            }
        }
    );

    }

    public void GetReviewVersion(Action endAction = null)
    {

#if UNITY_IOS
        string dataKey = "ios_ReviewVersion";
        //string dataKey = "editor_ReviewVersion";
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        string dataKey = "aos_ReviewVersion";
#elif (UNITY_ANDROID && UNITY_EDITOR)
        string dataKey = "editor_ReviewVersion";
#else
        string dataKey = "editor_ReviewVersion";
#endif
        if(endAction != null)
            SetEndCallFunc(endAction);

        mDatabaseRef.Child(dataKey).GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int version = 0;
                if (snapshot != null && snapshot.Exists)
                {
                    version = Convert.ToInt32(snapshot.Value);
                }

                ReviewMode = version < ReviewVersion;
                //ReviewMode = true;
                if(endAction == null)
                    AddFirstLoadingComplete();
                else
                    FirebaseProgress = false;

            }
        }
      );

    }

    public void GetExamineMode()
    {

#if UNITY_IOS
        string dataKey = "ios_ExamineVersion";
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        string dataKey = "aos_ExamineVersion";
#elif (UNITY_ANDROID && UNITY_EDITOR)
        string dataKey = "editor_ExamineVersion";
#endif
        mDatabaseRef.Child(dataKey).GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int version = 0;
                if (snapshot != null && snapshot.Exists)
                {
                    version = Convert.ToInt32(snapshot.Value);
                }

                ExamineMode = ReviewVersion <= version;// < ReviewVersion;

                AddFirstLoadingComplete();

            }
        }
      );

    }

    // 로또 레퍼런스 번호 파이어베이스에서 로드
    public void GetLottoRefNumber()
    {

        mDatabaseRef.Child("LottoRefNumber").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    LottoRefNnumber = Convert.ToInt32(snapshot.Value);
                }
                else
                    LottoRefNnumber = 0;

                AddFirstLoadingComplete();

            }
        }
      );

    }

    // 로또 당첨 번호 파이어베이스에서 로드
    public void GetLottoLuckyNumber()
    {

        mDatabaseRef.Child("LottoLuckyNumber").OrderByKey().LimitToLast(3)
       .GetValueAsync().ContinueWith(task =>

        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.Exists)
                {
                    foreach (var tempChild in snapshot.Children)
                    {

                        String tempSeries = tempChild.Key;
                        int tempNumber = Convert.ToInt32(tempChild.Value.ToString());

                        tempSeries = tempSeries.Substring(0, tempSeries.IndexOf("_"));
                        int tempSeriesToInt = Convert.ToInt32(tempSeries);
                        tempSeriesToInt -= CommonData.LottoRefSeries;

                        TKManager.Instance.SetLottoLuckyNumber(tempSeriesToInt, tempNumber);
                    }
                }

                AddFirstLoadingComplete();


            }
        }
      );

    }

    // 공지사항 파이어베이스에서 받아오기
    public void GetPushAlarm()
    {

        string tempAlarmIndex = "0";

        mDatabaseRef.Child("PushAlarm").OrderByKey().LimitToLast(4)
       .GetValueAsync().ContinueWith(task =>

       {

           if (task.IsFaulted)
           {
               // Handle the error...
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               if (snapshot != null && snapshot.Exists)
               {
                   foreach (var tempChild in snapshot.Children)
                   {
                       int tempIndex = Convert.ToInt32(tempChild.Key);
                       if (tempIndex > PushLastIndex)
                           PushLastIndex = tempIndex;
                       var tempData = tempChild.Value as Dictionary<string, object>;
                       var tempAlarmTitle = tempData["Title"].ToString();
                       var tempAlarmContent = tempData["Content"].ToString();

                       PushList.Add(new KeyValuePair<string, string>(tempAlarmTitle, tempAlarmContent));
                   }
                   PushList.Reverse();

                   if (TKManager.Instance.PushLastIndex < PushLastIndex)
                       TKManager.Instance.PushNotiEnable = true;
                   else
                       TKManager.Instance.PushNotiEnable = false;
               }
               else
               {
                   TKManager.Instance.PushNotiEnable = false;
               }

               AddFirstLoadingComplete();
           }
       }
      );

    }


    // 공지사항 파이어베이스에서 받아오기
    public void GetUpdatePopup()
    {
        mDatabaseRef.Child("UpdatePopup").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.Exists)
                {
                    ExamineContrext = snapshot.Value.ToString();
                }

                AddFirstLoadingComplete();


            }
        }
     );

    }

    public void SetLottoWinUserData(int Series, String Name, String Bank, String Account)
    {

        Series += CommonData.LottoRefSeries;
        String LottoWinSeries = Series.ToString() + "_L";

        mDatabaseRef.Child("LottoWinUsers").Child(LottoWinSeries).Child("Name").SetValueAsync(Name);
        mDatabaseRef.Child("LottoWinUsers").Child(LottoWinSeries).Child("Bank").SetValueAsync(Bank);
        mDatabaseRef.Child("LottoWinUsers").Child(LottoWinSeries).Child("Account").SetValueAsync(Account);

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("LottoWin").Child(LottoWinSeries).SetValueAsync(1);

    }


    public string GetToday()
    {
        string strToday = System.DateTime.Now.ToString("yyyyMMdd");
        return strToday;
    }

    char[] stringChars = new char[4];


    public void SetRecommendUser(string RecommendCode)
    {
        mDatabaseRef.Child("Recommend").Child(RecommendCode).Child(TKManager.Instance.MyData.Index).SetValueAsync(TKManager.Instance.MyData.NickName);
    }
    // 내 추천인 받아오기
    public void GetRecommendUser()
    {
        if(TKManager.Instance.MyData.RecommenderCode == "")
        {
            FirebaseManager.Instance.SetRecommenderCode();
            AddFirstLoadingComplete();
            return;
        }  

        mDatabaseRef.Child("Recommend").Child(TKManager.Instance.MyData.RecommenderCode).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                string tempUserNickName = null;
                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.Exists)
                {
                    foreach (var tempChild in snapshot.Children)
                    {
                        String tempUserIndex = tempChild.Key;
                        tempUserNickName = tempChild.Value.ToString();
                        TKManager.Instance.RecommendUsers.Add(new KeyValuePair<string, string>(tempUserIndex, tempUserNickName));
                    }
                    mDatabaseRef.Child("Recommend").Child(TKManager.Instance.MyData.RecommenderCode).RemoveValueAsync();
                }
                
                AddFirstLoadingComplete();

            }
        }
     );

    }


    public void SetRecommenderCode()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
     
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        TKManager.Instance.MyData.RecommenderCode = new String(stringChars);
        TKManager.Instance.MyData.RecommenderCode += TKManager.Instance.MyData.Index;

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("RecommenderCode").SetValueAsync(TKManager.Instance.MyData.RecommenderCode);
        TKManager.Instance.SaveFile();
    }

    public string GetRecommenderCode()
    {
        return TKManager.Instance.MyData.RecommenderCode;
    }

    // 가위바위보 우승 상금 가져오기
    public void GetRPSWinnerPrizeMoney()
    {
        mDatabaseRef.Child("RPSWinnerPrizeMoney").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    FirebaseRPSWinnerPrizeMoney = Convert.ToInt32(snapshot.Value);
                }

                AddFirstLoadingComplete();

            }
        }
    );

    }

    // 가위바위보 준우승 상금 가져오기
    public void GetRPSWinnerSecPrizeMoney()
    {
        mDatabaseRef.Child("RPSWinnerSecPrizeMoney").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    FirebaseRPSWinnerSecPrizeMoney = Convert.ToInt32(snapshot.Value);
                }

                AddFirstLoadingComplete();

            }
        }
    );

    }

    // 가위바위보 현재 회차
    public void GetRPSGameCurSeries()
    {
        mDatabaseRef.Child("RPSGameCurSeries").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    FirebaseRPSGameCurSeries = Convert.ToInt32(snapshot.Value);
                }

                AddFirstLoadingComplete();

            }
        }
    );

    }
    

    static int tempIndex = 0;
    // Rock–Paper–Scissors : RPS 
    public void EnterRPSGame()
    {

        // TODO 광고 보는 코드 추가 
        FirebaseRPSGame_EnemyNick = "";
        FirebaseRPSGame_EnemyValue = 0;
        FirebaseRPSGame_EnemyIndex = 0;
        FirebaseRPSGameSeries = -1;
        FirebaseRPSGameMyRoom = -1;

        mDatabaseRef.Child("RPSUserList").Child(TKManager.Instance.MyData.Index).SetValueAsync(TKManager.Instance.MyData.NickName);
        
        /*

        mDatabaseRef.Child("RPSUserCount").RunTransaction(mutableData =>
        {
            int tempCount = Convert.ToInt32(mutableData.Value);          

            if (tempCount == 0)
            {
                tempCount = 0;
            }
            else
            {
                FirebaseRPSGameMyRoom = (tempCount - 1) / 2;
                FirebaseRPSGameMyPosition = tempCount - 1;

                mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyRoom").SetValueAsync(FirebaseRPSGameMyRoom);
                mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyPosition").SetValueAsync(FirebaseRPSGameMyPosition);       

                mutableData.Value = tempCount + 1;

                AddHandler();


                //SetRecommenderCode();

                //mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
                //.Child(tempIndex.ToString()).Child("NickName").SetValueAsync(GetRecommenderCode());

                //mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
                //.Child(tempIndex.ToString()).Child("NickName").SetValueAsync(GetRecommenderCode());

                //mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
                //.Child(tempIndex.ToString()).Child("Value").SetValueAsync(0);

                //tempIndex += 1;
                

                //mutableData.Value = tempCount + 1;

                //AddHandler();
            }

            CreateRPSGameRoom();
            return TransactionResult.Success(mutableData);

        });
        */
    }

    public void CreateRPSGameRoom()
    {
        mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
      .Child(FirebaseRPSGameMyPosition.ToString()).Child("Index").SetValueAsync(TKManager.Instance.MyData.Index);

        mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
        .Child(FirebaseRPSGameMyPosition.ToString()).Child("NickName").SetValueAsync(TKManager.Instance.MyData.NickName);

        mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
        .Child(FirebaseRPSGameMyPosition.ToString()).Child("Value").SetValueAsync(0);
    }

    void HandleGameRoomChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        if (snapshot != null && snapshot.Exists)
        {
            foreach (var tempChild in snapshot.Children)
            {
                //int tempIndex = Convert.ToInt32(tempChild.Key);
                var tempData = tempChild.Value as Dictionary<string, object>;

                var tempIndex = tempData["Index"].ToString();
                if (!tempIndex.Equals(TKManager.Instance.MyData.Index))
                {
                    var tempNickName = tempData["NickName"].ToString();
                    FirebaseRPSGame_EnemyNick = tempNickName;
                    FirebaseRPSGame_EnemyValue = Convert.ToInt32(tempData["Value"]);
                    FirebaseRPSGame_EnemyIndex = Convert.ToInt32(tempIndex);

                    Debug.Log("@@@@@@@ HandleGameRoomChanged " + FirebaseRPSGame_EnemyIndex + " " + FirebaseRPSGame_EnemyNick + " " + FirebaseRPSGame_EnemyValue);
                    Debug.Log("@@@@@@@ HandleGameRoomChanged_1 " + FirebaseRPSGameSeries.ToString() + " " + FirebaseRPSGameMyRoom.ToString());
                }
                
            }    
        }

        //FirebaseRPSGame_EnemyNick = Convert.ToInt32(args.Snapshot.Value);
    }

    public void AddHandler()
    {
        // TODO 환웅 ValueChanged이 비어 있는지 확인해야함 찾아보셈

        if(RPSGameRoomChangedHandle != null)
        {
            FirebaseDatabase.DefaultInstance
                 .GetReference("RPSGame").Child(RPSGameRoomChangedHandle_SaveSeries).Child(RPSGameRoomChangedHandle_SaveMyRoom)
                 .ValueChanged -= RPSGameRoomChangedHandle;

            Debug.Log("@@@@@@@ AddHandler Remove  " + RPSGameRoomChangedHandle_SaveSeries + " " + RPSGameRoomChangedHandle_SaveMyRoom);

            RPSGameRoomChangedHandle = null;
            RPSGameRoomChangedHandle_SaveSeries = "";
            RPSGameRoomChangedHandle_SaveMyRoom = "";
        }

        FirebaseDatabase.DefaultInstance
                 .GetReference("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
                 .ValueChanged += HandleGameRoomChanged;

        RPSGameRoomChangedHandle_SaveSeries = FirebaseRPSGameSeries.ToString();
        RPSGameRoomChangedHandle_SaveMyRoom = FirebaseRPSGameMyRoom.ToString();
        RPSGameRoomChangedHandle = HandleGameRoomChanged;

        Debug.Log("@@@@@@@ AddHandler Add  " + RPSGameRoomChangedHandle_SaveSeries + " " + RPSGameRoomChangedHandle_SaveMyRoom);
    }

    public void SelectRPSGame(int Value)
    {
        mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
        .Child(FirebaseRPSGameMyPosition.ToString()).Child("Value").SetValueAsync(Value);
    }

    public void ResetMyRPSGame()
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyPosition").SetValueAsync(-1);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("FirebaseRPSGameMyRoom").SetValueAsync(-1);
    }

    // 가위 : 1 바위 : 2 보 : 3
    // 0 비김 , 1 이김, 2 짐

    //public void CheckRPSGame()
    //{
    //    var result = (3 + tempMyValue - FirebaseRPSGame_EnemyValue) % 3;

    //    if(result == 0)
    //    {
    //        // 비김
    //    }
    //    else if (result == 1)
    //    {

    //        //이김

    //        FirebaseDatabase.DefaultInstance
    //          .GetReference("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
    //          .ValueChanged -= HandleGameRoomChanged;

    //        mDatabaseRef.Child("RPSUserCount").RunTransaction(mutableData =>
    //        {
    //        int tempCount = Convert.ToInt32(mutableData.Value);

    //        if (tempCount == 0)
    //        {
    //            tempCount = 0;
    //        }
    //        else
    //        {
    //            FirebaseRPSGameMyRoom = (tempCount - 1) / 2;

    //            // 핸드러를 달기위해서 클라에서 변경
    //            FirebaseRPSGameSeries += 1;

    //            /*                     
    //               mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
    //               .Child(TKManager.Instance.MyData.Index).Child("Index").SetValueAsync(TKManager.Instance.MyData.Index);

    //               mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
    //               .Child(TKManager.Instance.MyData.Index).Child("NickName").SetValueAsync(TKManager.Instance.MyData.NickName);

    //               mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
    //               .Child(TKManager.Instance.MyData.Index).Child("Value").SetValueAsync(0);

    //                mutableData.Value = tempCount + 1;

    //                AddHandler();
    //           */


    //                SetRecommenderCode();

    //            mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
    //            .Child(tempIndex.ToString()).Child("NickName").SetValueAsync(GetRecommenderCode());

    //            mDatabaseRef.Child("RPSGame").Child(FirebaseRPSGameSeries.ToString()).Child(FirebaseRPSGameMyRoom.ToString())
    //            .Child(tempIndex.ToString()).Child("Value").SetValueAsync(0);

    //            tempIndex += 1;


    //            mutableData.Value = tempCount + 1;

    //            AddHandler();

    //            }

    //            return TransactionResult.Success(mutableData);

    //        });

    //    }
    //    else if (result == 2)
    //    {
    //        //짐
    //    }

    //}

    // 공지사항 파이어베이스에서 받아오기
    public void GetReviewRank()
    {
        mDatabaseRef.Child("ReviewRank").GetValueAsync().ContinueWith(task =>
       {

           if (task.IsFaulted)
           {
               // Handle the error...
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               if (snapshot != null && snapshot.Exists)
               {
                   foreach (var tempChild in snapshot.Children)
                   {
                       var tempData = tempChild.Value as Dictionary<string, object>;
                       var tempId = tempData["id"].ToString();
                       var tempScore = Convert.ToInt32(tempData["score"].ToString());

                       TKManager.Instance.ReviewRank.Add(new KeyValuePair<string, int>(tempId, tempScore));
                   }
               }

               AddFirstLoadingComplete();
           }
       }
      );

    }

    public void GetRPSGameEnterStatus()
    {
        mDatabaseRef.Child("RPSUserList").Child(TKManager.Instance.MyData.Index).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.Exists)
                {
                    // 있다
                    FirebaseRPSGameEnterEnable = true;
                }
                else
                    FirebaseRPSGameEnterEnable = false;

                AddFirstLoadingComplete();
            }
        }

        );

    }

    public void GetRPSGameEnterTime()
    {
        mDatabaseRef.Child("RPSGameEnterTime").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }

            else if (task.IsCompleted)
            {
                // 파베 데이터베이스 루트에 RPSGameEnterTime 추가
                // 하단에 [ 540, 900 ] 추가
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    foreach (var tempChild in snapshot.Children)
                    {
                        var min = long.Parse(tempChild.Value.ToString());
                        var StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        StartTime = StartTime.AddMinutes(min);
                        var EndTime = StartTime.AddHours(3);

                        TKManager.Instance.RPSEnterTimeList.Add(new KeyValuePair<long, long>(StartTime.Ticks, EndTime.Ticks));
                    }
                }
                AddFirstLoadingComplete();
            }
        });

    }

    public void GetRPSGamePlayTime()
    {
         long[] tempFirebaseRPSGamePlayTime = new long[2];

        mDatabaseRef.Child("RPSGamePlayTime").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }

            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    foreach (var tempChild in snapshot.Children)
                    {
                        var min = long.Parse(tempChild.Value.ToString());
                        var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        date = date.AddMinutes(min);
                        TKManager.Instance.RPSPlayStartTimeList.Add(date.Ticks);
                    }            
                }
                AddFirstLoadingComplete();
            }

        });

    }


    public void GetRPSGameWinnerGroup()
    {
        FirebaseDatabase.DefaultInstance.GetReference("RPSGameWinnerGroup").OrderByKey().LimitToLast(5)
       .GetValueAsync().ContinueWith(task =>
       {
           if (task.IsFaulted)
           {
               // Handle the error...
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               if (snapshot != null && snapshot.Exists)
               {
                   foreach (var tempChild in snapshot.Children)
                   {
                       var tempData = tempChild.Value as Dictionary<string, object>;
                       var tempFirstNickName = tempData["first"].ToString();
                       var tempSecondNickName = tempData["second"].ToString();
                       int tempCount = Convert.ToInt32(tempChild.Key);


                       bool plusEnable = true;
                       for (int i = 0; i < TKManager.Instance.RPSWinUserList.Count; i++)
                       {
                           if (TKManager.Instance.RPSWinUserList[i].Count == tempCount)
                           {
                               plusEnable = false;
                               break;
                           }

                       }
                       if (plusEnable)
                       {
                           TKManager.RPSGameWinnerData tempWinnerData = new TKManager.RPSGameWinnerData();
                           tempWinnerData.FirstName = tempFirstNickName;
                           tempWinnerData.SecondName = tempSecondNickName;
                           tempWinnerData.Count = tempCount;
                           TKManager.Instance.RPSWinUserList.Add(tempWinnerData);

                           Debug.Log("##### RPSGameWinnerGroup " + tempCount);
                       }
                   }
               }

               AddFirstLoadingComplete();

           }
       });


    }



    public void AddAdsCount(int count)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("AdsViewCount").SetValueAsync(count);
    }


    public void AddTotalPoint(int point)
    {
        DateTime dt = DateTime.Today;
        var tempToday = dt.ToString("yyyyMMdd");

        mDatabaseRef.Child("TotalPoint").Child(tempToday).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    var tempAdsView = Convert.ToInt32(snapshot.Value);

                    mDatabaseRef.Child("TotalPoint").Child(tempToday).RunTransaction(mutableData =>
                    {
                        int tempCount = Convert.ToInt32(mutableData.Value);
                        if (tempCount == 0)
                        {
                            tempCount = 1;
                        }
                        else
                        {
                            mutableData.Value = tempCount + point;
                        }
                        return TransactionResult.Success(mutableData);
                    });
                }
                else
                {
                    mDatabaseRef.Child("TotalPoint").Child(tempToday).SetValueAsync(point);
                }

            }
        }
    );             
    }


    // 닉네임 중복 체크
    public void IsExistNickName(string NickName, Action endAction)
    {
        SetEndCallFunc(endAction);
        NickNameExist = false;

        mDatabaseRef.Child("UserNameList").Child(NickName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;


                if (snapshot != null)
                {
                    if (snapshot.Exists || snapshot.Value != null)
                    {
                        NickNameExist = true;
                    }
                    else
                    {
                        NickNameExist = false;
                    }
                }
                else
                    NickNameExist = false;



                FirebaseProgress = false;
                //  Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
            }
        });
    }

    // 포인트 쌓이는 모드인지 판단
    public void IsAcquirePointMode()
    {
        var rtValue = 0;

        mDatabaseRef.Child("AcquirePointMode").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    rtValue = Convert.ToInt32(snapshot.Value);
                }
                else
                {
                    rtValue = 0;
                }
                if (rtValue >= 1)
                    AcquirePointMode = true;
                else
                    AcquirePointMode = false;
            }
            AddFirstLoadingComplete();
        }
    );
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetEndCallFunc(Action func)
    {
        FirebaseProgress = true;
        FirebaseProgressEndCallFunc = func;
        StartCoroutine(Co_AdEndCall());
    }

    private IEnumerator Co_AdEndCall()
    {
        TKManager.Instance.ShowHUD();
        while (true)
        {
            if (FirebaseProgress == false)
                break;

            yield return null;
        }

        TKManager.Instance.HideHUD();

        if (FirebaseProgressEndCallFunc != null)
            FirebaseProgressEndCallFunc();
    }


}
