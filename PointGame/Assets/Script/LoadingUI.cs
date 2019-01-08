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
        yield return null;
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
