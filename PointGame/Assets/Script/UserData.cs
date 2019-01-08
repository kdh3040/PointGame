using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string Index { get; private set; }
    public string NickName = "";
    public int Point { get; private set; }

    public List<KeyValuePair<int, string>> GiftconURLList = new List<KeyValuePair<int, string>>();

    public int MyLottoSeriesCount = 0;
    public int MyLottoNumber = 0;

    public void SetData(string index, string nickName, int point)
    {
        Index = index;
        NickName = nickName;
        Point = point;

        /*
        GiftconURLList.Add(new KeyValuePair<int, string>(1,"http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2"));
        GiftconURLList.Add(new KeyValuePair<int, string>(2,"http://attach.s.op.gg/forum/20171221114845_549392.jpg"));
        GiftconURLList.Add(new KeyValuePair<int, string>(3,"http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2"));
        GiftconURLList.Add(new KeyValuePair<int, string>(4,"http://attach.s.op.gg/forum/20171221114845_549392.jpg"));
        GiftconURLList.Add(new KeyValuePair<int, string>(5,"http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2"));
        GiftconURLList.Add(new KeyValuePair<int, string>(6,"http://attach.s.op.gg/forum/20171221114845_549392.jpg"));
        GiftconURLList.Add(new KeyValuePair<int, string>(7,"http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2"));
        GiftconURLList.Add(new KeyValuePair<int, string>(8,"http://attach.s.op.gg/forum/20171221114845_549392.jpg"));
        GiftconURLList.Add(new KeyValuePair<int, string>(9,"http://mblogthumb2.phinf.naver.net/20130120_157/liebe3722_13586901613759XDol_JPEG/%C7%C7%C0%DA%C7%EA%B1%E2%C7%C1%C6%BC%C4%DC.jpg?type=w2"));
        GiftconURLList.Add(new KeyValuePair<int, string>(10, "http://attach.s.op.gg/forum/20171221114845_549392.jpg"));
        */
    }

    public void SetLottoData(int LottoSeries, int LottoNumber)
    {
        MyLottoSeriesCount = LottoSeries;
        MyLottoNumber = LottoNumber;
    }

    public void SetGiftconData(int index, string src)
    {
       GiftconURLList.Add(new KeyValuePair<int, string>(index, src));        
    }

    public void AddPoint(int point)
    {
        Point += point;
    }
}