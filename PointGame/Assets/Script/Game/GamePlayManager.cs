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

    public static int GameGetPoint = 20;
    public static int GameCoinGetPoint = 2;

    public int StageCount = 1;
    public int GamePoint = 0;
    public int BlockLimitCount = 20;
    public int BlockCount = 0;
    public int BlockClearCount = 0;
    public List<GameBlock> BlockList = new List<GameBlock>();
    public GameChar Char;
    public GameUI UI;

    private bool IsGameReady = false;
    private bool IsGameStart = false;
    private Vector3 BlockCharCenterPos = new Vector3(0, 1.2f, -9f);
    private int BlockCharIndex = 0;
    private bool LastJumpLeft = true;
    private bool IsJumping = false;

    private float CharDeathPosY = -8.5f;

    private float BlockSpeed = 0.05f;
    private float BlockSpeedOffset = 0.0001f;
    private float BlockSpeedStageClearOffset = 0.005f;


    void Start()
    {
        TKManager.Instance.GameOverRouletteStart = false;
        // 게임에 들어 올때마다 생성
        BlockClearCount = 0;
        StageCount = 0;
        GameReady();
    }

    public void GameReady()
    {
        // 게임 화면 들어와서 준비
        // 블럭 생성
        StageCount++;
        BlockCharIndex = 0;
        BlockCount = 0;
        IsJumping = false;
        AllResetBlock();
        ResetChar();
        UI.GameReady();

        Char.CharIdle();
    }

    public void GameStart()
    {
        // 게임 화면 들어와서 시작
        IsGameReady = true;
    }

    public void GameClear()
    {
        // 스테이지 클리어
        IsGameReady = true;
        IsGameStart = false;

        GamePoint += GameGetPoint;

        UI.UpdateGameInfo();

        StartCoroutine(Co_StageClearMove());

        Char.CharIdle();
    }

    IEnumerator Co_StageClearMove()
    {
        while(true)
        {
            for (int index = 0; index < BlockList.Count; ++index)
            {
                var pos = BlockList[index].gameObject.transform.localPosition;
                pos.y = pos.y - 0.5f;
                BlockList[index].gameObject.transform.localPosition = pos;

                if(BlockList[index].BlockType == GameBlock.BLOCK_TYPE.CLEAR &&
                    BlockList[index].gameObject.transform.localPosition.y < 5.2f)
                {
                    UI.GameClear();
                    yield break;
                }

                if (pos.y < -2.5f)
                    ResetBlock(index);
            }

            yield return null;
        }
        
    }

    public void GameEnd()
    {
        // 스테이지 종료
        IsGameReady = false;
        IsGameStart = false;
        UI.GameEnd();
        Char.CharIdle();

        iTween.MoveTo(Char.gameObject, iTween.Hash("x", Char.gameObject.transform.localPosition.x, "y",CharDeathPosY - 10f, "islocal", true, "movetopath", false, "time", 0.8f, "easetype", iTween.EaseType.easeInBack)); 
    }

    public void CheckGameOver()
    {
        if(Char.BlockLeftDir && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.LEFT_SAW ||
            Char.BlockLeftDir == false && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.RIGHT_SAW )
        {
            GameEnd();
        }
    }

    public void CheckGameCoinGet()
    {
       if( BlockList[BlockCharIndex].isGetCoin(Char.BlockLeftDir))
        {
            GamePoint += GameCoinGetPoint;
            UI.UpdateGameInfo();
        }
    }

    private void Update()
    {
        if (IsGameStart)
        {
            for (int index = 0; index < BlockList.Count; ++index)
            {
                var pos = BlockList[index].gameObject.transform.localPosition;
                pos.y = pos.y - (BlockSpeed + (BlockSpeedOffset * BlockClearCount)+ (BlockSpeedStageClearOffset * StageCount));
                BlockList[index].gameObject.transform.localPosition = pos;

                if (pos.y < -6.5f)
                    ResetBlock(index);
            }

            if (Char.gameObject.transform.TransformPoint(Vector3.zero).y < CharDeathPosY)
                GameEnd();
        }

        Debug.LogFormat("{0} {1} {2} {3}", BlockSpeed, BlockSpeedOffset, (BlockSpeedStageClearOffset * StageCount), (BlockSpeed + BlockSpeedOffset + (BlockSpeedStageClearOffset * StageCount)));
    }

    public void CharJump(bool left)
    {
        // 좌우 터치가 들어 왔을떄 점프 처리
        // 점프액션이 두번 들어오지 않게 막아야함
        // 화면 밖으로 나가지 못하게 해야함
        if(IsJumping == false)
            StartCoroutine(Co_CharJump(left));
    }

    IEnumerator Co_CharJump(bool left)
    {
        IsJumping = true;
        float time = 1.0f;

        var tempIndex = BlockCharIndex;
        BlockCharIndex = BlockCharIndex + 1;
        if (BlockCharIndex > BlockList.Count - 1)
            BlockCharIndex = 0;

        if (BlockList[BlockCharIndex].gameObject.transform.localPosition.y > 17f)
        {
            BlockCharIndex = tempIndex;
            IsJumping = false;
            yield break;
        }
            

        var startPos = Vector3.zero;
        var centerPos = Vector3.zero;
        var endPos = Vector3.zero;
        Char.BlockLeftDir = left;

        Char.CharJump(left);

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

        bool charDown = false;
        if (left && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.RIGHT ||
            left == false && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.LEFT)
        {
            charDown = true;
            yield return Co_CharDown(left);
        }
            

        if(BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.CLEAR)
        {
            Char.transform.localPosition = endPos;
            GameClear();
            yield break;
        }

        Char.CharIdle();
        Char.transform.localPosition = endPos;
        CheckGameOver();
        CheckGameCoinGet();
        IsJumping = false;
        if(charDown == false && IsGameReady == true)
        {
            IsGameReady = false;
            IsGameStart = true;
        }

        if (charDown == false)
            BlockClearCount++;
    }

    IEnumerator Co_CharDown(bool left)
    {
        BlockCharIndex -= 1;
        if (BlockCharIndex < 0)
            BlockCharIndex = BlockList.Count - 1;

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
            if (IsGameStart == false)
                yield break;
            yield return null;
        }

        if (left && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.RIGHT ||
            left == false && BlockList[BlockCharIndex].BlockType == GameBlock.BLOCK_TYPE.LEFT)
            yield return Co_CharDown(left);


        Char.transform.localPosition = endPos;
        CheckGameOver();
        CheckGameCoinGet();
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
            if (index == 0)
                ResetBlock(index, true);
            else
                ResetBlock(index);
        }
    }

    private void ResetBlock(int index, bool zeroPos = false)
    {
        int posIndex = index - 1;
        if (posIndex < 0)
            posIndex = BlockList.Count - 1;

        var nextPos = BlockList[posIndex].transform.localPosition;
        if(zeroPos == false)
            BlockList[index].gameObject.transform.localPosition = new Vector3(0, nextPos.y + 3.5f);
        else
            BlockList[index].gameObject.transform.localPosition = new Vector3(0, 1f);

        if (BlockCount == 0)
        {
            BlockList[index].Initialize(GameBlock.BLOCK_TYPE.SAFE);
        }
        else if(BlockCount == BlockLimitCount)
        {
            BlockList[index].Initialize(GameBlock.BLOCK_TYPE.CLEAR);
        }
        else if (BlockCount < BlockLimitCount)
        {
            GameBlock.BLOCK_TYPE type = (GameBlock.BLOCK_TYPE)Random.Range((int)GameBlock.BLOCK_TYPE.LEFT_SAW, (int)GameBlock.BLOCK_TYPE.RIGHT + 1);
            BlockList[index].Initialize(type);
        }
        else
        {
            BlockList[index].Initialize(GameBlock.BLOCK_TYPE.NONE);
        }

        BlockCount++;
    }

    private void ResetChar()
    {
        Char.transform.parent = BlockList[0].LeftBlockObject.transform;
        Char.transform.localPosition = BlockCharCenterPos;
        LastJumpLeft = true;
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
