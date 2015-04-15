using System;
using System.Collections.Generic;
using UnityEngine;
using AnimationOrTween;
using UIFrameWork;

#if UNITY_EDITOR
using UnityEditor;
#endif

/**=============================================================
 * <summary> UI管堙器</summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIManager : MonoBehaviour
{
	private Dictionary<Enum_UI, IUIInterface> _uiList = new Dictionary<Enum_UI, IUIInterface>();
	
	/** singleton */
	private static UIManager 	_instance  		= 	null;
	
	void Awake()
    {        
		if(_instance == null)
		{			
			_instance = this;
			GameObject.DontDestroyOnLoad(gameObject);	
		}
		else if(_instance != this)
		{			
			Destroy(gameObject);
		}

        _instance.Initialize();	
    }

	/**=============================================
	 * instance getter
	 * ===========================================*/
	public static UIManager instance
	{	
		get
		{
			return _instance;
		}
	}
	
	/**=============================================
	 * 初始化管理器
	 * ===========================================*/
	public void Initialize()
	{
        _uiList.Clear();

		GameObject root = GameObject.Find("UI Root");

		if(!root)
			return;

		UIController[] objs = root.GetComponentsInChildren<UIController> (true);
		UIController gui;

		for(int i = 0; i < objs.Length; ++i)
		{
			gui = objs[i];

			// 重覆註冊
			if (_uiList.ContainsKey(gui.uiKey))
			{
				Debug.LogWarning("same Key UI in UIList");
			}
            
			_uiList.Add(gui.uiKey, gui);
			gui.visible = true;
			gui.Close();
		}
	}

	/**=============================================
	 * 註冊UI
	 * ===========================================*/
	public void Register(Enum_UI key)
	{	
		// 重覆註冊
		if (_uiList.ContainsKey(key))
		{
			Debug.LogWarning("same Key UI in UIList");
			return;
		}

		// 載入 UIPrefab
		//GameObject uiPrefab = Resources.Load("UIPrefab/" + key) as GameObject;
// 使用本機載入方式
#if UNITY_EDITOR && !DOWNLOAD
        GameObject uiPrefab = AssetDatabase.LoadAssetAtPath("Assets/UIPrefabs/" + key + ".prefab", typeof(GameObject)) as GameObject;
#else
		GameObject uiPrefab = AssetsManager.instance.GetAsset("Assets/UIPrefabs/" + key + ".ui") as GameObject;
#endif

		UIController t = uiPrefab.GetComponent<UIController>();

		// prefab不存在
		if(uiPrefab == null || t == null)
			return;

		// 取得 Root
		GameObject root = GameObject.Find("UI Root");
		
		if (root != null)
		{
			UIController gui = NGUITools.AddChild(root, uiPrefab).GetComponent<UIController>();
			gui.gameObject.name = uiPrefab.name;

			if (gui != null)
			{
				_uiList.Add(key, gui);
				gui.visible = true;
				gui.Close();
			}
		}
		else
		{
			Debug.LogWarning("can't find UIRoot , can't clone GUI:" + key);
		}
	}
	
	/**=============================================
	 * 開啟
	 * ===========================================*/
	public void Open(Enum_UI value)
	{
		if(!_uiList.ContainsKey(value))
			return;

		IUIInterface ui = _uiList[value];

		if(ui == null)
			return;

		if(ui.visible)
			return;

		//ui.Open();

		UIPlayTween pt = ((MonoBehaviour)ui).GetComponent<UIPlayTween>();

		if (pt != null) 
		{
			ui.OpenTween();
		}
		else
		{
			ui.Open();
		}
	}
	
	/**=============================================
	 * 關閉
	 * ===========================================*/
	public void Close(Enum_UI value)
	{
		if(!_uiList.ContainsKey(value))
			return;
		
		IUIInterface ui = _uiList[value];
		
		if(ui == null)
			return;

		if(!ui.visible)
			return;

		ui.Close();
		
		UIPlayTween pt = ((MonoBehaviour)ui).GetComponent<UIPlayTween>();


		if (pt != null) 
		{
			ui.CloseTween();
		}
		else
		{
			ui.Close();
		}
	}
	
	/**=============================================
	 * 是否開啟
	 * ===========================================*/
	public bool IsOpen(Enum_UI value)
	{
		if(!_uiList.ContainsKey(value))
			return false;
		
		IUIInterface ui = _uiList[value];
		
		if(ui == null)
			return false;

		return ui.visible;
	}

	/**=============================================
	 * 釋放
	 * ===========================================*/
	public void Free()
	{
		_uiList.Clear();
	}
	
	/**=============================================
	 * 回傳UI
	 * ===========================================*/
	public T GetUI<T>()
	{
		foreach(KeyValuePair<Enum_UI, IUIInterface> i in _uiList)
		{
			if(i.Value is T)
			{
				return (T)i.Value;
			}
		}

		return default(T);
	}    
}
