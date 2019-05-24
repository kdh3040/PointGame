using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class TKManager : MonoBehaviour
{
    public static TKManager _instance = null;
    public static TKManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TKManager>() as TKManager;
            }
            return _instance;
        }
    }

    public UserData MyData = null;

    public bool GameOverRouletteStart = false;
    public List<KeyValuePair<int, int>> RoulettePercent = new List<KeyValuePair<int, int>>();

    public List<KeyValuePair<int, string>> LottoWinUserList = new List<KeyValuePair<int, string>>();
    public List<KeyValuePair<int, int>> LottoLuckyNumber = new List<KeyValuePair<int, int>>();

    public List<RPSGameWinnerData> RPSWinUserList = new List<RPSGameWinnerData>();
    public struct RPSGameWinnerData
    {
        public string FirstName;
        public string SecondName;
        public int Count;
    }
    public List<KeyValuePair<long, long>> RPSEnterTimeList = new List<KeyValuePair<long, long>>();
    public List<long> RPSPlayStartTimeList = new List<long>();

    // 0 베이스
    public int CurrLottoSeriesCount = 0;
    public int LottoSeriesCountMin = 0;
    //public int LottoSeriesCountMax = 0;

    public LoadingHUD HUD;
    private SaveData MySaveData = new SaveData();

    public int PushLastIndex = 0;
    public bool PushNotiEnable = false;

    public string FirebaseUserId = "";

    public int ReviewRankPlusScore = 0;
    public List<KeyValuePair<string, int>> ReviewRank = new List<KeyValuePair<string, int>>();

    public List<KeyValuePair<string, string>> RecommendUsers = new List<KeyValuePair<string, string>>();
    public bool MainUIView = false;
    public bool SoundMute { get; private set; }
    
    void Start()
    {
        DontDestroyOnLoad(this);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 50;
#if UNITY_ANDROID || UNITY_EDITOR
        Screen.SetResolution((int)Screen.safeArea.width, ((int)Screen.safeArea.width * 16) / 9, false);
#endif
        //Screen.SetResolution((int)Screen.safeArea.width, ((int)Screen.safeArea.width * 16) / 9, false);
    }

    public void init()
    {
        MyData = new UserData();
    }

    public bool IsLottoLuckyUser(int series)
    {
        for (int i = 0; i < LottoWinUserList.Count; i++)
        {
            if (LottoWinUserList[i].Key == series)
                return true;
        }

        return false;
    }

    public void SetLottoWinUserData(int series, string nickName)
    {
        if(!IsLottoLuckyUser(series))
        {
            LottoWinUserList.Add(new KeyValuePair<int, string>(series, nickName));
        }        
    }

    public void SetCurrentLottoSeriesCount(int count)
    {
        CurrLottoSeriesCount = count;
        LottoSeriesCountMin = CurrLottoSeriesCount - 3;
    }

    public void SetLottoLuckyNumber(int LottoSeries, int LottoNumber)
    {
        if (IsLottoLuckyNumber(LottoSeries))
        {
            for (int i = 0; i < LottoLuckyNumber.Count; i++)
            {
                if (LottoLuckyNumber[i].Key == LottoSeries)
                {
                    LottoLuckyNumber[i] = new KeyValuePair<int, int>(LottoSeries, LottoNumber);
                    break;
                }
            }
        }
        else
            LottoLuckyNumber.Add(new KeyValuePair<int, int>(LottoSeries, LottoNumber));

        LottoLuckyNumber.Sort(delegate (KeyValuePair<int, int> a, KeyValuePair<int, int> b)
        {
            if (a.Key < b.Key)
                return -1;
            return 1;
        });

        //CurrLottoSeriesCount = LottoLuckyNumber[LottoLuckyNumber.Count - 1].Key + 1;
    }

    public bool IsLottoLuckyNumber(int LottoSeries)
    {
        for (int i = 0; i < LottoLuckyNumber.Count; i++)
        {
            if (LottoLuckyNumber[i].Key == LottoSeries)
                return true;
        }

        return false;
    }

    public KeyValuePair<int, int> GetLottoLuckyNumber(int LottoSeries)
    {
        for (int i = 0; i < LottoLuckyNumber.Count; i++)
        {
            if (LottoLuckyNumber[i].Key == LottoSeries)
                return LottoLuckyNumber[i];
        }

        return new KeyValuePair<int, int>(0,0);
    }


    public void ShowHUD(string msg = "로딩중", string subMsg = "", float waitTime = 0, string waitTimeMsg = "")
    {
        HUD.gameObject.SetActive(true);
        HUD.ShowHUD(msg, subMsg, waitTime, waitTimeMsg);
    }

    public void HideHUD()
    {
        HUD.gameObject.SetActive(false);
        HUD.HideHUD();
    }

    public void SetSoundMute(bool enable)
    {
        SoundMute = enable;
        SaveFile();
    }
    private void Update()
    {
        if(MainUIView == false)
        {
            if (FirebaseManager.Instance.FirebaseRPSGameSeries > -1 &&
            FirebaseManager.Instance.FirebaseRPSGameEnterEnable)
            {
                FirebaseManager.Instance.FirebaseRPSGameEnterEnable = false;
            }
        }
        
    }

    public bool IsRPSEnterTime()
    {
        for (int i = 0; i < RPSEnterTimeList.Count; i++)
        {
            if(RPSEnterTimeList[i].Key <= DateTime.Now.Ticks &&
               RPSEnterTimeList[i].Value >= DateTime.Now.Ticks)
            {
                return true;
            }
        }

        return false;
    }

    public int ChangeCashToPoint(int cash)
    {
        if (cash < 0)
            return 0;

        int oneCashPoint = CommonData.PointToCashChange / CommonData.PointToCashChangeValue;
        int tempPoint = cash * oneCashPoint;

        int count = 0;
        int bonusCount = 0;
        while(false)
        {
            bool whileBreak = false;
            for (int i = 0; i < CommonData.ChangePointBonusArr.Length; i++)
            {
                if (tempPoint < CommonData.ChangePointBonusArr[i] + CommonData.ChangePointMax * count)
                {
                    whileBreak = true;
                    break;
                }

                bonusCount++;
            }

            count++;

            if (whileBreak)
                break;
        }

        tempPoint -= bonusCount * CommonData.BonusPoint;

        return tempPoint;
    }

    public int ChangePointToCash(int point)
    {
        if (point < 0)
            return 0;

        int tempPoint = point;
        //int tempCash = (int)(((float)tempPoint / (float)CommonData.PointToCashChange) * CommonData.PointToCashChangeValue);
        int tempCash = (tempPoint / CommonData.PointToCashChange) * CommonData.PointToCashChangeValue;

        int count = 0;
        int bonusCount = 0;
        while (false)
        {
            bool whileBreak = false;
            for (int i = 0; i < CommonData.ChangePointBonusArr.Length; i++)
            {
                if (tempPoint < CommonData.ChangePointBonusArr[i] + CommonData.ChangePointMax * count)
                {
                    whileBreak = true;
                    break;
                }

                bonusCount++;
            }

            count++;

            if (whileBreak)
                break;
        }

        tempCash += bonusCount * CommonData.BonusCash;

        return tempCash;
    }


    [System.Serializable]
    public class SaveData
    {
        public string UserIndex = "";
        public Dictionary<int, bool> LottoResultShowSeriesList = new Dictionary<int, bool>();
        public string FirebaseUserId = "";
        public int BestStage = 0;
        public int PushLastIndex = 0;
        public string RecommendCode = "";
        public bool SoundMute = false;

        public void Save()
        {
            LottoResultShowSeriesList = TKManager.Instance.MyData.LottoResultShowSeriesList;
            UserIndex = TKManager.Instance.MyData.Index;
            FirebaseUserId = TKManager.Instance.FirebaseUserId;
            BestStage = TKManager.Instance.MyData.BestStage;
            PushLastIndex = TKManager.Instance.PushLastIndex;
            RecommendCode = TKManager.Instance.MyData.RecommenderCode;
            SoundMute = TKManager.Instance.SoundMute;
        }

        public void Load()
        {
            if(LottoResultShowSeriesList != null)
                TKManager.Instance.MyData.LottoResultShowSeriesList = LottoResultShowSeriesList;

            if(UserIndex != null)
                TKManager.Instance.MyData.SetUserIndex(UserIndex);

            if(FirebaseUserId != null)
                TKManager.Instance.FirebaseUserId = FirebaseUserId;

            TKManager.Instance.MyData.BestStage = BestStage;
            TKManager.Instance.PushLastIndex = PushLastIndex;
            TKManager.Instance.MyData.RecommenderCode = RecommendCode;
            TKManager.Instance.SoundMute = SoundMute;
        }
    }



    public void SaveFile()
    {
        MySaveData.Save();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = pathForDocumentsFile("PlayerData.ini");
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, MySaveData);
        stream.Close();
    }

    public void LoadFile()
    {
        string path = pathForDocumentsFile("PlayerData.ini");
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            MySaveData = (SaveData)formatter.Deserialize(stream);
            stream.Close();
            MySaveData.Load();
        }
        else
        {
            SaveFile();
        }
    }

    public string pathForDocumentsFile(string filename)
    {
#if UNITY_EDITOR
        string path_pc = Application.dataPath;
        path_pc = path_pc.Substring(0, path_pc.LastIndexOf('/'));
        return Path.Combine(path_pc, filename);
#elif UNITY_ANDROID
        string path = Application.persistentDataPath;
        path = path.Substring(0, path.LastIndexOf('/'));
        return Path.Combine(path, filename);
#elif UNITY_IOS
        return Application.persistentDataPath + "/" + filename;
        //string path = Application.dataPath.Substring(0, Application.dataPath.Length);
        //path = path.Substring(0, path.LastIndexOf('/'));
        //return Path.Combine(Path.Combine(path, "Documents"), filename);
#endif
    }
}