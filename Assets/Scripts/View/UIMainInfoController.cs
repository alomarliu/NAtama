using System.Collections;
using UnityEngine;
using UIFrameWork;

/**=============================================================
 * <summary> 主介面 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIMainInfoController : UIController
{
	private static UIMainInfoController _instance = null;

    [SerializeField]
    UILabel _lbLv = null;
    [SerializeField]
    UILabel _lbCoin = null;
    [SerializeField]
    UILabel _lbGold = null;
    [SerializeField]
    UISlider _pbExp = null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIMainInfoController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIMainInfoController>();

			return _instance;
		}
	}

    void Start()
    {
        PropertyBinding pb = _lbCoin.GetComponent<PropertyBinding>();
        pb.source.target = Character.ins;
        pb.source.name = "coin";
        pb = _lbGold.GetComponent<PropertyBinding>();
        pb.source.target = Character.ins;
        pb.source.name = "gold";
        pb = _lbLv.GetComponent<PropertyBinding>();
        pb.source.target = Character.ins;
        pb.source.name = "lv";
        pb = _pbExp.GetComponent<PropertyBinding>();
        pb.source.target = Character.ins;
        pb.source.name = "expPer";
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
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    private void UpdateUI()
    {
        _lbCoin.text = Character.ins.coin.ToString();
        _lbLv.text = Character.ins.lv.ToString();
        _lbGold.text = Character.ins.gold.ToString();
    }

    public void LvUp()
    {
        Character.ins.AddExp(50);
    }
}
