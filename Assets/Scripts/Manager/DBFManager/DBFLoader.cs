using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Model.DBF;

#if UNITY_EDITOR
using UnityEditor;
#endif

/**=============================================================
 * <summary>DBF載入器</summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class DBFLoader
{
	/** Dictionary 不適合走訪, 效率不好, 故分為list及dictionary 存放兩份資料 */
	private List<object> _list = new List<object>();
	// 
	private Dictionary<int, object> _dataDic= new Dictionary<int, object>();
	private int pos = -1;
	
	/**=============================================
	 * 載入檔案
	 * ===========================================*/
	public void Load<T>(string fileName)
	{
#if UNITY_EDITOR 
        string path = "Assets/DBF/" + fileName + ".json";

        TextAsset s = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
		if (!s)  
			return;  
				
		string strData = s.text;

        Dictionary<string, T> dic = JsonMapper.ToObject<Dictionary<string, T>>(strData);        
		DBFItemBase item;

		foreach(KeyValuePair<string , T> obj in dic)
        {
			item = obj.Value as DBFItemBase;

			if(item != null)
            {
                item.GUID = int.Parse(obj.Key);
                _list.Add(item);
				_dataDic.Add(item.GUID, item);
            }
        }
#endif
	}	

	/**=============================================
	 * 載入資料
	 * ===========================================*/
	public void LoadFromText<T>(string text)
	{
		string strData = text; 
		
        Dictionary<string, T> dic = JsonMapper.ToObject<Dictionary<string, T>>(strData);        
		DBFItemBase item;

		foreach(KeyValuePair<string , T> obj in dic)
        {
			item = obj.Value as DBFItemBase;

			if(item != null)
            {
                item.GUID = int.Parse(obj.Key);
                _list.Add(item);
				_dataDic.Add(item.GUID, item);
            }
        }
	}

	/**=============================================
	 * 取得指定的資料
	 * ===========================================*/
	public object Data(int id)
	{
        if (!_dataDic.ContainsKey(id))
            return null;

		return _dataDic[id];
	}

	/**=============================================
	 * 重置
	 * ===========================================*/
	public void Reset()
	{
		pos = -1;
	}

	/**=============================================
	 * 下一筆
	 * ===========================================*/
	public bool MoveNext()
	{
		pos++;
		return (pos < _list.Count);
	}
	
	/**=============================================
	 * 取得目前這筆資料
	 * ===========================================*/
	public object Current
	{
		get 
		{
			return _list[pos];
		}
	}
    
	/**=============================================
	 * 取得最後一筆
	 * ===========================================*/
    public object Last
    {
        get
        {
            return _list[_list.Count - 1];
        }
    }

    public int Count
    {
        get
        {
            return _list.Count;
        }
    }
    
    public object[] GetList<T>()
    {
        return _list.ToArray();
    }
}
