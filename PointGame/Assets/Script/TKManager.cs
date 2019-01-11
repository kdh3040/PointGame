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

        // TODO 임시
        CurrentLottoSeriesCount = 1;
    }

    public void Temp_LottoNumberGet()
    {
        MyData.MyLottoNumber = Random.Range(52534, 123424);
        MyData.MyLottoSeriesCount = CurrentLottoSeriesCount;
    }

    public void Temp_LuckyLotto()
    {
        ResultLottoSeriesCount = CurrentLottoSeriesCount;

        if (Random.Range(0, 2) == 1)
            ResultLottoNumber = MyData.MyLottoNumber;
        else
            ResultLottoNumber = Random.Range(52534, 123424);

        CurrentLottoSeriesCount++;
    }
}