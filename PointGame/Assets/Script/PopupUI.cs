using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum POPUP_TYPE
{
    NONE,
    MSG,
    LOTTO_MSG,
    ROULETTE,
    ROULETTE_POINT_RESULT,
    GIFT_CON_LIST,
    PUSH_BOX,
    LOTTO,
    LOTTO_WIN_INFO,
    POINT_CASH_SWAP,
    ADS,
    MINI_GAME,
    HELP,
    RPS_HELP,
    PUSH_MSG,
    RECOMMENDER_CODE,
    RPS_GAME,
    WINNER_LIST,
    RANKING_LIST,
    HAPPY_BOX,
}

public class PopupUI : MonoBehaviour {

    public List<POPUP_TYPE> QueuePopupType = new List<POPUP_TYPE>();
    public POPUP_TYPE CurrPopupType = POPUP_TYPE.NONE;

    public RoulettePopup RoulettePopupObj;
    public RoulettePointResultPopup RoulettePointResultPopupObj;
    public GiftconListPopup GiftconListPopupObj;
    public PushBoxPopup PushBoxPopupObj;
    public MsgPopup MsgPopupObj;
    public LottoPopup LottoPopupObj;
    public LottoWinPopup LottoWinPopupObj;
    public LottoMsgPopup LottoMsgPopupObj;
    public PointCashSwapPopup PointCashSwapPopupObj;
    public MiniGamePopup MiniGamePopupObj;
    public HelpPopup HelpPopupObj;
    public PushMsgPopup PushMsgPopupObj;
    public RecommenderCodePopup RecommenderCodePopupObj;
    public RPSPopup RPSPopupObj;
    public WinnerListPopup WinnerListPopupObj;
    public RankingPopup RankingPopupObj;
    public RPSHelpPopup RPSHelpPopupObj;
    public HappyBoxPopup HappyBoxPopupObj;

    public void Start()
    {
        RoulettePopupObj.gameObject.SetActive(false);
        RoulettePopupObj.Initialize(this, ClosePopup);
        RoulettePointResultPopupObj.gameObject.SetActive(false);
        RoulettePointResultPopupObj.Initialize(this, ClosePopup);
        //GiftconListPopupObj.gameObject.SetActive(false);
        //GiftconListPopupObj.Initialize(this, ClosePopup);
        PushBoxPopupObj.gameObject.SetActive(false);
        PushBoxPopupObj.Initialize(this, ClosePopup);
        MsgPopupObj.gameObject.SetActive(false);
        MsgPopupObj.Initialize(this, ClosePopup);
        LottoPopupObj.gameObject.SetActive(false);
        LottoPopupObj.Initialize(this, ClosePopup);
        LottoWinPopupObj.gameObject.SetActive(false);
        LottoWinPopupObj.Initialize(this, ClosePopup);
        LottoMsgPopupObj.gameObject.SetActive(false);
        LottoMsgPopupObj.Initialize(this, ClosePopup);
        PointCashSwapPopupObj.gameObject.SetActive(false);
        PointCashSwapPopupObj.Initialize(this, ClosePopup);
        MiniGamePopupObj.gameObject.SetActive(false);
        MiniGamePopupObj.Initialize(this, ClosePopup);
        HelpPopupObj.gameObject.SetActive(false);
        HelpPopupObj.Initialize(this, ClosePopup);
        PushMsgPopupObj.gameObject.SetActive(false);
        PushMsgPopupObj.Initialize(this, ClosePopup);
        RecommenderCodePopupObj.gameObject.SetActive(false);
        RecommenderCodePopupObj.Initialize(this, ClosePopup);
        RPSPopupObj.gameObject.SetActive(false);
        RPSPopupObj.Initialize(this, ClosePopup);
        WinnerListPopupObj.gameObject.SetActive(false);
        WinnerListPopupObj.Initialize(this, ClosePopup);
        RankingPopupObj.gameObject.SetActive(false);
        RankingPopupObj.Initialize(this, ClosePopup);
        RPSHelpPopupObj.gameObject.SetActive(false);
        RPSHelpPopupObj.Initialize(this, ClosePopup);
        HappyBoxPopupObj.gameObject.SetActive(false);
        HappyBoxPopupObj.Initialize(this, ClosePopup);
    }

    public void ShowPopup(Popup.PopupData data)
    {
        if (CurrPopupType == data.PopupType)
            ClosePopup();

        QueuePopupType.Add(data.PopupType);
        CurrPopupType = data.PopupType;

        switch (CurrPopupType)
        {
            case POPUP_TYPE.NONE:
                break;
            case POPUP_TYPE.MSG:
                MsgPopupObj.gameObject.SetActive(true);
                MsgPopupObj.SetData(data);
                break;
            case POPUP_TYPE.ROULETTE:
                RoulettePopupObj.gameObject.SetActive(true);
                RoulettePopupObj.SetData(data);
                break;
            case POPUP_TYPE.ROULETTE_POINT_RESULT:
                RoulettePointResultPopupObj.gameObject.SetActive(true);
                RoulettePointResultPopupObj.SetData(data);
                break;
            case POPUP_TYPE.GIFT_CON_LIST:
                GiftconListPopupObj.gameObject.SetActive(true);
                GiftconListPopupObj.SetData(data);
                break;
            case POPUP_TYPE.PUSH_BOX:
                PushBoxPopupObj.gameObject.SetActive(true);
                PushBoxPopupObj.SetData(data);
                break;
            case POPUP_TYPE.LOTTO:
                LottoPopupObj.gameObject.SetActive(true);
                LottoPopupObj.SetData(data);
                break;
            case POPUP_TYPE.LOTTO_WIN_INFO:
                LottoWinPopupObj.gameObject.SetActive(true);
                LottoWinPopupObj.SetData(data);
                break;
            case POPUP_TYPE.LOTTO_MSG:
                LottoMsgPopupObj.gameObject.SetActive(true);
                LottoMsgPopupObj.SetData(data);
                break;
            case POPUP_TYPE.POINT_CASH_SWAP:
                PointCashSwapPopupObj.gameObject.SetActive(true);
                PointCashSwapPopupObj.SetData(data);
                break;
            case POPUP_TYPE.MINI_GAME:
                MiniGamePopupObj.gameObject.SetActive(true);
                MiniGamePopupObj.SetData(data);
                break;
            case POPUP_TYPE.HELP:
                HelpPopupObj.gameObject.SetActive(true);
                HelpPopupObj.SetData(data);
                break;
            case POPUP_TYPE.PUSH_MSG:
                PushMsgPopupObj.gameObject.SetActive(true);
                PushMsgPopupObj.SetData(data);
                break;
            case POPUP_TYPE.RECOMMENDER_CODE:
                RecommenderCodePopupObj.gameObject.SetActive(true);
                RecommenderCodePopupObj.SetData(data);
                break;
            case POPUP_TYPE.RPS_GAME:
                RPSPopupObj.gameObject.SetActive(true);
                RPSPopupObj.SetData(data);
                break;
            case POPUP_TYPE.WINNER_LIST:
                WinnerListPopupObj.gameObject.SetActive(true);
                WinnerListPopupObj.SetData(data);
                break;
            case POPUP_TYPE.RANKING_LIST:
                RankingPopupObj.gameObject.SetActive(true);
                RankingPopupObj.SetData(data);
                break;
            case POPUP_TYPE.RPS_HELP:
                RPSHelpPopupObj.gameObject.SetActive(true);
                RPSHelpPopupObj.SetData(data);
                break;
            case POPUP_TYPE.HAPPY_BOX:
                HappyBoxPopupObj.gameObject.SetActive(true);
                HappyBoxPopupObj.SetData(data);
                break;
            default:
                break;
        }
    }

    public void ClosePopup()
    {
        if (QueuePopupType.Count <= 0)
            return;

        switch (CurrPopupType)
        {
            case POPUP_TYPE.NONE:
                break;
            case POPUP_TYPE.MSG:
                MsgPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.ROULETTE:
                RoulettePopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.ROULETTE_POINT_RESULT:
                RoulettePointResultPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.GIFT_CON_LIST:
                GiftconListPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.PUSH_BOX:
                PushBoxPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.LOTTO:
                LottoPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.LOTTO_WIN_INFO:
                LottoWinPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.LOTTO_MSG:
                LottoMsgPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.POINT_CASH_SWAP:
                PointCashSwapPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.MINI_GAME:
                MiniGamePopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.HELP:
                HelpPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.PUSH_MSG:
                PushMsgPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.RECOMMENDER_CODE:
                RecommenderCodePopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.RPS_GAME:
                RPSPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.WINNER_LIST:
                WinnerListPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.RANKING_LIST:
                RankingPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.RPS_HELP:
                RPSHelpPopupObj.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.HAPPY_BOX:
                HappyBoxPopupObj.gameObject.SetActive(false);
                break;
            default:
                break;
        }

        
        QueuePopupType.RemoveAt(QueuePopupType.Count - 1);
        if (QueuePopupType.Count <= 0)
            CurrPopupType = POPUP_TYPE.NONE;
        else
            CurrPopupType = QueuePopupType[QueuePopupType.Count - 1];
    }

    public bool IsShowPopup(POPUP_TYPE type)
    {
        return CurrPopupType == type;
    }

    public bool IsOpenShowPopup(POPUP_TYPE type)
    {
        for (int i = 0; i < QueuePopupType.Count; i++)
        {
            if (QueuePopupType[i] == type)
                return true;
        }

        return false;
    }
    
}
