using NLNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using Model.DBF;

/**=============================================================
 * <summary> 主介面 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIMainController : UIController
{
	private static UIMainController _instance = null;
    
    [SerializeField]
    UILabel _lbPlayerName = null;
    [SerializeField]
    UILabel _lbTime = null;
    [SerializeField]
    UILabel _lbPay = null;
    [SerializeField]
    UISprite _uiTrigger = null;
    
    [SerializeField]
    UILabel _uiLabel = null;
    [SerializeField]
    GameObject _learningObj = null;
    [SerializeField]
    UILabel _lbLearnBuffTime = null;
    [SerializeField]
    UILabel _lbBuffName = null;
    [SerializeField]
    UIProgressBar _pbPassLearnTime = null;
    [SerializeField]
    UIProgressBar _pbPay = null;
    [SerializeField]
    UISprite _uiLearningTrigger = null;
    
    /// <summary>學到BUFF的panel</summary>
    [SerializeField]
    GameObject _goBuffGet = null;

    int maxPayUnit = 60;
    int maxPayAmount = 0;
    int _totalGetPaySec = 10;
    int _eachGetPaySec = 60;
    int _payAmount = 0;
    float _lastGetPay = 0;

    /// <summary>學習Buff的總秒數</summary>
    int _totalLearnBuffSec = 10;
    bool _learning = false;

    void Awake()
    {
        CloseBuffGet();
    }

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIMainController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIMainController>();

			return _instance;
		}
	}

	override public void Open(params object[] values)
	{		
		base.Open();        
        
        UIMainInfoController.instance.Open();

        // FB 還沒初始化
        if (!FB.IsInitialized)
            FBManager.instance.CallFBInit(null, null);

        _totalGetPaySec = _eachGetPaySec * maxPayUnit;
        maxPayAmount = 2 * maxPayUnit;

        UpdatePayTime();
        UpdateUI();

        // 檢查玩家是否有升級
        CheckLvUp();
	}
    
	/**================================
	 * <summary> 檢查玩家是否有升級 </summary>
	 *===============================*/
    private void CheckLvUp()
    {
        if(Character.ins.LevelUp())
        {
            // 打開升級畫面
            UILevelUpController.instance.OpenTween(Character.ins.lv);
        }
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
        _lbPlayerName.text = Character.ins.playName;

        // 更新BUFF學習相關資訊
        if (Character.learningBuffID > 0)
        {
            // 設定學習時間
            SetLearning();
            //_totalLearnBuffSec = 5;
        }

        NGUITools.SetActiveSelf(_learningObj, Character.learningBuffID > 0);
        _learning = Character.learningBuffID > 0;
    }
        
    void Update()
    {
#if !UNITY_EDITOR
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            UIDialogController.instance.Open(UIDialogController.DialogType.OKCancel,
                                         "確定離開遊戲嗎?", "OnConfirmQuit", this);
        }
#endif      
    }

    void FixedUpdate()
    {       
        UpdatePayTime();

        // 更新學習Buff
        UpdateLearnBuffTime();
    }
    
	/**================================
	 * <summary> 更新薪水的時間 </summary>
	 *===============================*/
    public void UpdatePayTime()
    {
        if (_payAmount >= maxPayAmount)
            return;

        long passTick = ServerTime.time - Character.ins.GetLastPayTick();
        TimeSpan passSpan = new TimeSpan(passTick);
        TimeSpan elapsedSpan = new TimeSpan(0,0, Mathf.Max(0, _totalGetPaySec - (int)passSpan.TotalSeconds));
        
        _payAmount = Math.Min(Mathf.FloorToInt((float)passSpan.TotalSeconds / _eachGetPaySec) * 2, maxPayAmount);

        _lbPay.text = _payAmount.ToString() + "/" + maxPayAmount .ToString();
        //_lbTime.text = string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", elapsedSpan.Hours, elapsedSpan.Minutes, elapsedSpan.Seconds);
        _lbTime.text = elapsedSpan.ToString();

        _pbPay.value = (float)_payAmount / maxPayAmount;
        
        if(_payAmount >= maxPayAmount && !_uiTrigger.gameObject.activeInHierarchy)
        {            
            NGUITools.SetActiveSelf(_uiTrigger.gameObject, true);
        }
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

        _lbLearnBuffTime.text = elapsedSpan.ToString();
        _pbPassLearnTime.value = (float)passSpan.TotalSeconds / _totalLearnBuffSec;

        if (elapsedSpan.TotalSeconds <= 0)
        {
            _learning = false;            
            NGUITools.SetActiveSelf(_uiLearningTrigger.gameObject, true);
        }
    }

	/**================================
	 * <summary> 按下按鈕 </summary>
	 *===============================*/
    public void OnTweenEnd(UIButton btn)
    {
        Close();

        switch(btn.name)  
        {
            case "btnBattle":
                UIModeController.instance.OpenTween();
                break;
            case "btnPlayerInfo":
                UIPlayerInfoController.instance.OpenTween();
                break;
            case "btnFriend":
                UIFriendController.instance.OpenTween();
                break;
            case "btnSkill":                
                UIBuffController.instance.OpenTween();
                break;
        }
    }
    
	/**================================
	 * <summary> 確定離開 </summary>
	 *===============================*/
    public void OnConfirmQuit(DialogOption.OptionType option, params object[] param)
    {
        if (option != DialogOption.OptionType.OK)
            return;

        Application.Quit();
    }
    
	/**================================
	 * <summary> 領薪水 </summary>
	 *===============================*/
    public void OnGetPay()
    {
        if (RealTime.time - _lastGetPay < 0.5f)
            return;

        _lastGetPay = RealTime.time;

        if(_payAmount > 0)
        {
            // 給金幣
            Character.GiveItem(GameBase.ITEM_COIN, _payAmount);

            _payAmount = 0;

            Character.ins.SetLastGetPayTick(ServerTime.time);
            UpdatePayTime();

            NGUITools.SetActiveSelf(_uiTrigger.gameObject, false);

            // 傳送指令
            NetworkLib.instance.SendGetPay(Character.ins.uid);
        }
        else
        {
            Bounds b = NGUIMath.CalculateAbsoluteWidgetBounds(UICamera.hoveredObject.transform);
            Vector3 v = UICamera.hoveredObject.transform.position;
            v.y += b.extents.y * 0.5f;
            WarningManager.instance.ShowMessage(v , "別急, 請稍等一會");
        }
    }
    
	/**================================
	 * <summary> 設定開始學習 </summary>
     * <param name="totalTime">總學習時間(分鐘</param>
	 *===============================*/
    public void SetLearning()
    {
        BuffEffectLib effectObj = DBFManager.buffEffectLib.Data(Character.learningBuffID) as BuffEffectLib;
        
        if (effectObj == null)
            return;

        BuffTabelLib obj = DBFManager.buffTabelLib.Data(effectObj.Type) as BuffTabelLib;

        if (obj == null)
            return;

        int totalTime = effectObj.Time;

        NGUITools.SetActiveSelf(_learningObj, true);
        NGUITools.SetActiveSelf(_uiLearningTrigger.gameObject, false);
        _learning = true;
        _totalLearnBuffSec = totalTime * 60;
                
        _lbBuffName.text = obj.Name + " Lv" + (effectObj.Lv+1);
    }

	/**================================
	 * <summary> 按下學習BUFF </summary>
	 *===============================*/
    public void OnLearnBuff(UIPlayTween pt)
    {
        // 還在學習
        if (Character.learningBuffID > 0)
        {
            if (_learning)
            {
                // 開啟BUFF加速視窗
                UIBuffImmeController.instance.Open("ConfirmBuffSpeedUp", this);
                return;
            }
            else
            {                
                BuffEffectLib effectObj = DBFManager.buffEffectLib.Data(Character.learningBuffID) as BuffEffectLib;

                if(null != effectObj)
                    Character.LearnBuff(effectObj.Type);

                NGUITools.SetActiveSelf(_learningObj, false);
                NGUITools.SetActiveSelf(_uiLearningTrigger.gameObject, true);

                // 傳送學完指令
                NetworkLib.instance.SendBuffLvUpFinish(Character.ins.uid, false);

                // 顯示獲得 Buff 畫面
                ShowBuffGet();
            }
        }
        else
        {
            pt.Play(true);
        }
    }
    
	/**================================
	 * <summary> 確定要加速 </summary>
	 *===============================*/
    void ConfirmBuffSpeedUp(DialogOption.OptionType option)
    {        
        if (DialogOption.OptionType.OK != option)
            return;
        
        // 樣版
        BuffEffectLib obj = DBFManager.buffEffectLib.Data(Character.learningBuffID) as BuffEffectLib;

        if (null == obj)
            return;

        // 檢查道具夠不夠
        if(!Character.CheckDelItem(obj.Immediate.ItemID, obj.Immediate.Value))
        {
            ItemBaseData itemObj = DBFManager.itemObjLib.Data(obj.Immediate.ItemID) as ItemBaseData;
            UIDialogController.instance.Open(UIDialogController.DialogType.OK, itemObj.Name + "不足");
            return;
        }
        
        ShowBuffGet();

        // 扣除道具
        Character.DelItem(obj.Immediate.ItemID, obj.Immediate.Value);
        // 直接學習
        Character.LearnBuff(obj.Type);

        // 傳送學完指令
        NetworkLib.instance.SendBuffLvUpFinish(Character.ins.uid, true);

        NGUITools.SetActiveSelf(_learningObj, false);
        NGUITools.SetActiveSelf(_uiLearningTrigger.gameObject, true);
    }

	/**================================
	 * <summary> 顯示獲得Buff的畫面 </summary>
	 *===============================*/
    void ShowBuffGet()
    {
        BuffEffectLib obj = DBFManager.buffEffectLib.Data(Character.learningBuffID+1) as BuffEffectLib;

        // 樣版不存在
        if (null == obj)
            return;

        BuffTabelLib tableObj = DBFManager.buffTabelLib.Data(obj.Type) as BuffTabelLib;

        if(null == tableObj)
            return;

        Transform t = _goBuffGet.transform;
        t.FindChild("Container/lbBuffName").GetComponent<UILabel>().text = tableObj.Name + " Lv" + obj.Lv;
        
        if(tableObj.GUID == (int)BuffTabelLib.BuffType.Type4)
            t.FindChild("Container/lbBuffDesc").GetComponent<UILabel>().text = string.Format(tableObj.Tip, obj.Coin, obj.SaveTime);
        else
            t.FindChild("Container/lbBuffDesc").GetComponent<UILabel>().text = string.Format(tableObj.Tip, obj.BuffValue) ;

        NGUITools.SetActiveSelf(_goBuffGet, true);
    }
    
	/**================================
	 * <summary> 關閉獲得BUFF的畫面 </summary>
	 *===============================*/
    void CloseBuffGet()
    {        
        NGUITools.SetActiveSelf(_goBuffGet, false);
    }
    
	/**================================
	 * <summary> 點擊了取得Buff的畫面 </summary>
	 *===============================*/
    public void OnBuffGet()
    {

        CloseBuffGet();
    }

    public void OnTest()
    {
        LitJson.JsonData jd = new LitJson.JsonData();
        jd["og:title"] = Character.ins.name + " Profiles";
        jd["og:type"] = "mytestgatama:profiles";
        jd["mytestgatama:info"] = "{\"Score\":200, \"Stage\":4, \"Win\":5, \"Lose\":10}";
        
        var querySmash = new Dictionary<string, string>();
        querySmash["object"] = jd.ToJson();
        FB.API ("/me/objects/" + "mytestgatama:profiles", Facebook.HttpMethod.POST,
            delegate(FBResult r) { Debug.Log("Result: " + r.Text); _uiLabel.text = r.Text; }, querySmash);
    }
}
