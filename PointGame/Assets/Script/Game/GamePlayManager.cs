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

    public int StageCount = 1;
    public List<GameBlock> BlockList = new List<GameBlock>();
    public GameObject Char;
    public GameUI UI;

    private bool IsGameStart = false;
    private Vector3 BlockCharCenterPos = new Vector3(0, 1.2f, -1f);
    private int BlockCharIndex = 0;

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
        StageCount = 1;

        // 블럭 생성
        AllResetBlock();
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
        if (IsGameStart)
        {
            for (int index = 0; index < BlockList.Count; ++index)
            {
                var pos = BlockList[index].gameObject.transform.localPosition;
                pos.y = pos.y - 0.05f;
                BlockList[index].gameObject.transform.localPosition = pos;

                if (pos.y < -2.5f)
                    ResetBlock(index);
            }
        }
    }

    public void CharJump(bool left)
    {
        // 좌우 터치가 들어 왔을떄 점프 처리
        // 점프액션이 두번 들어오지 않게 막아야함

        IsGameStart = true;

        BlockCharIndex = BlockCharIndex + 1;
        if (BlockCharIndex > BlockList.Count - 1)
            BlockCharIndex = 0;

        if(left)
        {
            Char.transform.parent = BlockList[BlockCharIndex].LeftBlock.transform;
            Char.transform.localPosition = BlockCharCenterPos;
        }
        else
        {
            Char.transform.parent = BlockList[BlockCharIndex].RightBlock.transform;
            Char.transform.localPosition = BlockCharCenterPos;
        }
        
        //Char.transform.position = BlockList[0].RightBlock.transform.InverseTransformPoint(BlockCharCenterPos);

    }

    public void AllResetBlock()
    {
        for(int index = 0; index < BlockList.Count; ++index)
        {
            // 맨 아래 있는 블럭은 안전 블럭
            if(index == 0)
            {
                BlockList[index].Initialize(GameBlock.BLOCK_TYPE.SAFE);
            }
            else
            {
                ResetBlock(index);
            }

            BlockList[index].gameObject.transform.localPosition = new Vector3(0, index * 3.5f);
        }
    }

    private void ResetBlock(int index)
    {
        int posIndex = index - 1;
        if (posIndex < 0)
            posIndex = BlockList.Count - 1;

        var nextPos = BlockList[posIndex].transform.localPosition;
        BlockList[index].gameObject.transform.localPosition = new Vector3(0, nextPos.y + 3.5f);

        GameBlock.BLOCK_TYPE type = (GameBlock.BLOCK_TYPE)Random.Range((int)GameBlock.BLOCK_TYPE.LEFT_SAW, (int)GameBlock.BLOCK_TYPE.RIGHT + 1);
        BlockList[index].Initialize(type);
    }
}
