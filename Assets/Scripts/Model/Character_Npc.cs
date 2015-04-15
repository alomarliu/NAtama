using LitJson;
using Model.DBF;
using Model.Item;
using NLNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Character : MonoBehaviour
{
    public class NpcInfo
    {
        public int roleID = -1;
        public int lv = 0;
    }

    // NPC 列表
    static Dictionary<int, NpcInfo> _npcDic = new Dictionary<int, NpcInfo>();
    
    /**========================================
    * <summary>檢查Npc升級</summary>
    * ======================================*/
    public static bool CheckNpcLvUp(int npcID)
    {
        bool getAll = CheckGetAllStone(npcID);

        if (!getAll)
            return false;

        ConfigLib configObj = DBFManager.configLib.Data(1) as ConfigLib;
        NpcInfo info = null;

        _npcDic.TryGetValue(npcID, out info);

        if (null == configObj)
            return false;

        // 加入
        if (null == info)
        {
            info = new NpcInfo() { roleID = npcID, lv = 0 };
            _npcDic.Add(npcID, info);
        }
        else
        {
            // 等級已滿
            if (info.lv >= configObj.RoleMaxLv)
                return false;
        }
       
        return true;
    }
    
    /**========================================
    * <summary>取得NPC</summary>
    * ======================================*/
    public static NpcInfo GetNpc(int npcID)
    {
        NpcInfo npcInfo = null;
        _npcDic.TryGetValue(npcID, out npcInfo);

        return npcInfo;
    }

    /**========================================
    * <summary>NPC升級(走訪全NPC)</summary>
    * ======================================*/
    public static void NpcLevelUP()
    {
        SnatchStageLib obj = null;
        DBFManager.snatchStageLib.Reset();

        while(DBFManager.snatchStageLib.MoveNext())
        {
            obj = DBFManager.snatchStageLib.Current as SnatchStageLib;
            NpcLevelUP(obj.GUID);
        }
    }

    /**========================================
    * <summary>NPC升級</summary>
    * ======================================*/
    public static void NpcLevelUP(int npcID)
    {
        if (!CheckNpcLvUp(npcID))
            return;
        
        NpcInfo info = null;

        _npcDic.TryGetValue(npcID, out info);
        
        // 清掉石頭
        ClearAllStone(npcID);

        // 等級上升
        info.lv++;

        // 傳送指令
        NetworkLib.instance.SendNpcLvUp(ins.uid, npcID);
    }
}
