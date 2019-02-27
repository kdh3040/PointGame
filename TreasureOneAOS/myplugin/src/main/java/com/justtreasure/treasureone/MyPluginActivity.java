package com.justtreasure.treasureone;

import android.os.Bundle;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class MyPluginActivity extends UnityPlayerActivity{

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_main);
        //call from native
        //UnityPlayer.UnitySendMessage("Main Camera","CallFromNative","NativeCallFromUnity call test");
    }
    public static void CheckRooted()
    {
        try {
            Runtime.getRuntime().exec("su");
            UnityPlayer.UnitySendMessage("MainUI", "CheckRooted", "true");
        } catch(Exception e) {
            UnityPlayer.UnitySendMessage("MainUI", "CheckRooted", "false");
        }
        UnityPlayer.UnitySendMessage("MainUI", "CheckRooted", "false");
    }
}