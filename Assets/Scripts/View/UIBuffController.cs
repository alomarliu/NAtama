using Model.DBF;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> BUFF介面 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIBuffController : UIController
{
	private static UIBuffController _instance = null;
    
    /// <summary> grid </summary>
    [SerializeField]
    UIGrid _dataGrid = null;
    UIMyWrapContent _wrap = null;
    [SerializeField]
    UIScrollView _scrollView = null;
    object[] _data = {};
    
    public UIBackButton backBtn = null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIBuffController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIBuffController>();

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

        UpdateUI();
	}
    	
	override public void Close()
	{		
		// do Something
		
		base.Close();
	}    
    
	/**=============================================
	 * 更新畫面
	 * ===========================================*/
    private void UpdateUI()
    {
        _data = DBFManager.buffTabelLib.GetList<BuffTabelLib>();

        _wrap.maxIndex = Mathf.Max(4, _data.Length-1);

        List<Transform> childList = _dataGrid.GetChildList();
        Transform t = null;
        BuffSlot slot = null;
        BuffTabelLib obj = null;
        //int unLockCount = 0;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];
            
            if(i < _data.Length)
            {
                obj = _data[i] as BuffTabelLib;
                slot = t.GetComponent<BuffSlot>();
                slot.SetData(obj);

                //if (obj.Lv > Character.ins.lv)
                   // unLockCount++;
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
        
        BuffSlot slot = null;

        if (go.activeInHierarchy)
        {
            slot = go.GetComponent<BuffSlot>();
            slot.SetData(_data[realIndex] as BuffTabelLib);            
        }
    }
}
