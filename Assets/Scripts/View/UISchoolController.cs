using Model.DBF;
using NLNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 學校選擇介面 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UISchoolController : UIController
{
	private static UISchoolController _instance = null;
    
    /// <summary> grid </summary>
    [SerializeField]
    UIGrid _dataGrid = null;
    UIMyWrapContent _wrap = null;
    [SerializeField]
    UIScrollView _scrollView = null;
    
    [SerializeField]
    UIInput _input = null;

    int _type = -1;

    object[] _data;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UISchoolController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UISchoolController>();

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

        _data = DBFManager.schoolLib.GetList<SchoolLib>();
        
        // 更新畫面
        UpdateUI();
	}
    
	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    private void UpdateUI()
    {
        _wrap.maxIndex = Mathf.Max(6, _data.Length-1);

        List<Transform> childList = _dataGrid.GetChildList();
        Transform t = null;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];

            if(i < _data.Length)
            {
                t.gameObject.name = ((SchoolLib)_data[i]).GUID.ToString();
                NGUITools.SetActiveSelf(t.gameObject, true);
                t.GetComponentInChildren<UILabel>().text = ((SchoolLib)_data[i]).Name;
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
	 * <summary> 關閉 </summary>
	 *===============================*/
	override public void Close()
	{		
		// do Something
		
		base.Close();
	}
    
	/**================================
	 * <summary> Wrap 項目有改變 </summary>
	 *===============================*/
    private void ItemChange(GameObject go, int wrapIndex, int realIndex)
    {
        string str = (realIndex >= 0 && realIndex < _data.Length) ? ((SchoolLib)_data[realIndex]).Name : "";

        if (go.activeInHierarchy)
        {
            go.name = ((SchoolLib)_data[realIndex]).GUID.ToString();
            go.GetComponentInChildren<UILabel>().text = str;
        }
    }
    
	/**================================
	 * <summary> 輸入字串有改變 </summary>
	 *===============================*/
    public void OnInputChange()
    {
        if (null == UIInput.current)
            return;
        
        Filter();
        UpdateUI();
    }
    
	/**================================
	 * <summary> 選擇學校 </summary>
	 *===============================*/
    public void OnSelectSchool()
    {
        Debug.Log("MySchool: " + UIButton.current.name);
        Debug.Log("FBID: " + UIFBConnectController.instance.fbID);

        string fbid = UIFBConnectController.instance.fbID;
        NetworkLib.instance.SendCreateCharacter(fbid, (fbid != "0")? "1" : "0", UIButton.current.name);

        Close();
    }
    
	/**================================
	 * <summary> 學校類型有改變 </summary>
	 *===============================*/
    public void OnTypeChange()
    {
        if (null == UIMyPopupList.current)
            return;

        string val = UIMyPopupList.current.value;
        _type = FindTypeID(val);

        Filter();
        UpdateUI();
    }
    
    void Filter()
    {        
        _data = DBFManager.schoolLib.GetList<SchoolLib>();

        if (_type != -1)
        {
            _data = Array.FindAll(_data, x => ((SchoolLib)x).Name.Contains(_input.value) &&
                                                ((SchoolLib)x).Type == _type);
        }
        else
        {
            _data = Array.FindAll(_data, x => ((SchoolLib)x).Name.Contains(_input.value));
        }

       
    }

	/**================================
	 * <summary> 學校類型有改變 </summary>
	 *===============================*/
    int FindTypeID(string val)
    {
        int id = -1;

        if(val.Contains("大學學院"))
            id = 1;
        else if(val.Contains("科技大學"))
            id = 2;
        else if(val.Contains("技術學院"))
            id = 3;
        else if(val.Contains("醫護專校"))
            id = 4;
        else if(val.Contains("軍警學校"))
            id = 5;
        else if(val.Contains("宗教學院"))
            id = 6;
        
        return id;
    }
}
