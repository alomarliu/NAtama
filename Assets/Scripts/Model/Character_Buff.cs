using LitJson;
using Model.DBF;
using Model.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Character : MonoBehaviour
{
    /// <summary>
    /// 技能資訊
    /// </summary>
    public class BuffInfo
    {
        public int buffID = -1;
        public int lv = 0;
    }

    static Dictionary<int, BuffInfo> _buffDic = new Dictionary<int, BuffInfo>();
    static long _lastLearnBuffTick = 0;
    /// <summary>正在學的BuffID</summary>
    public static int learningBuffID = 0;
    
	/**================================
	 * <summary> 
     * 最後一次按下Buff學習的時間 
     * </summary>
	 *===============================*/
    public static long GetLastLearnBuffTick()
    {
        return _lastLearnBuffTick;
    }

	/**================================
	 * <summary> 
     * 設定按下Buff學習的時間 
     * </summary>
	 *===============================*/
    public static void SetLastLearnBuffTick(long value, bool server)
    {        
        _lastLearnBuffTick = value;

        if (server)
        {
            _lastLearnBuffTick *= TimeSpan.TicksPerSecond;
            _lastLearnBuffTick += ins._baseTick;
        }
    }
    
	/**================================
	 * <summary> 
     * 檢查可否再往上學習該Buff 
     * </summary>
	 *===============================*/
    public static bool CheckLearnBuff(int id)
    {
        if (learningBuffID > 0)
            return false;

        BuffInfo info = null;

        if (_buffDic.TryGetValue(id, out info))
        {
            BuffEffectLib obj = DBFManager.buffEffectLib.Data(info.buffID+1) as BuffEffectLib;

            if (null == obj)
                return false;
        }

        return true;
    }

	/**================================
	 * <summary> 
     * 學習Buff 
     * </summary>
	 *===============================*/
    public static void LearnBuff(int id)
    {
        BuffInfo info = null;
        
        if(_buffDic.TryGetValue(id, out info))
        {
            info.lv++;
            info.buffID = id * 100 + info.lv;
        }
        else
        {
            _buffDic.Add(id, new BuffInfo() { buffID = id * 100 });
        }
        
        learningBuffID = 0;
        _lastLearnBuffTick = 0;

        // 計算能力
        ins.Calculate();
    }
    
	/**================================
	 * <summary> 
     * 取得Buff 
     * </summary>
	 *===============================*/
    public static BuffInfo GetBuff(int id)
    {
        BuffInfo info = null;
        _buffDic.TryGetValue(id, out info);

        return info;
    }
    
	/**================================
	 * <summary> 
     * 解鎖Buff 
     * </summary>
	 *===============================*/
    public static void UnLockBuff()
    {
        BuffTabelLib obj = null;

        DBFManager.buffTabelLib.Reset();

        while(DBFManager.buffTabelLib.MoveNext())
        {
            obj = DBFManager.buffTabelLib.Current as BuffTabelLib;

            // 等級到了但還沒解鎖
            if(ins.lv >= obj.Lv && null == GetBuff(obj.GUID))
            {
                LearnBuff(obj.GUID);
            }
        }
    }
}
