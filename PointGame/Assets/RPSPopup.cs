﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPSPopup : Popup
{
    public Button Close;

    public Text SeriesCount;
    public Text UserCount;
    public Text EnemyId;
    public Image EnemyRPS;
    public GameObject EnemyRPSEmpty;
    public Text MyId;
    public Image MyRPS;
    public GameObject MyRPSEmpty;

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
        Close.onClick.AddListener(OnClickExit);
        RPSSelect_S.onClick.AddListener(OnClick_S);
        RPSSelect_R.onClick.AddListener(OnClick_R);
        RPSSelect_P.onClick.AddListener(OnClick_P);
    }

    public override void SetData(PopupData data)
    {
        ResultObj.gameObject.SetActive(false);
        RPSGame_MyValue = 0;
        SeriesCount.text = string.Format("{0} 회", 0);
        EnemyId.text = "ID : ";
        MyId.text = string.Format("ID : {0}", TKManager.Instance.MyData.NickName);

        EnemyRPSEmpty.gameObject.SetActive(true);
        EnemyRPS.gameObject.SetActive(false);
        MyRPSEmpty.gameObject.SetActive(true);
        MyRPS.gameObject.SetActive(false);

        //FirebaseManager.Instance.CreateRPSGameRoom();

        StartCoroutine(Co_RPSGame());
    }

    public void RefreshUI()
    {
        SeriesCount.text = string.Format("{0} 회", FirebaseManager.Instance.FirebaseRPSGameSeries + 1);
        EnemyId.text = string.Format("ID : {0}", FirebaseManager.Instance.FirebaseRPSGame_EnemyNick);
        UserCount.text = string.Format("남은인원 : {0}명", FirebaseManager.Instance.FirebaseRPSGameUserCount);
    }

    IEnumerator Co_RPSGame()
    {
        yield return Co_RPSGame_Ready();
        while (true)
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

                if (FirebaseManager.Instance.FirebaseRPSGameUserCount <= 2)
                {
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
                        yield return Co_RPSGame_Result(result);
                        yield return Co_RPSGame_Select(true);
                        yield return Co_RPSGame_SelectWait();
                    }

                    result = GetRPSResult();

                    // 또 비김
                    if (result == 0)
                    {
                        TKManager.Instance.HideHUD();
                        FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
                        FirebaseManager.Instance.FirebaseRPSGameMyRoom = -1;
                        CloseAction();
                        // 우승
                        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("가위바위보 준우승!\n50캐쉬 획득!", () =>
                        {
                            TKManager.Instance.MyData.AddCash(50);
                        }));
                        yield break;
                    }
                }
                else
                {
                    // 또 비김
                    if (result == 0)
                    {
                        Debug.Log("결과창_1_1 " + FirebaseManager.Instance.FirebaseRPSGame_EnemyIndex + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyNick + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyValue);
                        Debug.Log("결과창_1_2 " + RPSGame_MyValue);
                        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("패배하였습니다.", () =>
                        {
                            FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
                            FirebaseManager.Instance.FirebaseRPSGameMyRoom = -1;
                            CloseAction();
                        }));
                        yield break;
                    }
                }
            }

            // 패배
            if(result == 2)
            {
                if (FirebaseManager.Instance.FirebaseRPSGameUserCount <= 2)
                {
                    TKManager.Instance.HideHUD();
                    FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
                    FirebaseManager.Instance.FirebaseRPSGameMyRoom = -1;
                    CloseAction();
                    // 우승
                    ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("가위바위보 준우승!\n50캐쉬 획득!", () =>
                    {
                        TKManager.Instance.MyData.AddCash(50);
                    }));
                    yield break;
                }
                else
                {
                    Debug.Log("결과창_2_1 " + FirebaseManager.Instance.FirebaseRPSGame_EnemyIndex + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyNick + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyValue);
                    Debug.Log("결과창_2_2 " + RPSGame_MyValue);
                    ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("패배하였습니다.", () =>
                    {
                        FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
                        FirebaseManager.Instance.FirebaseRPSGameMyRoom = -1;
                        CloseAction();
                    }));
                    yield break;
                }
            }

            // 승리
            if(result == 1)
            {
                FirebaseManager.Instance.FirebaseRPSGame_EnemyIndex = 0;
                FirebaseManager.Instance.FirebaseRPSGame_EnemyValue = 0;
                FirebaseManager.Instance.FirebaseRPSGame_EnemyNick = "";
                RefreshUI();
                yield return Co_RPSGame_Result(result);
            }
        }
    }

    IEnumerator Co_RPSGame_Ready()
    {
        // Step 1 상대방의 데이터를 받아왔는지 체크
        float waitTime = 5.0f;
        TKManager.Instance.ShowHUD("준비중 입니다.", 5.0f);

        while (true)
        {
            if(waitTime < 0)
            {
                TKManager.Instance.HideHUD();
                break;
            }

            waitTime -= Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator Co_RPSGame_Search()
    {
        // Step 1 상대방의 데이터를 받아왔는지 체크
        TKManager.Instance.ShowHUD("상대방을 검색중입니다.", 30.0f);

        while (true)
        {
            if (FirebaseManager.Instance.FirebaseRPSGameUserCount <= 1)
            {
                TKManager.Instance.HideHUD();
                CloseAction();
                // 우승
                ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("가위바위보 우승!\n100캐쉬 획득!", () =>
                {
                    TKManager.Instance.MyData.AddCash(100);
                }));
                break;
            }

            // 상대방이 검색이 됨
            if (FirebaseManager.Instance.FirebaseRPSGame_EnemyIndex != 0)
            {
                Debug.Log("매칭됨 " + FirebaseManager.Instance.FirebaseRPSGame_EnemyIndex + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyNick + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyValue);
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

        EnemyRPSEmpty.gameObject.SetActive(false);
        EnemyRPS.gameObject.SetActive(true);
        MyRPSEmpty.gameObject.SetActive(true);
        MyRPS.gameObject.SetActive(false);

        float enemyRPSChangeTime = 0.05f;
        float maxSelectTime = draw ? CommonData.RPS_GAME_DRAW_PLAY_TIME : CommonData.RPS_GAME_PLAY_TIME;
        float myPRSSelectTime = maxSelectTime;
        RandEnemyRPS();

        while (true)
        {
            enemyRPSChangeTime -= Time.deltaTime;

            if (enemyRPSChangeTime < 0)
            {
                enemyRPSChangeTime = 0.05f;
                RandEnemyRPS();
            }

            myPRSSelectTime -= Time.deltaTime;
            MyRPSTime.value = myPRSSelectTime / maxSelectTime;

            if (myPRSSelectTime < 0)
                yield break;

            yield return null;
        }
    }

    IEnumerator Co_RPSGame_SelectWait()
    {
        // Step 1 상대방의 데이터를 받아왔는지 체크
        float waitTime = 5.0f;
        TKManager.Instance.ShowHUD("결과를 확인중 입니다.", 5.0f);

        Debug.Log("내가 선택했다 " + RPSGame_MyValue);
        if (RPSGame_MyValue != 0)
            FirebaseManager.Instance.SelectRPSGame(RPSGame_MyValue);

        while (true)
        {
            waitTime -= Time.deltaTime;

            if(waitTime < 0)
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

        EnemyRPSEmpty.gameObject.SetActive(false);
        EnemyRPS.gameObject.SetActive(false);
        if (FirebaseManager.Instance.FirebaseRPSGame_EnemyValue > 0 &&
            FirebaseManager.Instance.FirebaseRPSGame_EnemyValue <= 3)
        {
            EnemyRPS.gameObject.SetActive(true);
            EnemyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[FirebaseManager.Instance.FirebaseRPSGame_EnemyValue], typeof(Sprite));
        }
        else
            EnemyRPSEmpty.gameObject.SetActive(true);

        Debug.Log("결과창_1 " + FirebaseManager.Instance.FirebaseRPSGame_EnemyIndex + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyNick + " " + FirebaseManager.Instance.FirebaseRPSGame_EnemyValue);
        Debug.Log("결과창_2 " + RPSGame_MyValue);

        ResultObj.gameObject.SetActive(true);
        if(result == 1)
        {
            if (FirebaseManager.Instance.FirebaseRPSGameUserCount <= 2)
            {
                TKManager.Instance.HideHUD();
                CloseAction();
                // 우승
                ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("가위바위보 우승!\n100캐쉬 획득!", () =>
                {
                    TKManager.Instance.MyData.AddCash(100);
                }));
                yield break;
            }
            else
            {
                // 승리
                ResultDesc.text = "승리하였습니다.\n잠시만 기달려주세요.";
            }
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
        MyRPSEmpty.gameObject.SetActive(false);
        MyRPS.gameObject.SetActive(true);
        MyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[RPSGame_MyValue], typeof(Sprite));
    }

    public void OnClick_R()
    {
        RPSGame_MyValue = 2;
        MyRPSEmpty.gameObject.SetActive(false);
        MyRPS.gameObject.SetActive(true);
        MyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[RPSGame_MyValue], typeof(Sprite));
    }
    public void OnClick_P()
    {
        RPSGame_MyValue = 3;
        MyRPSEmpty.gameObject.SetActive(false);
        MyRPS.gameObject.SetActive(true);
        MyRPS.sprite = (Sprite)Resources.Load(CommonData.RPS_GAME_IMG[RPSGame_MyValue], typeof(Sprite));
    }

    public int GetRPSResult()
    {
        if (RPSGame_MyValue == 0)
            return 2;

        if (FirebaseManager.Instance.FirebaseRPSGame_EnemyValue == 0)
            return 1;

        return (3 + RPSGame_MyValue - FirebaseManager.Instance.FirebaseRPSGame_EnemyValue) % 3;
    }

    public void OnClickExit()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("나가시겠습니까?", () =>
        {
            FirebaseManager.Instance.FirebaseRPSGameEnterEnable = false;
            FirebaseManager.Instance.FirebaseRPSGamePlayTime = long.MaxValue;
        }));
    }
}
