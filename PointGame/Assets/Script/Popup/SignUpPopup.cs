using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPopup : MonoBehaviour
{
    public GameObject Login;
    public Button GoogleLogin;
    public Button GuestLogin;

    public GameObject InfoInput;
    public InputField NickName;
    public InputField RecommenderCode;
    public Button OkButton;

    public GameObject MsgPopup;
    public Text MsgText;
    public Button MsgOkButton;

    private Action<string, string> EndAction;

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOkButton);
        MsgOkButton.onClick.AddListener(OnClickMsgOkButton);
        GoogleLogin.onClick.AddListener(OnClickGoogleLogin);
        GuestLogin.onClick.AddListener(OnClickGuestLogin);
    }

    public void init(Action<string, string> endAction)
    {
        EndAction = endAction;

        MsgPopup.gameObject.SetActive(false);
        Login.gameObject.SetActive(true);
        InfoInput.gameObject.SetActive(false);
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
        string recommenderCode = RecommenderCode.text;

        bool emptyString = true;
        for (int i = 0; i < nickName.Length; i++)
        {
            if (nickName[i] != ' ')
                emptyString = false;
        }

        if (emptyString == false)
        {
            Regex engRegex = new Regex(@"[a-zA-Z0-9]");
            bool ismatch = engRegex.IsMatch(nickName);
            if (ismatch == false)
            {
                MsgPopup.gameObject.SetActive(true);
                MsgText.text = "닉네임은 영어와 숫자만 가능합니다.";
                return;
            }
        }

        if (emptyString)
            nickName = string.Format("guest_{0:D4}", UnityEngine.Random.Range(1, 9999));

        EndAction(nickName, recommenderCode);

        //CloseAction();
    }

    public void OnClickMsgOkButton()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        MsgPopup.gameObject.SetActive(false);
    }

    public void OnClickGoogleLogin()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        // 구글 로그인 진행
        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }

    public void OnClickGuestLogin()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        // 게스트 로그인 진행
        Login.gameObject.SetActive(false);
        InfoInput.gameObject.SetActive(true);
    }
}
