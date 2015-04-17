using Model.DBF;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 角色資訊介面 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIPlayerInfoController : UIController
{
	private static UIPlayerInfoController _instance = null;

    [SerializeField]
    UILabel _lbPlayerName = null;
    [SerializeField]
    UILabel _lbNpcName = null;
    [SerializeField]
    UIGrid _infoGrid = null;
    [SerializeField]
    UIGrid _buffInfoGrid = null;
    [SerializeField]
    UISprite _uiNpc = null;
    /// <summary>NPC等級</summary>
    [SerializeField]
    UILabel _lbNpcLv = null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIPlayerInfoController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIPlayerInfoController>();

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
    
	/**================================
	 * <summary> 更新 </summary>
	 *===============================*/
    private void UpdateUI()
    {
        _lbPlayerName.text = Character.ins.playName;

        List<Transform> childList = _infoGrid.GetChildList();
        Transform t = null;

        Character chr = Character.ins;

        string[] values = {chr.lv.ToString(), chr.exp.ToString(), chr.winRound.ToString(),
                           chr.schoolName, chr.hp.ToString(), chr.schoolLv.ToString(),
                           chr.winRate.ToString()};

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];
            t.FindChild("lbValue").GetComponent<UILabel>().text = values[i];
        }
        
        values = new string[] { "10", "100", Character.ins.abilityF.PowerPer.ToString()+"%",
                                Character.ins.abilityF.ExpPer.ToString()+"%",
                                Character.ins.abilityF.BonusPer.ToString()+"%",
                                "30"};
        
        childList = _buffInfoGrid.GetChildList();

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];
            t.FindChild("lbValue").GetComponent<UILabel>().text = values[i];
        }

        SnatchStageLib obj = DBFManager.snatchStageLib.Data(Character.ins.charID) as SnatchStageLib;

        // 更新角色名稱及圖示
        if (null != obj)
        {
            // 等級
            _lbNpcLv.text = "Lv" + Character.GetNpc(obj.GUID).lv.ToString();

            _lbNpcName.text = obj.NAME;
            string uiName = string.Format("Npc{0:000}", obj.GUID);
            _uiNpc.spriteName = uiName;

            // 更換NPC圖片
            _uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.Free;
            _uiNpc.MakePixelPerfect();
            _uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
            _uiNpc.SetDimensions(260, 200);
        }
    }
        
	/**================================
	 * <summary> Tween結束 </summary>
	 *===============================*/
    public void OnTweenEnd()
    {
        UITweener tweener = TweenPosition.current;

        if (tweener.direction == AnimationOrTween.Direction.Reverse)
        {
            Close();
            UIChangeNpcController.instance.OpenTween();
        }
    }
}
