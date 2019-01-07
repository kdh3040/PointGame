using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    [System.NonSerialized]
    public PopupUI ParentPopup;
    [System.NonSerialized]
    public Action CloseAction;

    public class PopupData
    {
        public POPUP_TYPE PopupType = POPUP_TYPE.NONE;
    }

    public void Initialize(PopupUI parentPopup, Action closeAction)
    {
        ParentPopup = parentPopup;
        CloseAction = closeAction;
    }

    abstract public void SetData(PopupData data);
}
