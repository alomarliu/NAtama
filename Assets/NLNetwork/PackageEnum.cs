using UnityEngine;
using System.Collections;

namespace NLNetwork
{
    public enum PackageEnum
    {
        /// <summary>創角</summary>
        createAccount                    = 0x001  ,        
        /// <summary>繼承</summary>
        searchAccount                   ,        
        /// <summary>帳號綁定</summary>
        AccountBinding                  ,
        /// <summary>登入</summary>
        login                           ,        
        /// <summary>玩家升級</summary>
        lvup                            ,
        /// <summary>角色石滿了升級</summary>
        roleStoneFull                   ,

        //==============================================
        /// 道具相關
        /// <summary>領薪水</summary>
        bankReward                      = 0x100,
        /// <summary>寶石換金幣</summary>
        goldToCoin                      ,
        
        //==============================================
        /// 關卡相關
        /// <summary>進入關卡</summary>
        enterStage                      = 0x200,
        /// <summary>關卡資料清除</summary>
        cleanStage                      ,
        
        //==============================================
        /// Buff相關
        /// <summary>BUFF提升訓練</summary>
        buffUpgrade                     = 0x300,
        /// <summary>Buff 結束訓練</summary>
        buffUpgradeFinish                ,
        
        //==============================================
        /// Npc相關
        /// <summary>更換角色</summary>
        changeRole                      = 0x400,
    }
}
