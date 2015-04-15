using Model.DBF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**=============================================================
 * <summary> 道具基本類別 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

namespace Model.Item
{
    public class ItemBase
    {
        /// <summary> 道具ID </summary>
        public int GUID = -1;
        /// <summary> 道具類型 </summary>
        public Enum_ItemType type = Enum_ItemType.Normal;
        /// <summary> 名稱 </summary>
        public string name = "我是天邊一朵雲";
        /// <summary> 數量 </summary>
        public int count = 0;       
    
        /**========================
        * 建構子
        * ======================*/
        public ItemBase()
        {
        }

        /**========================
        * 建構子
        * ======================*/
        public ItemBase(int id)
        {
            Init(id);
        }
        
        /**========================
        * 初始化
        * ======================*/
        private void Init(int id)
        {
            ItemBaseData obj = DBFManager.itemObjLib.Data(id) as ItemBaseData;

            if (null == obj)
                return;

            GUID = id;
            name = obj.Name;
            type = (Enum_ItemType)obj.Type;
        }
    }
}