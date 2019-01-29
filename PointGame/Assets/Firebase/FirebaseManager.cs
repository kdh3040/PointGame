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

    public Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
   

    private string googleIdToken;
    private string googleAccessToken;
    public DatabaseReference mDatabaseRef;
    int LottoRefNnumber = 0;
    int LottoCurSeries = 0;
    int LottoTodaySeries = 0;

    public bool FirstLoadingComplete = false;
    public int LoadingCount = 0;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
   
        LoadingCount = 0;
        FirstLoadingComplete = false;

    }

    public void Init()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://treasureone-4472e.firebaseio.com/");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public void TokenRefresh(Firebase.Auth.FirebaseUser user)
    {

        user.TokenAsync(true).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.Log("!!!!! TokenAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.Log("!!!!! TokenAsync encountered an error: " + task.Exception);
                return;
            }

            string idToken = task.Result;

            Debug.Log("!!!!! Token: " + idToken);
        });

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

        GetLottoRefNumber();
        //GetLottoTodaySeries();
        GetLottoCurSeries();
        GetLottoLuckyNumber();
        GetLottoLuckGroup();

        GetGiftProb();
    }



    private void AddFirstLoadingComplete()
    {
        if (FirstLoadingComplete == false)
            LoadingCount++;

        if (LoadingCount == 6)
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

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TotalAccumPoint").SetValueAsync(0);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TodayAccumPoint").Child(GetToday()).SetValueAsync(0);
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Cash").SetValueAsync(0);

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

                if(snapshot.Exists)
                {
                    var tempData = snapshot.Value as Dictionary<string, object>;
                    int tempPoint = Convert.ToInt32(tempData["Point"]);
                    TKManager.Instance.MyData.SetData(tempData["Index"].ToString(), tempData["NickName"].ToString(), tempPoint);

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
                        TKManager.Instance.MyData.AddCash(tempCash);
                    }
                    else
                    {
                        TKManager.Instance.MyData.AddCash(0);
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
                if (snapshot != null && snapshot.Exists)
                    rtPoint = Convert.ToInt32(snapshot.Value);
                else
                    rtPoint = 0;

            }
        });
    }
    
    public void SetTotalAccumPoint(int point)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TotalAccumPoint").SetValueAsync(point);
    }
    public void GetTotalAccumPoint()
    {
        int rtPoint = 0;

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TotalAccumPoint").GetValueAsync().ContinueWith(task =>
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
            }
        });
    }

    public void SetTodayAccumPoint(int point)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TodayAccumPoint").Child(GetToday()).SetValueAsync(point);
    }
    public void GetTodayAccumPoint()
    {
        int rtPoint = 0;

        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("TodaytAccumPoint").Child(GetToday()).GetValueAsync().ContinueWith(task =>
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
            }
        });
    }

    public void SetCash(int Cash)
    {
        mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Cash").SetValueAsync(Cash);
    }
    public void GetCash()
    {
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
            }
        });
    }
    public void SetCashInfo(String Name, String BankName, String Account, int CachBack)
    {
        mDatabaseRef.Child("CashBack").Child(TKManager.Instance.MyData.Index).Child("Name").SetValueAsync(Name);
        mDatabaseRef.Child("CashBack").Child(TKManager.Instance.MyData.Index).Child("BankName").SetValueAsync(BankName);
        mDatabaseRef.Child("CashBack").Child(TKManager.Instance.MyData.Index).Child("Account").SetValueAsync(Account);
        mDatabaseRef.Child("CashBack").Child(TKManager.Instance.MyData.Index).Child("CachBack").SetValueAsync(CachBack);
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
                tempSeries += CommonData.LottoRefSeries;

               mDatabaseRef.Child("Lotto").Child(tempSeries + "_L").Child(TKManager.Instance.MyData.NickName).SetValueAsync(LottoRefNnumber * tempCount);
               mDatabaseRef.Child("Users").Child(TKManager.Instance.MyData.Index).Child("Lotto").Child(tempSeries + "_L").SetValueAsync(LottoRefNnumber * tempCount);

                tempSeries -= CommonData.LottoRefSeries;

                TKManager.Instance.MyData.SetLottoData(tempSeries, LottoRefNnumber * tempCount);

               mutableData.Value = tempCount + 1;
               mDatabaseRef.Child("LottoCount").SetValueAsync(mutableData.Value);

                TKManager.Instance.GetLottoNumberProgress = false;
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

    // Update is called once per frame
    void Update()
    {

    }
}
