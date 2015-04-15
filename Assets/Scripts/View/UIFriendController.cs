using LitJson;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 好友視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIFriendController : UIController
{
	private static UIFriendController _instance = null;

    /// <summary> grid </summary>
    [SerializeField]
    UIGrid _dataGrid = null;
    UIMyWrapContent _wrap = null;
    [SerializeField]
    UIScrollView _scrollView = null;

    [SerializeField]
    GameObject _btnFB = null;

    class FriendData
    {
        public string name = "";
        public string id = "";
    }
    List<FriendData> fbList = new List<FriendData>();

    public string lastResponse = "";
    
	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIFriendController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIFriendController>();

			return _instance;
		}
	}
    
    void Awake()
    {                
        _wrap = _dataGrid.GetComponent<UIMyWrapContent>();
        _wrap.onInitializeItem += ItemChange;
    }

    void OnDestroy()
    {        
        _wrap.onInitializeItem -= ItemChange;
    }

	override public void Open(params object[] values)
	{		
		base.Open();
        
        NGUITools.SetActiveSelf(_btnFB.gameObject, !FB.IsLoggedIn);
        NGUITools.SetActiveSelf(_scrollView.gameObject, FB.IsLoggedIn);

        if(FB.IsLoggedIn)
        {
            // 呼叫 graph API 取得 name
            FBManager.instance.CallFBAPI("/me?fields=name,friends", APICallback);
        }
	}
	
	override public void Close()
	{		
		// do Something
		
		base.Close();
	}    

    public void OnFbConnect()
    {   
        NGUITools.SetActiveSelf(_btnFB.gameObject, false);
        // loading
        LoadingManager.instance.SetLoading(true);

        if(!FB.IsLoggedIn)
            FBManager.instance.CallFBLogin("public_profile,email,user_friends", LoginCallback);        
    }

	/**================================
	 * <summary> 臉書登入後回呼 </summary>
	 *===============================*/
    void LoginCallback(FBResult result)
    {
        // loading
        LoadingManager.instance.SetLoading(false);
        
        if (result.Error != null)
            lastResponse = "Error Response:\n" + result.Error;
        else if (!FB.IsLoggedIn)
        {
            lastResponse = "Login cancelled by Player";
        }
        else
        {
            lastResponse = "Login was successful!";
            
            // loading
            LoadingManager.instance.SetLoading(true);
            // 呼叫 graph API 取得 name
            FBManager.instance.CallFBAPI("/me?fields=name,friends", APICallback);
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
            FBManager.instance.CallFBAPI("/me?fields=name,friends", APICallback); 
            return;                                                                                                                
        }
        
        // 解除鎖定滑鼠
        LoadingManager.instance.SetLoading(false);

        JsonData jd = JsonMapper.ToObject(result.Text);
        string playName = jd["name"].ToString();
        PlayerPrefManager.instance.SetPlayerName(playName);
        //object obj = jd["friends"]["data"];
        
        fbList = JsonMapper.ToObject<List<FriendData>>(jd["friends"]["data"].ToJson());
        fbList.Add(new FriendData());
        fbList.Add(new FriendData());
        
        NGUITools.SetActiveSelf(_btnFB.gameObject, false);
        NGUITools.SetActiveSelf(_scrollView.gameObject, true);

        UpdateUI();
    }
    
	/**================================
	 * <summary> 更新好友 </summary>
	 *===============================*/
    private void UpdateUI()
    {
        _wrap.maxIndex = Mathf.Max(4, fbList.Count-1);

        List<Transform> childList = _dataGrid.GetChildList();
        Transform t = null;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];

            if(i < fbList.Count)
            {                
                UpdateFriends(t, i);
            }
            else
            {
                NGUITools.SetActiveSelf(t.gameObject, false);
            }
        }

        _dataGrid.Reposition();
        _scrollView.ResetPosition();
    }
    
	/**================================
	 * <summary> Wrap 項目有改變 </summary>
	 *===============================*/
    private void ItemChange(GameObject go, int wrapIndex, int realIndex)
    {
        if (fbList.Count <= 0)
            return;
        
        if (go.activeInHierarchy)
        {
            UpdateFriends(go.transform, realIndex);
        }
    }
    
	/**================================
	 * <summary> 更新按鈕資訊 </summary>
	 *===============================*/
    void UpdateFriends(Transform t, int idx)
    {
        t.gameObject.name = fbList[idx].id;
        t.FindChild("lbName").GetComponent<UILabel>().text = fbList[idx].name;

        StartCoroutine(DownPic(t, fbList[idx].id));
    }
        
	/**================================
	 * <summary> 更新按鈕資訊 </summary>
	 *===============================*/
    IEnumerator DownPic(Transform parent,  string fbID)
    {
        WWW www = new WWW("http://graph.facebook.com/" + fbID + "/picture?type=large");

        yield return www;

        if(www.error != null)
        {
            Debug.Log(www.error);
        }
        Texture2D newTexture = www.texture;

        Transform child = parent.FindChild("t");

        if(null != child)
            parent.FindChild("t").GetComponent<UITexture>().mainTexture = newTexture;
    }
}
