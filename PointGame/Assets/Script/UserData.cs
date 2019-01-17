using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string Index { get; private set; }
    public string NickName = "";
    public int Point { get; private set; }

    public List<KeyValuePair<int, string>> GiftconURLList = new List<KeyValuePair<int, string>>();
    
    public Dictionary<int, int> LottoList = new Dictionary<int, int>();
    public Dictionary<int, bool> LottoResultShowSeriesList = new Dictionary<int, bool>();
    public Dictionary<int, bool> LottoWinSeriesList = new Dictionary<int, bool>();

    // TODO 내정보, 로또정보, 가지고 있는 기프티콘 이미지 로드 할때까지 로딩 페이지에서 머무르게끔 해야함
    public void SetData(string index, string nickName, int point)
    {
        Index = index;
        NickName = nickName;
        Point = point;

       // GiftconURLList.Add(new KeyValuePair<int, string>(1, "https://d2192bm55jmxp1.cloudfront.net/resize/l/article/201807/e213537fb5e07554dbad59654c51f4e289e6d33844aeeddb8480ccf7b357de16.jpg"));
    }

    public void SetLottoData(int LottoSeries, int LottoNumber)
    {
        LottoList.Add(LottoSeries, LottoNumber);
    }

    public void SetLottoWinSeriesData(int LottoSeries)
    {
        LottoWinSeriesList.Add(LottoSeries, true);
    }

    public void SetGiftconData(int index, string src)
    {
       GiftconURLList.Add(new KeyValuePair<int, string>(index, src));        
    }

    public void DeleteGiftconData(int index)
    {
        // TODO 파베 연동
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
    public void RemovePoint(int point)
    {
        Point -= point;
        if (Point < 0)
            Point = 0;
    }
}