using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class LoadingUI : MonoBehaviour {

    public SignUpPopup SignUpPopupObj;
    public bool GetLoginProgress = false;

    void Start()
    {
        FirebaseManager.Instance.Init();
        TKManager.Instance.init();

        TKManager.Instance.ShowHUD();

        TKManager.Instance.LoadFile();
#if (UNITY_ANDROID && !UNITY_EDITOR)
        SignUpPopupObj.gameObject.SetActive(false);
        if (TKManager.Instance.MyData.Index == null)
        {
            GetLoginProgress = true;

            StartCoroutine(Co_Login());
            SignUpAnonymously();
        }
        else
        {

            if (FirebaseManager.Instance.SingedInFirebase())
            {
                StartCoroutine(LoadingData());
            }
            else
            {
                GetLoginProgress = true;

                StartCoroutine(Co_Login());
                SignUpAnonymously();
            }
        }
#elif (UNITY_ANDROID && UNITY_EDITOR) || UNITY_IOS
        SignUpPopupObj.gameObject.SetActive(false);
        if (TKManager.Instance.MyData.Index != null && TKManager.Instance.FirebaseUserId != string.Empty)
        {
            StartCoroutine(LoadingData());
        }
        else
        {
            GetLoginProgress = true;

            StartCoroutine(Co_Login());
            SignUpAnonymously();
        }
#endif




    }


    public void SignUpAnonymously()
    {
        FirebaseManager.Instance.auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            FirebaseManager.Instance.TokenRefresh(newUser);


#if UNITY_IOS || UNITY_EDITOR
            TKManager.Instance.FirebaseUserId = newUser.UserId;
            TKManager.Instance.SaveFile();
#endif
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            TKManager.Instance.HideHUD();
            SignUpPopupObj.gameObject.SetActive(true);
            SignUpPopupObj.init(SignUpUser);
        });
    }

    public void SignUpUser(string NickName)
    {
        FirebaseManager.Instance.mDatabaseRef.Child("UsersCount").RunTransaction(mutableData =>
        {
            int tempCount = Convert.ToInt32(mutableData.Value);

            if (tempCount == 0)
            {
                tempCount = 0;
            }
            else
            {
                TKManager.Instance.MyData.SetData(tempCount.ToString(), NickName, CommonData.UserDefaultPoint);

                FirebaseManager.Instance.SetUserData();

                mutableData.Value = tempCount + 1;
                FirebaseManager.Instance.mDatabaseRef.Child("UsersCount").SetValueAsync(mutableData.Value);

                GetLoginProgress = false;
            }

            return TransactionResult.Success(mutableData);
        });
    }

    IEnumerator Co_Login()
    {
        while (true)
        {
            if (GetLoginProgress == false)
                break;

            yield return null;
        }

        TKManager.Instance.SaveFile();

        StartCoroutine(LoadingData());
    }

    IEnumerator LoadingData()
    {
        TKManager.Instance.ShowHUD();
        yield return null;
        TKManager.Instance.LoadFile();
        yield return null;
        FirebaseManager.Instance.GetData();
        yield return null;

        while (true)
        {
            if (FirebaseManager.Instance.FirstLoadingComplete)
                break;

            yield return null;
        }

        var giftList = TKManager.Instance.MyData.GiftconURLList;
        var giftUrlList = new List<string>();
        for (int i = 0; i < giftList.Count; i++)
        {
            giftUrlList.Add(giftList[i].Value);
        }

        yield return ImageCache.Instance.GetTexture(giftUrlList);

        yield return null;
        TKManager.Instance.HideHUD();
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
