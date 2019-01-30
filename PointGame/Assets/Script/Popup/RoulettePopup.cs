using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoulettePopup : Popup
{
    public Button StartButton;
    public GameObject RoulettePanObj;
    public List<Text> RoulettePointText;
    public List<Image> RouletteGiftconImg;

    private List<KeyValuePair<int, int>> RoulettePercent = new List<KeyValuePair<int, int>>();
    private List<int> RouletteAngle = new List<int>();
    private bool RoulettePlay = false;

    public class RoulettePopupData : PopupData
    {
        public RoulettePopupData()
        {
            PopupType = POPUP_TYPE.ROULETTE;
        }
    }

    private void Awake()
    {
        StartButton.onClick.AddListener(OnClickStart);

        // 룰렛 최소값 최대값 에서 약 10도씩 뺴야할것 같음 
        RouletteAngle.Add(0);
        RouletteAngle.Add(60);
        RouletteAngle.Add(120);
        RouletteAngle.Add(180);
        RouletteAngle.Add(240);
        RouletteAngle.Add(300);
    }

    public override void SetData(PopupData data) 
    {
        RoulettePlay = false;
        RoulettePanObj.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0) ;

        RoulettePercent.Clear();
        RoulettePercent.AddRange(TKManager.Instance.RoulettePercent);
        //ShuffleList(RoulettePercent);

        for (int i = 0; i < RoulettePercent.Count; i++)
        {
            RoulettePointText[i].gameObject.SetActive(false);
            RouletteGiftconImg[i].gameObject.SetActive(false);

            if (i == 0)
            {
                RoulettePointText[i].gameObject.SetActive(true);
                RoulettePointText[i].text = string.Format("{0:n0}C", RoulettePercent[i].Key);
            }
            else
            {
                RoulettePointText[i].gameObject.SetActive(true);
                RoulettePointText[i].text = string.Format("{0:n0}P", RoulettePercent[i].Key);
            }
        }
    }

    public void OnClickOk()
    {
        CloseAction();
    }
    public void OnClickStart()
    {
        if (RoulettePlay)
            return;

        StartCoroutine(Co_Roulette());
    }

    IEnumerator Co_Roulette()
    {
        RoulettePlay = true;


        KeyValuePair<int, int> keyValue = new KeyValuePair<int, int>();
        var roulettePercent = TKManager.Instance.RoulettePercent;
        if (roulettePercent.Count == 0)
            yield break;

        int maxValue = roulettePercent[roulettePercent.Count - 1].Value;

        var percentValue = Random.Range(0, maxValue + 1); // 100으로 하면 99까지만 나옴

        for (int index = 0; index < roulettePercent.Count; ++index)
        {
            if (roulettePercent[index].Value <= 0)
                continue;

            if ((index == 0 && roulettePercent[index].Value >= percentValue) ||
                (index > 0 && roulettePercent[index - 1].Value < percentValue && roulettePercent[index].Value >= percentValue))
            {
                keyValue = roulettePercent[index];

                iTween.RotateAdd(RoulettePanObj, iTween.Hash("z", 360 * 2 + RouletteAngle[index], "time", 2f, "easetype", iTween.EaseType.easeInOutQuart));
                yield return new WaitForSeconds(2f);

                break;
            }
        }

        yield return new WaitForSeconds(0.3f);

        CloseAction();

        ParentPopup.ShowPopup(new RoulettePointResultPopup.RoulettePointResultPopupData(keyValue.Key));
    }

    public void RouletteGiftconResult(int giftconIndex)
    {
        KeyValuePair<int, string>  data = TKManager.Instance.MyData.GetGiftconData(giftconIndex);
        ParentPopup.ShowPopup(new GiftconPopup.GiftconPopupData(data.Key, data.Value, false));
    }

    public void ShuffleList<T>(List<T> list)
    {
        int random1;
        int random2;

        T tmp;

        for (int index = 0; index < list.Count; ++index)
        {
            random1 = UnityEngine.Random.Range(0, list.Count);
            random2 = UnityEngine.Random.Range(0, list.Count);

            tmp = list[random1];
            list[random1] = list[random2];
            list[random2] = tmp;
        }
    }
}
