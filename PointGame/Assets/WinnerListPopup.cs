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
        var winList = TKManager.Instance.LottoWinUserList;
        StringBuilder winCount = new StringBuilder();

        for (int i = 0; i < winList.Count - 1; i++)
        {
            winCount.Append(string.Format("- {0:D2}회 당첨자", winList[i].Key + 1));
            winCount.AppendLine();
        }

        LottoWinnerCount.text = winCount.ToString();

        StringBuilder winUser = new StringBuilder();

        for (int i = 0; i < winList.Count - 1; i++)
        {
            winUser.Append(string.Format(" : {0}", winList[i].Value));
            winUser.AppendLine();
        }

        LottoWinnerName.text = winUser.ToString();

        RPSWinnerCount.text = "";
        RPSWinnerName.text = "";
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }
}
