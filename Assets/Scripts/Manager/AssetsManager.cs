using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class AssetsManager : MonoBehaviour 
{	
	/** singleton */
	private static AssetsManager 	_instance  		= 	null;

	/** 存放所有 Asset 的字典 */
	public Dictionary<string, UnityEngine.Object> assetsList= 	new Dictionary<string, UnityEngine.Object>();

	public static AssetsManager instance
	{
		get
		{
			return _instance;
		}
	}
    
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
    }
	
	/**=============================================
	 * 加入Asset
	 * ===========================================*/
	public void AddAsset(string name, UnityEngine.Object obj)
	{
		if(assetsList.ContainsKey(name))
		{
			Debug.LogError("Duplicate: " + name);
			return;
		}

		assetsList.Add(name, obj);
	}
	
	/**=============================================
	 * 取得Asset
	 * ===========================================*/
	public Object GetAsset(string name)
	{
		if(!assetsList.ContainsKey(name))
			return null;

		return assetsList[name];
	}
}
