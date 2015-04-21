using GamePlay;
using Model.DBF;
using System;
using System.Collections;
using UnityEngine;

/**=============================================================
 * <summary> UI對戰模式A </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/
public class BattleModeA : MonoBehaviour, IBattleMode
{
    public enum Mode
    {
        Start = 0x01,
        End,
    }

    QuestionPlay _questPlay = null;

    /// <summary>main Controller</summary>
    UIBattleController _mainController = null;

    /// <summary>玩家棋盤</summary>
    TTT _myTTT = null;
    /// <summary>對手棋盤</summary>
    TTT _opponentTTT = null;
    
    /// <summary>目前命中號碼</summary>
    int toggleNum = -1;
    
    /// <summary>總時間</summary>
    float _totalTime = 10f;  

    /// <summary>0, 1, 2</summary>
    int _leftStatus = 0;
    /// <summary>0, 1, 2</summary>
    int _rightStatus = 0;

    int _winCount1P = 0;
    int _winCount2P = 0;
    
    /// <summary>基礎攻擊力</summary>
    const int baseATK = 100;
    const int fullHP = 1000;

    /// <summary>1p血量</summary>
    int _hp1P = fullHP;
    /// <summary>2p血量</summary>
    int _hp2P = fullHP;
    
    Queue NumQueue;
        
    /// <summary>連續答對1P</summary>
    int _continueWin1P = 0;
    public int maxContinueWin1P = 0;
    /// <summary>連續答對2P</summary>
    int _continueWin2P = 0;
    public int maxContinueWin2P = 0;
    
    int continueWin1P
    {
        get { return _continueWin1P; }
        set
        {
            _continueWin1P = value;
            maxContinueWin1P = Math.Max(_continueWin1P, maxContinueWin1P);
        }
    }
    
    int continueWin2P
    {
        get { return _continueWin2P; }
        set
        {
            _continueWin2P = value;
            maxContinueWin2P = Math.Max(_continueWin2P, maxContinueWin2P);
        }
    }

    /// <summary>開始</summary>
    Mode _mode = Mode.Start;

    void Awake()
    {
        _mainController = GetComponent<UIBattleController>();
        _questPlay = _mainController.questionPlay;

        NGUITools.SetActiveSelf(_questPlay.gameObject, false);
    }
    
	/**================================
	 * <summary> 設定每回合時間 </summary>
	 *===============================*/
    public void SetRoundTime(int time)
    {
        _totalTime = time;
    }

	void Start()
	{
        // 初始化
        Init();
	}

	/**================================
	 * <summary> 初始化 </summary>
	 *===============================*/
    private void Init()
    {
        _myTTT = new TTT();
        _opponentTTT = new TTT();

        _mainController.UpdateMyTTT(_myTTT);
        _mainController.UpdateOpponentTTT(_opponentTTT);

        StartCoroutine(GameLoop());
    }
    
	/**================================
	 * <summary> 檢查遊戲是否已結束 </summary>
	 *===============================*/
    bool CheckGameEnd()
    {
        // 沒題目了
        if (NumQueue.Count == 0)
            return true;

        // 有一方死了
        if (_hp1P == 0 || _hp2P == 0)
            return true;

        if (_mode == Mode.End)
            return true;

        return false;
    }
    
	/**================================
	 * <summary> 遊戲開始 </summary>
	 *===============================*/
    IEnumerator GameLoop()
    {
        // 開始
        _mode = Mode.Start;
        int[] nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        //int[] nums = { 1 };
        //int[] nums = { 1,3,2,4,5 };

        ToolLib.Shuffle<int>(nums);
        NumQueue = new Queue(nums);
        _continueWin1P = 0;
        _continueWin2P = 0;
        maxContinueWin1P = 0;
        maxContinueWin2P = 0;

        yield return new WaitForSeconds(0.5f);

        while(!CheckGameEnd())
        {
            yield return StartCoroutine(RoundLoop());
        }

        // 顯示戰鬥結束字串
        NGUITools.SetActive(_mainController.lbBattleEnd.gameObject, true);
        
        yield return new WaitForSeconds(1f);

        _mainController.GameEnd();
    }
    
	/**================================
	 * <summary> 回合開始 </summary>
	 *===============================*/
    IEnumerator RoundLoop()
    {
        // 亂數
        PickNum();
        
        // 回合開始
        RoundStart();

        // 顯示動態
        yield return StartCoroutine(ShowPickNumAni());

        TTTLineFlag newLine1P = _myTTT.CheckNewLine(toggleNum);
        TTTLineFlag newLine2P = _opponentTTT.CheckNewLine(toggleNum);
        bool fight = false;

        if ((newLine1P | newLine2P) > 0)
        {
            fight = true;
            yield return StartCoroutine(ShowFightAni());
        }

        // 亂數題目
        int qID = PickQuestion();
        
        NGUITools.SetActiveSelf(_questPlay.gameObject, true);

        // 開始問答
        yield return StartCoroutine(_questPlay.QAStart(qID, _totalTime, fight));

        // 等候
        //yield return new WaitForSeconds(0.3f);
        
        NGUITools.SetActiveSelf(_questPlay.gameObject, false);
        
        // hp動態用
        _mainController.GetBattleInfo(0).remainsHp = _hp1P / (float)fullHP;
        _mainController.GetBattleInfo(1).remainsHp = _hp2P / (float)fullHP;

        // 顯示戰鬥動態
        yield return StartCoroutine(ShowBattleAni());

        // 等候
        yield return new WaitForSeconds(0.6f);
    }
    
	/**================================
	 * <summary> 激鬥動態 </summary>
	 *===============================*/
    IEnumerator ShowFightAni()
    {        
        yield return StartCoroutine(_mainController.ShowFightingAni());
    }

	/**================================
	 * <summary> 戰鬥動態 </summary>
	 *===============================*/
    IEnumerator ShowBattleAni()
    {        
        yield return StartCoroutine(_mainController.ShowBattleAni());
    }
    
	/**================================
	 * <summary> 檢查是否已該回合結束 </summary>
	 *===============================*/
    void RoundStart()
    {
        _leftStatus = 0;
        _rightStatus = 0;
        _mainController.lbRightToggleTime.text = "答題時間: 0s";
        _mainController.lbLeftToggleTime.text = "答題時間: 0s";
        // 清戰鬥資訊
        _mainController.ClearBattleInfo();
    }
    
	/**================================
	 * <summary> 選號碼 </summary>
	 *===============================*/
    void PickNum()
    {
        toggleNum = (int)NumQueue.Dequeue();
    }
    
	/**================================
	 * <summary> 挑選題目動態 </summary>
	 *===============================*/
    IEnumerator ShowPickNumAni()
    {
        yield return StartCoroutine(_mainController.ShowPickNumEffect(toggleNum));
    }

	/**================================
	 * <summary> 選問題 </summary>
	 *===============================*/
    int PickQuestion()
    {
        IEnumerator itor = Enum.GetValues(typeof(QuestionLib.QuestionType)).GetEnumerator();
        int[] rand = { 0, 100, 0 };
        int toggle = UnityEngine.Random.Range(0, 100) + 1;
        int sum = 0;

        for (int i = 0; i < rand.Length; ++i )
        {
            sum += rand[i];

            if (!itor.MoveNext())
                break;

            if(toggle <= sum)
                break;
        }

        QuestionLib.QuestionType type = (QuestionLib.QuestionType)itor.Current;

        int diffcult = UnityEngine.Random.Range(0, 5) + 1;
        return QuestionSys.instance.RandQuestion(type, 1, diffcult);
    }
     
	/**================================
	 * <summary> 取得玩家TTT </summary>
	 *===============================*/
    public TTT GetMyTTT()
    {
        return _myTTT;
    }

	/**================================
	 * <summary> 取得對手TTT </summary>
	 *===============================*/
    public TTT GetOpponentTTT()
    {
        return _opponentTTT;
    }
    
	/**================================
	 * <summary> 左邊答題 </summary>
	 *===============================*/
    public void LeftAnswer(bool correct)
    {        
        // 已答過
        if (_leftStatus != 0 || _mode == Mode.End)
            return;

        int side = 0;

        _leftStatus = (correct) ? 2 : 1;
        
        int idx = _myTTT.IndexOf(toggleNum);

        if (idx != -1 && correct)
            _myTTT.Toggle(idx, correct);
        
        float time = (float)Math.Round(_mainController.questionPlay.leftAnsTime, 2);

        if (correct)
        {
            _mainController.lbLeftToggleTime.text = "答題時間: " + time.ToString() + "s";
            ++_winCount1P;
        }

        // 答題時間
        _mainController.GetBattleInfo(side).answerTime = (correct) ? time : _totalTime;

        StartCoroutine(Try(side, correct));

        // 血量扣到0了
        if (_hp2P <= 0)
            _mode = Mode.End;

        // 答對+1 , 答錯歸0
        continueWin1P = (_leftStatus == 2) ? continueWin1P+1 : 0;
    }
    
	/**================================
	 * <summary> 右邊答題 </summary>
	 *===============================*/
    public void RightAnswer(bool correct)
    {
        // 已答過
        if (_rightStatus != 0 || _mode == Mode.End)
            return;

        int side = 1;

        _rightStatus = (correct) ? 2 : 1;

        int idx = _opponentTTT.IndexOf(toggleNum);

        if(idx != -1 && correct)
            _opponentTTT.Toggle(idx, correct);

        float time = (float)Math.Round(_mainController.questionPlay.rightAnsTime, 2);

        if (correct)
        {
            _mainController.lbRightToggleTime.text = "答題時間: " + time.ToString() + "s";
            ++_winCount2P;
        }
        
        // 答題時間
        _mainController.GetBattleInfo(side).answerTime = (correct) ? time : _totalTime;

        StartCoroutine(Try(side, correct));
        
        // 血量扣到0了
        if (_hp1P <= 0)
            _mode = Mode.End;
        
        // 答對+1 , 答錯歸0
        continueWin2P = (_rightStatus == 2) ? continueWin2P+1 : 0;    
    }
    
    IEnumerator Try(int side, bool correct)
    {
        TTTLineFlag newLines = ((side == 0) ? _myTTT: _opponentTTT).GetNewLine();
        
        if (correct)
        {            
            IEnumerator itor = Enum.GetValues(typeof(TTTLineFlag)).GetEnumerator();
            int count = 0 ;
            int multiple = 1;            

            // 計算連成幾條線
            while(itor.MoveNext())
            {
                if (((TTTLineFlag)itor.Current & newLines) > 0)
                    count++;
            }

            multiple += count;

            if (side == 0)
            {
                _hp2P -= baseATK * multiple;
            }
            else
            {
                _hp1P -= baseATK * multiple;
            }
            
            _mainController.GetBattleInfo(side).lineLinked = count;
        }

        // 顯示動態
        yield return StartCoroutine(_mainController.ShowAnsEffect(toggleNum, side, correct));

        // 顯示連線動態
        yield return StartCoroutine(_mainController.ShowLineEffect(side, newLines));
    }
    
    /// <summary>
    /// 取得對戰結果
    /// </summary>
    /// <returns>資訊</returns>
    public object GameInfo()
    {
        UIBattleController.GameInfo info = new UIBattleController.GameInfo();
        info.winSide = (_hp1P ==_hp2P) ? 2 : ((_hp1P > _hp2P)? 1: 0);
        //info.winSide = 1;
        info.winTimes = maxContinueWin1P;
        
        return info;
    }
}
