using System.Collections;
using UnityEngine;
using UIFrameWork;

/**=============================================================
 * <summary> 模式選擇視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIModeController : UIController
{
	private static UIModeController _instance = null;

    Enum_BattleMode _mode = 0;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIModeController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIModeController>();

			return _instance;
		}
	}

	override public void Open(params object[] values)
	{		
		base.Open();
        _mode = 0;
	}
	
	override public void Close()
	{		
		// do Something
		
		base.Close();
	}
    
	/**================================
	 * <summary> 設定對戰模式 </summary>
	 *===============================*/
    public void OnModeSelect()
    {
        Character.ins.winStone = -1;

        switch(UIButton.current.name)
        {
            case "btnMode1":
                _mode = Enum_BattleMode.ModeA;
                break;
            case "btnMode2":
                _mode = Enum_BattleMode.ModeD;
                break;
            case "btnMode3":
                _mode = Enum_BattleMode.ModeE;
                _mode = 0;
                return;
        }
    }

	/**================================
	 * <summary> 設定對戰模式 </summary>
	 *===============================*/
    public void OnTweenEnd(UIPlayTween pt)
    {
        UITweener tweener = TweenPosition.current;

        if (tweener.direction == AnimationOrTween.Direction.Reverse)
        {
            if (_mode == 0)
                return;

            Close();
            UIGateSelect.instance.OpenTween(_mode);
        }
        else
        {
            if(null != UIMainController.instance)
                UIMainController.instance.Close();
        }
    }
}
