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