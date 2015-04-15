using GamePlay;
using Model.DBF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using QuestionType = Model.DBF.QuestionLib.QuestionType;

public class QuestionPlay : MonoBehaviour 
{
    /// <summary>題目內容</summary>
    [SerializeField]
    UILabel _lbQuestion = null;
    /// <summary>選項Grid</summary>
    [SerializeField]
    UIGrid _optionGrid = null;

    // 問題樣版物件
    QuestionLib questionLib = null;
    
    [SerializeField]
    GameObject _typeYorN = null;
    [SerializeField]
    GameObject _typeSort = null;
    [SerializeField]
    GameObject _typeChoose = null;

    string[] _sortAns = { "-1", "-1", "-1", "-1" };
    int _sortAnsIdx = 0;
    int _correctIdx = 0;
    int _npcIdx = 0;
    
    /// <summary>總時間</summary>
    float _totalTime = 0;
    /// <summary>開始時間</summary>
    float _startTime = 0;
    /// <summary>玩家答題時間</summary>
    [HideInInspector]
    public float leftAnsTime = 0f;
    /// <summary>對手答題時間</summary>
    [HideInInspector]
    public float rightAnsTime = 0f;
    
    /// <summary>主介面</summary>
    [SerializeField]
    UIBattleController _mainController = null;
    
    /// <summary>激鬥模式</summary>
    bool _fightMode = false;

    /// <summary>0, 1, 2</summary>
    int _leftStatus = 0;
    /// <summary>0, 1, 2</summary>
    int _rightStatus = 0;

    /**========================================
    * <summary>問答開始</summary>
     * <param name="fight">激鬥模式</param>
    * =======================================*/
    public IEnumerator QAStart(int questionID, float totalTime, bool fight)
    {   
        questionLib = DBFManager.questionList.Data(questionID) as QuestionLib;

        if (null == questionLib)
            yield break;

        // 激鬥模式
        _fightMode = fight;

        _totalTime = totalTime;
        // 初始化時間
        _mainController.timeCircle.Init((int)_totalTime);
        // 初始化
        Init();

        // 更新畫面
        UpdateUI();
        
        // 開啟滑鼠
        MouseLocker.instance.SetMouseLock(false);

        // 等待題目跑完
        yield return StartCoroutine(WaitQuetion());        
        
        _startTime = Time.time;
        // 開始跑
        _mainController.timeCircle.Begin();

        // 更新選項
        UpdateOptions(); 

        // NPC回答
        StartCoroutine(NPCAnswer());

        // 等待雙方回答
        yield return StartCoroutine(WaitAnswer());

        // 等玩家答題完才顯示NPC回答
        yield return StartCoroutine(ShowNPCAni(_rightStatus == 2));

        // 顯示正確答案動態
        yield return StartCoroutine(ShowCorrectAni());
    }
    
    /**=================================
    * <summary>等待題目跑完</summary>
    * ================================*/
    IEnumerator WaitQuetion()
    {
        // 字還在跑
        while(_lbQuestion.GetComponent<TypewriterEffect>().isActive)
            yield return new WaitForSeconds(0.1f);        
        
        yield return new WaitForSeconds(1f);        
    }
    
    /**=================================
    * <summary>顯示正確答案動態</summary>
    * ================================*/
    IEnumerator ShowCorrectAni()
    {
        NGUITools.SetActiveSelf(_typeChoose.transform.FindChild("LeftO").gameObject, false);
        NGUITools.SetActiveSelf(_typeChoose.transform.FindChild("LeftX").gameObject, false);
        NGUITools.SetActiveSelf(_typeChoose.transform.FindChild("RightO").gameObject, false);
        NGUITools.SetActiveSelf(_typeChoose.transform.FindChild("RightX").gameObject, false);
        NGUITools.SetActiveSelf(_typeChoose.transform.FindChild("uiYBG").gameObject, false);
        NGUITools.SetActiveSelf(_typeChoose.transform.FindChild("uiNBG").gameObject, false);

        // 取得正確答案
        string ans = QuestionSys.instance.GetAns();

        List<Transform> childList = _optionGrid.GetChildList();
        Transform t = null;        

        for(int i = 0; i < childList.Count ; ++i)
        {
            t = childList[i];
            
            // 不是正確答案
            if(t.GetComponentInChildren<UILabel>().text != ans)
            {
                t.GetComponent<TweenAlpha>().ResetToBeginning();
                NGUITools.SetActiveSelf( t.gameObject, false);
            }
        }

        yield return new WaitForSeconds(1f);
    }
    
    /**========================
    * <summary>等待雙方回答</summary>
    * =======================*/
    IEnumerator WaitAnswer()
    {
        float passTime = 0;

        // 有1方沒答題, 且時間還沒到
        while ((_leftStatus == 0 || _rightStatus == 0) && passTime < _totalTime)
        {
            passTime = Time.time - _startTime;
            yield return new WaitForSeconds(0.1f);
        }
        
        // 停止時間動態
        _mainController.timeCircle.Stop();   

        // 沒答題
        if (_leftStatus == 0)
            _mainController.mode.LeftAnswer(false);
        
        // 沒答題
        if (_rightStatus == 0)
            _mainController.mode.RightAnswer(false);

        StopCoroutine("NPCAnswer");
    }
    
    /**========================
    * <summary>初始化</summary>
    * =======================*/
    private void Init()
    {        
        leftAnsTime = 0;
        rightAnsTime = 0;
        _leftStatus = 0;
        _rightStatus = 0;
        _sortAns = new string[]{ "-1", "-1", "-1", "-1" };
        _sortAnsIdx = 0;
        _correctIdx = 0;
        _npcIdx = 0;
    }
    
    /**========================
    * <summary>更新介面</summary>
    * =======================*/
    void UpdateUI()
    {
        _lbQuestion.text = questionLib.Question + " ?";
        // 重設逐字打印效果
        _lbQuestion.GetComponent<TypewriterEffect>().ResetToBeginning();
                

        NGUITools.SetActiveSelf(_optionGrid.gameObject, questionLib.Type == (int)QuestionType.Sort || questionLib.Type == (int)QuestionType.Choose);
        NGUITools.SetActiveSelf(_typeYorN, questionLib.Type == (int)QuestionType.YorN);
        NGUITools.SetActiveSelf(_typeSort, questionLib.Type == (int)QuestionType.Sort);

        UISprite[] ss = _typeChoose.GetComponentsInChildren<UISprite>();
        foreach(UISprite s in ss)
        {
            NGUITools.SetActiveSelf(s.gameObject, false);
        }

        switch((QuestionType)questionLib.Type)
        {
        case QuestionType.YorN:
                Debug.Log(questionLib.Option1);
            break;
        case QuestionType.Choose:
                Debug.Log(questionLib.Option1);
            break;
        case QuestionType.Sort:
                
                Debug.Log(string.Format("{0},{1},{2},{3}" , questionLib.Option1, questionLib.Option2, questionLib.Option3, questionLib.Option4));
            break;
        }       
    }    
    
    /**========================
    * <summary>選項Tween完畢</summary>
    * =======================*/
    public void OnOptionTweenEnd()
    {
        UITweener ta = TweenAlpha.current;

        ta.GetComponent<UIButton>().enabled = true;
    }

    /**========================
    * <summary>更新選項</summary>
    * =======================*/
    private void UpdateOptions()
    {
        if (questionLib.Type != (int)QuestionType.Sort && questionLib.Type != (int)QuestionType.Choose)
            return;
        
        int[] nums = { 1,2,3,4 };
        ToolLib.Shuffle<int>(nums);        

        List<Transform> childList = _optionGrid.GetChildList();
        Transform t = null;
        string fieldName = "";
        FieldInfo field = null;

        for (int i = 0; i < nums.Length; ++ i )
        {
            fieldName = "Option" + nums[i];
            field = typeof(QuestionLib).GetField(fieldName);

            if(field != null)
            {
                t = childList[i];
                t.GetComponent<UIButton>().isEnabled = true;
                t.GetComponent<UIButton>().enabled = false;
                t.GetComponent<TweenAlpha>().ResetToBeginning();
                t.GetComponent<TweenAlpha>().PlayForward();
                NGUITools.SetActiveSelf(t.gameObject, true);
                t.GetComponentInChildren<UILabel>().text = (string)field.GetValue(questionLib);
            }

            if (nums[i] == 1)
            {                
                _correctIdx = i;
            }
        }
    }
    
    /**========================
    * <summary>NPC回答</summary>
    * =======================*/
    IEnumerator NPCAnswer()
    {
        // 已答過
        if (_rightStatus != 0)
            yield break;

        if (Time.time - _startTime > _totalTime)
            yield break;

        // 決定機率        
        int rand = UnityEngine.Random.Range(0, 100) + 1;
        // 正確機率80
        int cRate = 60;

        // 幾秒答題
        float sec = UnityEngine.Random.Range(2f, 7f);

        // 等候時間
        yield return new WaitForSeconds(sec);

        bool correct = rand <= cRate;

        List<int> list = new List<int>();

        for (int i = 0; i < 4; ++i )
        {
            if (i != _correctIdx)
                list.Add(i);
        }

        int[] temp = list.ToArray();
        ToolLib.Shuffle<int>(temp);
        
        _npcIdx = (correct)? _correctIdx : temp[0];        
        
        _rightStatus = (correct) ? 2 : 1;
        rightAnsTime = (correct)? Time.time - _startTime : 0;
        _mainController.mode.RightAnswer(correct);
        
        // 激鬥模式
        if (_fightMode && correct)
        {
            // 對手未答題
            if (_leftStatus == 0)
            {
                _leftStatus = 1;
                _mainController.mode.LeftAnswer(!correct);

                // 鎖定滑鼠
                MouseLocker.instance.SetMouseLock(true);
            }

            _mainController.timeCircle.Stop();
        }
        else
        {
            // 玩家還沒回答
            if (_leftStatus == 0)
                _mainController.timeCircle.Answer(0);
            // 玩家已回答
            else
                _mainController.timeCircle.Stop();
        }
    }

    /**========================
    * <summary>按下回答</summary>
    * =======================*/
    public void OnAnswer()
    {
        // 已答過
        if (_leftStatus != 0)
            return;

        if (Time.time - _startTime > _totalTime)
            return;

        UIButton btn = UIButton.current;
        string objName = btn.name;
        string myAns = "";

        if ((QuestionType)questionLib.Type == QuestionType.Choose ||
            (QuestionType)questionLib.Type == QuestionType.Sort)
        {
            btn.isEnabled = false;
        }

        switch((QuestionType)questionLib.Type)
        {
        case QuestionType.YorN:
            myAns = objName;
            break;
        case QuestionType.Choose:
            {
                myAns = btn.GetComponentInChildren<UILabel>().text;
            }
            break;
        case QuestionType.Sort:
            {
                _sortAns[_sortAnsIdx++] = btn.GetComponentInChildren<UILabel>().text;
                
                if(_sortAnsIdx < 4)
                    return;

                for(int i = 0; i < _sortAns.Length; ++i)
                {
                    myAns += _sortAns[i] + ",";
                }

                myAns = myAns.Remove(myAns.LastIndexOf(','));
            }
            break;
        }

        // 正確回答?
        bool correct = QuestionSys.instance.Answer(myAns);

        leftAnsTime = (correct)? Time.time - _startTime : 0;
        _leftStatus = (correct) ? 2 : 1;
        _mainController.mode.LeftAnswer(correct);
        
        // 顯示問答動態
        ShowChooseAni(correct, btn.gameObject);
        // 鎖定滑鼠
        MouseLocker.instance.SetMouseLock(true);

        // 激鬥模式
        if(_fightMode && correct)
        {
            // 對手未答題
            if (_rightStatus == 0)
            {
                _rightStatus = 1;
                _mainController.mode.RightAnswer(!correct);
            }

            _mainController.timeCircle.Stop();
        }
        else
        {
            // npc還沒回答
            if (_rightStatus == 0)
                _mainController.timeCircle.Answer(1);
            // npc已回答
            else
                _mainController.timeCircle.Stop();    
        }
    }
    
    /**========================
    * <summary>NPC回答動態</summary>
    * =======================*/
    IEnumerator ShowNPCAni(bool correct)
    {
        if ((QuestionType)questionLib.Type != QuestionType.Choose)
            yield break;

        // 激鬥模式下, NPC答題較慢不顯示動態
        if (_fightMode && rightAnsTime > leftAnsTime)
            yield break;

        List<Transform> childList = _optionGrid.GetChildList();
        Transform btn = childList[_npcIdx];
        GameObject O = _typeChoose.transform.FindChild("RightO").gameObject;
        GameObject X = _typeChoose.transform.FindChild("RightX").gameObject;

        Vector2 pos = btn.position;
        pos.x = O.transform.position.x;
        O.transform.position = pos;
        pos.x = X.transform.position.x;
        X.transform.position = pos;

        NGUITools.SetActiveSelf(O, correct);
        NGUITools.SetActiveSelf(X, !correct);

        Animation ani = ((correct) ? O : X).GetComponent<Animation>();

        // 動態還在播
        while (ani.isPlaying)
            yield return new WaitForSeconds(0.1f);
    }

    /**========================
    * <summary>問答動態</summary>
    * =======================*/
    private void ShowChooseAni(bool correct, GameObject now)
    {
        if ((QuestionType)questionLib.Type != QuestionType.Choose)
            return;

        GameObject LeftO = _typeChoose.transform.FindChild("LeftO").gameObject;
        GameObject LeftX = _typeChoose.transform.FindChild("LeftX").gameObject;
        GameObject goY = _typeChoose.transform.FindChild("uiYBG").gameObject;
        GameObject goN = _typeChoose.transform.FindChild("uiNBG").gameObject;
        GameObject temp = null;
        GameObject temp2 = null;
        Vector2 pos = Vector2.zero;

        if (now != null)
        {
            temp = (correct) ? LeftO : LeftX;
            temp2 = (correct) ? goY : goN;
            pos = now.transform.position;
            pos.x = temp.transform.position.x;
            temp.transform.position = pos;
            pos.x = temp2.transform.position.x;
            temp2.transform.position = pos;
            NGUITools.SetActiveSelf(temp, true);
            NGUITools.SetActiveSelf(temp2, true);
        }
    }
    
    void OnDisable()
    {
        List<Transform> childList = _optionGrid.GetChildList();
        Transform t = null;

        for (int i = 0; i < childList.Count; ++i )
        {
            t = childList[i];
            NGUITools.SetActiveSelf(t.gameObject, false);
        }

        // 停止
        _mainController.timeCircle.Stop();

        // 開啟滑鼠
        MouseLocker.instance.SetMouseLock(false);
        
        UISprite[] ss = _typeChoose.GetComponentsInChildren<UISprite>();
        foreach(UISprite s in ss)
        {
            NGUITools.SetActiveSelf(s.gameObject, false);
        }
    }
}
