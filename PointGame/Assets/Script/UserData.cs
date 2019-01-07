using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string Index { get; private set; }
    public string NickName = "";
    public int Point { get; private set; }

    public List<string> GiftconURLList = new List<string>();

    public void SetData(string index, string nickName, int point)
    {
        Index = index;
        NickName = nickName;
        Point = point;

        GiftconURLList.Add("http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2");
        GiftconURLList.Add("http://attach.s.op.gg/forum/20171221114845_549392.jpg");
        GiftconURLList.Add("http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2");
        GiftconURLList.Add("http://attach.s.op.gg/forum/20171221114845_549392.jpg");
        GiftconURLList.Add("http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2");
        GiftconURLList.Add("http://attach.s.op.gg/forum/20171221114845_549392.jpg");
        GiftconURLList.Add("http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2");
        GiftconURLList.Add("http://attach.s.op.gg/forum/20171221114845_549392.jpg");
        GiftconURLList.Add("http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2");
        GiftconURLList.Add("http://attach.s.op.gg/forum/20171221114845_549392.jpg");
    }

    public void AddPoint(int point)
    {
        Point += point;
    }
}