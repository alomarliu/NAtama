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

public class UICashFlowDialogController : UIController
{
    class ItemInfo
    {
        public int itemID = -1;
        public int count = 0;

        public ItemInfo(int id, int val)
        {
            itemID = id;
            count = val;
        }
    }


	private static UICashFlowDialogController _instance = null;
    
	/** 內容 */
    [SerializeField]
	UILabel _lbContent					        = null;
	/** OK按鈕的文字 */
    [SerializeField]
	UILabel _lbOK					            = null;
	/** 需要的寶石數量 */
    [SerializeField]
	UILabel _lbGold					            = null;
	/** 提示 */
    [SerializeField]
	UILabel _lbHint					            = null;
	/** 需求條件為1 */
    [SerializeField]
	Transform _require1					        = null;
	/** 需求條件為2 */
    [SerializeField]
	Transform _require2					        = null;
    
	private DialogRoot _dialog					= null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UICashFlowDialogController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UICashFlowDialogController>();

			return _instance;
		}
	}
	
	/**=============================================
	 * <summary>開啟</summary>
	 * <param name="values">values[0]: 內容 </param>
	 * <param name="values">values[1]: 金幣需求 </param>
	 * <param name="values">values[2]: 寶石需求 </param>
	 * ===========================================*/
	override public void Open(params object[] values)
	{		
		if(visible)
			return;

		base.Open();

        if (values.Length < 3)
            return;
                
        // 計算差多少金幣
        long needCoin = 0;
        // 計算差多少寶石
        long needGold = 0;
        int count = 0;        
        uint requireCoin = 0;
        uint requireGold = 0;

        if ((int)values[1] > 0)
        {
            requireCoin = Convert.ToUInt32(values[1]);
            needCoin = Math.Max(0, Convert.ToInt64(requireCoin) - Character.ins.coin);

            if(needCoin > 0)
                count++;
        }

        if ((int)values[2] > 0)
        {
            requireGold = Convert.ToUInt32(values[2]);
            needGold = Math.Max(0, Convert.ToInt64(requireGold) - Character.ins.gold);

            if(needGold > 0)
                count++;
        }

        _require1.FindChild("lbRequire1").GetComponent<UILabel>().text = (needGold > 0) ? needGold.ToString() : needCoin.ToString();
        _require1.FindChild("uiIcon1").GetComponent<UISprite>().spriteName = (needGold > 0) ? "gold" : "coin";
        _require2.FindChild("lbRequire1").GetComponent<UILabel>().text = needCoin.ToString();
        _require2.FindChild("lbRequire2").GetComponent<UILabel>().text = needGold.ToString();
        
        NGUITools.SetActiveSelf(_require1.gameObject, count == 1);
        NGUITools.SetActiveSelf(_require2.gameObject, count == 2);

        int transGold = (needCoin > 0)? Mathf.CeilToInt(needCoin / 1000f) : 0;
        // 每1寶石可換1000金幣
        needGold = Convert.ToInt64(requireGold) + transGold - Character.ins.gold;

		_lbContent.text = values[0].ToString();
        _lbHint.text = (needGold > 0 ? "前往購買寶石" : "使用寶石兌換");
        _lbGold.text = (needGold > 0) ? needGold.ToString() : transGold.ToString();
        _lbOK.text = needGold > 0 ? "進行購買" : "進行兌換";

		string[] mask = {"Dialog"};
		MouseLocker.instance.SetMouseLock(true, mask);
        
        // 設定回呼
        _dialog.SetCallBackInfo("OnConfirm", this, new object[]{transGold});
	}

	override public void Close()
	{				
		if(!visible)
			return;
        
        // 解除滑鼠鎖定
		MouseLocker.instance.SetMouseLock(false);
		base.Close();
	}

	void Awake()
	{
		_dialog = GetComponent<DialogRoot>();
	}

    void OnConfirm(DialogOption.OptionType option, int transGold)
    {        
        if (option != DialogOption.OptionType.OK)
            return;
        
        if(transGold > 0)
        {
            // 將寶石換成金幣
            Character.Gold2Coin(transGold);

            // 傳送指令
            NetworkLib.instance.SendGold2Coin(Character.ins.uid, transGold);
        }
        else
        {
            Debug.Log("開啟商城購買");
        }
    }
}
