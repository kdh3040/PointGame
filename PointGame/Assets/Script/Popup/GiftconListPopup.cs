using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftconListPopup : Popup
{
    public GridLayoutGroup GiftconListSlotGrid;
    public ScrollRect ScrollRect;
    public Button OkButton;

    public List<GiftconListSlot> GiftconListSlotList = new List<GiftconListSlot>();

    public class GiftconListPopupData : PopupData
    {
        public GiftconListPopupData()
        {
            PopupType = POPUP_TYPE.GIFT_CON_LIST;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOk);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SetData(PopupData data)
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        var urlList = TKManager.Instance.MyData.GiftconURLList;
        for (int i = 0; i < urlList.Count; i++)
        {
            var obj = Instantiate(Resources.Load("Prefab/GiftconListSlot"), GiftconListSlotGrid.gameObject.transform) as GameObject;
            var slot = obj.GetComponent<GiftconListSlot>();
            slot.ParentPopup = ParentPopup;
            slot.SetData(urlList[i].Value);
            GiftconListSlotList.Add(slot);
            //int index = i;
            //slot.SlotButton.onClick.AddListener(() => { OnClickSkin(index); });
            //slot.SetData(SelectSkinType, skinIdList[i]);
        }

        ScrollRect.content.sizeDelta = new Vector2(ScrollRect.content.sizeDelta.x, GiftconListSlotList.Count * 250);
    }

    public void OnClickOk()
    {
        CloseAction();
    }
}
