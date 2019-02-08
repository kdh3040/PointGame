using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGamePopup : Popup
{
    public Text InfoText;
    public List<Button> JumpButton = new List<Button>();
    public List<GameObject> BlockList = new List<GameObject>();
    public List<GameObject> BlockSawList = new List<GameObject>();
    public List<GameObject> BlockSawImgList = new List<GameObject>();
    public List<GameObject> BlockCoinList = new List<GameObject>();
    public List<Image> BlockCoinImgList = new List<Image>();
    public List<bool> BlockSafeList = new List<bool>();

    public GameObject SafeBlock;
    public GameObject Char;
    public Animator CharAnim;

    public GameObject PopupObj;
    public Text PopupText;
    public Button PopupOk;
    public Button PopupCancel;

    private bool JumpEnable = true;

    public void Awake()
    {
        JumpButton[0].onClick.AddListener(OnClickJump_1);
        JumpButton[1].onClick.AddListener(OnClickJump_2);
        JumpButton[2].onClick.AddListener(OnClickJump_3);
        PopupOk.onClick.AddListener(OnClickOk);
        PopupCancel.onClick.AddListener(OnClickCancel);
    }

    private void Start()
    {
        for (int i = 0; i < BlockSawList.Count; i++)
        {
            iTween.MoveTo(BlockSawList[i], iTween.Hash("position", new Vector3(-124f, 119f, 0), "islocal", true, "movetopath", false, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutQuad));
        }
        for (int i = 0; i < BlockSawImgList.Count; i++)
        {
            iTween.RotateTo(BlockSawImgList[i], iTween.Hash("z", -180f, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.linear));
        }
    }

    public class MiniGamePopupData : PopupData
    {
        public MiniGamePopupData()
        {
            PopupType = POPUP_TYPE.MINI_GAME;
        }
    }

    public override void SetData(PopupData data)
    {
        InfoText.text = string.Format("미니게임\n{0}포인트\n동전을 골라보세요~", CommonData.AdsPointReward);

        GameReady();
    }

    public void GameReady()
    {
        PopupObj.gameObject.SetActive(false);
        JumpEnable = true;
        Char.transform.parent = SafeBlock.transform;
        Char.transform.localPosition = new Vector3(0, 125, 0);
        CharAnim.SetTrigger("idle");

        BlockSafeList.Clear();
        BlockSafeList.Add(false);
        BlockSafeList.Add(false);
        BlockSafeList.Add(false);

        int coinIndex = Random.Range(0, BlockSafeList.Count);
        BlockSafeList[coinIndex] = true;

        for (int i = 0; i < BlockSawList.Count; i++)
        {
            BlockSawList[i].SetActive(false);
            BlockCoinList[i].SetActive(false);
            BlockCoinList[i].transform.localPosition = new Vector3(0, 129, 0);
            Color tempColor = BlockCoinImgList[i].color;
            tempColor.a = 1f;
            BlockCoinImgList[i].color = tempColor;
        }

        for (int i = 0; i < BlockSafeList.Count; i++)
        {
            if (BlockSafeList[i] == false)
                BlockSawList[i].SetActive(true);
            else
                BlockCoinList[i].SetActive(true);
        }
    }

    public void OnClickJump_1()
    {
        JumpIndex(0);
    }
    public void OnClickJump_2()
    {
        JumpIndex(1);
    }
    public void OnClickJump_3()
    {
        JumpIndex(2);
    }

    public void JumpIndex(int index)
    {
        if (JumpEnable == false)
            return;

        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.JUMP);
        JumpEnable = false;

        StartCoroutine(Co_CharJump(index));
    }

    IEnumerator Co_CharJump(int index)
    {
        Char.transform.parent = BlockList[index].transform;
        var startPos = Char.transform.localPosition;
        var centerPos = new Vector3(0, Char.transform.localPosition.y + 800f, 0);
        var endPos = new Vector3(0, 125, 0);

        float time = 1.0f;
        while (time > 0f)
        {
            time -= 0.1f;
            Char.transform.localPosition = BezierCurve(1.0f - time, startPos, centerPos, endPos);
            yield return null;
        }

        if (BlockSafeList[index] == false)
        {
            iTween.MoveTo(Char.gameObject, iTween.Hash("x", Char.gameObject.transform.localPosition.x, "y", -1000f, "islocal", true, "movetopath", false, "time", 0.8f, "easetype", iTween.EaseType.easeInBack));
            yield return new WaitForSeconds(0.8f);

            ShowInfoPopup(false);
        }
        else
        {
            iTween.MoveTo(BlockCoinList[index], iTween.Hash("y", 275f, "time", 0.3f, "islocal", true, "movetopath", false, "easetype", iTween.EaseType.easeOutQuad));

            float fadeTime = 0.3f;
            while(fadeTime > 0)
            {
                fadeTime -= Time.deltaTime;
                if(fadeTime <= 0.1f)
                {
                    Color tempColor = BlockCoinImgList[index].color;
                    tempColor.a = Mathf.Lerp(fadeTime / 0.1f, 1f, 0f);
                    BlockCoinImgList[index].color = tempColor;
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.2f);

            ShowInfoPopup(true);
            AdsManager.Instance.ShowMiniGameRewardAd();

            if(FirebaseManager.Instance.ReviewMode)
                TKManager.Instance.MyData.AddPoint(CommonData.AdsPointReward);
        }

        yield return null;
    }

    public void ShowInfoPopup(bool success)
    {
        PopupObj.gameObject.SetActive(true);
        if (success)
            PopupText.text = string.Format("{0}포인트를 획득했습니다.\n재시작 하시겠습니까?", CommonData.AdsPointReward);
        else
            PopupText.text = "재시작 하시겠습니까?";
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        GameReady();
    }

    public void OnClickCancel()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        ParentPopup.ClosePopup();
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
}
