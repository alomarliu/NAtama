using UnityEngine;
using System.Collections;
using GamePlay;

public interface IGateSelectMode
{
	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    void UpdateUI();
    
	/**================================
	 * <summary> 關卡選擇 </summary>
	 *===============================*/
    void OnGateSelect(string gate = "", int npc = 0);
    
	/**================================
	 * <summary> 確認 </summary>
	 *===============================*/
    void OnConfirm(DialogOption.OptionType option, params object[] param);
}