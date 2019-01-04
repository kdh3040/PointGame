using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {

    public enum MAIN_UI_TYPE
    {
        MAIN,
        READY,
        ROULETTE,
        LOTTO,
        LOTTO_NUMBER_GET,
        LOTTO_RESULT
    }

    public GameObject MainObj;
    public Button Main_StartButton;
    public Button Main_GiftBoxButton;
    public Button Main_SignUpButton;
    public Text Main_LottoWinText;

    public GameObject ReadyObj;
    public Button Ready_GameStartButton;
    public Button Ready_AdViewButton;
    public Button Ready_PointRewardButton;
    public Button Ready_LottoButton;

    public GameObject RouletteObj;
    public Button Roulette_StartButton;

    public GameObject LottoObj;
    public Text Lotto_AllPointText;
    public Button Lotto_GetButton;

    public GameObject LottoNumberGetObj;
    public Text LottoNumberGet_AllPointText;
    public Button LottoNumberGet_ResultButton;
    
    public GameObject LottoResultObj;
    public Button LottoResult_ReturnButton;

    private MAIN_UI_TYPE UIType = MAIN_UI_TYPE.MAIN;

    private void Awake()
    {
        Main_StartButton.onClick.AddListener(OnClickMainStart);
        Main_GiftBoxButton.onClick.AddListener(OnClickMainGiftBox);
        Main_SignUpButton.onClick.AddListener(OnClickMainSignUp);
        Ready_GameStartButton.onClick.AddListener(OnClickReadyGameStart);
        Ready_AdViewButton.onClick.AddListener(OnClickReadyAdView);
        Ready_PointRewardButton.onClick.AddListener(OnClickReadyPointReward);
        Ready_LottoButton.onClick.AddListener(OnClickReadyLotto);
        Roulette_StartButton.onClick.AddListener(OnClickRouletteStart);
        Lotto_GetButton.onClick.AddListener(OnClickLottoGet);
        LottoNumberGet_ResultButton.onClick.AddListener(OnClickLottoResult);
        LottoResult_ReturnButton.onClick.AddListener(OnClickReturn);
    }

    // Use this for initialization
    void Start () {
        if (TKManager.Instance.GameOverRouletteStart == false)
            UIType = MAIN_UI_TYPE.MAIN;
        else
            UIType = MAIN_UI_TYPE.ROULETTE;

        UpdateUIType();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateUIType()
    {
        MainObj.SetActive(false);
        ReadyObj.SetActive(false);
        RouletteObj.SetActive(false);
        LottoObj.SetActive(false);
        LottoNumberGetObj.SetActive(false);
        LottoResultObj.SetActive(false);

        switch (UIType)
        {
            case MAIN_UI_TYPE.MAIN:
                ViewMainObj();
                break;
            case MAIN_UI_TYPE.READY:
                ViewReadyObj();
                break;
            case MAIN_UI_TYPE.ROULETTE:
                ViewRouletteObj();
                break;
            case MAIN_UI_TYPE.LOTTO:
                ViewLottoObj();
                break;
            case MAIN_UI_TYPE.LOTTO_NUMBER_GET:
                ViewLottoNumberGetObj();
                break;
            case MAIN_UI_TYPE.LOTTO_RESULT:
                ViewLottoResultObj();
                break;
            default:
                break;
        }
    }

    public void ViewMainObj()
    {
        MainObj.SetActive(true);
    }

    public void OnClickMainStart()
    {
        UIType = MAIN_UI_TYPE.READY;
        UpdateUIType();
    }

    public void OnClickMainGiftBox()
    {

    }

    public void OnClickMainSignUp()
    {

    }

    public void ViewReadyObj()
    {
        ReadyObj.SetActive(true);
    }

    public void OnClickReadyGameStart()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void OnClickReadyAdView()
    {
        UIType = MAIN_UI_TYPE.ROULETTE;
        UpdateUIType();
    }

    public void OnClickReadyPointReward()
    {

    }

    public void OnClickReadyLotto()
    {
        UIType = MAIN_UI_TYPE.LOTTO;
        UpdateUIType();
    }

    public void ViewRouletteObj()
    {
        RouletteObj.SetActive(true);
    }
    public void OnClickRouletteStart()
    {
        UIType = MAIN_UI_TYPE.READY;
        UpdateUIType();
    }

    public void ViewLottoObj()
    {
        LottoObj.SetActive(true);

        Lotto_AllPointText.text = string.Format("총 포인트 : {0:n0}", TKManager.Instance.MyData.Point);
    }
    public void OnClickLottoGet()
    {
        UIType = MAIN_UI_TYPE.LOTTO_NUMBER_GET;
        UpdateUIType();
    }

    public void ViewLottoNumberGetObj()
    {
        LottoNumberGetObj.SetActive(true);
    }
    public void OnClickLottoResult()
    {
        UIType = MAIN_UI_TYPE.LOTTO_RESULT;
        UpdateUIType();
    }

    public void ViewLottoResultObj()
    {
        LottoResultObj.SetActive(true);
    }
    public void OnClickReturn()
    {
        UIType = MAIN_UI_TYPE.MAIN;
        UpdateUIType();
    }
}
