using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPopup : MonoBehaviour
{
    public GameObject Login;
    public Button KakaoLogin;
    public Button NaverLogin;

    public GameObject InfoInput;
    public InputField NickName;
    public Button OkButton;

    private Action<string> EndAction;

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOkButton);
        KakaoLogin.onClick.AddListener(OnClickKakaoLogin);
        NaverLogin.onClick.AddListener(OnClickNaverLogin);
    }

    public void init(Action<string> endAction)
    {
        EndAction = endAction;

        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }

    public void OnClickOkButton()
    {
        EndAction(NickName.text);
        //CloseAction();
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
