using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public enum SOUND_TYPE
    {
        BUTTON,
        JUMP,
        GAME_END,
    }
    public static SoundManager _instance = null;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>() as SoundManager;
            }
            return _instance;
        }

    }

    public AudioClip[] mFxSound = new AudioClip[3];
    private AudioSource mFxAudio;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        mFxAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayFXSound(SOUND_TYPE type)
    {
        if (TKManager.Instance.SoundMute == false)
        {
            mFxAudio.clip = mFxSound[(int)type];
            mFxAudio.Play();
        }
    }
}
