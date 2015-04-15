using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**=============================================================
 * <summary> 井字遊戲 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

namespace GamePlay
{
    /// <summary>
    /// 判斷是否連線的旗標
    /// <c>
    /// 0, 1, 2
    /// 3, 4, 5
    /// 6, 7, 8
    /// </c>
    /// </summary>
	/// 
	[Flags]
    public enum TTTLineFlag
	{
		/// <summary>第一條0,1,2</summary>
		Line1 = 0x01,
		/// <summary>第二條3,4,5</summary>
		Line2 = 0x02,
		/// <summary>第三條6,7,8</summary>
		Line3 = 0x04,
		/// <summary>第四條0,3,6</summary>
		Line4 = 0x08,
		/// <summary>第五條1,4,7</summary>
		Line5 = 0x10,
		/// <summary>第六條2,5,8</summary>
		Line6 = 0x20,
		/// <summary>第七條0,4,8</summary>
		Line7 = 0x40,
		/// <summary>第八條2,4,6</summary>
		Line8 = 0x80,
    }

    /// <summary>
    /// 井字遊戲每一格資料
    /// </summary>
    public class TTTData
    {
        public int ID = 1;
        /// <summary>-1:未答 0: 答錯, 1: 答對</summary>
        public int status = -1;

        public TTTData(int id)
        {
            ID = id;

        }
    }

    public class TTT
	{
		/// <summary>
		/// <舊的已連線組合>
		/// </summary>		 
		private TTTLineFlag _oldLinkLine = 0;
		// 用來判斷是否有連線的組合
		public List<int>[] lineList = 
		{
			new List<int>(new int[]{0,1,2}),
			new List<int>(new int[]{3,4,5}),
			new List<int>(new int[]{6,7,8}),
			new List<int>(new int[]{0,3,6}),
			new List<int>(new int[]{1,4,7}),
			new List<int>(new int[]{2,5,8}),
			new List<int>(new int[]{0,4,8}),
			new List<int>(new int[]{2,4,6}),
		};

        public TTTData[] grids = new TTTData[9];

        public TTT()
        {
            RandGrid();

            //InitTestData();
        }
        
	    /**================================
	     * <summary> 打亂棋盤 </summary>
	     *===============================*/
        private void RandGrid()
        {
            int[] temp = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // 打亂
            ToolLib.Shuffle<int>(temp);

            grids = new TTTData[temp.Length];

            for(int i = 0; i < grids.Length; ++i)
            {
                grids[i] = new TTTData(temp[i]);
            }
        }
        
	    /**================================
	     * <summary> 初始測試資料 </summary>
	     *===============================*/
        private void InitTestData()
        {
			//grids [5].flag |= TTTFlag.Toggle;
			//grids [6].flag |= TTTFlag.Toggle;
			//grids [7].flag |= TTTFlag.Toggle;
			//grids [8].flag |= TTTFlag.Toggle;

            Toggle(0, true);
            Toggle(1, true);
            Toggle(2, true);
        }
		
		/**================================
	     * <summary> 該格答對 </summary>
	     *===============================*/
		public void Toggle(int idx, bool correct)
		{
			if (idx < 0 || idx >= grids.Length)
				return;

            grids[idx].status = (correct) ? 1 : 0;
		}
        
	    /**================================
	     * <summary> 檢查是否有新連線 </summary>
	     *===============================*/
		public TTTLineFlag CheckNewLine(int newNum)
        {
            if (newNum < 1 || newNum > 9)
                return 0;

			IEnumerator lineItor = Enum.GetValues (typeof(TTTLineFlag)).GetEnumerator ();
			List<int> line;
			TTTData data;
			bool link = true;
			// 有連線的組合
			TTTLineFlag linkLine = 0;
			// 新連線
			TTTLineFlag diffLinkLine;

			for (int i = 0; i < lineList.Length; ++i) 
			{
				// 取得連線組合
				line = lineList[i];
				link = true;

				// 沒有下一條線了
				if(!lineItor.MoveNext())
					break;

				foreach(int idx in line)
				{
					data = grids[idx];


                    // 該格答錯
					if(data.status == 0 ||
                      (data.status == -1 && data.ID != newNum))
					{
                        link = false;
                        break;
					}
				}

				if(link)
					linkLine |= (TTTLineFlag)lineItor.Current;
			}

			diffLinkLine = linkLine ^ _oldLinkLine;

			// 回傳新的連線
			return diffLinkLine;
        }

	    /**================================
	     * <summary> 取得新達成的連線 </summary>
	     *===============================*/
		public TTTLineFlag GetNewLine()
        {
			IEnumerator lineItor = Enum.GetValues (typeof(TTTLineFlag)).GetEnumerator ();
			List<int> line;
			TTTData data;
			bool link = true;
			// 有連線的組合
			TTTLineFlag linkLine = 0;
			// 新連線
			TTTLineFlag diffLinkLine;

			for (int i = 0; i < lineList.Length; ++i) 
			{
				// 取得連線組合
				line = lineList[i];
				link = true;

				// 沒有下一條線了
				if(!lineItor.MoveNext())
					break;

				foreach(int idx in line)
				{
					data = grids[idx];

					if(data.status != 1)
					{
						link = false;
						break;
					}
				}

				if(link)
					linkLine |= (TTTLineFlag)lineItor.Current;
			}

			diffLinkLine = linkLine ^ _oldLinkLine;
			_oldLinkLine = linkLine;

			// 回傳新的連線
			return diffLinkLine;
        }
        
	    /**================================
	     * <summary> 找出索引 </summary>
	     *===============================*/
        public int IndexOf(int val)
        {
            for(int i = 0; i < grids.Length; ++i)
            {
                if (null == grids[i])
                    return -1;

                if (grids[i].ID == val)
                    return i;
            }

            return -1;
        }
    }
}