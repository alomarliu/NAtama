using Model.DBF;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using NLNetwork;

/**=============================================================
 * <summary> 變更角色介面 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2015 All Rights Reserved
 =============================================================*/

public class UIChangeNpcController : UIController
{
	private static UIChangeNpcController _instance = null;
    
    object[] _data = {};
    /// <summary> grid </summary>
    [SerializeField]
    UIGrid _dataGrid = null;
    UIMyWrapContent _wrap = null;
    [SerializeField]
    UIScrollView _scrollView = null;
    
    [SerializeField]
    UIBackButton _btnBack = null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIChangeNpcController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIChangeNpcController>();

			return _instance;
		}
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
    
    
    void Awake()
    {                
        _wrap = _dataGrid.GetComponent<UIMyWrapContent>();
        _wrap.onInitializeItem += ItemChange;
    }

    void OnDestroy()
    {        
        _wrap.onInitializeItem -= ItemChange;
    }

	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    public void UpdateUI()
    {        
        _data = DBFManager.snatchStageLib.GetList<SnatchStageLib>();

        _wrap.maxIndex = Mathf.Max(3, _data.Length-1);

        List<Transform> childList = _dataGrid.GetChildList();
        Transform t = null;
        SnatchSlot slot = null;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];

            if(i < _data.Length)
            {
                slot = t.GetComponent<SnatchSlot>();
                slot.SetData(_data[i] as SnatchStageLib);
                slot.name = (_data[i] as SnatchStageLib).GUID.ToString();
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
        if (realIndex < 0 || realIndex >= _data.Length)
            return;

        if (go.activeInHierarchy)
        {
            SnatchSlot slot = go.GetComponent<SnatchSlot>();
            slot.SetData(_data[realIndex] as SnatchStageLib);
            slot.name = (_data[realIndex] as SnatchStageLib).GUID.ToString();
        }
    }

	/**================================
	 * <summary> 選擇了npc </summary>
	 *===============================*/
    public void OnNpcSelect()
    {
        int npcID = int.Parse(UIButton.current.name);
        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        if (null == obj)
            return;

        // 檢查角色是否可解鎖
        if(!Character.CheckSelectNpc(npcID))
        {
            UIDialogController.instance.Open(UIDialogController.DialogType.OK,
                                            "該角色未解鎖");
            return;
        }

        string temp = string.Format("確定要更換為{0}嗎?", obj.NAME);
        
        // 更換確認
        UIDialogController.instance.Open(UIDialogController.DialogType.OKCancel,
                                         temp, "OnConfirm", this, 
                                         new object[]{npcID});
    }
    
	/**================================
	 * <summary> 確認 </summary>
	 *===============================*/
    public void OnConfirm(DialogOption.OptionType option, params object[] param)
    {        
        if (option != DialogOption.OptionType.OK)
            return;

        int npcID = (int)param[0];
        Character.SelectNpc(npcID);
        _btnBack.OnClick();

        // 傳送指令
        NetworkLib.instance.SendChangeNpc(Character.ins.uid, npcID);
    }
}
