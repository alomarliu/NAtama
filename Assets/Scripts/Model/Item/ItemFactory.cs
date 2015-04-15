using Model.DBF;
using Model.Item;
using System.Collections;
using UnityEngine;

/**=============================================================
 * <summary> 裝備結構工廠 (簡易工廠模式,  裝備就是卡片) </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/
public class ItemFactory
{
    /**========================
    * <summary>
    * 簡單工廠模式
    * </summary>
    * ======================*/
    public static ItemBase CreateItem(int GUID)
    {
        ItemBaseData obj = DBFManager.itemObjLib.Data(GUID) as ItemBaseData;

        if (null == obj)
        {
           Debug.LogError("Item not Exists: " + GUID);
           return null;
        }
                
        ItemBase item = CreateItem(GUID, (Enum_ItemType)obj.Type);

        return item;
    }

    /**========================
    * <summary>
    * 簡單工廠模式
    * </summary>
    * ======================*/
    public static ItemBase CreateItem(int GUID, Enum_ItemType type)
    {
        ItemBase item = null;

        switch(type)
        {
            default:
                item = new ItemBase(GUID);
                break;
        }

        return item;
    }
}
