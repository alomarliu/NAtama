using UnityEngine;
using System.Collections;
using GamePlay;

/**=============================================================
 * <summary> 對戰模式B </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/
public class BattleModeB : MonoBehaviour, IBattleMode
{
    /// <summary>main Controller</summary>
    [SerializeField] UIBattleController _mainController = null;

    void Awake()
    {
        _mainController = GetComponent<UIBattleController>();
        Debug.Log(_mainController);
    }
    
    public TTT GetMyTTT()
    {
        return null;
    }

    public TTT GetOpponentTTT()
    { 
        
        return null;
    }
    
    public void RightAnswer(bool Correct)
    {
    }

    public void LeftAnswer(bool Correct)
    {
    }
    
    /// <summary>
    /// 取得對戰結果
    /// </summary>
    /// <returns>資訊</returns>
    public object GameInfo()
    {

        return null;
    }
    
	/**================================
	 * <summary> 設定每回合時間 </summary>
	 *===============================*/
    public void SetRoundTime(int time)
    {

    }
}
