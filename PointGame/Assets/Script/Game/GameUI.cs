using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public Button LeftButton;
    public Button RightButton;

    public void Awake()
    {
        LeftButton.onClick.AddListener(OnClickLeftJump);
        RightButton.onClick.AddListener(OnClickRightJump);
    }

    private void OnClickLeftJump()
    {
        GamePlayManager.Instance.CharJump(true);
    }

    private void OnClickRightJump()
    {
        GamePlayManager.Instance.CharJump(false);
    }
}
