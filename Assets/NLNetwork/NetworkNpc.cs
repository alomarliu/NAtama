using UnityEngine;
using System.Collections;
using System;
using LitJson;

namespace NLNetwork
{
    public partial class NetworkLib
    {
        /**================================
	    * <summary>註冊角色相關封包</summary>
        * ===============================*/
        void RegisterNpc()
        {
            NetworkManager.instance.RegisterCallBack(PackageEnum.changeRole, RecvChangeNpcRs);
        }
        
        /**================================
	    * <summary>傳送更換角色指令</summary>
        * <param name="npcID"> 角色ID </param>
        * ===============================*/
        public void SendChangeNpc(string uid, int npcID)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["role_id"] = npcID;

            NetworkManager.instance.Send(PackageEnum.changeRole, jd);
        }
        
        /**================================
	    * <summary>收到更換角色結果</summary>
        * ===============================*/
        void RecvChangeNpcRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
            }
        }
    }
}