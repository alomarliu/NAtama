using System;
using System.Collections.Generic;
using UnityEngine;

/**=============================================================
 * <summary> 任務旗標 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2015 All Rights Reserved
 =============================================================*/

public class Flag
{
    private int[] bits = { };
        
    public Flag(int max)
    {
        int count = (int)Math.Ceiling(max / 32f);
        bits = new int[count];
    }

    /**========================================
    * <summary>取得旗標</summary>
    * <param name="bit">第幾個bit, 由0開始</param>
    * ======================================*/
    public bool GetFlag(int bit)
    {
        int idx = (int)Math.Floor(bit / 32f);
        bit = bit % 32;

        if (idx >= bits.Length)
            return false;

        int value = bits[idx];
            
        return (uint)(value & (1 << bit)) > 0;
    }
        
    /**========================================
    * <summary>設定旗標</summary>
    * ======================================*/
    public void SetFlag(int bit, bool on)
    {        
        int idx = (int)Math.Floor(bit / 32f);
        bit = bit % 32;
        
        if (idx >= bits.Length)
            return;

        int value = (int)bits[idx];

        // 開
        if(on)
        {
            value |= (1 << bit);
        }
        // 關
        else
        {
            value ^= (1 << bit);
        }

        bits[idx] = value;
    }
        
    /**========================================
    * <summary>初始</summary>
    * ======================================*/
    public void Init()
    {
        int oldLen = bits.Length;
        bits = new int[oldLen];
    }
}
