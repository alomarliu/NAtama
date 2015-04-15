using Facebook;
using LitJson;
using NLNetwork;
using System.Collections;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> FB連結選擇 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIFBConnectController : UIController
{
	private static UIFBConnectController _instance = null;
    
    protected string lastResponse = "";

    public UILabel _label = null;

    [HideInInspector]
    public string fbID = "0";

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIFBConnectController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIFBConnectController>();

			return _instance;
		}
	}

    void Awake()
    {
    }

	override public void Open(params object[] values)
	{		
		base.Open();
		
		// do Something
	}
	
	override public void Close()
	{		
		// do Something
		
		base.Close();
	}
    
	/**================================
	 * <summary> 設定對戰模式 </summary>
	 *===============================*/
    public void OnModeSelect()
    {
        // 鎖定滑鼠
        MouseLocker.instance.SetMouseLock(true);

        switch(UIButton.current.name)
        {
        /// <summary> FB連結按鈕 </summary>
        case "btnFBConnect":
            {
                _label.text = "Connect";
                // 初始化FB
                FBManager.instance.CallFBInit(OnInitComplete, OnHideUnity);
            }
            break;
                
        /// <summary> 直接開始按鈕 </summary>
        case "btnGo":
            LoginSuccess("0");
            break;
        }
    }
    
	/**================================
	 * <summary> 初始完成 </summary>
	 *===============================*/
    void OnInitComplete()
    {        
        Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
        // FB 未登入
        if(!FB.IsLoggedIn)
        {
            _label.text = "notLogin";
            FBManager.instance.CallFBLogin("public_profile,email,user_friends,publish_actions", LoginCallback);
        }
        else
        {            
            _label.text = "Logined";
            
            LoginSuccess(FB.UserId);
        }
    }
    
	/**================================
	 * <summary> 設定對戰模式 </summary>
	 *===============================*/
    void OnHideUnity(bool isGameShown)
    {
        // 鎖定滑鼠
        MouseLocker.instance.SetMouseLock(false);
    }
    
	/**================================
	 * <summary> 臉書登入後回呼 </summary>
	 *===============================*/
    void LoginCallback(FBResult result)
    {
        // 鎖定滑鼠
        MouseLocker.instance.SetMouseLock(false);
        
        if (result.Error != null)
            lastResponse = "Error Response:\n" + result.Error;
        else if (!FB.IsLoggedIn)
        {
            lastResponse = "Login cancelled by Player";
        }
        else
        {
            lastResponse = "Login was successful!";
            _label.text = "login success";

            // 呼叫 graph API 取得 name
            FBManager.instance.CallFBAPI("/me?fields=name", APICallback);
        }
    }    
    
	/**================================
	 * <summary> API callback </summary>
	 *===============================*/
    void APICallback(FBResult result)                                                                                              
    {                                                                                                                         
        if (result.Error != null)                                                                                                  
        {                                                                     
            // 呼叫 graph API 取得 name
            FBManager.instance.CallFBAPI("/me?fields=name", APICallback); 
            return;                                                                                                                
        }

        JsonData jd = JsonMapper.ToObject(result.Text);
        string playName = jd["name"].ToString();
        PlayerPrefManager.instance.SetPlayerName(playName);    

        LoginSuccess(FB.UserId);                                 
    } 

	/**================================
	 * <summary> 登入成功 </summary>
	 *===============================*/
    void LoginSuccess(string fbid)
    {
        // 鎖定滑鼠
        MouseLocker.instance.SetMouseLock(true);

        _label.text = "FBID: " + fbid;
        fbID = fbid;

        Debug.Log(fbid);
        // 選擇FB登入
        if(fbid != "0")
        {
            // 傳送繼承指令
            NetworkLib.instance.SendInheritCharacter(fbid, 1);
        }
        else
        {            
            // 創角
            NetworkLib.instance.SendCreateCharacter("0", "0", "10");
        }
    }
}
