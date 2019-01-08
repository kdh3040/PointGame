using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftconListSlot : MonoBehaviour {

    [System.NonSerialized]
    public PopupUI ParentPopup;
    public Button GiftConClickButton;

    private string GiftConUrl;

    private void Awake()
    {
        GiftConClickButton.onClick.AddListener(OnClickGiftcon);
    }

    public void SetData(string url)
    {
        GiftConUrl = url;
    }

    public void OnClickGiftcon()
    {
        ParentPopup.ShowPopup(new GiftconPopup.GiftconPopupData(GiftConUrl));
    }
}
