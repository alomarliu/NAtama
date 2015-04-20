using UnityEngine;
using System.Collections;
using System;
using LitJson;

namespace NLNetwork
{
    public partial class NetworkLib
    {
        /**================================
	    * <summary>註冊商店相關封包</summary>
        * ===============================*/
        void RegisterShop()
        {
            NetworkManager.instance.RegisterCallBack(PackageEnum.purchaseVerify, RecvSendShopBuyRs);
        }
        
        /**================================
	    * <summary>傳送進入關卡指令</summary>
        * <param name="uid">玩家ID</param>
        * <param name="itemID">商品ID</param>
        * <param name="verifyCode">購買後的驗證碼</param>
        * <param name="verifyType">驗證平台</param>
        * ===============================*/
        public void SendShopBuy(string uid, int itemID, string verifyCode, string verifyType)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["shop_id"] = itemID;
            jd["verify_code"] = "1234214";
            jd["verify_type"] = "1";

            NetworkManager.instance.Send(PackageEnum.purchaseVerify, jd);
        }
        
        /**================================
	    * <summary>收到進入關卡結果</summary>
        * ===============================*/
        void RecvSendShopBuyRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
            }
        }        
    }
}