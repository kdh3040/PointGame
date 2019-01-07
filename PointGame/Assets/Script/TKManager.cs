using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKManager : MonoBehaviour
{
    public static TKManager _instance = null;
    public static TKManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TKManager>() as TKManager;
            }
            return _instance;
        }
    }

    public UserData MyData = null;

    public bool GameOverRouletteStart = false;

    void Start()
    {
        MyData = new UserData();

        ImageCache.Instance.LoadImageCache("http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2");

        DontDestroyOnLoad(this);
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Application.targetFrameRate = 50;
        //Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);

        //PlayerData.Instance.Initialize();
    }
}