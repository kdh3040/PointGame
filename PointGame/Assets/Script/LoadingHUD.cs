using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHUD : MonoBehaviour {

    public Canvas HUDCanvas;
    public Image ButonImg;
    public Text Desc;
    public Image LoadingMark;
    public Text SubDesc;

    private string Msg;
    private float WaitTime = 0;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        HUDCanvas.sortingOrder = 10;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (WaitTime > 1)
            Desc.text = string.Format("{0}\n{1}초", Msg, (int)WaitTime);
        else
            Desc.text = Msg;

        WaitTime -= Time.deltaTime;
    }

    public void ShowHUD(string msg = "로딩중", float waitTime = 0, string subMsg = "")
    {
        Msg = msg;
        WaitTime = waitTime + 1;
        Desc.text = Msg;
        SubDesc.text = subMsg;
        StartCoroutine(Co_ShowHUD());

        LoadingMark.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void HideHUD()
    {
        StopAllCoroutines();
    }

    IEnumerator Co_ShowHUD()
    {
        while(true)
        {
            LoadingMark.gameObject.transform.localRotation = Quaternion.Euler(0, 0, Mathf.PingPong(Time.time, 380f) * 200);
            yield return null;
        }
    }
}
