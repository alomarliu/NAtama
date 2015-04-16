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
    /// 檢查是否可奪石
    /// </summary>
    /// <returns> 0: 不可 1: 可 2: 全石擁有 3: 該角色未解鎖</returns>
    public static int CheckSnatch(int npcID)
    {
        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        if (null == obj)
            return 0;

        if (null == Character.GetNpc(npcID))
            return 3;

        int stoneID = -1;
        int count = 0;
        int allCount = 0;

        for (int i = 0; i < obj.Require.Length; ++i )
        {
            stoneID = obj.Require[i];

            if(stoneID != -1)
            {
                allCount ++;

                if (GetItemCount(stoneID) > 0)
                    count++;
            }
        }

        if (count <= 0)
            return 0;
        else
        {
            return (count == allCount) ? 2 : 1;
        }
    }
        
    /// <summary>
    /// 取得目前擁有的石頭列表
    /// </summary>
    /// <returns></returns>
    public static List<int> GetOwnStoneList(int npcID)
    {
        List<int> list = new List<int>();

        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        if (null == obj)
            return list;
        
        int stoneID = -1;
        
        for (int i = 0; i < obj.Require.Length; ++i )
        {
            stoneID = obj.Require[i];

            if(stoneID != -1)
            {
                if (GetItemCount(stoneID) > 0)
                    list.Add(stoneID);
            }
        }

        return list;
    }
    
    /**========================================
    * <summary>檢查是否已搜集滿全部石頭</summary>
    * ======================================*/
    public static bool CheckGetAllStone(int npcID)
    {
        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        if (null == obj)
            return false;

        bool getAll = true;
        int stoneID = -1;

        for (int i = 0; i < obj.Require.Length; ++i )
        {
            stoneID = obj.Require[i];

            if(stoneID != -1 && GetItemCount(stoneID) < 1)
            {
                getAll = false;
                break;
            }
        }

        return getAll;
    }
    
    /**========================================
    * <summary>清除該NPC的所有石頭</summary>
    * ======================================*/
    public static void ClearAllStone(int npcID)
    {
        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        if (null == obj)
            return;

        int stoneID = -1;
        ItemBase item = null;            

        for (int i = 0; i < obj.Require.Length; ++i )
        {
            stoneID = obj.Require[i];

            if(stoneID != -1)
            {
                _itemList.TryGetValue(stoneID, out item);

                // 清除
                if (item != null)
                    item.count = 0;
            }
        }
    }

    /// <summary>
    /// 隨機挑一顆自己沒有的石頭
    /// </summary>
    /// <returns></returns>
    public static int PickStone(int npcID)
    {        
        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        if (null == obj)
            return -1;

        int stoneID = -1;
        List<int> nonStoneList = new List<int>();
        
        for (int i = 0; i < obj.Require.Length; ++i )
        {
            stoneID = obj.Require[i];

            if(stoneID != -1)
            {
                // 尚未擁有該石
                if (GetItemCount(stoneID) <= 0)
                    nonStoneList.Add(stoneID);
            }
        }

        int[] ary = nonStoneList.ToArray();
        ToolLib.Shuffle<int>(ary);
        int giveStone = 0;

        if(ary.Length > 0)
        {
            giveStone = ary[0];
            // 給石頭
            GiveItem(giveStone, 1);
        }

        return giveStone;
    }
}
