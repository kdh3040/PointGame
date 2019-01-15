using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPopup : Popup
{
    public GameObject Login;
    public Button KakaoLogin;
    public Button NaverLogin;

    public GameObject InfoInput;
    public InputField NickName;
    public Button OkButton;

    public class SignUpPopupData : PopupData
    {
        public SignUpPopupData()
        {
            PopupType = POPUP_TYPE.SIGN_UP;
        }
    }

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOkButton);
        KakaoLogin.onClick.AddListener(OnClickKakaoLogin);
        NaverLogin.onClick.AddListener(OnClickNaverLogin);
    }

    public override void SetData(PopupData data)
    {
        Login.gameObject.SetActive(true);
        InfoInput.gameObject.SetActive(false);
    }

    public void OnClickOkButton()
    {
        // TODO 닉네임 파베로 전송!!!!!!!
        CloseAction();
    }

    public void OnClickKakaoLogin()
    {
        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }

    public void OnClickNaverLogin()
    {
        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }
}
