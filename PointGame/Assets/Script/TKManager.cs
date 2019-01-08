using System.Collections;
using System.Collections.Generic;
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
    public string RouletteGiftconUrl = "http://attach.s.op.gg/forum/20171221114845_549392.jpg";

    public int CurrentLottoSeriesCount = 0;
    public int ResultLottoSeriesCount = 0;
    public int ResultLottoNumber = 0;

    void Start()
    {
        MyData = new UserData();

        DontDestroyOnLoad(this);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 50;
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);
    }
}