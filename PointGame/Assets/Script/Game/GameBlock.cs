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
                break;
            case BLOCK_TYPE.RIGHT_SAW:
                LeftBlock.SetActive(true);
                RightBlock.SetActive(true);
                RightSaw.SetActive(true);
                break;
            case BLOCK_TYPE.LEFT:
                LeftBlock.SetActive(true);
                break;
            case BLOCK_TYPE.RIGHT:
                RightBlock.SetActive(true);
                break;
            default:
                break;
        }

        // 타입에 맞게 이미지 수정
    }
}
