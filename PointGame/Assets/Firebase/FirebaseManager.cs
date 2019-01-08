using System;
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

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pointgame-2177a.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        GetUserData();
        GetLottoRefNumber();
        GetGiftProb();

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
                String tempPoint = tempData["Point"].ToString();
                TKManager.Instance.MyData.SetData(tempData["Index"].ToString(), tempData["NickName"].ToString(), Convert.ToInt32(tempPoint));
                Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);
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
                rtPoint = (int)snapshot.Value;
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

                    TKManager.Instance.RoulettePercent.Add(new KeyValuePair<int, int>(Convert.ToInt32(tempName), Convert.ToInt32(tempProb)));

                    Debug.LogFormat("UserInfo: Index : {0} NickName {1} Point {2}", TKManager.Instance.MyData.Index, TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point);

                    //TKManager.Instance.MyData.SetData(tempData["Index"].ToString(), tempData["NickName"].ToString(), Convert.ToInt32(tempPoint));
                }
            }
        });
    }

    // 상품권 이미지 주소
    public void GetGiftImage()
    {
        string GiftImageSrc = null;

        FirebaseDatabase.DefaultInstance.GetReference("Gift").OrderByKey().LimitToLast(1)
       .GetValueAsync().ContinueWith(task =>
       {
           if (task.IsFaulted)
           {
               // Handle the error...
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               GiftImageSrc = (string)snapshot.Value;
           }
       });
    }

    // 로또 번호 파이어베이스에서 로드
    public void SetLottoNumber()
    {
        int lottoCount = 0;

        mDatabaseRef.Child("lottocount").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                lottoCount = (int)snapshot.Value;

                mDatabaseRef.Child("lotto").Child(lottoCount.ToString()).Child("User").SetValueAsync(TKManager.Instance.MyData.Index);
                mDatabaseRef.Child("lotto").Child(lottoCount.ToString()).Child("Number").SetValueAsync(LottoRefNnumber * lottoCount);

                mDatabaseRef.Child("lottocount").RunTransaction(mutableData =>
                {
                    int tempCount = (int)mutableData.Value;

                    if (tempCount == 0)
                    {
                        tempCount = 0;
                    }

                    mutableData.Value = tempCount++;
                    return TransactionResult.Success(mutableData);
                });
            }
        }
      );
    }

    // 로또 레퍼런스 번호 파이어베이스에서 로드
    public void GetLottoRefNumber()
    {
        mDatabaseRef.Child("lottoRefNumber").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                LottoRefNnumber = (int)snapshot.Value;
            }
        }
      );
    }

    // 로또 당첨 번호 파이어베이스에서 로드
    public void GetLottoLuckyNumber()
    {
        int rtLottonumber = 0;

        mDatabaseRef.Child("lottoLuckyNumber").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                rtLottonumber = (int)snapshot.Value;
            }
        }
      );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
