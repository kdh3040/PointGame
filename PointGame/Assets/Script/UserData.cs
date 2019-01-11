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

        GiftconURLList.Add(new KeyValuePair<int, string>(1, "https://d2192bm55jmxp1.cloudfront.net/resize/l/article/201807/e213537fb5e07554dbad59654c51f4e289e6d33844aeeddb8480ccf7b357de16.jpg"));
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

    public void DeleteGiftconData(int index)
    {
        for(int i = 0; i < GiftconURLList.Count; ++i)
        {
            if(GiftconURLList[i].Key == index)
            {
                GiftconURLList.RemoveAt(i);
                break;
            }
        }
    }


    public void AddPoint(int point)
    {
        Point += point;
    }
}