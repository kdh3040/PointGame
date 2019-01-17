using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoulettePopup : Popup
{
    public Button StartButton;
    public Button OkButton;
    public GameObject RoulettePanObj;
    public List<Text> RoulettePointText;

    public class RoulettePopupData : PopupData
    {
        public RoulettePopupData()
        {
            PopupType = POPUP_TYPE.ROULETTE;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
        StartButton.onClick.AddListener(OnClickStart);
    }

    public override void SetData(PopupData data)
    {
        RefreshUI(false);
    }

    public void RefreshUI(bool resultView)
    {
        OkButton.gameObject.SetActive(resultView);
        StartButton.gameObject.SetActive(!resultView);
    }

    public void OnClickOk()
    {
        CloseAction();
    }
    public void OnClickStart()
    {
        StartCoroutine(Co_Roulette());
    }

    IEnumerator Co_Roulette()
    {
        yield return null;

        RefreshUI(true);

        var percentValue = Random.Range(0, 101); // 100으로 하면 99까지만 나옴

        var roulettePercent = TKManager.Instance.RoulettePercent;

        for (int index = 0; index < roulettePercent.Count; ++index)
        {
            if ((index == 0 && roulettePercent[index].Value >= percentValue) ||
                (index > 0 && roulettePercent[index - 1].Value < percentValue && roulettePercent[index].Value >= percentValue))
            {
                if (roulettePercent[index].Key == 0)
                {
                    // TODO 룰렛 결과에서 받을 기프티콘의 url과 index를 받아야함
                    ParentPopup.ShowPopup(new GiftconPopup.GiftconPopupData(0, TKManager.Instance.RouletteGiftconUrl, false));
                }
                else
                    ParentPopup.ShowPopup(new RoulettePointResultPopup.RoulettePointResultPopupData(roulettePercent[index].Key));
                break;
            }
        }
    }
}
