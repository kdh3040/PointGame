using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour {

    public static GamePlayManager _instance = null;
    public static GamePlayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GamePlayManager>() as GamePlayManager;
            }
            return _instance;
        }
    }

    public List<GameStairs> StairsList = new List<GameStairs>();
    public GameChar Char;
    public GameUI UI;

    void Start()
    {
        // 게임에 들어 올때마다 생성

        //DontDestroyOnLoad(this);
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Application.targetFrameRate = 50;
        //Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);

        //PlayerData.Instance.Initialize();

        GameFirstReady();
    }

    public void GameFirstReady()
    {
        // 게임 화면 들어와서 준비
    }

    public void GameFirstStart()
    {
        // 게임 화면 들어와서 시작
    }

    public void GameClear()
    {
        // 스테이지 클리어
    }

    public void GameStart()
    {
        // 스테이지 클리어후 시작
    }

    public void GameEnd()
    {
        // 스테이지 종료
    }

    private void Update()
    {
        
        
    }

    public void CharJump(bool left)
    {
        // 좌우 터치가 들어 왔을떄 점프 처리
        // 점프액션이 두번 들어오지 않게 막아야함
    }

}
