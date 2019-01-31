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
    GIFT_CON,
    LOTTO,
    LOTTO_WIN_INFO,
    POINT_CASH_SWAP,
    ADS,
    MINI_GAME,
}

public class PopupUI : MonoBehaviour {

    public List<POPUP_TYPE> QueuePopupType = new List<POPUP_TYPE>();
    public POPUP_TYPE CurrPopupType = POPUP_TYPE.NONE;

    public RoulettePopup RoulettePopupObj;
    public RoulettePointResultPopup RoulettePointResultPopupObj;
    public GiftconListPopup GiftconListPopupObj;
    public GiftconPopup GiftconPopupObj;
    public MsgPopup MsgPopupObj;
    public LottoPopup LottoPopupObj;
    public LottoWinPopup LottoWinPopupObj;
    public LottoMsgPopup LottoMsgPopupObj;
    public PointCashSwapPopup PointCashSwapPopupObj;
    public MiniGamePopup MiniGamePopupObj;

    public void Start()
    {
        RoulettePopupObj.gameObject.SetActive(false);
        RoulettePopupObj.Initialize(this, ClosePopup);
        RoulettePointResultPopupObj.gameObject.SetActive(false);
        RoulettePointResultPopupObj.Initialize(this, ClosePopup);
        //GiftconListPopupObj.gameObject.SetActive(false);
        //GiftconListPopupObj.Initialize(this, ClosePopup);
        GiftconPopupObj.gameObject.SetActive(false);
        GiftconPopupObj.Initialize(this, ClosePopup);
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
            case POPUP_TYPE.GIFT_CON:
                GiftconPopupObj.gameObject.SetActive(true);
                GiftconPopupObj.SetData(data);
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
            case POPUP_TYPE.GIFT_CON:
                GiftconPopupObj.gameObject.SetActive(false);
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
            default:
                break;
        }

        
        QueuePopupType.RemoveAt(QueuePopupType.Count - 1);
        if (QueuePopupType.Count <= 0)
            CurrPopupType = POPUP_TYPE.NONE;
        else
            CurrPopupType = QueuePopupType[QueuePopupType.Count - 1];
    }
    
}
