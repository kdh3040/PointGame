using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public GameObject GameStartObj;
    public Button GameStartButton;
    public Text GameStageCount;

    public GameObject GameInfoObj;
    public CountImgFont GameAllPoint;
    public CountImgFont GamePoint;
    public CountImgFont GameInfoStageCount;

    public GameObject GameOverObj;
    public GameObject GameOverPopupObj;
    public Text GameOverGetPoint;
    public Button GameOverRouletteButton;
    public Button GameOverRestartButton;

    public GameObject GameClearObj;
    public Button GameClearButton;

    public GameObject InGameObj;
    public Button InGameLeftButton;
    public Button InGameRightButton;

    public void Awake()
    {
        InGameLeftButton.onClick.AddListener(OnClickLeftJump);
        InGameRightButton.onClick.AddListener(OnClickRightJump);
        GameStartButton.onClick.AddListener(OnClickGameStart);
        GameClearButton.onClick.AddListener(OnClickGameClear);
        GameOverRouletteButton.onClick.AddListener(OnClickRoulette);
        GameOverRestartButton.onClick.AddListener(OnClickGameRestart);
    }

    public void ResetUI()
    {
        GameStartObj.SetActive(false);
        GameInfoObj.SetActive(false);
        GameOverObj.SetActive(false);
        InGameObj.SetActive(false);
        GameClearObj.SetActive(false);
    }

    public void GameReady()
    {
        ResetUI();
        GameStartObj.SetActive(true);
        //GameStageCount.text = string.Format("Stage {0}", GamePlayManager.Instance.StageCount);
    }

    public void GameStart()
    {
        ResetUI();
        InGameObj.SetActive(true);
        GameInfoObj.SetActive(true);

        UpdateGameInfo();
    }

    public void GameClear()
    {
        ResetUI();
        GameClearObj.SetActive(true);
    }

    public void GameEnd()
    {
        ResetUI();
        GameOverPopupObj.gameObject.SetActive(false);
        GameOverObj.SetActive(true);

        StartCoroutine(Co_GameOverRouletteStart());
    }


    IEnumerator Co_GameOverRouletteStart()
    {
        yield return new WaitForSeconds(1f);
        GameOverPopupObj.gameObject.SetActive(true);

        GameOverGetPoint.text = string.Format("{0:n0} 포인트\n획득!", GamePlayManager.Instance.GamePoint);
    }

    public void UpdateGameInfo()
    {
        GameInfoStageCount.SetValue(string.Format("s{0}", GamePlayManager.Instance.StageCount), CountImgFont.IMG_RANGE.RIGHT, CountImgFont.IMG_TYPE.YELLOW);
        GamePoint.SetValue(string.Format("{0}", GamePlayManager.Instance.GamePoint), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);
        GameAllPoint.SetValue(string.Format("{0}", TKManager.Instance.MyData.Point), CountImgFont.IMG_RANGE.LEFT, CountImgFont.IMG_TYPE.YELLOW);
    }

    private void OnClickLeftJump()
    {
        GamePlayManager.Instance.CharJump(true);
    }

    private void OnClickRightJump()
    {
        GamePlayManager.Instance.CharJump(false);
    }

    private void OnClickGameStart()
    {
        GameStart();
        GamePlayManager.Instance.GameStart();
    }

    private void OnClickRoulette()
    {
        TKManager.Instance.MyData.AddPoint(GamePlayManager.Instance.GamePoint);
        TKManager.Instance.GameOverRouletteStart = true;
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    private void OnClickGameRestart()
    {
        AdsManager.Instance.ShowSkipRewardedAd();
        GamePlayManager.Instance.GameRestart();
    }

    private void OnClickGameClear()
    {
        if(GamePlayManager.Instance.StageCount % 3 == 0)
            AdsManager.Instance.ShowSkipRewardedAd();
        else
            AdsManager.Instance.ShowInterstitialAds();
        
        GamePlayManager.Instance.GameReady();
    }
}
