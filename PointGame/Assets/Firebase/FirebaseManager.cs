﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


public class FirebaseManager : MonoBehaviour {

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
    

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pointgame-2177a.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        GetUserData();

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
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
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

        mDatabaseRef.Child("Users").Child(userIdx).GetValueAsync().ContinueWith(task => {

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
        
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Point").GetValueAsync().ContinueWith(task => {

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


    // 로또 당첨 번호 파이어베이스에서 로드
    public void GetLottoNumber()
    {
        int rtLottonumber = 0;

        mDatabaseRef.Child("lottonumber").GetValueAsync().ContinueWith(task => {

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

    // 상품권 이미지 주소
    public void GetGiftImage()
    {
        string GiftImageSrc = null;

        FirebaseDatabase.DefaultInstance.GetReference("Gift").OrderByKey().LimitToLast(1)
       .GetValueAsync().ContinueWith(task => {
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

    // Update is called once per frame
    void Update () {
		
	}
}
