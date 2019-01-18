using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingUI : MonoBehaviour {

    void Start()
    {
        StartCoroutine(LoadingData());
    }

    IEnumerator LoadingData()
    {
        TKManager.Instance.ShowHUD();
        yield return null;
        TKManager.Instance.LoadFile();
        yield return null;
        while (true)
        {
            if (FirebaseManager.Instance.FirstLoadingComplete)
                break;

            yield return null;
        }

        var giftList = TKManager.Instance.MyData.GiftconURLList;
        var giftUrlList = new List<string>();
        for (int i = 0; i < giftList.Count; i++)
        {
            giftUrlList.Add(giftList[i].Value);
        }

        yield return ImageCache.Instance.GetTexture(giftUrlList);

        yield return null;
        TKManager.Instance.HideHUD();
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
