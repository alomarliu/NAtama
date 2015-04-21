using System.Collections;
using UnityEngine;
using UIFrameWork;
using LitJson;

/**=============================================================
 * <summary> 設定視窗 </summary>
 * 
 * <author>林佳葦</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2015 All Rights Reserved
 =============================================================*/

public class UISettingController : UIController
{

    public UIToggle[] togSettings = new UIToggle[4];
    public UITweener[] tweenSettings = new UITweener[4];
    public UISlider[] volumeSlider = new UISlider[2];

    private static UISettingController _instance = null;

    private bool[] settings = { true, true, true, true };

    /**=============================================
     * 取得 Singleton
     * ===========================================*/
    public static UISettingController instance
    {
        get
        {
            if (_instance == null)
                _instance = UIManager.instance.GetUI<UISettingController>();

            return _instance;
        }
    }

    override public void Open(params object[] values)
    {
        UpdateUI();
        base.Open();
    }

    override public void Close()
    {
        // do Something
        base.Close();

    }


    /**=============================================
     * <summary>更新畫面</summary>
     * ===========================================*/

    private void UpdateUI()
    {
        JsonData jdSet = PlayerPrefManager.instance.GetSetting();

        bool[] loadSetting = new bool[settings.Length];

        //讀取設定紀錄
        if (jdSet.Keys.Contains("Music"))
        {
            loadSetting[0] = (jdSet["Music"].ToString() == "True");
            loadSetting[1] = (jdSet["Sound"].ToString() == "True");
            loadSetting[2] = (jdSet["Vibrate"].ToString() == "True");
            loadSetting[3] = (jdSet["Conversation"].ToString() == "True");
        }

        //改變Toggle顯示狀態
        for (int i = 0; i < settings.Length; i++)
        {
            if (loadSetting[i] != settings[i])
            {
                tweenSettings[i].Toggle();
                togSettings[i].value = loadSetting[i];
                settings[i] = loadSetting[i];
            }
        }

        //隱藏或顯示音量拉條
        for (int i = 0; i < volumeSlider.Length; i++)
        {   
            volumeSlider[i].gameObject.SetActive(settings[i]);
         }


        //載入設定音量
        float[] vol = PlayerPrefManager.instance.GetVolume();
        

        //設定音量大小
        for (int i = 0; i < volumeSlider.Length; i++)
        {
            volumeSlider[i].value = vol[i];
        }

    }


    /**================================
     * <summary> Tween結束 </summary>
     *===============================*/
    public void OnTweenEnd()
    {
        //將音量暫時儲存至volume中
        float[] volume = new float[volumeSlider.Length];
        for (int i = 0; i < volumeSlider.Length; i++)
        {
            volume[i] = volumeSlider[i].value;
        }

        //保存資料
        PlayerPrefManager.instance.SetSettings(settings);
        PlayerPrefManager.instance.SetVolume(volume);
        
        Close();
    }


    /**================================
     * <summary> 改變設定時臨時儲存至settings </summary>
     *===============================*/


    public void Setting(UIToggle tog)
    {
        //依照不同的Toggle做不同設定
        switch (tog.name)
        {
            case "togMusic":
                settings[(int)PlayerPrefManager.SettingsName.music] = tog.value;
                volumeSlider[(int)PlayerPrefManager.SettingsName.music].gameObject.SetActive(tog.value);
                break;
            case "togSound":
                settings[(int)PlayerPrefManager.SettingsName.sound] = tog.value;
                volumeSlider[(int)PlayerPrefManager.SettingsName.sound].gameObject.SetActive(tog.value);
                break;
            case "togVibrate":
                settings[(int)PlayerPrefManager.SettingsName.vibrate] = tog.value;
                break;
            case "togConversation":
                settings[(int)PlayerPrefManager.SettingsName.conversation] = tog.value;
                break;

        }
    }

}
