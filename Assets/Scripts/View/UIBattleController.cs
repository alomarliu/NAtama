using GamePlay;
using Model.DBF;
using NLNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

/**=============================================================
 * <summary> 戰鬥視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIBattleController : UIController
{
    public class GameInfo
    {
        /// <summary>獲勝方 0: 輸, 1: 玩家, 2:平手</summary>
        public int winSide = 0;
        public int costCoin = 0;
        public int winCoin = 0;
        public int exp = 0;
        public int lostStoneID = 0;
        public int winStoneID = 0;
        public int winTimes = 0;
    }

	private static UIBattleController _instance = null;
    IBattleMode _mode = null;

    [SerializeField]
    UIGrid _myGrid = null;
    [SerializeField]
    UIGrid _opponentGrid = null;

    [SerializeField]
    Transform _LeftLines = null;
    [SerializeField]
    Transform _RightLines = null;
    
    [SerializeField]
    GameObject _toggleNum = null;

    /// <summary>激鬥元件</summary>
    [SerializeField]
    GameObject _fighting = null;
    
    /// <summary>問答 component</summary>
    public QuestionPlay questionPlay = null;

    /// <summary>左邊答題時間</summary>
    public UILabel lbLeftToggleTime;
    /// <summary>右邊答題時間</summary>
    public UILabel lbRightToggleTime;
    /// <summary>戰鬥結束</summary>
    public UILabel lbBattleEnd;

    /// <summary>左邊剩餘時間條</summary>
    public UIProgressBar pbHp1P;
    /// <summary>右邊剩餘時間條</summary>
    public UIProgressBar pbHp2P;
    
    /// <summary>1P戰鬥資訊</summary>
    public BattleInfo battleInfo1P = null;
    /// <summary>1P戰鬥資訊</summary>
    public BattleInfo battleInfo2P = null;
    
    /// <summary>倒數時間元件</summary>
    public TimeCircle timeCircle = null;

    float _hp1PVal = 1f;
    float _hp2PVal = 1f;

    Enum_BattleMode _playMode = Enum_BattleMode.ModeA;    
    int _gateID = -1;      
    int _npcID = -1;   

    public IBattleMode mode
    {
        get
        {
            return _mode;
        }
    }

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIBattleController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIBattleController>();

			return _instance;
		}
	}

	override public void Open(params object[] values)
	{		
		base.Open();

        NGUITools.SetActiveSelf(battleInfo1P.gameObject, false);
        NGUITools.SetActiveSelf(battleInfo2P.gameObject, false);

        _playMode = (Enum_BattleMode)values[0];
        _gateID = (int)values[1];
        _npcID = (int)values[2];
        int time = 10;

        // C模式以內才會查表改變時間
        if (_playMode <= Enum_BattleMode.ModeC)
        {
            ChallengeStageLib obj = DBFManager.challengeStageLib.Data(_gateID) as ChallengeStageLib;

            time = (obj != null) ? obj.TimeLimit : 10;
        }
        
        // 對戰模式工廠
        BattleModeFactory(Enum_BattleMode.ModeA);

        // 設定每回合秒數
        _mode.SetRoundTime(time);
        timeCircle.GetComponentInChildren<UILabel>().text = time.ToString();
	}
    
	/**================================
	 * <summary> 對戰模式工廠 </summary>
     * <param name="mode">模式</param>
	 *===============================*/
    private void BattleModeFactory(Enum_BattleMode mode)
    {
        switch(mode)
        {
        case Enum_BattleMode.ModeA:
            {
                _mode = this.gameObject.AddComponent<BattleModeA>();
            }
            break;
        case Enum_BattleMode.ModeB:
            {
                _mode = this.gameObject.AddComponent<BattleModeB>();
            }
            break;
        }
    }
	
	override public void Close()
	{		
		// do Something
		
		base.Close();

        // 移除
        Destroy((UnityEngine.Object)_mode);

        // 解除鎖定
        MouseLocker.instance.SetMouseLock(false);
	}
    
	/**================================
	 * <summary> 更新 </summary>
	 *===============================*/
    public void Update()
    {
        pbHp1P.value = Mathf.Lerp(pbHp1P.value, _hp1PVal, 0.05f);
        pbHp2P.value = Mathf.Lerp(pbHp2P.value, _hp2PVal, 0.05f);
    }    
    
	/**================================
	 * <summary> 清戰鬥資訊 </summary>
	 *===============================*/
    public void ClearBattleInfo()
    {
        battleInfo1P.Init();
        battleInfo2P.Init();
    }

	/**================================
	 * <summary> 取得戰鬥資訊 </summary>
	 *===============================*/
    public BattleInfo GetBattleInfo(int side)
    {
        return (side == 0) ? battleInfo1P : battleInfo2P;
    }

	/**================================
	 * <summary> 設定HP </summary>
     * <param name="side">0: 1p , 1: 2p</param>
     * <param name="val">數值</param>
	 *===============================*/
    public void SetHp(int side, float val)
    {
        if (side == 0)
        {
            _hp1PVal = val;
        }
        else
        {
            _hp2PVal = val;
        }
    }

	/**================================
	 * <summary> 更新玩家棋盤 </summary>
	 *===============================*/
    public void UpdateMyTTT(TTT ttt)
    {
        List<Transform> childList = _myGrid.GetChildList();
        Transform t;
        TTTData tttData;

        if (ttt.grids.Length != childList.Count)
            return;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];
            tttData = ttt.grids[i];
            NGUITools.SetActiveSelf(t.FindChild("O").gameObject, false);
            NGUITools.SetActiveSelf(t.FindChild("X").gameObject, false);
            NGUITools.SetActiveSelf(t.FindChild("uiCurrent").gameObject, false);
            t.GetComponent<UILabel>().text = tttData.ID.ToString();
        }
    }

	/**================================
	 * <summary> 更新對手棋盤 </summary>
	 *===============================*/
    public void UpdateOpponentTTT(TTT ttt)
    {        
        List<Transform> childList = _opponentGrid.GetChildList();
        Transform t;
        TTTData tttData;

        if (ttt.grids.Length != childList.Count)
            return;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];
            tttData = ttt.grids[i];
            NGUITools.SetActiveSelf(t.FindChild("O").gameObject, false);
            NGUITools.SetActiveSelf(t.FindChild("X").gameObject, false);
            NGUITools.SetActiveSelf(t.FindChild("uiCurrent").gameObject, false);
            t.GetComponent<UILabel>().text = tttData.ID.ToString();
        }
    }
    
	/**================================
	 * <summary> 顯示特效 </summary>
	 *===============================*/
    public IEnumerator ShowPickNumEffect(int num)
    {
        NGUITools.SetActiveSelf(_toggleNum, true);

        _toggleNum.GetComponentInChildren<UILabel>().text = num.ToString();

        Animation ani = _toggleNum.GetComponent<Animation>();

        while(ani.isPlaying)
            yield return new WaitForSeconds(0.1f);

        NGUITools.SetActiveSelf(_toggleNum, false);

        TTT myTTT = _mode.GetMyTTT();
        ani = null;
        int idx = myTTT.IndexOf(num);
        
        if(idx != -1)
        {            
            List<Transform> childList = _myGrid.GetChildList();
            Transform t = childList[idx];
        
            ani = t.FindChild("uiCurrent").GetComponent<Animation>();
            NGUITools.SetActiveSelf(ani.gameObject, true);
        }
                
        TTT opTTT = _mode.GetOpponentTTT();
        idx = opTTT.IndexOf(num);
        Animation ani2 = null;

        if(idx != -1)
        {            
            List<Transform> childList = _opponentGrid.GetChildList();
            Transform t = childList[idx];
        
            ani2 = t.FindChild("uiCurrent").GetComponent<Animation>();
            NGUITools.SetActiveSelf(ani2.gameObject, true);
        }        
           
        while((ani != null && ani.isPlaying) || (ani2 != null && ani2.isPlaying))
            yield return new WaitForSeconds(0.1f);
    }
    
	/**================================
	 * <summary> 顯示回答的動態 </summary>
     * <param name="side">0:左邊 , 1:右邊</param>
	 *===============================*/
    public IEnumerator ShowAnsEffect(int num, int side, bool correct)
    {
        TTT ttt = (side == 0) ? _mode.GetMyTTT() : _mode.GetOpponentTTT();
        Animation ani = null;
        Animation ani2= null;
        int idx = ttt.IndexOf(num);
        
        if(idx != -1)
        {            
            List<Transform> childList = (side == 0) ? _myGrid.GetChildList() : _opponentGrid.GetChildList();
            Transform t = childList[idx];

            string childName = (correct) ? "O" : "X";
            ani = t.FindChild(childName).GetComponent<Animation>();
            NGUITools.SetActiveSelf(ani.gameObject, true);

            // 關掉 Current提示
            ani2 = t.FindChild("uiCurrent").GetComponent<Animation>();
            NGUITools.SetActiveSelf(ani2.gameObject, false);
        }
                
        while((ani != null && ani.isPlaying))
            yield return new WaitForSeconds(0.1f);
    }
    
	/**================================
	 * <summary> 顯示連線的動態 </summary>
     * <param name="side">0:左邊 , 1:右邊</param>
	 *===============================*/
    public IEnumerator ShowLineEffect(int side, TTTLineFlag newlines)
    {
	    IEnumerator lineItor = Enum.GetValues (typeof(TTTLineFlag)).GetEnumerator ();
        TTTLineFlag line;
        Transform lineTrans = (side == 0) ? _LeftLines : _RightLines;

        while(lineItor.MoveNext())
        {
            line = (TTTLineFlag)lineItor.Current;
            // 該條線有連線成功
            if((newlines & line) > 0)
            {
                NGUITools.SetActive(lineTrans.FindChild(line.ToString()).gameObject, true);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

	/**================================
	 * <summary> 戰鬥動態 </summary>
	 *===============================*/
    public IEnumerator ShowBattleAni()
    {
        BattleInfo first = (battleInfo1P.answerTime > battleInfo2P.answerTime)? battleInfo2P : battleInfo1P;
        BattleInfo second = (battleInfo1P.answerTime > battleInfo2P.answerTime)? battleInfo1P : battleInfo2P;
        int firstSide = (battleInfo1P.answerTime > battleInfo2P.answerTime) ? 1 : 0;
        int secondSide = (battleInfo1P.answerTime > battleInfo2P.answerTime) ? 0 : 1;
        
        if(first.lineLinked > 0)
        {
            first.lbSkillName.text = "發動" + first.lineLinked + "連線技";
            NGUITools.SetActiveSelf(first.gameObject, true);
            yield return new WaitForSeconds(1.5f);
            NGUITools.SetActiveSelf(first.gameObject, false);
        }                

        SetHp(secondSide, second.remainsHp);

        if(second.lineLinked > 0)
        {
            second.lbSkillName.text = "發動" + second.lineLinked + "連線技";
            NGUITools.SetActiveSelf(second.gameObject, true);
            yield return new WaitForSeconds(1.5f);        
            NGUITools.SetActiveSelf(second.gameObject, false);
        }        

        SetHp(firstSide, first.remainsHp);
    }
    
	/**================================
	 * <summary> 激鬥動態 </summary>
	 *===============================*/
    public IEnumerator ShowFightingAni()
    {   
        NGUITools.SetActiveSelf(_fighting.gameObject, true);

        while (_fighting.animation.isPlaying)
            yield return new WaitForSeconds(0.1f);
        
        NGUITools.SetActiveSelf(_fighting.gameObject, false);
    }

	/**================================
	 * <summary> 回答 </summary>
	 *===============================*/
    public void Answer()
    {
        switch(UIButton.current.name)
        {
            case "btnO1":
                ((IBattleMode)_mode).LeftAnswer(true);
                break;
            case "btnX1":
                ((IBattleMode)_mode).LeftAnswer(false);
                break;
            case "btnO2":
                ((IBattleMode)_mode).RightAnswer(true);
                break;
            case "btnX2":
                ((IBattleMode)_mode).RightAnswer(false);
                break;
        }
    }
    
	/**================================
	 * <summary> 遊戲結束 </summary>
	 *===============================*/
    public void GameEnd()
    {
        UIPlayTween pt = GetComponent<UIPlayTween>();

        pt.Play(true);
    }

    public void OnTweenEnd()
    {
        GameInfo info = _mode.GameInfo() as GameInfo;
        ChallengeStageLib challengeObj = DBFManager.challengeStageLib.Data(_gateID) as ChallengeStageLib;
        SnatchStageLib snatchObj = DBFManager.snatchStageLib.Data(_npcID) as SnatchStageLib;
        int bet = 0;

        switch(_playMode)
        {
        case Enum_BattleMode.ModeA:
            {                
                if (null == challengeObj)
                    return;

                info.exp = challengeObj.GetGameExp(info.winSide);
                // 入場費
                bet = challengeObj.RequireItem.Value;

                // 贏的話才給錢
               // info.itemID = (info.winSide == 1) ? challengeObj.RequireItem.ItemID : -1;
                info.winCoin = (info.winSide == 1)? bet : 0;
                info.costCoin = (info.winSide == 0) ? bet : 0;
            }
            break;
        case Enum_BattleMode.ModeB:
            {                
                if (null == challengeObj)
                    return;

                info.exp = challengeObj.GetGameExp(info.winSide);
                info.winStoneID = (info.winSide == 1)? Character.ins.winStone : 0;

                // 給石頭
                if (info.winSide == 1)
                {
                    Character.GiveItem(info.winStoneID, 1);
                }
            }
            break;
        case Enum_BattleMode.ModeC:
            {                
                if (null == challengeObj)
                    return;

                info.exp = challengeObj.GetGameExp(info.winSide);
                // 入場費
                bet = challengeObj.RequireItem.Value;
                // 贏的話才給錢
                info.winCoin = (info.winSide == 1)? bet : 0;
                info.costCoin = (info.winSide == 0) ? bet : 0;

                if (info.winSide == 1)
                    Character.ins.SchoolLvUp();
            }
            break;
        case Enum_BattleMode.ModeD:
            {
                if (null == snatchObj)
                    return;

                info.exp = snatchObj.GetGameExp(info.winSide);
                info.winStoneID = (info.winSide == 1)? Character.PickStone(_npcID) : -1;
            }
            break;
        }

        // 
        if(info.winCoin > 0)
            Character.GiveItem(GameBase.ITEM_COIN, info.winCoin);
        
        // 輸了扣錢
        if (info.costCoin > 0)
            Character.DelItem(GameBase.ITEM_COIN, info.costCoin);

        if (info.exp > 0)
            Character.ins.AddExp(info.exp);

        // 輸了, 必須扣除入場石
        if(info.winSide == 0 && Character.ins.loseStone > 0)
        {
            info.lostStoneID = Character.ins.loseStone;
            Character.DelItem(Character.ins.loseStone, 1);
            Character.ins.loseStone = 0;
        }

        NetworkLib.instance.SendCleanStage(Character.ins.uid, info.winSide, info.winCoin, info.winStoneID);

        // 開啟結算畫面
        UIBattleInfoController.instance.Open(info);

        Close();
    }
}
