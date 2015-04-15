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
        void RegisterStage()
        {
            NetworkManager.instance.RegisterCallBack(PackageEnum.enterStage, RecvEnterStageRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.cleanStage, RecvCleanStageRs);
        }
        
        /**================================
	    * <summary>傳送進入關卡指令</summary>
        * <param name="uid">玩家ID</param>
        * <param name="stageID">關卡ID</param>
        * <param name="npcID">npcID</param>
        * <param name="mode">模式 1:一般 2:挑戰 3:踢館, 4:奪石</param>
        * ===============================*/
        public void SendEnterStage(string uid, int stageID, int npcID, int mode)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["stage_id"] = stageID;
            jd["npc_id"] = npcID;
            jd["mode"] = mode;

            NetworkManager.instance.Send(PackageEnum.enterStage, jd);
        }
        
        /**================================
	    * <summary>收到進入關卡結果</summary>
        * ===============================*/
        void RecvEnterStageRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
                Debug.Log(jd["data"].ToJson());
                jd = JsonMapper.ToObject(jd["data"].ToJson());
                int stoneID = -1;
                Debug.Log("stage_id: " + jd["stage_id"]);
                Debug.Log("npc_id: " + jd["npc_id"]);
                Debug.Log("mode: " + jd["mode"]);
                Debug.Log("losestoneID: " + (stoneID = int.Parse(jd["bet_stone"].ToString())));

                Character.ins.loseStone = stoneID;
                UIFindTargetController.instance.Battle();
            }
        }

        
        /**================================
	    * <summary>傳送清除關卡指令</summary>
        * <param name="uid">玩家ID</param>
        * <param name="state">0:負 1:勝 2: 平手</param>
        * <param name="coin">獲勝後的錢</param>
        * <param name="stone">獲得的腳色石頭</param>
        * ===============================*/
        public void SendCleanStage(string uid, int state, int coin, int stone)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["state"] = state;
            jd["coin_rwd"] = coin;
            jd["stone_rwd"] = stone;

            NetworkManager.instance.Send(PackageEnum.cleanStage, jd);
        }
        
        /**================================
	    * <summary>收到清除關卡結果</summary>
        * ===============================*/
        void RecvCleanStageRs(object result)
        {
            JsonData jd = result as JsonData;

            if (jd["res"].ToString() == "1")
            {
                // server時間
                ServerTime.ins.SetServerTime(Convert.ToInt32(jd["data"]["system_time"].ToString()));
            }
        }
    }
}