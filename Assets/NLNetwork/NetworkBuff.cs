using UnityEngine;
using System.Collections;
using System;
using LitJson;

namespace NLNetwork
{
    public partial class NetworkLib
    {
        /**================================
	    * <summary>註冊關卡相關封包</summary>
        * ===============================*/
        void RegisterBuff()
        {
            NetworkManager.instance.RegisterCallBack(PackageEnum.buffUpgrade, RecvBuffLvUpRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.buffUpgrade, RecvBuffLvUpFinishRs);
        }
        
        /**================================
	    * <summary>傳送Buff升級指令</summary>
        * <param name="uid">玩家ID</param>
        * <param name="buffType">Buff類型</param>
        * ===============================*/
        public void SendBuffLvUp(string uid, int buffType)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["buff_type"] = buffType;

            NetworkManager.instance.Send(PackageEnum.buffUpgrade, jd);
        }
        
        /**================================
	    * <summary>收到Buff升級結果</summary>
        * ===============================*/
        void RecvBuffLvUpRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
            }
        }

        /**================================
	    * <summary>傳送Buff升級結束指令</summary>
        * <param name="uid">玩家ID</param>
        * <param name="pay">付費</param>
        * ===============================*/
        public void SendBuffLvUpFinish(string uid, bool pay)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["pay_flag"] = (!pay)? 0 : 1;

            NetworkManager.instance.Send(PackageEnum.buffUpgradeFinish, jd);
        }
        
        /**================================
	    * <summary>收到Buff升級結果</summary>
        * ===============================*/
        void RecvBuffLvUpFinishRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
            }
        }
    }
}