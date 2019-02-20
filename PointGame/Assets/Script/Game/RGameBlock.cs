using System.Collections;
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
    private GameObject SawObj;
    private GameObject SawImg;
    public GameObject Coin;
    public SpriteRenderer CoinImg;
    public R_BLOCK_TYPE Type;
    public RGameBlockGroup.R_BLOCK_STEP StepType = RGameBlockGroup.R_BLOCK_STEP.NONE;
    public bool CharAttach;

    public GameObject SawObj_2;
    public GameObject SawImg_2;
    public GameObject SawObj_3;
    public GameObject SawImg_3;

    private Vector3 CoinDefaultPos = new Vector3(0, 1.39f, 4);
    private Vector3 CoinMovePos = new Vector3(0f, 3f, 4);

    private void Start()
    {
        

        //iTween.RotateTo(SawImg, iTween.Hash("z", -180f, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.linear));
    }

    public void SetData(R_BLOCK_TYPE type, RGameBlockGroup.R_BLOCK_STEP stepType)
    {
        Type = type;
        CharAttach = false;

        RefreahObj();

        Block.gameObject.SetActive(false);
        SawObj.gameObject.SetActive(false);
        Coin.gameObject.SetActive(false);
        Coin.gameObject.transform.localPosition = CoinDefaultPos;

        StopAllCoroutines();
        Color tempColor = CoinImg.color;
        tempColor.a = 1f;
        CoinImg.color = tempColor;
        //iTween.FadeTo(CoinImg.gameObject, iTween.Hash("alpha", 1f, "time", 0.1f));
        StepType = stepType;
        //if(StepType != stepType)
        //{
        //    StepType = stepType;
        //    if (stepType == RGameBlockGroup.R_BLOCK_STEP.TWO)
        //    {
        //        SawObj.gameObject.transform.localPosition = new Vector3(1.7f, 1.39f, 4);
        //        iTween.MoveTo(SawObj, iTween.Hash("position", new Vector3(-1.7f, 1.39f, 4), "islocal", true, "movetopath", false, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutQuad));
        //    }
        //    else
        //    {
        //        SawObj.gameObject.transform.localPosition = new Vector3(1.27f, 1.39f, 4);
        //        iTween.MoveTo(SawObj, iTween.Hash("position", new Vector3(-1.27f, 1.39f, 4), "islocal", true, "movetopath", false, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutQuad));
        //    }
        //}


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

    public void Update()
    {
        if (gameObject.activeSelf == false)
            return;

        RefreahObj();

        if (StepType == RGameBlockGroup.R_BLOCK_STEP.TWO)
            SawObj.gameObject.transform.localPosition = new Vector3(Mathf.PingPong(Time.time * 5, 3.4f) - 1.7f, SawObj.gameObject.transform.localPosition.y, SawObj.gameObject.transform.localPosition.z);
        else
            SawObj.gameObject.transform.localPosition = new Vector3(Mathf.PingPong(Time.time * 5, 2.54f) - 1.27f, SawObj.gameObject.transform.localPosition.y, SawObj.gameObject.transform.localPosition.z);
        
        SawImg.gameObject.transform.localRotation = Quaternion.Euler(SawImg.gameObject.transform.localRotation.x, SawImg.gameObject.transform.localRotation.y, Mathf.PingPong(Time.time, 380f) * 200);
    }

    public void RefreahObj()
    {
        if (SawObj_2 != null)
            SawObj = SawObj_2;
        else
            SawObj = SawObj_3;

        if (SawImg_2 != null)
            SawImg = SawImg_2;
        else
            SawImg = SawImg_3;
    }

    public void GetCoin()
    {
        if(Type == R_BLOCK_TYPE.COIN)
        {
            
            StartCoroutine(Co_GetCoin());
            //iTween.MoveTo(Coin, iTween.Hash("position", CoinMovePos, "time", 0.3f, "islocal", true, "movetopath", false, "easetype", iTween.EaseType.easeOutQuad));
            //iTween.FadeTo(CoinImg.gameObject, iTween.Hash("alpha", 0f, "delay", 0.1f, "time", 0.1f, "easetype", iTween.EaseType.easeOutQuad));
        }

    }
    public IEnumerator Co_GetCoin()
    {
        float tempTime = 1f;
        while(tempTime > 0)
        {
            tempTime -= 0.1f;
            var t = 1f - tempTime;
            var pos = ((1 - t) * CoinDefaultPos) + ((t) * CoinMovePos);
            Coin.gameObject.transform.localPosition = pos;

            Color tempColor = CoinImg.color;
            tempColor.a = tempTime;
            CoinImg.color = tempColor;
            yield return null;
        }
        yield return null;
    }
    //   StartCoroutine(IEnumerator)




    public void SetCharAttach(bool enable)
    {
        CharAttach = enable;
    }
}
