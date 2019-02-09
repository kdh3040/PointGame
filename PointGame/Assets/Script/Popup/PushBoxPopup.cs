using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushBoxPopup : Popup
{
    public Image GiftconImage;
    public Button OkButton;
    public Text EmptyText;

    public GameObject PushGridList;
    public List<Button> PushListButtonList = new List<Button>();
    public List<Text> PushListTextList = new List<Text>();

    public class PushBoxPopupData : PopupData
    {
        public PushBoxPopupData()
        {
            PopupType = POPUP_TYPE.PUSH_BOX;
        }
    }

    // Use this for initialization
    void Start ()
    {
        OkButton.onClick.AddListener(OnClickOk);
        PushListButtonList[0].onClick.AddListener(OnClickPush_1);
        PushListButtonList[1].onClick.AddListener(OnClickPush_2);
        PushListButtonList[2].onClick.AddListener(OnClickPush_3);
        PushListButtonList[3].onClick.AddListener(OnClickPush_4);
    }

    public override void SetData(PopupData data)
    {
        if(FirebaseManager.Instance.ReviewMode)
        {
            EmptyText.gameObject.SetActive(true);
            PushGridList.gameObject.SetActive(false);
        }
        else
        {
            List<KeyValuePair<string, string>> pushList = FirebaseManager.Instance.PushList;

            TKManager.Instance.PushNotiEnable = false;
            TKManager.Instance.PushLastIndex = FirebaseManager.Instance.PushLastIndex;
            TKManager.Instance.SaveFile();

            EmptyText.gameObject.SetActive(false);
            PushGridList.gameObject.SetActive(false);

            if (pushList.Count <= 0)
            {
                EmptyText.gameObject.SetActive(true);
            }
            else
            {
                PushGridList.gameObject.SetActive(true);

                for (int i = 0; i < PushListButtonList.Count; i++)
                {
                    PushListButtonList[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < pushList.Count; i++)
                {
                    if (PushListButtonList.Count <= i)
                        break;

                    PushListButtonList[i].gameObject.SetActive(true);
                    PushListTextList[i].text = string.Format("{0}. {1}", i + 1, pushList[i].Key);
                }
            }
        }
        
    }

    public void RefreshUI()
    {
       
    }

    public void OnClickOk()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        CloseAction();
    }

    public void OnClickPush_1()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        var data = FirebaseManager.Instance.PushList[0];
        ParentPopup.ShowPopup(new PushMsgPopup.PushMsgPopupData(data.Key, data.Value));
    }
    public void OnClickPush_2()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        var data = FirebaseManager.Instance.PushList[1];
        ParentPopup.ShowPopup(new PushMsgPopup.PushMsgPopupData(data.Key, data.Value));
    }
    public void OnClickPush_3()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        var data = FirebaseManager.Instance.PushList[2];
        ParentPopup.ShowPopup(new PushMsgPopup.PushMsgPopupData(data.Key, data.Value));
    }
    public void OnClickPush_4()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        var data = FirebaseManager.Instance.PushList[3];
        ParentPopup.ShowPopup(new PushMsgPopup.PushMsgPopupData(data.Key, data.Value));
    }

}
