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

        // TODO 김도형 파이어베이스에서 확률을 받아와야함
        // TODO 배환웅 받아온 확률 데이터를 재가공 해야함

       // RoulettePercent.Add(new KeyValuePair<int, int>(0, 10));
        //RoulettePercent.Add(new KeyValuePair<int, int>(100, RoulettePercent[RoulettePercent.Count - 1].Value + 18));
        //RoulettePercent.Add(new KeyValuePair<int, int>(200, RoulettePercent[RoulettePercent.Count - 1].Value + 18));
        //RoulettePercent.Add(new KeyValuePair<int, int>(300, RoulettePercent[RoulettePercent.Count - 1].Value + 18));
        //RoulettePercent.Add(new KeyValuePair<int, int>(400, RoulettePercent[RoulettePercent.Count - 1].Value + 18));
        //RoulettePercent.Add(new KeyValuePair<int, int>(500, RoulettePercent[RoulettePercent.Count - 1].Value + 18));

        DontDestroyOnLoad(this);
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Application.targetFrameRate = 50;
        //Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);

        //PlayerData.Instance.Initialize();
    }
}