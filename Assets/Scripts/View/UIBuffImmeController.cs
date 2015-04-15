using Model.DBF;
using NLNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 金流確認視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIBuffImmeController : UIController
{
	private static UIBuffImmeController _instance = null;
        
	private DialogRoot _dialog					= null;

    /// <summary>BUFF名稱</summary>
    [SerializeField]
    UILabel _lbBuffName = null;
    /// <summary>BUFF說明</summary>
    [SerializeField]
    UILabel _lbBuffDesc = null;
    /// <summary>學習BUFF剩餘時間</summary>
    [SerializeField]
    UILabel _lbBuffTime = null;
    /// <summary>學習BUFF的花費</summary>
    [SerializeField]
    UILabel _lbCost = null;
    /// <summary>加速按鈕的文字</summary>
    [SerializeField]
    UILabel _lbSpeedUp = null;
    /// <summary>加速按鈕</summary>
    [SerializeField]
    UIButton _btnSpeedUp = null;

    bool _learning = true;
    int _totalLearnBuffSec = 0;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIBuffImmeController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIBuffImmeController>();

			return _instance;
		}
	}
	
	/**=============================================
	 * <summary>開啟</summary>
	 * <param name="values">values[0]: callBack </param>
	 * <param name="values">values[1]: MonoBehavior </param>
	 * ===========================================*/
	override public void Open(params object[] values)
	{		
		if(visible)
			return;

        if (Character.learningBuffID <= 0)
            return;

        if (values.Length < 2)
            return;

		base.Open();
        
        // 更新畫面
        UpdateUI();

		string[] mask = {"Dialog"};
		MouseLocker.instance.SetMouseLock(true, mask);
        
        // 設定回呼
        _dialog.SetCallBackInfo(values[0].ToString(), values[1] as MonoBehaviour);
	}

    private void UpdateUI()
    {
        // 樣版
        BuffEffectLib obj = DBFManager.buffEffectLib.Data(Character.learningBuffID) as BuffEffectLib;
        // 樣版
        BuffEffectLib nextObj = DBFManager.buffEffectLib.Data(Character.learningBuffID+1) as BuffEffectLib;

        if (null == obj || null == nextObj)
            return;

        BuffTabelLib tableObj = DBFManager.buffTabelLib.Data(obj.Type) as BuffTabelLib;

        if (null == tableObj)
            return;

        _learning = true;
        _btnSpeedUp.isEnabled = true;
        _lbSpeedUp.text = "加速";

        _lbBuffName.text = tableObj.Name + " Lv" + (nextObj.Lv);

        if(tableObj.GUID == (int)BuffTabelLib.BuffType.Type4)
            _lbBuffDesc.text = string.Format(tableObj.Tip, nextObj.Coin, nextObj.SaveTime);
        else
            _lbBuffDesc.text = string.Format(tableObj.Tip, nextObj.BuffValue);

        // 花費
        _lbCost.text = obj.Immediate.Value.ToString();

        // 所需時間
        _totalLearnBuffSec = obj.Time * 60;
    }

	override public void Close()
	{				
		if(!visible)
			return;
        
        // 解除滑鼠鎖定
		MouseLocker.instance.SetMouseLock(false);
		base.Close();
	}
    
	/**================================
	 * <summary> 更新 </summary>
	 *===============================*/
    public void FixedUpdate()
    {
        UpdateLearnBuffTime();
    }
    
	/**================================
	 * <summary> 更新Buff的學習時間 </summary>
	 *===============================*/
    public void UpdateLearnBuffTime()
    {
        if (!_learning)
            return;

        long passTick = ServerTime.time - Character.GetLastLearnBuffTick();

        TimeSpan passSpan = new TimeSpan(passTick);        
        TimeSpan elapsedSpan = new TimeSpan(0,0, Mathf.Max(0, _totalLearnBuffSec - (int)passSpan.TotalSeconds));

        _lbBuffTime.text = elapsedSpan.ToString();

        if (elapsedSpan.TotalSeconds <= 0)
        {
            _learning = false;
            _lbSpeedUp.text = "已完成";
            _btnSpeedUp.isEnabled = false;
            
        }
    }

	void Awake()
	{
		_dialog = GetComponent<DialogRoot>();
	}
}
