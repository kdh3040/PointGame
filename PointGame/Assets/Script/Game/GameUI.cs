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
    public Text GameAllPoint;
    public Text GamePoint;
    public Text GameInfoStageCount;

    public GameObject GameOverObj;
    public GameObject GameOverPopupObj;
    public Text GameOverGetPoint;
    public Button GameOverRouletteButton;

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
        GameStageCount.text = string.Format("Stage {0}", GamePlayManager.Instance.StageCount);
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
        GameInfoStageCount.text = string.Format("Stage {0}", GamePlayManager.Instance.StageCount);
        GamePoint.text = string.Format("{0:n0}", GamePlayManager.Instance.GamePoint);
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

    private void OnClickGameClear()
    {
        GamePlayManager.Instance.GameReady();
    }
}
