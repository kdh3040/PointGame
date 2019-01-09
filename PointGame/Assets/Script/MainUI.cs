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
    public Text LottoNumberGet_MyNumber;
    public Text LottoNumberGet_AllPointText;
    public Button LottoNumberGet_ResultButton;
    
    public GameObject LottoResultObj;
    public Text LottoResult_MyNumber;
    public Text LottoResult_ResultNumber;
    public Button LottoResult_ReturnButton;
    public Text LottoResult_ReturnButtonText;

    public PopupUI Popup;

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
        Popup.ShowPopup(new MsgPopup.MsgPopupData("아직 당첨번호가 생성되지 않았습니다."));
        //Popup.ShowPopup(new GiftconListPopup.GiftconListPopupData());
    }

    public void OnClickMainSignUp()
    {
        Popup.ShowPopup(new SignUpPopup.SignUpPopupData());
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
        // 광고 보면 100포인트
        TKManager.Instance.MyData.AddPoint(CommonData.AdsPointReward);
    }

    public void OnClickReadyLotto()
    {
        if(TKManager.Instance.MyData.MyLottoNumber <= 0)
        {
            UIType = MAIN_UI_TYPE.LOTTO;
            UpdateUIType();
        }
        else
        {
            UIType = MAIN_UI_TYPE.LOTTO_RESULT;
            UpdateUIType();
        }
    }

    public void ViewRouletteObj()
    {
        // 기프티콘 갯수 파베에서 확인
        RouletteObj.SetActive(true);
    }
    public void OnClickRouletteStart()
    {
        // 룰렛 진행 하고

        var percentValue = Random.Range(0, 101); // 100으로 하면 99까지만 나옴

        var roulettePercent = TKManager.Instance.RoulettePercent;

        for(int index = 0; index < roulettePercent.Count; ++index)
        {
            if(index == 0)
            {
                if(roulettePercent[index].Value >= percentValue)
                {
                    Popup.ShowPopup(new RouletteResultPopup.RouletteResultPopupData(TKManager.Instance.RouletteGiftconUrl, RouletteResultClose));
                    break;
                }
            }
            else if(roulettePercent[index - 1].Value < percentValue &&
                roulettePercent[index].Value >= percentValue)
            {
                Popup.ShowPopup(new RouletteResultPopup.RouletteResultPopupData(roulettePercent[index].Key, RouletteResultClose));
                break;
            }
        }
    }

    public void RouletteResultClose()
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
        if(CommonData.LottoNumberCost > TKManager.Instance.MyData.Point)
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData("포인트가 충분하지 않습니다.", LottoGetMsgClose));
        }
        else
        {
            // 번호 획득
            UIType = MAIN_UI_TYPE.LOTTO_NUMBER_GET;
            UpdateUIType();
        }
    }

    public void LottoGetMsgClose()
    {
        UIType = MAIN_UI_TYPE.READY;
        UpdateUIType();
    }

    public void ViewLottoNumberGetObj()
    {
        LottoNumberGet_MyNumber.text = string.Format("{0}회 Happy 번호\n{1}", TKManager.Instance.MyData.MyLottoSeriesCount, TKManager.Instance.MyData.MyLottoNumber);
        LottoNumberGetObj.SetActive(true);
    }
    public void OnClickLottoResult()
    {
        if(TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.CurrentLottoSeriesCount)
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData("아직 당첨번호가 생성되지 않았습니다."));
        }
        else
        {
            UIType = MAIN_UI_TYPE.LOTTO_RESULT;
            UpdateUIType();
        }
    }

    public void ViewLottoResultObj()
    {
        LottoResult_MyNumber.text = string.Format("{0}회 Happy 번호\n{1}", TKManager.Instance.MyData.MyLottoSeriesCount, TKManager.Instance.MyData.MyLottoNumber);
        LottoResult_ResultNumber.text = string.Format("{0}회 Happy 당첨번호\n{1}", TKManager.Instance.ResultLottoSeriesCount, TKManager.Instance.ResultLottoNumber);

        if(TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.ResultLottoSeriesCount &&
            TKManager.Instance.MyData.MyLottoNumber == TKManager.Instance.ResultLottoNumber)
        {
            Popup.ShowPopup(new MsgPopup.MsgPopupData("당첨을 축하 드립니다!"));
            LottoResult_ReturnButtonText.text = "당첨금 받기";
        }
        else
        {
            LottoResult_ReturnButtonText.text = "다음 기회에..";
        }

        
        LottoResultObj.SetActive(true);
    }
    public void OnClickReturn()
    {
        if (TKManager.Instance.MyData.MyLottoSeriesCount == TKManager.Instance.ResultLottoSeriesCount &&
            TKManager.Instance.MyData.MyLottoNumber == TKManager.Instance.ResultLottoNumber)
        {
            Popup.ShowPopup(new LottoWinPopup.LottoWinPopupData());
        }
        else
        {
            UIType = MAIN_UI_TYPE.MAIN;
            UpdateUIType();
        }

        // TODO 배환웅 로또 번호 제거
        TKManager.Instance.MyData.MyLottoNumber = 0;
    }
}
