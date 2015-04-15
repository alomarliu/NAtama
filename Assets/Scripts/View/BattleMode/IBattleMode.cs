using UnityEngine;
using System.Collections;
using GamePlay;

public interface IBattleMode
{
    TTT GetMyTTT();

    TTT GetOpponentTTT();

    void RightAnswer(bool Correct);
    void LeftAnswer(bool Correct);

    /// <summary>
    /// 取得對戰結果
    /// </summary>
    /// <returns>資訊</returns>
    object GameInfo();
    
	/**================================
	 * <summary> 設定每回合時間 </summary>
	 *===============================*/
    void SetRoundTime(int time);
}