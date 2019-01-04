using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public int Index { get; private set; }
    public string NickName = "";
    public int Point { get; private set; }

    public void SetData(int index, string nickName, int point)
    {
        Index = index;
        NickName = nickName;
        Point = point;
    }

    public void AddPoint(int point)
    {
        Point += point;
    }
}