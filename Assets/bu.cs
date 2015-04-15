using UnityEngine;
using System.Collections;

public class bu : MonoBehaviour {

    public UILabel _label = null;

    public string lastResponse = "";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Onclick()
    {
        
        _label.text = "Click";
        FBManager.instance.CallFBInit(OnFBInit, OnHideUnity);
    }

    void OnHideUnity(bool isGameShown)
    {    
        _label.text = "jj";
        if (!isGameShown)                                                                        
        {                                                                                        
            // pause the game - we will need to hide                                             
            Time.timeScale = 0;                                                                  
        }                                                                                        
        else                                                                                     
        {                                                                                        
            // start the game back up - we're getting focus again                                
            Time.timeScale = 1;                                                                  
        }        
    }

    public void OnFBInit()
    {          
        enabled = false;       
        _label.text = "init";

        if(!FB.IsLoggedIn)
            FBManager.instance.CallFBLogin("public_profile,email,user_friends", LoginCallback);
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

            // 呼叫 graph API 取得 name
            //FBManager.instance.CallFBAPI("/me?fields=name,friends", APICallback);
        }
    }    
    
}
