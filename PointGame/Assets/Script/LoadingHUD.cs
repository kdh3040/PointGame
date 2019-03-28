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
    private string WaitTimeMsg = "";
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
        if (WaitTime > 0)
        {
            if(WaitTimeMsg == "")
                Desc.text = string.Format("{0}\n{1}초", Msg, (int)WaitTime);
            else
                Desc.text = string.Format("{0}{1}", Msg, string.Format(WaitTimeMsg, (int)WaitTime));
        }
            
        else
            Desc.text = Msg;

        WaitTime -= Time.deltaTime;
    }

    public void ShowHUD(string msg = "로딩중", string subMsg = "", float waitTime = 0, string waitTimeMsg = "")
    {
        Msg = "";
        WaitTime = 0;
        Desc.text = Msg;
        SubDesc.text = "";
        WaitTimeMsg = "";

        Msg = msg;
        WaitTime = waitTime;
        Desc.text = Msg;
        SubDesc.text = subMsg;
        WaitTimeMsg = waitTimeMsg;
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
