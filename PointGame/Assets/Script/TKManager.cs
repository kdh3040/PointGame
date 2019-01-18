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
    // 0 베이스
    public int CurrLottoSeriesCount = 0;
    public int LottoSeriesCountMin = 0;
    public int LottoSeriesCountMax = 0;

    public LoadingHUD HUD;
    private SaveData MySaveData = new SaveData();

    public bool GetLottoNumberProgress = false;

    void Start()
    {
        MyData = new UserData();
        
        DontDestroyOnLoad(this);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 50;
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);
    }

    public void SetLottoWinUserData(int series, string nickName)
    {
        LottoWinUserList.Add(new KeyValuePair<int, string>(series, nickName));
    }

    public void SetTodayLottoSeriesMinCount(int min)
    {
        LottoSeriesCountMin = min;
        LottoSeriesCountMax = LottoSeriesCountMin + 3;
        CurrLottoSeriesCount = LottoSeriesCountMin;
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

        CurrLottoSeriesCount = LottoLuckyNumber[LottoLuckyNumber.Count - 1].Key + 1;
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


    public void ShowHUD()
    {
        HUD.gameObject.SetActive(true);
    }

    public void HideHUD()
    {
        HUD.gameObject.SetActive(false);
    }





    [System.Serializable]
    public class SaveData
    {
        public Dictionary<int, bool> LottoResultShowSeriesList = new Dictionary<int, bool>();

        public void Save()
        {
            LottoResultShowSeriesList = TKManager.Instance.MyData.LottoResultShowSeriesList;
        }

        public void Load()
        {
            TKManager.Instance.MyData.LottoResultShowSeriesList = LottoResultShowSeriesList;
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
        string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
        path = path.Substring(0, path.LastIndexOf('/'));
        return Path.Combine(Path.Combine(path, "Documents"), filename);
#endif
    }
}