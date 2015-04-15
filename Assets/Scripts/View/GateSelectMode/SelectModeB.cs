using Model.DBF;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

public class SelectModeB : MonoBehaviour, IGateSelectMode
{    
    object[] _data = {};
    /// <summary> grid </summary>
    [SerializeField]
    UIGrid _dataGrid = null;
    UIMyWrapContent _wrap = null;
    [SerializeField]
    UIScrollView _scrollView = null;
    UIController _uiMain = null;
    
    void Awake()
    {                
        _wrap = _dataGrid.GetComponent<UIMyWrapContent>();
        _wrap.onInitializeItem += ItemChange;
        _uiMain = GetComponentInParent<UIController>();
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
	 * <summary> 關卡選擇 </summary>
	 *===============================*/
    public void OnGateSelect(string gate = "", int npc = 0)
    {        
        int npcID = int.Parse(UIButton.current.name);
        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        if (null == obj)
            return;

        int rs = Character.CheckSnatch(npcID);
        string temp = "";

        switch(rs)
        {
            case 0:
                temp = "角色石不足!";
                break;
            case 2:
                temp = "角色石已滿!";
                break;
        }

        if(!string.IsNullOrEmpty(temp))
        {
            UIDialogController.instance.Open(UIDialogController.DialogType.OK,
                                            temp);
            return;
        }

        string tempstr = "";

        tempstr = string.Format("將隨機索取1顆角色石, 確定嗎?");

        UIDialogController.instance.Open(UIDialogController.DialogType.OKCancel,
                                         tempstr, "OnConfirm", _uiMain, 
                                         new object[]{npcID});
    }
    
	/**================================
	 * <summary> 確認 </summary>
	 *===============================*/
    public void OnConfirm(DialogOption.OptionType option, params object[] param)
    {        
        if (option != DialogOption.OptionType.OK)
            return;        

        UIFindTargetController.instance.Open();

        // 設定對戰資訊
        PlayerPrefManager.instance.SetBattleInfo(Enum_BattleMode.ModeD, 0, (int)param[0]);
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
}
