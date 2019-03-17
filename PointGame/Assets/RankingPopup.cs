using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RankingPopup : Popup
{
    public Text RankingCount;
    public Text RankingName;

    public Button OkButton;

    public class RankingPopupData : PopupData
    {
        public RankingPopupData()
        {
            PopupType = POPUP_TYPE.RANKING_LIST;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    public override void SetData(PopupData data)
    {
        List<KeyValuePair<string, int>> rankList = new List<KeyValuePair<string, int>>();
        rankList.AddRange(TKManager.Instance.ReviewRank);
        for (int i = 0; i < rankList.Count; i++)
        {
            if (rankList[i].Value <= TKManager.Instance.MyData.Point + TKManager.Instance.ReviewRankPlusScore)
            {
                rankList.Insert(i, new KeyValuePair<string, int>(TKManager.Instance.MyData.NickName, TKManager.Instance.MyData.Point + TKManager.Instance.ReviewRankPlusScore));
                break;
            }
        }

        StringBuilder winCount = new StringBuilder();
        int viewRank = 0;
        for (int i = 0; i < rankList.Count; i++)
        {
            winCount.Append(string.Format("- {0:D2}위", i + 1));
            if (i < rankList.Count - 1)
                winCount.AppendLine();

            viewRank++;

            if (viewRank >= 4)
                break;
        }

        RankingCount.text = winCount.ToString();

        StringBuilder winUser = new StringBuilder();
        viewRank = 0;
        for (int i = 0; i < rankList.Count; i++)
        {
            winUser.Append(string.Format(" : {0} ({1:n0}점)", rankList[i].Key, rankList[i].Value));
            if (i < rankList.Count - 1)
                winUser.AppendLine();

            viewRank++;

            if (viewRank >= 4)
                break;
        }

        RankingName.text = winUser.ToString();
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
