using UnityEngine;
using System.Collections;

/**=============================================================
 * <summary> 角色屬性類別 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2015 All Rights Reserved
 =============================================================*/

public class Ability
{
    // 威力加成
    public int PowerPer = 0;
    // 連擊金幣加成
    public int BonusPer = 0;
    // 經驗加成
    public int ExpPer = 0;
    
	/**================================
	 * <summary> 複製 </summary>
	 *===============================*/
    public Ability Copy()
    {
        return (Ability)this.MemberwiseClone();
    }
}
