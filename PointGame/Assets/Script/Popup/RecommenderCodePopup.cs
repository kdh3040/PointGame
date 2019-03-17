using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecommenderCodePopup : Popup
{
    public Text Code;
    public Button OkButton;
    public Button CopyButton;

    public class RecommenderCodePopupData : PopupData
    {
        public RecommenderCodePopupData()
        {
            PopupType = POPUP_TYPE.RECOMMENDER_CODE;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
        CopyButton.onClick.AddListener(OnClickCopy);
    }

    public override void SetData(PopupData data)
    {
        if(TKManager.Instance.MyData.RecommenderCode == "")
        {
            FirebaseManager.Instance.SetRecommenderCode();
        }        

        Code.text = FirebaseManager.Instance.GetRecommenderCode();
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }

    public void OnClickCopy()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();

        GUIUtility.systemCopyBuffer = Code.text;
        ParentPopup.ShowPopup(new MsgPopup.MsgPopupData("복사하였습니다"));
    }
}
