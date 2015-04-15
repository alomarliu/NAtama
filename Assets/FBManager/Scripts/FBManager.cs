using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBManager : MonoBehaviour
{
    /** singleton */
    private static FBManager _instance = null;

    public static FBManager instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
	/**================================
	 * <summary> FB初始化 </summary>
     * <param name="onInitComplete"> </param>
     * <param name="onHideUnity"> </param>
	 *===============================*/
    public void CallFBInit(Facebook.InitDelegate onInitComplete, Facebook.HideUnityDelegate onHideUnity)
    {
        FB.Init(onInitComplete, onHideUnity);
    }
    
	/**================================
	 * <summary> FB登入 </summary>
	 *===============================*/
    public void CallFBLogin(string scope = "", Facebook.FacebookDelegate loginCallback = null)
    {
        FB.Login(scope, loginCallback);
    }
    
	/**================================
	 * <summary> 呼叫FB Graph API </summary>
	 *===============================*/
    public void CallFBAPI(string scope = "", Facebook.FacebookDelegate apiCallBack = null, Dictionary<string, string> formData = null)
    {
        if (!FB.IsLoggedIn)
            return;

        FB.API(scope, Facebook.HttpMethod.GET, apiCallBack, formData);
    }
}