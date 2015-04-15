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
    /// <summary>道具表</summary>
    static Dictionary<int, ItemBase> _itemList = new Dictionary<int, ItemBase>();
    
    /**========================================
        * <summary>檢查是否可給道具</summary>
        * ======================================*/
    public static bool CheckAddItem(int id, int count)
    {
        ItemBaseData obj = DBFManager.itemObjLib.Data(id) as ItemBaseData;

        if (null == obj)
            return false;
            
        if (count <= 0)
            return false;

        if (!ToolLib.IsValidEnumValue<Enum_ItemType>((Enum_ItemType)obj.Type))
            return false;                 
            
        // 角色數值
        if(obj is ItemRoleValueData)
        {            
            string fieldName = ((ItemRoleValueData)obj).ClientVar;

            if (string.IsNullOrEmpty(fieldName) || fieldName == "-1")
                return false;

            string[] fields = fieldName.Split('.');
            FieldInfo info = null;
            Type tempType = typeof(Character);

            try 
            {
                // 走訪
                for(int i = 0; i < fields.Length; ++i)
                {
                    fieldName = fields[i];
                    info = tempType.GetField(fieldName);
                    tempType = info.FieldType;                    
                }
            }
            catch
            {
                throw new SystemException("field error");
            }

            // 有取得正確 type
            if(info != null)
            {
                object valobj = info.GetValue(_instance);

                if(valobj is int)
                {                    
                    int value = (int)valobj + count;
                    return value <= ((ItemRoleValueData)obj).Max;
                }
                else if(valobj is uint)
                {
                    uint value = (uint)valobj + (uint)count;
                    return value <= ((ItemRoleValueData)obj).Max;
                }
            }
        }
        else
        {
            ItemBase item = null;
            _itemList.TryGetValue(id, out item);

            if (null == item)
                return true;

            Int64 value = (Int64)item.count + count;

            return value <= int.MaxValue;
        }

        return false;
    }
    
    /**========================================
        * <summary>檢查是否可給道具</summary>
        * ======================================*/
    public static bool CheckDelItem(int id, int count)
    {
        ItemBaseData obj = DBFManager.itemObjLib.Data(id) as ItemBaseData;

        if (null == obj)
            return false;
            
        if (count <= 0)
            return false;

        if (!ToolLib.IsValidEnumValue<Enum_ItemType>((Enum_ItemType)obj.Type))
            return false;                 
           
        // 角色數值
        if(obj is ItemRoleValueData)
        {            
            string fieldName = ((ItemRoleValueData)obj).ClientVar;

            if (string.IsNullOrEmpty(fieldName) || fieldName == "-1")
                return false;

            string[] fields = fieldName.Split('.');
            FieldInfo info = null;
            Type tempType = typeof(Character);

            try 
            {
                // 走訪
                for(int i = 0; i < fields.Length; ++i)
                {
                    fieldName = fields[i];
                    info = tempType.GetField(fieldName);
                    tempType = info.FieldType;                    
                }
            }
            catch
            {
                throw new SystemException("field error");
            }

            // 有取得正確 type
            if(info != null)
            {                
                object valobj = info.GetValue(_instance);

                if(valobj is int)
                {
                    int value = (int)valobj;
                    return value >= count;
                }
                else if(valobj is uint)
                {
                    uint value = (uint)valobj;
                    return value >= count;
                }
            }
        }
        else
        {
            ItemBase item = null;
            _itemList.TryGetValue(id, out item);
            
            if (null == item)
                return false;

            return item.count >= count;
        }

        return false;
    }
    
    /**========================================
        * <summary>給道具</summary>
        * ======================================*/
    public static bool GiveItem(int id, int count)
    {
        // 檢查
        if (!CheckAddItem(id, count))
            return false;
        
        ItemBaseData obj = DBFManager.itemObjLib.Data(id) as ItemBaseData;    
        
        // 角色數值
        if(obj is ItemRoleValueData)
        {            
            string fieldName = ((ItemRoleValueData)obj).ClientVar;

            if (string.IsNullOrEmpty(fieldName) || fieldName == "-1")
                return false;

            string[] fields = fieldName.Split('.');
            FieldInfo info = null;
            Type tempType = typeof(Character);

            try 
            {
                // 走訪
                for(int i = 0; i < fields.Length; ++i)
                {
                    fieldName = fields[i];
                    info = tempType.GetField(fieldName);
                    tempType = info.FieldType;                    
                }
            }
            catch
            {
                throw new SystemException("field error");
            }

            // 有取得正確 type
            if(info != null)
            {
                object valobj = info.GetValue(_instance);

                if(valobj is int)
                {                    
                    int value = (int)valobj + count;
                    info.SetValue(_instance, value);
                }
                else if(valobj is uint)
                {
                    uint value = (uint)valobj + (uint)count;
                    info.SetValue(_instance, value);
                }
            }
        }
        else
        {
            ItemBase item = null;
            _itemList.TryGetValue(id, out item);

            if (null == item)
            {
                item = ItemFactory.CreateItem(id);
                _itemList.Add(id, item);
            }

            item.count += count;

            if(item.type == Enum_ItemType.Stone)
            {
                // 升級
                NpcLevelUP();
            }
        }

        return true;
    }
    
    /**========================================
    * <summary>刪除道具</summary>
    * ======================================*/
    public static bool DelItem(int id, int count)
    {
        if (!CheckDelItem(id, count))
            return false;
            
        ItemBaseData obj = DBFManager.itemObjLib.Data(id) as ItemBaseData;     
        
        // 角色數值
        if(obj is ItemRoleValueData)
        {            
            string fieldName = ((ItemRoleValueData)obj).ClientVar;

            if (string.IsNullOrEmpty(fieldName) || fieldName == "-1")
                return false;

            string[] fields = fieldName.Split('.');
            FieldInfo info = null;
            Type tempType = typeof(Character);

            try 
            {
                // 走訪
                for(int i = 0; i < fields.Length; ++i)
                {
                    fieldName = fields[i];
                    info = tempType.GetField(fieldName);
                    tempType = info.FieldType;                    
                }
            }
            catch
            {
                throw new SystemException("field error");   
            }

            // 有取得正確 type
            if(info != null)
            {
                object valobj = info.GetValue(_instance);

                if(valobj is int)
                {
                    int value = (int)valobj - count;
                    info.SetValue(_instance, value);
                }
                else if(valobj is uint)
                {
                    uint value = (uint)valobj - (uint)count;
                    info.SetValue(_instance, value);
                }
            }
        }
        else
        {
            ItemBase item = null;
            _itemList.TryGetValue(id, out item);
            
            if (null == item)
                return false;

            item.count -= count;
        }

        return true;
    }

    /**========================================
    * <summary>取得道具數量</summary>
    * ======================================*/
    public static Int64 GetItemCount(int id)
    {            
        ItemBaseData obj = DBFManager.itemObjLib.Data(id) as ItemBaseData;

        if (null == obj)
            return 0;
            
        // 角色屬性
        if(obj is ItemRoleValueData)
        {
            string fieldName = ((ItemRoleValueData)obj).ClientVar;

            if (string.IsNullOrEmpty(fieldName) || fieldName == "-1")
                return 0;

            string[] fields = fieldName.Split('.');
            FieldInfo info = null;
            Type tempType = typeof(Character);

            try 
            {
                // 走訪
                for(int i = 0; i < fields.Length; ++i)
                {
                    fieldName = fields[i];
                    info = tempType.GetField(fieldName);
                    tempType = info.FieldType;                    
                }
            }
            catch
            {
                throw new SystemException("field error");
            }

            // 有取得正確 type
            if(info != null)
            {
                return (Int64)info.GetValue(_instance);
            }
        }
        else
        {                         
            ItemBase item = null;
            _itemList.TryGetValue(id, out item);
            return (item == null) ? 0 : item.count;
        }

        return 0;
    }
    
    /**========================================
    * <summary>寶石轉換為金幣</summary>
    * ======================================*/
    public static void Gold2Coin(int count)
    {
        if (count <= 0)
            return;

        // 檢查可否扣除金幣
        if (!CheckDelItem(GameBase.ITEM_GOLD, count))
            return;

        // 檢查可否加金幣
        if (!CheckAddItem(GameBase.ITEM_COIN, count * 1000))
            return;

        // 扣寶石
        DelItem(GameBase.ITEM_GOLD, count);
        // 給金幣
        GiveItem(GameBase.ITEM_COIN, count * 1000);
    }
}
