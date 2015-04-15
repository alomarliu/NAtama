using Model.DBF;
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

public class UIGateSelect : UIController
{
	private static UIGateSelect _instance = null;        

    Enum_BattleMode _mode = 0;

    [SerializeField]
    SelectModeA _modeA = null;
    [SerializeField]
    SelectModeB _modeB = null;
    IGateSelectMode _selectMode = null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIGateSelect instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIGateSelect>();

			return _instance;
		}
	}

	override public void Open(params object[] values)
	{		
		base.Open();
        
        if(values.Length > 0)
            _mode = (Enum_BattleMode)values[0];

        NGUITools.SetActiveSelf(_modeA.gameObject, _mode <= Enum_BattleMode.ModeC);
        NGUITools.SetActiveSelf(_modeB.gameObject, _mode == Enum_BattleMode.ModeD);

        switch(_mode)
        {
            case Enum_BattleMode.ModeA:
                _selectMode = _modeA;
                break;
            case Enum_BattleMode.ModeD:
                _selectMode = _modeB;
                break;
            default:
                return;
        }

        UpdateUI();
	}
	
	override public void Close()
	{
        NGUITools.SetActiveSelf(_modeA.gameObject, false);
        NGUITools.SetActiveSelf(_modeB.gameObject, false);

        base.Close();
	}
    
    
	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    private void UpdateUI()
    {
        _selectMode.UpdateUI();
    }
    
	/**================================
	 * <summary> 挑戰模式 </summary>
	 *===============================*/
    public void OnGateSelectModeB()
    {
        UIButton btn = UIButton.current;
        int gate = int.Parse(btn.transform.parent.name);
        int npcID = (gate <= Character.ins.everyDayNpcs.Length)? Character.ins.everyDayNpcs[gate - 1] : 0;

        _selectMode.OnGateSelect(gate.ToString(), npcID);
    }

	/**================================
	 * <summary> 關卡選擇 </summary>
	 *===============================*/
    public void OnGateSelect(string gate)
    {    
        _selectMode.OnGateSelect(gate);
    }

	/**================================
	 * <summary> 確認 </summary>
	 *===============================*/
    public void OnConfirm(DialogOption.OptionType option, params object[] param)
    {
        if (option != DialogOption.OptionType.OK)
            return;

        _selectMode.OnConfirm(option, param);
        Close();
    }
}
