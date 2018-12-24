using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStairs : MonoBehaviour
{
    public enum STAIRS_TYPE
    {
        NONE,
        SAFE,
        SAW,
        CLAER,
    }

    public Sprite Img;
    public Sprite SawImg;
    public STAIRS_TYPE StairsType = STAIRS_TYPE.NONE;

    public void Initialize(STAIRS_TYPE type)
    {
        // 캐릭터 초기화
        StairsType = type;

        // 타입에 맞게 이미지 수정

    }
}
