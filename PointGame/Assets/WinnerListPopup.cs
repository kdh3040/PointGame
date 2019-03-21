using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WinnerListPopup : Popup
{
    public Text LottoWinnerCount;
    public Text LottoWinnerName;
    public Text RPSWinnerCount;
    public Text RPSWinnerName;

    public Button OkButton;

    public class WinnerListPopupData : PopupData
    {
        public WinnerListPopupData()
        {
            PopupType = POPUP_TYPE.WINNER_LIST;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    public override void SetData(PopupData data)
    {
        var winList = new List<KeyValuePair<int, string>>();
        winList.AddRange(TKManager.Instance.LottoWinUserList);
        StringBuilder winCount = new StringBuilder();

        for (int i = winList.Count - 5; i <= winList.Count - 2; i++)
        {
            if (i < 0 || winList.Count <= i)
                continue;

            winCount.Append(string.Format("- {0:D2}회 당첨자", winList[i].Key + 1));
            winCount.AppendLine();
        }

        LottoWinnerCount.text = winCount.ToString();

        StringBuilder winUser = new StringBuilder();

        for (int i = winList.Count - 5; i <= winList.Count - 2; i++)
        {
            if (i < 0 || winList.Count <= i)
                continue;

            winUser.Append(string.Format(" : {0}", winList[i].Value));
            winUser.AppendLine();
        }

        LottoWinnerName.text = winUser.ToString();

        var RPSwinList = TKManager.Instance.RPSWinUserList;
        StringBuilder RPSwinCount = new StringBuilder();
        StringBuilder RPSwinUser = new StringBuilder();
        for (int i = RPSwinList.Count - 2; i <= RPSwinList.Count; i++)
        {
            if (i < 0 || RPSwinList.Count <= i)
                continue;

            RPSwinCount.Append(string.Format("- {0:D2}회 우승", RPSwinList[i].Count + 1));
            RPSwinCount.Append(string.Format("- {0:D2}회 준우승", RPSwinList[i].Count + 1));
            RPSwinCount.AppendLine();

            RPSwinUser.Append(string.Format(" : {0}", RPSwinList[i].FirstName));
            RPSwinUser.Append(string.Format(" : {0}", RPSwinList[i].SecondName));
            RPSwinUser.AppendLine();
        }

        RPSWinnerCount.text = RPSwinCount.ToString();
        RPSWinnerName.text = RPSwinUser.ToString();
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
