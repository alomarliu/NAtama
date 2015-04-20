using Model.DBF;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 模式選擇視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIShopController : UIController
{
	private static UIShopController _instance = null;
    
    object[] _data = {};
    /// <summary> grid </summary>
    [SerializeField]
    UIGrid _dataGrid = null;
    UIMyWrapContent _wrap = null;
    [SerializeField]
    UIScrollView _scrollView = null;

    public enum Type
    {
        Coin = 0x01,
        Gold = 0x02,
    }

    // 畫面類型
    Type _type = Type.Coin;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIShopController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIShopController>();

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
        if (visible)
            return;

		base.Open();

        _type = (Type)values[0];

        UpdateUI();
	}
	
	override public void Close()
	{		
        if (!visible)
            return;
		// do Something
		
		base.Close();
	}
    
	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    public void UpdateUI()
    {        
        _data = DBFManager.shopLib.GetList<ShopLib>();
        
        // 找出資料中 Type相符的資料
        _data = Array.FindAll<object>(_data, x=> (((ShopLib)x).Type == (int)_type));

        // 2筆算1個index
        int totalIndex = Mathf.CeilToInt(_data.Length / 2f);
        int idx = 0;

        _wrap.maxIndex = Mathf.Max(2, totalIndex-1);

        List<Transform> childList = _dataGrid.GetChildList();
        Transform t = null;
        ShopSlot slot = null;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];

            if(i < totalIndex)
            {
                slot = t.FindChild("1").GetComponent<ShopSlot>();
                idx = i * 2 + 0;
                // 設定資料
                slot.SetData(_data[idx] as ShopLib);
                
                idx = i * 2 + 1;
                
                slot = t.FindChild("2").GetComponent<ShopSlot>();
                if (idx < _data.Length)
                {
                    // 設定資料
                    slot.SetData(_data[idx] as ShopLib);
                    NGUITools.SetActiveSelf(slot.gameObject, true);
                }
                else
                {
                    NGUITools.SetActiveSelf(slot.gameObject, false);
                }
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
        if (_data.Length <= 0)
            return;
        
        if (go.activeInHierarchy)
        {
            // 2筆算1個index
            int totalIndex = Mathf.CeilToInt(_data.Length / 2f);
            Transform t = go.transform;
            ShopSlot slot;

            slot = t.FindChild("1").GetComponent<ShopSlot>();
            int idx = realIndex * 2 + 0;
            // 設定資料
            slot.SetData(_data[idx] as ShopLib);
                
            idx = realIndex * 2 + 1;
                
            slot = t.FindChild("2").GetComponent<ShopSlot>();
            if (idx < _data.Length)
            {
                // 設定資料
                slot.SetData(_data[idx] as ShopLib);
                NGUITools.SetActiveSelf(slot.gameObject, true);
            }
            else
            {
                NGUITools.SetActiveSelf(slot.gameObject, false);
            }
        }
    }
}
