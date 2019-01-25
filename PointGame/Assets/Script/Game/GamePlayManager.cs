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
    public static int GameCoinGetPoint = 1;

    public int StageCount = 1;
    public int GamePoint = 0;
    public int BlockLimitCount = 20;
    public int BlockCount = 0;
    public int BlockClearCount = 0;
    public List<RGameBlockGroup> BlockGroupList = new List<RGameBlockGroup>();




    //public List<GameBlock> BlockList = new List<GameBlock>();
    public List<GameObject> BackgrounList = new List<GameObject>();
    public GameChar Char;
    public GameUI UI;

    private bool IsGameReady = false;
    private bool IsGameStart = false;
    private Vector3 BlockCharCenterPos = new Vector3(0, 1.39f, -9f);
    private int BlockCharIndex = 0;
    private bool LastJumpLeft = true;
    private bool IsJumping = false;

    private float CharDeathPosY = -8.5f;

    private float BlockSpeed = 0.05f;
    private float BlockSpeedOffset = 0.0001f;
    private float BlockSpeedStageClearOffset = 0.01f;


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
        AllResetBackground();
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

    public void GameRestart()
    {
        GamePoint = 0;
        BlockClearCount = 0;
        StageCount = 0;
        GameReady();
    }

    IEnumerator Co_StageClearMove()
    {
        while(true)
        {
            float speed = 0.5f;
            for (int index = 0; index < BlockGroupList.Count; ++index)
            {
                var pos = BlockGroupList[index].gameObject.transform.localPosition;
                pos.y = pos.y - speed;
                BlockGroupList[index].gameObject.transform.localPosition = pos;

                if(BlockGroupList[index].Type == RGameBlockGroup.R_BLOCK_GROUP_TYPE.CLEAR&&
                    BlockGroupList[index].gameObject.transform.localPosition.y < 5.2f)
                {
                    UI.GameClear();
                    yield break;
                }

                if (pos.y < -2.5f)
                    ResetBlock(index);
            }

            for (int index = 0; index < BackgrounList.Count; ++index)
            {
                var pos = BackgrounList[index].gameObject.transform.localPosition;
                pos.y = pos.y - speed / 3;
                BackgrounList[index].gameObject.transform.localPosition = pos;

                if (pos.y < -19.21f)
                    ResetBackground(index);
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
        StartCoroutine(Co_GameEndAd());
    }

    IEnumerator Co_GameEndAd()
    {
        yield return new WaitForSeconds(0.8f);
        AdsManager.Instance.ShowSkipRewardedAd();
    }

    public void CheckGameOver()
    {
        for (int index = 0; index < BlockGroupList.Count; ++index)
        {
            if (BlockGroupList[index].IsGameOverCheck())
            {
                GameEnd();
            }
        }
    }

    public void CheckGameCoinGet()
    {
        for (int index = 0; index < BlockGroupList.Count; ++index)
        {
            if (BlockGroupList[index].IsGameCoinGetCheck())
            {
                GamePoint += GameCoinGetPoint;
                UI.UpdateGameInfo();
            }
        }
    }

    private void Update()
    {
        if (IsGameStart)
        {
            float speed = (BlockSpeed + (BlockSpeedOffset * BlockClearCount) + (BlockSpeedStageClearOffset * StageCount));
            for (int index = 0; index < BlockGroupList.Count; ++index)
            {
                var pos = BlockGroupList[index].gameObject.transform.localPosition;
                pos.y = pos.y - speed;
                BlockGroupList[index].gameObject.transform.localPosition = pos;

                if (pos.y < -6.5f)
                    ResetBlock(index);
            }

            for (int index = 0; index < BackgrounList.Count; ++index)
            {
                var pos = BackgrounList[index].gameObject.transform.localPosition;
                pos.y = pos.y - speed / 3;
                BackgrounList[index].gameObject.transform.localPosition = pos;

                if (pos.y < -19.21f)
                    ResetBackground(index);
            }

            

            if (Char.gameObject.transform.TransformPoint(Vector3.zero).y < CharDeathPosY)
                GameEnd();
        }

        
    }

    public void CharJump(int index)
    {
        // 좌우 터치가 들어 왔을떄 점프 처리
        // 점프액션이 두번 들어오지 않게 막아야함
        // 화면 밖으로 나가지 못하게 해야함
        if(IsJumping == false)
            StartCoroutine(Co_CharJump(index));
    }

    IEnumerator Co_CharJump(int index)
    {
        IsJumping = true;
        float time = 1.0f;

        var tempIndex = BlockCharIndex;
        BlockCharIndex = BlockCharIndex + 1;
        if (BlockCharIndex > BlockGroupList.Count - 1)
            BlockCharIndex = 0;

        if (BlockGroupList[BlockCharIndex].gameObject.transform.localPosition.y > 17f)
        {
            BlockCharIndex = tempIndex;
            IsJumping = false;
            yield break;
        }
            

        var startPos = Vector3.zero;
        var centerPos = Vector3.zero;
        var endPos = Vector3.zero;

        Char.CharJump(false);


        int prevCharAttachIndex = -1;
        for (int i = 0; i < BlockGroupList.Count; ++i)
        {
            if (prevCharAttachIndex < 0)
                prevCharAttachIndex = BlockGroupList[i].GetCharAttachBlockindex();

            BlockGroupList[i].ResetCharAttach();
        }

        Char.transform.parent = BlockGroupList[BlockCharIndex].SetCharAttach(index, true);

        startPos = Char.transform.localPosition;
        centerPos = new Vector3(0, Char.transform.localPosition.y + 5, 0);

        //if (index < prevCharAttachIndex)
            
        //else
        //    centerPos = new Vector3(3, Char.transform.localPosition.y + 5, 0);
        endPos = BlockCharCenterPos;

        while (time > 0f)
        {
            time -= 0.1f;
            Char.transform.localPosition = BezierCurve(1.0f - time, startPos, centerPos, endPos);
            yield return null;
        }

        bool charDown = false;
        bool gameClear = false;
        if (BlockGroupList[BlockCharIndex].Type == RGameBlockGroup.R_BLOCK_GROUP_TYPE.CLEAR)
        {
            gameClear = true;
            Char.transform.localPosition = endPos;
            Char.transform.parent = BlockGroupList[BlockCharIndex].gameObject.transform;
            GameClear();
            yield break;
        }

        if (gameClear == false && BlockGroupList[BlockCharIndex].GetBlockType(index) == RGameBlock.R_BLOCK_TYPE.NONE)
        {
            charDown = true;
            yield return Co_CharDown();
        }

        Char.CharIdle();
        Char.transform.localPosition = endPos;
        CheckGameOver();
        CheckGameCoinGet();
        IsJumping = false;
        if (charDown == false && IsGameReady == true)
        {
            IsGameReady = false;
            IsGameStart = true;
        }

        if (charDown == false)
            BlockClearCount++;
    }

    IEnumerator Co_CharDown()
    {
        BlockCharIndex -= 1;
        if (BlockCharIndex < 0)
            BlockCharIndex = BlockGroupList.Count - 1;

        var startPos = Vector3.zero;
        var endPos = Vector3.zero;

        int prevCharAttachIndex = -1;
        for (int i = 0; i < BlockGroupList.Count; ++i)
        {
            if (prevCharAttachIndex < 0)
                prevCharAttachIndex = BlockGroupList[i].GetCharAttachBlockindex();

            BlockGroupList[i].ResetCharAttach();
        }

        Char.transform.parent = BlockGroupList[BlockCharIndex].SetCharAttach(prevCharAttachIndex, true);
        startPos = Char.transform.localPosition;
        endPos = BlockCharCenterPos;

        float time = 1.0f;
        while (time > 0f)
        {
            time -= 0.1f;
            Char.transform.localPosition = BezierCurve(1.0f - time, startPos, endPos);
            if (IsGameStart == false)
                yield break;
            yield return null;
        }

        if (BlockGroupList[BlockCharIndex].GetBlockType(prevCharAttachIndex) == RGameBlock.R_BLOCK_TYPE.NONE)
            yield return Co_CharDown();


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
        for(int index = 0; index < BlockGroupList.Count; ++index)
        {
            if (index == 0)
                ResetBlock(index, true);
            else
                ResetBlock(index);
        }
    }

    public void AllResetBackground()
    {
        for (int index = 0; index < BackgrounList.Count; ++index)
        {
            if (index == 0)
                ResetBackground(index, true);
            else
                ResetBackground(index);
        }
    }

    private void ResetBlock(int index, bool zeroPos = false)
    {
        int posIndex = index - 1;
        if (posIndex < 0)
            posIndex = BlockGroupList.Count - 1;

        var nextPos = BlockGroupList[posIndex].transform.localPosition;
        if(zeroPos == false)
            BlockGroupList[index].gameObject.transform.localPosition = new Vector3(0, nextPos.y + 2.8f);
        else
            BlockGroupList[index].gameObject.transform.localPosition = new Vector3(0, 1f);

        if (BlockCount == 0)
        {
            BlockGroupList[index].init(RGameBlockGroup.R_BLOCK_GROUP_TYPE.START);
        }
        else if(BlockCount == BlockLimitCount)
        {
            BlockGroupList[index].init(RGameBlockGroup.R_BLOCK_GROUP_TYPE.CLEAR);
        }
        else if (BlockCount < BlockLimitCount)
        {
            BlockGroupList[index].init(RGameBlockGroup.R_BLOCK_GROUP_TYPE.BLOCKS);
        }
        else
        {
            BlockGroupList[index].init(RGameBlockGroup.R_BLOCK_GROUP_TYPE.NONE);
        }

        BlockCount++;
    }

    private void ResetBackground(int index, bool zeroPos = false)
    {
        int posIndex = index - 1;
        if (posIndex < 0)
            posIndex = BackgrounList.Count - 1;

        if(index == 0)
        {
            if(zeroPos)
                BackgrounList[index].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("background", typeof(Sprite));
            else
                BackgrounList[index].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("backgroun_1", typeof(Sprite));
        }
        

        var nextPos = BackgrounList[posIndex].transform.localPosition;
        if (zeroPos == false)
            BackgrounList[index].gameObject.transform.localPosition = new Vector3(0, nextPos.y + 19f, 10f);
        else
            BackgrounList[index].gameObject.transform.localPosition = new Vector3(0, 0f, 10f);
    }

    private void ResetChar()
    {
        Char.transform.parent = BlockGroupList[0].transform;
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
