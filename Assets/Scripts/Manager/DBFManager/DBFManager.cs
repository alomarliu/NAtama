using System.Collections;
using System.Collections.Generic;
using System.IO;  
using System.Text; 
using UnityEngine;
using LitJson;
using Model.DBF;
/**=============================================================
 * <summary>DBF管理器類別</summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class DBFManager : MonoBehaviour
{		
	/** 問答樣版表 */
	public static DBFLoader questionList = new DBFLoader();
	/** 學校列表 */
	public static DBFLoader schoolLib = new DBFLoader();
	/** 關卡設定列表 */
	public static DBFLoader challengeStageLib = new DBFLoader();
	/** 關卡NPC列表 */
	public static DBFLoader challengeNpcLib = new DBFLoader();
    
	/** 道具列表 */
	public static DBFLoader itemObjLib = new DBFLoader();
	/** 奪石關卡表 */
	public static DBFLoader snatchStageLib = new DBFLoader();
	/** 參數設定表 */
	public static DBFLoader configLib = new DBFLoader();
	/** 玩家設定表 */
	public static DBFLoader roleConfigLib = new DBFLoader();
    
	/** Buff列表  */
	public static DBFLoader buffTabelLib = new DBFLoader();
	/** Buff效果表  */
	public static DBFLoader buffEffectLib = new DBFLoader();
	/** 商店表  */
	public static DBFLoader shopLib = new DBFLoader();

    /** singleton */
	private static DBFManager 	_instance   = 	null;
	
	void Awake()
    {        
		if(_instance == null)
		{			
			_instance = this;
			GameObject.DontDestroyOnLoad(gameObject);	

#if !DOWNLOAD
            _instance.Initialize();		
#endif
		}
		else if(_instance != this)
		{			
			Destroy(gameObject);
		}
    }

	/**=============================================
	 * instance getter
	 * ===========================================*/
	public static DBFManager instance
	{	
		get
		{
			return _instance;
		}
	}
	
	/**=============================================
	 * 從assetBundle解資料
	 * ===========================================*/
	public static void UnZipData(AssetBundle assetBundle)
	{
		if(null == assetBundle)
			return;

        TextAsset asset = null;

        // 問答表
		asset = assetBundle.Load("QuestionList") as TextAsset;
        questionList.LoadFromText<QuestionLib>(asset.text);
        
        // 學校列表
		asset = assetBundle.Load("SchoolList") as TextAsset;
        schoolLib.LoadFromText<SchoolLib>(asset.text);
        
        // 關卡設定列表
		asset = assetBundle.Load("ChallengeStage") as TextAsset;
        challengeStageLib.LoadFromText<ChallengeStageLib>(asset.text);
        // NPC
		asset = assetBundle.Load("ChallengeNpc") as TextAsset;
        challengeNpcLib.LoadFromText<ChallengeNpcLib>(asset.text);
        
        // 道具表
		asset = assetBundle.Load("ItemRoleStone") as TextAsset;
        itemObjLib.LoadFromText<ItemBaseData>(asset.text);
        // 道具表
		asset = assetBundle.Load("ItemSpecial") as TextAsset;
        itemObjLib.LoadFromText<ItemRoleValueData>(asset.text);

        // 奪石關卡表
		asset = assetBundle.Load("CustomRole") as TextAsset;
        snatchStageLib.LoadFromText<SnatchStageLib>(asset.text);

        // 參數設定表
		asset = assetBundle.Load("Config") as TextAsset;
        configLib.LoadFromText<ConfigLib>(asset.text);
        
        // 玩家基礎資料設定表
		asset = assetBundle.Load("PlayerLv") as TextAsset;
        roleConfigLib.LoadFromText<RoleConfigLib>(asset.text);
        
        // Buff列表
		asset = assetBundle.Load("BuffTable") as TextAsset;
        buffTabelLib.LoadFromText<BuffTabelLib>(asset.text);
        
        // Buff效果表(POWER)
		asset = assetBundle.Load("BuffofPower") as TextAsset;
        buffEffectLib.LoadFromText<BuffEffectLib>(asset.text);
        // Buff效果表(Combo)
		asset = assetBundle.Load("BuffofCombo") as TextAsset;
        buffEffectLib.LoadFromText<BuffEffectLib>(asset.text);
        // Buff效果表(Exp)
		asset = assetBundle.Load("BuffofExp") as TextAsset;
        buffEffectLib.LoadFromText<BuffEffectLib>(asset.text);
        // Buff效果表(Bank)
		asset = assetBundle.Load("BuffofBank") as TextAsset;
        buffEffectLib.LoadFromText<BuffEffectLib>(asset.text);
        // Buff效果表(Sentence)
		asset = assetBundle.Load("BuffofSentence") as TextAsset;
        buffEffectLib.LoadFromText<BuffEffectLib>(asset.text);
        // 商店表
		asset = assetBundle.Load("ShopPurchase") as TextAsset;
        shopLib.LoadFromText<ShopLib>(asset.text);
	}
	
	/**=============================================
	 * 管理器啟動
	 * ===========================================*/
	public void Initialize()
	{
        // 問答表
        questionList.Load<QuestionLib>("QuestionList");

        // 學校列表
        schoolLib.Load<SchoolLib>("SchoolList");
        
        // 關卡設定列表
        challengeStageLib.Load<ChallengeStageLib>("ChallengeStage");
        // NPC
        challengeNpcLib.Load<ChallengeNpcLib>("ChallengeNpc");

        // 道具表
        itemObjLib.Load<ItemBaseData>("ItemRoleStone");
        // 道具表
        itemObjLib.Load<ItemRoleValueData>("ItemSpecial");
        
        // 奪石關卡表
        snatchStageLib.Load<SnatchStageLib>("CustomRole");
        
        // 參數設定表
        configLib.Load<ConfigLib>("Config");

        // 玩家基礎資料設定表
        roleConfigLib.Load<RoleConfigLib>("PlayerLv");
        
        // Buff列表
        buffTabelLib.Load<BuffTabelLib>("BuffTable");
        
        // Buff效果表(POWER)
        buffEffectLib.Load<BuffEffectLib>("BuffofPower");
        // Buff效果表(Combo)
        buffEffectLib.Load<BuffEffectLib>("BuffofCombo");
        // Buff效果表(Exp)
        buffEffectLib.Load<BuffEffectLib>("BuffofExp");
        // Buff效果表(Bank)
        buffEffectLib.Load<BuffEffectLib>("BuffofBank");
        // Buff效果表(Sentence)
        buffEffectLib.Load<BuffEffectLib>("BuffofSentence");
        
        // 商店表
        shopLib.Load<ShopLib>("ShopPurchase");
	}  
}
    