using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPSPopup : Popup
{
    public Text SeriesCount;
    public Text EnemyId;
    public Image EnemyRPS;
    public Text MyId;
    public Image MyRPS;

    public Slider MyRPSTime;
    public Button RPSSelect_S;
    public Button RPSSelect_R;
    public Button RPSSelect_P;

    public GameObject ResultObj;
    public Text ResultDesc;

    private int RPSGame_MyValue = 0;
    private bool RPSGame_ResultWait = false;

    public class RPSPopupData : PopupData
    {
        public RPSPopupData()
        {
            PopupType = POPUP_TYPE.RPS_GAME;
        }
    }

    public void Awake()
    {
        RPSSelect_S.onClick.AddListener(OnClick_S);
        RPSSelect_R.onClick.AddListener(OnClick_R);
        RPSSelect_P.onClick.AddListener(OnClick_P);
    }

    public override void SetData(PopupData data)
    {
        RPSGame_MyValue = 0;
        SeriesCount.text = string.Format("{0} 회", 0);
        EnemyId.text = "ID : ";
        MyId.text = string.Format("ID : {0}", TKManager.Instance.MyData.NickName);

        EnemyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[1], typeof(Sprite));
        MyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[1], typeof(Sprite));

        StartCoroutine(Co_RPSGame());
    }

    public void RefreshUI()
    {
        // 현재 회차
        SeriesCount.text = string.Format("{0} 회", FirebaseManager.Instance.FirebaseRPSGameSeries);
        EnemyId.text = string.Format("ID : {0}", FirebaseManager.Instance.FirebaseRPSGame_EnemyNick);
        MyId.text = string.Format("ID : {0}", TKManager.Instance.MyData.NickName);
    }

    IEnumerator Co_RPSGame()
    {
        while(true)
        {
            yield return Co_RPSGame_Search();
            yield return Co_RPSGame_Select(false);
            yield return Co_RPSGame_SelectWait();

            int result = GetRPSResult();
            // TODO 비김
            if (result == 0)
            {
                yield return Co_RPSGame_Result(result);
                yield return Co_RPSGame_Select(true);
                yield return Co_RPSGame_SelectWait();

                result = GetRPSResult();
                // 또 비김
                if (result == 0)
                {
                    yield return Co_RPSGame_Result(result);
                    yield return Co_RPSGame_Select(true);
                    yield return Co_RPSGame_SelectWait();
                }

                result = GetRPSResult();
                // 또 비김
                if (result == 0)
                {
                    ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("패배하였습니다.", () =>
                    {
                        CloseAction();
                    }));
                    yield break;
                }
            }

            // 패배
            if(result == 2)
            {
                ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("패배하였습니다.", () =>
                {
                    CloseAction();
                }));
                yield break;
            }

            // 승리
            if(result == 1)
            {
                yield return Co_RPSGame_Result(result);
            }
        }
    }

    IEnumerator Co_RPSGame_Search()
    {
        // Step 1 상대방의 데이터를 받아왔는지 체크
        TKManager.Instance.ShowHUD("상대방을 검색중입니다.");

        while (true)
        {
            // 상대방이 검색이 됨
            if (FirebaseManager.Instance.FirebaseRPSGame_EnemyIndex != 0)
            {
                RefreshUI();
                TKManager.Instance.HideHUD();
                break;
            }

            yield return null;
        }
    }

    IEnumerator Co_RPSGame_Select(bool draw)
    {
        // Step 2 상대방의 가위바위보는 랜덤으로 돌아감
        // 선택 할 수 있는 시간도 같이 흘러감
        RPSGame_MyValue = 0;
        MyRPS.gameObject.SetActive(false);
        float enemyRPSChangeTime = 0.1f;
        float maxSelectTime = draw ? CommonData.RPS_GAME_DRAW_PLAY_TIME : CommonData.RPS_GAME_PLAY_TIME;
        float myPRSSelectTime = maxSelectTime;

        while (true)
        {
            RandEnemyRPS();
            enemyRPSChangeTime -= Time.deltaTime;

            if (enemyRPSChangeTime < 0)
            {
                enemyRPSChangeTime = 0.1f;
                RandEnemyRPS();
            }

            myPRSSelectTime -= Time.deltaTime;
            MyRPSTime.value = myPRSSelectTime / maxSelectTime;

            yield return null;
        }
    }

    IEnumerator Co_RPSGame_SelectWait()
    {
        // Step 1 상대방의 데이터를 받아왔는지 체크
        TKManager.Instance.ShowHUD("결과를 확인중 입니다.");
        float waitTime = 1.0f;
        RPSGame_ResultWait = true;
        while (true)
        {
            // 파베에 상대방벨류 요청
            RPSGame_ResultWait = false;
            waitTime -= Time.deltaTime;

            if(waitTime < 0 && RPSGame_ResultWait == false)
            {
                TKManager.Instance.HideHUD();
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator Co_RPSGame_Result(int result)
    {
        float resultWaitTime = CommonData.RPS_GAME_RESULT_WAIT_TIME;

        ResultObj.gameObject.SetActive(true);
        if(result == 1)
        {
            // 승리
            ResultDesc.text = "승리하였습니다.\n잠시만 기달려주세요.";
        }
        else if(result == 0)
        {
            // 비김
            ResultDesc.text = "비겼습니다.\n잠시만 기달려주세요.";
        }

        while (true)
        {
            resultWaitTime -= Time.deltaTime;

            if (resultWaitTime < 0)
            {
                ResultObj.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }

    public void RandEnemyRPS()
    {
        int index = Random.Range(1, CommonData.RPS_GAME_IMG.Length);
        var imgSprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[index], typeof(Sprite));
        if (imgSprite == null)
            return;

        EnemyRPS.sprite = imgSprite;
    }

    public void OnClick_S()
    {
        RPSGame_MyValue = 1;
        MyRPS.gameObject.SetActive(true);
        MyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[RPSGame_MyValue], typeof(Sprite));
    }

    public void OnClick_R()
    {
        RPSGame_MyValue = 2;
        MyRPS.gameObject.SetActive(true);
        MyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[RPSGame_MyValue], typeof(Sprite));
    }
    public void OnClick_P()
    {
        RPSGame_MyValue = 3;
        MyRPS.gameObject.SetActive(true);
        MyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[RPSGame_MyValue], typeof(Sprite));
    }

    public int GetRPSResult()
    {
        if (RPSGame_MyValue == 0)
            return 2;

        return (3 + RPSGame_MyValue - FirebaseManager.Instance.FirebaseRPSGame_EnemyValue) % 3;
    }

    public void OnClickExit()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
    }
}
