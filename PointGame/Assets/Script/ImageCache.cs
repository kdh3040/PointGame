using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ImageCache : MonoBehaviour
{
    private Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();
    private Dictionary<string, WWW> requestCache = new Dictionary<string, WWW>();

    public static ImageCache _instance = null;
    public static ImageCache Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ImageCache>() as ImageCache;
            }
            return _instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadImageCache(string url)
    {
        StartCoroutine(GetTexture(url));
    }

    public void LoadImageCache(List<string> urlList)
    {
        StartCoroutine(GetTexture(urlList));
    }
    public IEnumerator GetTexture(List<string> urlList)
    {
        for (int index = 0; index < urlList.Count; ++index)
        {
            yield return GetTexture(urlList[index]);
        }
    }

    private IEnumerator GetTexture(string url, Image image = null)
    {
        if (!this.imageCache.ContainsKey(url))
        {
            int retryTimes = 3; // Number of time to retry if we get a web error
            WWW request;
            do
            {
                --retryTimes;
                if (!this.requestCache.ContainsKey(url))
                {
                    // Create a new web request and cache is so any additional
                    // calls with the same url share the same request.
                    this.requestCache[url] = new WWW(url);
                }

                request = this.requestCache[url];
                yield return request;

                // Remove this request from the cache if it is the first to finish
                if (this.requestCache.ContainsKey(url) && this.requestCache[url] == request)
                {
                    this.requestCache.Remove(url);
                }
            } while (request.error != null && retryTimes >= 0);

            // If there are no errors add this is the first to finish,
            // then add the texture to the texture cache.
            if (request.error == null && !this.imageCache.ContainsKey(url))
            {
                this.imageCache[url] = request.texture;
            }
        }

        if(image != null && GetImage(url) != null)
        {
            var cacheImage = GetImage(url);
            Rect rect = new Rect(0, 0, cacheImage.width, cacheImage.height);
            image.sprite = Sprite.Create(cacheImage, rect, new Vector2(0.5f, 0.5f));
        }

        // 캐싱 이미지 바로 로드
        //if (callback != null)
        //{
        //    // By the time we get here there is either a valid image in the cache
        //    // or we were not able to get the requested image.
        //    Texture2D texture = null;
        //    this.imageCache.TryGetValue(url, out texture);
        //    callback(texture);
        //}
    }

    public void SetImage(string url, Image image)
    {
        var cacheImage = GetImage(url);
        if (cacheImage == null)
        {
            StartCoroutine(GetTexture(url, image));
            return;
        }

        Rect rect = new Rect(0, 0, cacheImage.width, cacheImage.height);
        image.sprite = Sprite.Create(cacheImage, rect, new Vector2(0.5f, 0.5f));
    }

    public Texture2D GetImage(string url)
    {
        if (imageCache.ContainsKey(url) == false)
            return null;

        return imageCache[url];
    }
}
