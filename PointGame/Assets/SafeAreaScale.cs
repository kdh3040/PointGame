using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaScale : MonoBehaviour
{
    RectTransform Panel;
    Canvas canvas;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);
    Rect testRect = new Rect(0f, 102f / 2436f, 1f, 2202f / 2436f);

    void Awake()
    {
        Panel = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
        Refresh();
    }

    void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != LastSafeArea)
            ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        Rect rtRect = new Rect(Screen.width * testRect.x, Screen.height * testRect.y, Screen.width * testRect.width, Screen.height * testRect.height);//Screen.safeArea; // new Rect(Screen.width * testRect.x, Screen.height * testRect.y, Screen.width * testRect.width, Screen.height * testRect.height);

        //return Screen.safeArea;
        return rtRect;
    }

    [SerializeField] bool ConformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
    [SerializeField] bool ConformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)

    void ApplySafeArea(Rect r)
    {

        var safeArea = r;// Screen.safeArea;

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;

        /*
         
        LastSafeArea = r;

        if (!ConformX)
        {
            r.x = 0;
            r.width = Screen.width;
        }

        // Ignore y-axis?
        if (!ConformY)
        {
            r.y = 0;
            r.height = Screen.height;
        }

        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;

    */
    }
}