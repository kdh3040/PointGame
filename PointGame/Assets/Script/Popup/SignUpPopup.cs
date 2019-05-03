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
    public Text LoginMsg;
    public Text InfoMsg;
    public Button GuestLogin;

    public GameObject InfoInput;
    public InputField NickName;
    public InputField RecommenderCode;
    public Button OkButton;

    public GameObject MsgPopup;
    public Text MsgText;
    public Button MsgOkButton;

    private Action<string, string> EndAction;
    private string NickNameStr;
    private string RecommenderCodeStr;

    private void Awake()
    {
        OkButton.onClick.AddListener(OnClickOkButton);
        
        GoogleLogin.onClick.AddListener(OnClickGoogleLogin);
        GuestLogin.onClick.AddListener(OnClickGuestLogin);
    }

    public void init(Action<string, string> endAction)
    {
        EndAction = endAction;

        MsgPopup.gameObject.SetActive(false);
        Login.gameObject.SetActive(true);
        InfoInput.gameObject.SetActive(false);

        FirebaseManager.Instance.GetReviewVersion(() => { });
#if UNITY_ANDROID

        LoginMsg.text = "구글 로그인";
        InfoMsg.text = "- 구글 로그인은 필수가 아닙니다";
#elif UNITY_IOS
 
        LoginMsg.text = "애플 로그인";
        InfoMsg.text = "- 애플 게임센터 로그인은 필수가 아닙니다";
#endif



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

        NickNameStr = NickName.text;
        RecommenderCodeStr = RecommenderCode.text;

        bool emptyString = true;
        for (int i = 0; i < NickNameStr.Length; i++)
        {
            if (NickNameStr[i] != ' ')
                emptyString = false;
        }

        if (emptyString == false)
        {
            Regex engRegex = new Regex(@"^[a-zA-Z0-9_]{0,20}$");
            bool ismatch = engRegex.IsMatch(NickNameStr);
            if (ismatch == false)
            {
                MsgOkButton.onClick.RemoveAllListeners();
                MsgOkButton.onClick.AddListener(OnClickMsgOkButton);
                MsgPopup.gameObject.SetActive(true);
                MsgText.text = "닉네임은 영어와 숫자만 가능합니다";
                return;
            }
        }

        NickNameStr = NickNameStr.Replace(".", "");
        NickNameStr = NickNameStr.Replace("#", "");
        NickNameStr = NickNameStr.Replace("$", "");
        NickNameStr = NickNameStr.Replace("[", "");
        NickNameStr = NickNameStr.Replace("]", "");

        if (emptyString)
        {
            NickNameStr = string.Format("guest_{0:D4}", UnityEngine.Random.Range(1, 9999));

            if (FirebaseManager.Instance.ReviewMode)
            {
                EndAction(NickNameStr, RecommenderCodeStr);
            }
            else
            {
                MsgOkButton.onClick.RemoveAllListeners();
                MsgOkButton.onClick.AddListener(OnClickMsgLoginOkButton);
                MsgPopup.gameObject.SetActive(true);
                MsgText.text = "게임 설명과 푸시 알림을\n확인 후 플레이 하시기 바랍니다";
            }
        }
        else
        {
            if (FirebaseManager.Instance.ReviewMode)
            {
                EndAction(NickNameStr, RecommenderCodeStr);
            }
            else
            {
                FirebaseManager.Instance.IsExistNickName(NickNameStr, () =>
                {
                    if (FirebaseManager.Instance.NickNameExist)
                    {
                        MsgOkButton.onClick.RemoveAllListeners();
                        MsgOkButton.onClick.AddListener(OnClickMsgOkButton);
                        MsgPopup.gameObject.SetActive(true);
                        MsgText.text = "닉네임이 중복 입니다";
                    }
                    else
                    {
                        if (FirebaseManager.Instance.ReviewMode)
                        {
                            EndAction(NickNameStr, RecommenderCodeStr);
                        }
                        else
                        {
                            MsgOkButton.onClick.RemoveAllListeners();
                            MsgOkButton.onClick.AddListener(OnClickMsgLoginOkButton);
                            MsgPopup.gameObject.SetActive(true);
                            MsgText.text = "게임 설명과 푸시 알림을\n확인 후 플레이 하시기 바랍니다";
                        }
                    }
                });
            }
        }

        //CloseAction();
    }

    public void OnClickMsgOkButton()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        MsgPopup.gameObject.SetActive(false);
    }

    public void OnClickMsgLoginOkButton()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        MsgPopup.gameObject.SetActive(false);

        EndAction(NickNameStr, RecommenderCodeStr);
    }

    public void OnClickGoogleLogin()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.SOUND_TYPE.BUTTON);
        // 구글 로그인 진행
        FirebaseManager.Instance.GoogleLogin();


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
