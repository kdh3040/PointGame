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
    public GameChar Char;
    public GameUI UI;

    private bool IsGameStart = false;
    private Vector3 BlockCharCenterPos = new Vector3(0, 1.2f, -9f);
    private int BlockCharIndex = 0;
    private bool LastJumpLeft = true;
    private bool IsJumping = false;


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

    public void CheckGameOver()
    {
        if(Char.BlockLeftDir && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.LEFT_SAW ||
            Char.BlockLeftDir == false && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.RIGHT_SAW )
            IsGameStart = false;
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

            if( Char.gameObject.transform.TransformPoint(Vector3.zero).y < -10f)
                IsGameStart = false;
        }

    }

    public void CharJump(bool left)
    {
        // 좌우 터치가 들어 왔을떄 점프 처리
        // 점프액션이 두번 들어오지 않게 막아야함
        IsGameStart = true;
        if(IsJumping == false)
            StartCoroutine(Co_CharJump(left));
    }

    IEnumerator Co_CharJump(bool left)
    {
        IsJumping = true;
        float time = 1.0f;

        BlockCharIndex = BlockCharIndex + 1;
        if (BlockCharIndex > BlockList.Count - 1)
            BlockCharIndex = 0;

        var startPos = Vector3.zero;
        var centerPos = Vector3.zero;
        var endPos = Vector3.zero;
        Char.BlockLeftDir = left;

        if (left)
        {
            Char.transform.parent = BlockList[BlockCharIndex].LeftBlockObject.transform;
            startPos = Char.transform.localPosition;
            if(LastJumpLeft)
                centerPos = new Vector3(0, Char.transform.localPosition.y + 5, 0);
            else
                centerPos = new Vector3(3, Char.transform.localPosition.y + 5, 0);
            endPos = BlockCharCenterPos;
            LastJumpLeft = true;
        }
        else
        {
            Char.transform.parent = BlockList[BlockCharIndex].RightBlockObject.transform;
            startPos = Char.transform.localPosition;
            if (LastJumpLeft == false)
                centerPos = new Vector3(0, Char.transform.localPosition.y + 5, 0);
            else
                centerPos = new Vector3(-3, Char.transform.localPosition.y + 5, 0);
            endPos = BlockCharCenterPos;

            LastJumpLeft = false;
        }

        while(time > 0f)
        {
            time -= 0.1f;
            Char.transform.localPosition = BezierCurve(1.0f - time, startPos, centerPos, endPos);
            yield return null;
        }

        if (left && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.RIGHT ||
            left == false && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.LEFT)
            yield return Co_CharDown(left);

        Char.transform.localPosition = endPos;
        CheckGameOver();
        IsJumping = false;
    }

    IEnumerator Co_CharDown(bool left)
    {
        BlockCharIndex -= 1;
        if (BlockCharIndex < 0)
            BlockCharIndex = BlockList.Count - 1;

        if (BlockList[BlockCharIndex].gameObject.transform.localPosition.y < -9.6f)
        {
            // 게임오버
            yield break;
        }
            

        var startPos = Vector3.zero;
        var endPos = Vector3.zero;

        if (left)
        {
            Char.transform.parent = BlockList[BlockCharIndex].LeftBlockObject.transform;
            startPos = Char.transform.localPosition;
            endPos = BlockCharCenterPos;
        }
        else
        {
            Char.transform.parent = BlockList[BlockCharIndex].RightBlockObject.transform;
            startPos = Char.transform.localPosition;
            endPos = BlockCharCenterPos;
        }

        float time = 1.0f;
        while (time > 0f)
        {
            time -= 0.1f;
            Char.transform.localPosition = BezierCurve(1.0f - time, startPos, endPos);
            yield return null;
        }

        if (left && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.RIGHT ||
            left == false && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.LEFT)
            yield return Co_CharDown(left);

        Char.transform.localPosition = endPos;
        CheckGameOver();
    }


    Vector3 BezierCurve(float t, Vector3 p0, Vector3 p1)
    {
        return ((1 - t) * p0) + ((t) * p1);
    }

    Vector3 BezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3 pa = BezierCurve(t, p0, p1);
        Vector3 pb = BezierCurve(t, p1, p2);
        return BezierCurve(t, pa, pb);
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










    //public void CharJump(bool left)
    //{
    //    // 좌우 터치가 들어 왔을떄 점프 처리
    //    // 점프액션이 두번 들어오지 않게 막아야함
    //    IsGameStart = true;
    //    StartCoroutine(Co_CharJump(left));

    //    return;



    //    BlockCharIndex = BlockCharIndex + 1;
    //    if (BlockCharIndex > BlockList.Count - 1)
    //        BlockCharIndex = 0;

    //    if (left)
    //    {
    //        //Char.transform.parent = BlockList[BlockCharIndex].LeftBlock.transform;
    //        //Char.transform.localPosition = BlockCharCenterPos;
    //        //iTween.Stop(Char);
    //        //Char.transform.localPosition = BlockCharCenterPos;
    //        Char.transform.parent = BlockList[BlockCharIndex].LeftBlock.transform;

    //        //var path = new Vector3[3];
    //        //path[0] = Char.transform.localPosition;
    //        //path[1] = new Vector3(Char.transform.localPosition.x, Char.transform.localPosition.y + 3, 0);
    //        //path[2] = BlockCharCenterPos;

    //        //iTween.MoveTo(Char, iTween.Hash("path", path, "islocal", true, "movetopath", false, "time", 1f, "easetype", iTween.EaseType.linear));
    //        iTween.MoveTo(Char, iTween.Hash("position", BlockCharCenterPos, "islocal", true, "movetopath", false, "time", 1f));
    //        //iTween.MoveTo(Char, BlockList[BlockCharIndex].LeftBlock.transform.TransformPoint(BlockCharCenterPos), 1f);
    //    }
    //    else
    //    {
    //        //Char.transform.parent = BlockList[BlockCharIndex].RightBlock.transform;
    //        //Char.transform.parent = BlockList[BlockCharIndex].RightBlock.transform;
    //        //Char.transform.localPosition = BlockCharCenterPos;

    //        //Char.transform.parent = BlockList[BlockCharIndex].RightBlock.transform;
    //        //iTween.MoveTo(Char, iTween.Hash("position", BlockCharCenterPos, "islocal", true, "time", 1f, "easetype", iTween.EaseType.easeInBounce));

    //        //iTween.MoveTo(Char, BlockList[BlockCharIndex].RightBlock.transform.TransformPoint(BlockCharCenterPos), 1f);

    //        //iTween.Stop(Char);
    //        //Char.transform.localPosition = BlockCharCenterPos;
    //        Char.transform.parent = BlockList[BlockCharIndex].RightBlock.transform;

    //        iTween.MoveTo(Char, iTween.Hash("position", BlockCharCenterPos, "islocal", true, "movetopath", false, "time", 1f));
    //    }

    //    //iTween.MoveTo(BlockList[BlockCharIndex].RightBlock, BlockCharCenterPos, 1f);

    //    //Char.transform.position = BlockList[0].RightBlock.transform.InverseTransformPoint(BlockCharCenterPos);

    //}




    //private void FixedUpdate()
    //{
    //    Jump();
    //}

    //public void Jump()
    //{
    //    return;
    //    if (isLeftJump == false && isRightJump == false)
    //        return;

    //    BlockCharIndex = BlockCharIndex + 1;
    //    if (BlockCharIndex > BlockList.Count - 1)
    //        BlockCharIndex = 0;
    //    IsGameStart = true;
    //    rigid.velocity = Vector2.zero;

    //    if (isLeftJump)
    //    {
    //        //var currPos = Char.transform.position;
    //        //var nextPos = BlockList[BlockCharIndex].LeftBlock.transform.TransformPoint(BlockCharCenterPos);
    //        //Vector3 v3Target = nextPos - currPos;
    //        //rigid.AddForceAtPosition(v3Target.normalized, v3Target);
    //        Vector2 jumpVelocity = new Vector2(-5.68f, jumpPower);
    //        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
    //    }
    //    else
    //    {
    //        //var currPos = Char.transform.position;
    //        //var nextPos = BlockList[BlockCharIndex].RightBlock.transform.TransformPoint(BlockCharCenterPos);
    //        //Vector3 v3Target = nextPos - currPos;
    //        //rigid.AddForceAtPosition(v3Target.normalized, v3Target);
    //        Vector2 jumpVelocity = new Vector2(5.68f, jumpPower);
    //        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
    //    }


    //    isLeftJump = false;
    //    isRightJump = false;
    //}
}
