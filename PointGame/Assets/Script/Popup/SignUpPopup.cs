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

    public GameObject MsgPopup;
    public Text MsgText;
    public Button MsgOkButton;

    private Action<string> EndAction;

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOkButton);
        MsgOkButton.onClick.AddListener(OnClickMsgOkButton);
        KakaoLogin.onClick.AddListener(OnClickKakaoLogin);
        NaverLogin.onClick.AddListener(OnClickNaverLogin);
    }

    public void init(Action<string> endAction)
    {
        EndAction = endAction;

        MsgPopup.gameObject.SetActive(false);
        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }

    public void OnClickOkButton()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        //bool emptyString = true;
        //for (int i = 0; i < NickName.text.Length; i++)
        //{
        //    if (NickName.text[i] != ' ')
        //        emptyString = false;
        //}


        //if (emptyString)
        //{
        //    MsgPopup.gameObject.SetActive(true);
        //    MsgText.text = "닉네임을 입력해주세요";
        //}
        //else
        //{
        //    EndAction(NickName.text);
        //}

        string nickName = NickName.text;

        bool emptyString = true;
        for (int i = 0; i < nickName.Length; i++)
        {
            if (nickName[i] != ' ')
                emptyString = false;
        }

        if (emptyString)
            nickName = string.Format("guest_{0:D4}", UnityEngine.Random.Range(1, 9999));

        EndAction(nickName);

        //CloseAction();
    }

    public void OnClickMsgOkButton()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        MsgPopup.gameObject.SetActive(false);
    }

    public void OnClickKakaoLogin()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }

    public void OnClickNaverLogin()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }
}
