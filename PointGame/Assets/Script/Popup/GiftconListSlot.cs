using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftconListSlot : MonoBehaviour {

    [System.NonSerialized]
    public PopupUI ParentPopup;
    public Button GiftConClickButton;

    private string GiftConUrl;
    private int GiftConIndex;

    private void Awake()
    {
        GiftConClickButton.onClick.AddListener(OnClickGiftcon);
    }

    public void SetData(int index, string url)
    {
        GiftConIndex = index;
        GiftConUrl = url;
    }

    public void OnClickGiftcon()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        ParentPopup.ShowPopup(new GiftconPopup.GiftconPopupData(GiftConIndex, GiftConUrl));
    }
}
