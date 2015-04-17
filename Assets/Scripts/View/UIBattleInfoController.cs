using LitJson;
using Model.DBF;
using System.Collections;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 戰鬥結果視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIBattleInfoController : UIController
{
	private static UIBattleInfoController _instance = null;
    
    [SerializeField]
    UILabel _lbWinLose = null;
    [SerializeField]
    UIButton _btnGoHome = null;
    [SerializeField]
    UILabel _lbCoin = null;
    [SerializeField]
    UILabel _lbExp = null;
    [SerializeField]
    UILabel _lbWinTimes = null;
    [SerializeField]
    UILabel _lbName1P = null;
    [SerializeField]
    UILabel _lbName2P = null;
    [SerializeField]
    UISprite _uiNpc1P = null;
    [SerializeField]
    UISprite _uiNpc2P = null;
    [SerializeField]
    UISprite _uiWinStoneNpc = null;
    [SerializeField]
    UISprite _uiLoseStoneNpc = null;
    [SerializeField]
    UISprite _uiWinStone = null;
    [SerializeField]
    UISprite _uiLostStone = null;
    
    [SerializeField]
    UIWidget _pos1 = null;
    [SerializeField]
    UIWidget _pos2 = null;
    
    [SerializeField]
    UIButton _btnSaveStone = null;

    UIBattleController.GameInfo _info = null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIBattleInfoController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIBattleInfoController>();

			return _instance;
		}
	}

	override public void Open(params object[] values)
	{		
		base.Open();

        _info = values[0] as UIBattleController.GameInfo;

        UpdateUI();
	}
    
	/**================================
	 * <summary> 更新介面 </summary>
	 *===============================*/
    private void UpdateUI()
    {
        /**
        _info.winCoin = 1000;
        _info.costCoin = 0;
        _info.exp = 10000;
        _info.winStoneID = 0;
        _info.lostStoneID = 100006;
        _info.winTimes = 5;
         * */
        // 名字
        ChangeIconAndName();

        // 連續答對
        NGUITools.SetActiveSelf(_lbWinTimes.gameObject, _info.winTimes > 0);
        _lbWinTimes.text = "最高連續答對 " + _info.winTimes + "題";

        // 金幣
        NGUITools.SetActiveSelf(_lbCoin.gameObject, _info.winCoin > 0 || _info.costCoin > 0);
        _lbCoin.text = (_info.winSide == 1)?  ("+" + _info.winCoin)  : ("-" + _info.costCoin);
        
        // exp
        Vector3 v = _lbExp.transform.position;
        v.y = (_info.winCoin > 0 || _info.costCoin > 0) ? _pos2.transform.position.y : _pos1.transform.position.y;
        NGUITools.SetActiveSelf(_lbExp.gameObject, _info.exp > 0);
        _lbExp.transform.position = v;
        _lbExp.text = "+" + _info.exp.ToString();

        _lbWinLose.text = (_info.winSide == 2) ? "平手" : ((_info.winSide == 1)?"競賽勝利" : "競賽失敗");
        //_lbStone.text = _info.winStoneID.ToString();
        
        // 贏得石頭
        NGUITools.SetActiveSelf(_uiWinStone.gameObject, _info.winStoneID > 0);
        // 輸了石頭
        NGUITools.SetActiveSelf(_uiLostStone.gameObject, _info.lostStoneID > 0);
        // 輸了石頭
        NGUITools.SetActiveSelf(_btnSaveStone.gameObject, _info.lostStoneID > 0);
        
        if(_info.lostStoneID > 0)
        {
            ItemBaseData obj = DBFManager.itemObjLib.Data(_info.lostStoneID) as ItemBaseData;

            if(null != obj)
            {
                 Debug.Log("lostStoneID: "+_info.lostStoneID);
                _uiLostStone.transform.FindChild("lbLostStone/uiStone").GetComponent<UISprite>().spriteName = obj.Act.Split('/')[1];
            }
        }
        
        if(_info.winStoneID > 0)
        {
            ItemBaseData obj = DBFManager.itemObjLib.Data(_info.winStoneID) as ItemBaseData;
            Debug.Log("winstone: "+_info.winStoneID);
            if(null != obj)
            {
                _uiWinStone.transform.FindChild("lbWinStone/uiStone").GetComponent<UISprite>().spriteName = obj.Act.Split('/')[1];
            }
        }        
    }
	
	override public void Close()
	{		
		// do Something
		
		base.Close();
	}    
    
	/**================================
	 * <summary> 回家 </summary>
	 *===============================*/
    public void OnGoHome()
    {
        SceneLoader.ins.LoadLevel(Scenes.Main);
    }

	/**================================
	 * <summary> 回家 </summary>
	 *===============================*/
    public void OnTweenEnd()
    {
        _btnGoHome.enabled = true;

    }
    
	/**================================
	 * <summary> 更換2p圖示 </summary>
	 *===============================*/
    void ChangeIconAndName()
    {        
        JsonData jd = PlayerPrefManager.instance.GetBattleInfo();
        int mode = int.Parse(jd["mode"].ToString());
        int npcID = Character.ins.charID;

        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        // 更新1P角色名稱及圖示
        if (null != obj)
        {
            _lbName1P.text = Character.ins.playName;
            string uiName = string.Format("Npc{0:000}", obj.GUID);
            UISprite uiNpc = _uiNpc1P.GetComponent<UISprite>();
            uiNpc.spriteName = uiName;

            // 更換NPC圖片
            uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.Free;
            uiNpc.MakePixelPerfect();
            uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
            uiNpc.SetDimensions(300, 200);
        }
        
        npcID = int.Parse(jd["npcID"].ToString());

        switch((Enum_BattleMode)mode)
        {
            case Enum_BattleMode.ModeB:
            case Enum_BattleMode.ModeC:
                {
                    ChallengeNpcLib npcObj = DBFManager.challengeNpcLib.Data(npcID) as ChallengeNpcLib;

                    if(null != npcObj)
                    {                        
                        _lbName2P.text = npcObj.Name;
                        string uiName = string.Format("Npc{0:000}", npcObj.GUID);
                        
                        UISprite uiNpc = _uiNpc2P.GetComponent<UISprite>();
                        uiNpc.spriteName = uiName;

                        // 更換NPC圖片
                        uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.Free;
                        uiNpc.MakePixelPerfect();
                        uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
                        uiNpc.SetDimensions(300, 200);
                    }
                }
                break;
            case Enum_BattleMode.ModeA:
            case Enum_BattleMode.ModeD:
                {                    
                    obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

                    // 更新角色名稱及圖示
                    if (null != obj)
                    {
                        _lbName2P.text = obj.NAME;
                        string uiName = string.Format("Npc{0:000}", obj.GUID);
                        UISprite uiNpc = _uiNpc2P.GetComponent<UISprite>();
                        uiNpc.spriteName = uiName;

                        // 更換NPC圖片
                        uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.Free;
                        uiNpc.MakePixelPerfect();
                        uiNpc.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
                        uiNpc.SetDimensions(300, 200);
                    }
                }
                break;
        }
    }
}
