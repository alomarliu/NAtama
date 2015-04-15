using Model.DBF;
using NLNetwork;
using System.Collections;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 升級確認視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UILevelUpController : UIController
{
	private static UILevelUpController _instance = null;

    /// <summary>等級</summary>
    [SerializeField]
    UILabel _lbLevel = null;
    /// <summary>遊戲幣</summary>
    [SerializeField]
    UILabel _lbCoin = null;
    /// <summary>鑽石</summary>
    [SerializeField]
    UILabel _lbGold = null;

    int _newLevel = 1;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UILevelUpController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UILevelUpController>();

			return _instance;
		}
	}

	override public void Open(params object[] values)
	{		
		base.Open();

        _newLevel = (int)values[0];

        UpdateUI();
	}

	override public void Close()
	{		
		// do Something
		
		base.Close();
	}
    
	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    private void UpdateUI()
    {
        _lbLevel.text = "第 " + _newLevel + " 級";
                
        RoleConfigLib roleObj = null;
        roleObj = DBFManager.roleConfigLib.Data(_newLevel) as RoleConfigLib;

        if(null != roleObj)
        {
            _lbCoin.text = roleObj.Reward[0].Value.ToString();
            _lbGold.text = roleObj.Reward[1].Value.ToString();
        }
    }	
    
	/**================================
	 * <summary> 按下確認 </summary>
	 *===============================*/
    public void OnBtnClick()
    {
        NetworkLib.instance.SendLvUp(Character.ins.uid);
    }
    
	/**================================
	 * <summary> Tween結束 </summary>
	 *===============================*/
    public void OnTweenEnd()
    {
        Close();
        
        // 還可以升級
        if(Character.ins.LevelUp())
        {
            OpenTween(Character.ins.lv);
        }
    }
}
