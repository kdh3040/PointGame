using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBlock : MonoBehaviour
{
    public enum BLOCK_TYPE
    {
        NONE,
        SAFE,
        CLEAR,
        LEFT_SAW,
        RIGHT_SAW,
        LEFT,
        RIGHT,
    }

    public GameObject LeftBlockObject;
    public GameObject LeftBlock;
    public GameObject LeftSaw;
    public GameObject LeftSawImg;
    public GameObject RightBlockObject;
    public GameObject RightBlock;
    public GameObject RightSaw;
    public GameObject RightSawImg;
    public GameObject SafeBlock;

    public GameObject LeftCoin;
    public GameObject LeftCoinImg;
    public GameObject RightCoin;
    public GameObject RightCoinImg;

    public BLOCK_TYPE BlockType = BLOCK_TYPE.NONE;

    private void Start()
    {
        iTween.MoveTo(LeftSaw, iTween.Hash("position", new Vector3(1.7f, 1f, 4), "islocal", true, "movetopath", false, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutQuad));
        iTween.RotateTo(LeftSawImg, iTween.Hash("z", -180f, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.linear));
        
        iTween.MoveTo(RightSaw, iTween.Hash("position", new Vector3(-1.7f, 1f, 4), "islocal", true, "movetopath", false, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutQuad));
        iTween.RotateTo(RightSawImg, iTween.Hash("z", -180f, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.linear));
    }

    public void Initialize(BLOCK_TYPE type)
    {
        // 캐릭터 초기화
        BlockType = type;

        LeftBlock.SetActive(false);
        LeftSaw.SetActive(false);
        RightBlock.SetActive(false);
        RightSaw.SetActive(false);
        SafeBlock.SetActive(false);
        LeftCoin.SetActive(false);
        LeftCoin.gameObject.transform.localPosition = new Vector3(0, 0, 4);
        RightCoin.SetActive(false);
        RightCoin.gameObject.transform.localPosition = new Vector3(0, 0, 4);

        var coinEnable = Random.Range(1, 100) <= 20;

        switch (BlockType)
        {
            case BLOCK_TYPE.NONE:
                break;
            case BLOCK_TYPE.SAFE:
            case BLOCK_TYPE.CLEAR:
                SafeBlock.SetActive(true);
                break;
            case BLOCK_TYPE.LEFT_SAW:
                LeftBlock.SetActive(true);
                LeftSaw.SetActive(true);
                RightBlock.SetActive(true);
                if (coinEnable)
                    RightCoin.SetActive(true);
                break;
            case BLOCK_TYPE.RIGHT_SAW:
                LeftBlock.SetActive(true);
                RightBlock.SetActive(true);
                RightSaw.SetActive(true);
                if (coinEnable)
                    LeftCoin.SetActive(true);
                break;
            case BLOCK_TYPE.LEFT:
                LeftBlock.SetActive(true);
                if (coinEnable)
                    LeftCoin.SetActive(true);
                break;
            case BLOCK_TYPE.RIGHT:
                RightBlock.SetActive(true);
                if (coinEnable)
                    RightCoin.SetActive(true);
                break;
            default:
                break;
        }

        // 타입에 맞게 이미지 수정
    }

    public bool isGetCoin(bool charLeft)
    {
        if (RightCoin.activeSelf && charLeft == false)
        {
            iTween.MoveTo(RightCoin, iTween.Hash("position", new Vector3(0f, 3f, 4), "time", 0.3f, "islocal", true, "movetopath", false, "easetype", iTween.EaseType.easeOutQuad));
            iTween.FadeTo(RightCoinImg, iTween.Hash("alpha", 0f, "delay", 0.1f, "time", 0.1f, "easetype", iTween.EaseType.easeOutQuad));
            return true;
        }
        else if (LeftCoin.activeSelf && charLeft)
        {
            iTween.MoveTo(LeftCoin, iTween.Hash("position", new Vector3(0f, 3f, 4), "time", 0.3f, "islocal", true, "movetopath", false, "easetype", iTween.EaseType.easeOutQuad));
            iTween.FadeTo(LeftCoinImg, iTween.Hash("alpha", 0f, "delay", 0.1f, "time", 0.1f, "easetype", iTween.EaseType.easeOutQuad));
            return true;
        }
            

        return false;
    }
}
