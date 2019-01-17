﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


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

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    private string googleIdToken;
    private string googleAccessToken;
    private DatabaseReference mDatabaseRef;
    int LottoRefNnumber = 0;
    int LottoCurSeries = 0;
    int LottoTodaySeries = 0;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pointgame-2177a.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        GetUserData();
        GetLottoRefNumber();
        GetLottoTodaySeries();
        GetLottoCurSeries();
        GetLottoLuckyNumber();
        GetLottoLuckGroup();

        GetGiftProb();
        
      //  GetGiftImage();

      //  SetLottoNumber();

        if (!SingedInFirebase())
        {
            //            LogInByGoogle();
        }
    }

    public bool SingedInFirebase()
    {

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            user = auth.CurrentUser;
            return true;
        }

        return false;
    }

    private void LogInByGoogle()
    {
        Firebase.Auth.Credential credential =
       Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    // 사용자 정보 파이어베이스에 세팅
    public void SetUserData()
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Index").SetValueAsync(TKManager.Instance.MyData.Index);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("NickName").SetValueAsync(TKManager.Instance.MyData.NickName);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Point").SetValueAsync(TKManager.Instance.MyData.Point);
    }

    // 사용자 정보 파이어베이스에서 로드
    public void GetUserData()
    {
        string userIdx = "0";

        mDatabaseRef.Child("Users").Child(userIdx).GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                var tempData = snapshot.Value as Dictionary<string, object>;
                int tempPoint = Convert.ToInt32(tempData["Point"]);
                TKManager.Instance.MyData.SetData(tempData["Index"].ToString(), tempData["NickName"].ToString(), tempPoint);

                int tempLottoCount = Convert.ToInt32(tempData["LottoCount"]);
                if (tempLottoCount != 0)
                {
                    var LottoInfo = tempData["Lotto"] as Dictionary<string, object>;
                    foreach (var pair in LottoInfo)
                    {
                        string tempLottoSeries = pair.Key.Substring(0, pair.Key.IndexOf("_"));
                        int tempLottoNumber = Convert.ToInt32(pair.Value);
                        TKManager.Instance.MyData.SetLottoData(Convert.ToInt32(tempLottoSeries), tempLottoNumber);
                          Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
                    }
                }

                int tempGiftCount = Convert.ToInt32(tempData["GiftCount"]);
                if (tempGiftCount != 0)
                {
                    var GiftInfo = tempData["Gift"] as Dictionary<string, object>;
                    foreach (var pair in GiftInfo)
                    {
                        string tempIndex = pair.Key.Substring(0, pair.Key.IndexOf("_"));
                        TKManager.Instance.MyData.SetGiftconData(Convert.ToInt32(tempIndex), pair.Value.ToString());
                      //  Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
                    }
                }
              
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
    public void GetPoint()
    {
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
                rtPoint = Convert.ToInt32(snapshot.Value);
            }
        });
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

                foreach (var tempChild in snapshot.Children)
                {
                    var tempData = tempChild.Value as Dictionary<string, object>;
                    String tempProb = tempData["Prob"].ToString();
                    String tempName = tempData["Name"].ToString();

                    if(TKManager.Instance.RoulettePercent.Count <= 0)
                        TKManager.Instance.RoulettePercent.Add(new KeyValuePair<int, int>(Convert.ToInt32(tempName), Convert.ToInt32(tempProb)));
                    else
                    {
                        var tempValue = TKManager.Instance.RoulettePercent[TKManager.Instance.RoulettePercent.Count - 1].Value;
                        TKManager.Instance.RoulettePercent.Add(new KeyValuePair<int, int>(Convert.ToInt32(tempName), tempValue + Convert.ToInt32(tempProb)));
                    }


                    //  Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);

                }
            }
        });
    }

    // 상품권 이미지 주소
    public void GetGiftImage()
    {
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

               foreach (var tempChild in snapshot.Children)
               {
                   String tempIndex = tempChild.Key;
                   String tempSrc = tempChild.Value.ToString();
              
                   mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Gift").Child(tempIndex).SetValueAsync(tempSrc);
                   tempIndex = tempIndex.Substring(0, tempIndex.IndexOf("_"));
                   TKManager.Instance.MyData.SetGiftconData(Convert.ToInt32(tempIndex), tempSrc);

               }
           }
       });

    }

    // 로또 번호 파이어베이스에서 로드
    public void SetLottoNumber()
    {
        mDatabaseRef.Child("LottoCount").RunTransaction(mutableData =>
        {
            int tempCount = Convert.ToInt32(mutableData.Value); 

            if (tempCount == 0)
            {
                tempCount = 0;
            }
            else
            {
                //int tempSeries = GetCurrSeries();
                int tempSeries = LottoCurSeries;

               mDatabaseRef.Child("Lotto").Child(tempSeries + "_L").Child(TKManager.Instance.MyData.NickName).SetValueAsync(LottoRefNnumber * tempCount);
               mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Lotto").Child(tempSeries + "_L").SetValueAsync(LottoRefNnumber * tempCount);

               TKManager.Instance.MyData.SetLottoData(tempSeries, LottoRefNnumber * tempCount);

               mutableData.Value = tempCount + 1;
               mDatabaseRef.Child("LottoCount").SetValueAsync(mutableData.Value);
            }

          
            return TransactionResult.Success(mutableData);
        });
    }

    public void GetLottoLuckGroup()
    {


        FirebaseDatabase.DefaultInstance.GetReference("LottoLuckyGroup").OrderByKey().LimitToLast(4)
       .GetValueAsync().ContinueWith(task =>
       {
           if (task.IsFaulted)
           {
               // Handle the error...
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;

               foreach (var tempChild in snapshot.Children)
               {
                   String tempIndex = tempChild.Key;
                   String tempSrc = tempChild.Value.ToString();
                   
                   tempIndex = tempIndex.Substring(0, tempIndex.IndexOf("_"));
                   TKManager.Instance.SetLottoWinUserData(Convert.ToInt32(tempIndex), tempSrc);

               }
           }
       });


        mDatabaseRef.Child("LottoLuckyGroup").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                LottoTodaySeries = Convert.ToInt32(snapshot.Value);
                TKManager.Instance.SetTodayLottoSeriesMinCount(LottoTodaySeries);
            }
        }
     );

    }
    public void GetLottoTodaySeries()
    {
        mDatabaseRef.Child("LottoTodaySeries").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                LottoTodaySeries = Convert.ToInt32(snapshot.Value);
                TKManager.Instance.SetTodayLottoSeriesMinCount(LottoTodaySeries);
            }
        }
     );
    }

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
                LottoCurSeries = Convert.ToInt32(snapshot.Value);
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
                LottoRefNnumber = Convert.ToInt32(snapshot.Value);
            }
        }
      );
    }

    // 로또 당첨 번호 파이어베이스에서 로드
    public void GetLottoLuckyNumber()
    {

        mDatabaseRef.Child("LottoLuckyNumber").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (var tempChild in snapshot.Children)
                {
                    String tempSeries = tempChild.Key;
                    int tempNumber = Convert.ToInt32(tempChild.Value.ToString());

                    tempSeries = tempSeries.Substring(0, tempSeries.IndexOf("_"));
                    TKManager.Instance.SetLottoLuckyNumber(Convert.ToInt32(tempSeries), tempNumber);
                }

            }
        }
      );
    }

    private int GetCurrSeries()
    {
        int rtSeries = 0;
        System.DateTime.Now.ToString("yyyy");
        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        rtSeries = Convert.ToInt32(DateTime.Now.ToString("hh"));

        Debug.LogFormat("asdasdasd    {0} ", rtSeries);

        if (rtSeries < 9)
        {
            rtSeries = 0;
        }
        else if( 9 <= rtSeries && rtSeries < 12)
        {
            rtSeries = 1;
        }
        else if (12 <= rtSeries && rtSeries < 15)
        {
            rtSeries = 2;
        }
        else if (15 <= rtSeries && rtSeries < 18)
        {
            rtSeries = 3;
        }
    
        return rtSeries;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
