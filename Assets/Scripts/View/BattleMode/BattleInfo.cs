using UnityEngine;
using System.Collections;

public class BattleInfo : MonoBehaviour 
{
    /// <summary>連線數</summary>
    public int lineLinked = 0;    
    /// <summary>剩餘 hp</summary>
    public float remainsHp = 0;
    /// <summary>擁有的Buff</summary>
    public int[] buffIDs = { };
    
    /// <summary>回答時間</summary>
    public float answerTime = 0;
    
    /// <summary>發動技能名稱</summary>
    public UILabel lbSkillName = null;
    
	/**================================
	 * <summary> 初始 </summary>
	 *===============================*/
    public void Init()
    {
        lineLinked = 0;
        remainsHp = 0;
        answerTime = 0;
    }
}
