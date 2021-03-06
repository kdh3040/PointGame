﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData : MonoBehaviour
{
    public static int[] RouletteReward = { 0, 10, 20, 30, 40, 50 };
    public static int AdsPointReward = 100;
    public static int LottoNumberCost = 1000;
    public static int UserDefaultPoint = 0;
    public static int RPSCost = 1000;
    public static int RPS_WINNER = 5000;
    public static int RPS_SECOND_WINNER = 3000;

    public static int TodayAccumulatePointLimit = 100000;
    public static int PointToCashChange = 1000;
    public static int PointToCashChangeValue = 25;
    public static int MinCashChange = 1000;
    public static int MinCashChangeUnit = 100;

    public static int LottoRefSeries = 1000000;

    public static int InGameStepChangeStage_Speed = 4;
    public static int InGameStepChangeStage = 9;

    public static int[] LottoWinTime = { 9, 12, 15, 18 };

    public static string[] RPS_GAME_IMG = { "", "RPS_S", "RPS_R", "RPS_P" };
    public static float RPS_GAME_PLAY_TIME = 5f;
    public static float RPS_GAME_RESULT_WAIT_TIME = 1f;
    public static float RPS_GAME_DRAW_PLAY_TIME = 3f;

    public static int RecommendGetCost = 500;

    public static int ChangePointMax = 100000;
    public static int[] ChangePointBonusArr = { 40000, 80000, 100000 };
    public static int BonusCash = 0;
    public static int BonusPoint = 0;

    public static int LottoWinBonus = 40000;

}
