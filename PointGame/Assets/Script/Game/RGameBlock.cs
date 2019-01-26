﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGameBlock : MonoBehaviour {

    public enum R_BLOCK_TYPE
    {
        SAW,
        COIN,
        SAFE,
        NONE,
    }

    public GameObject Block;
    public GameObject SawObj;
    public GameObject SawImg;
    public GameObject Coin;
    public GameObject CoinImg;
    public R_BLOCK_TYPE Type;
    public RGameBlockGroup.R_BLOCK_STEP StepType = RGameBlockGroup.R_BLOCK_STEP.NONE;
    public bool CharAttach;

    private Vector3 CoinDefaultPos = new Vector3(0, 1.39f, 4);
    private Vector3 CoinMovePos = new Vector3(0f, 3f, 4);

    private void Start()
    {
        iTween.RotateTo(SawImg, iTween.Hash("z", -180f, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.linear));
    }

    public void SetData(R_BLOCK_TYPE type, RGameBlockGroup.R_BLOCK_STEP stepType)
    {
        Type = type;
        CharAttach = false;

        Block.gameObject.SetActive(false);
        SawObj.gameObject.SetActive(false);
        Coin.gameObject.SetActive(false);
        Coin.gameObject.transform.localPosition = CoinDefaultPos;

        iTween.FadeTo(CoinImg.gameObject, iTween.Hash("alpha", 1f, "time", 0.1f));
        
        if(StepType != stepType)
        {
            StepType = stepType;
            if (stepType == RGameBlockGroup.R_BLOCK_STEP.TWO)
            {
                SawObj.gameObject.transform.localPosition = new Vector3(1.7f, 1.39f, 4);
                iTween.MoveTo(SawObj, iTween.Hash("position", new Vector3(-1.7f, 1.39f, 4), "islocal", true, "movetopath", false, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutQuad));
            }
            else
            {
                SawObj.gameObject.transform.localPosition = new Vector3(1.27f, 1.39f, 4);
                iTween.MoveTo(SawObj, iTween.Hash("position", new Vector3(-1.27f, 1.39f, 4), "islocal", true, "movetopath", false, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutQuad));
            }
        }
        

        switch (type)
        {
            case R_BLOCK_TYPE.SAW:
                Block.gameObject.SetActive(true);
                SawObj.gameObject.SetActive(true);
                break;
            case R_BLOCK_TYPE.COIN:
                Block.gameObject.SetActive(true);
                Coin.gameObject.SetActive(true);
                break;
            case R_BLOCK_TYPE.SAFE:
                Block.gameObject.SetActive(true);
                break;
            case R_BLOCK_TYPE.NONE:
                break;
            default:
                break;
        }
    }

    public void GetCoin()
    {
        if(Type == R_BLOCK_TYPE.COIN)
        {
            iTween.MoveTo(Coin, iTween.Hash("position", CoinMovePos, "time", 0.3f, "islocal", true, "movetopath", false, "easetype", iTween.EaseType.easeOutQuad));
            iTween.FadeTo(CoinImg.gameObject, iTween.Hash("alpha", 0f, "delay", 0.1f, "time", 0.1f, "easetype", iTween.EaseType.easeOutQuad));
        }
    }

    public void SetCharAttach(bool enable)
    {
        CharAttach = enable;
    }
}
