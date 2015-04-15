using UnityEngine;
using System.Collections;
using System;
using LitJson;

namespace NLNetwork
{
    public partial class NetworkLib
    {
        /**================================
	    * <summary>註冊道具相關封包</summary>
        * ===============================*/
        void RegisterItem()
        {
            NetworkManager.instance.RegisterCallBack(PackageEnum.bankReward, RecvGetPayRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.goldToCoin, RecvGold2CoinRs);
        }
        
        /**================================
	    * <summary>傳送領薪水指令</summary>
        * <param name="uid">玩家ID</param>
        * ===============================*/
        public void SendGetPay(string uid)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;

            NetworkManager.instance.Send(PackageEnum.bankReward, jd);
        }
        
        /**================================
	    * <summary>收到創角結果</summary>
        * ===============================*/
        void RecvGetPayRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
                Character.ins.coin = Convert.ToUInt32(jd["data"]["coin"].ToString());
                // server時間
                ServerTime.ins.SetServerTime(Convert.ToInt32(jd["data"]["system_time"].ToString()));
                Character.ins.SetLastGetPayTick(Convert.ToInt64(jd["data"]["bank_update"].ToString()), true);
            }
        }
        
        /**================================
	    * <summary>傳送寶石換金幣指令</summary>
        * <param name="uid">玩家ID</param>
        * <param name="count">要換的數量</param>
        * ===============================*/
        public void SendGold2Coin(string uid, int count)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["num"] = count;

            NetworkManager.instance.Send(PackageEnum.goldToCoin, jd);
        }
        
        /**================================
	    * <summary>收到寶石換金幣結果</summary>
        * ===============================*/
        void RecvGold2CoinRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
            }
        }
    }
}