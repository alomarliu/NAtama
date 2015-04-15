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
        void RegisterCharacter()
        {
            NetworkManager.instance.RegisterCallBack(PackageEnum.createAccount, RecvCreateCharacterRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.searchAccount, RecvInheritCharacterRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.AccountBinding, RecvBindingRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.login, RecvLoginRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.roleStoneFull, RecvNpcLvUpRs);
            NetworkManager.instance.RegisterCallBack(PackageEnum.lvup, RecvLvUpRs);
        }
        
        /**================================
	    * <summary>傳送創角指令</summary>
        * ===============================*/
        public void SendCreateCharacter(string connectID, string source, string schoolID)
        {
            JsonData jd = new JsonData();
            jd["connectId"] = connectID;
            jd["source"] = source;
            jd["school_id"] = schoolID;

            NetworkManager.instance.Send(PackageEnum.createAccount, jd);
        }
        
        /**================================
	    * <summary>收到創角結果</summary>
        * ===============================*/
        void RecvCreateCharacterRs(object result)
        {
            JsonData jd = result as JsonData;

            Debug.Log(jd.ToJson());
            if (jd["res"].ToString() == "1")
            {
                jd = JsonMapper.ToObject(jd["data"].ToJson());
                string uid = jd["uid"].ToString();
                string pwd = jd["pwd"].ToString();

                // 傳送登入指令
                NetworkLib.instance.SendLogin(uid, pwd);

                string fbID = (FB.IsLoggedIn)? FB.UserId : "0";
                PlayerPrefManager.instance.SetLoginInfo(uid, pwd, fbID);
            }
        }
        
        /**================================
	    * <summary>傳送繼承指令</summary>
         * <param name="connectID">串接的帳號</param>
         * <param name="source">串接的來源 0: 未知, 1: 臉書</param>
        * ===============================*/
        public void SendInheritCharacter(string connectID, int source)
        {
            JsonData jd = new JsonData();
            jd["connectId"] = connectID;
            jd["source"] = source;

            NetworkManager.instance.Send(PackageEnum.searchAccount, jd);
        }
        
        /**================================
	    * <summary>收到繼承結果</summary>
        * ===============================*/
        void RecvInheritCharacterRs(object result)
        {            
            JsonData jd = result as JsonData;

            Debug.Log(jd.ToJson());

            // 繼承成功
            if(jd["res"].ToString() == "1")
            {
                jd = JsonMapper.ToObject(jd["data"].ToJson());

                // login然後到主畫面                
                string uid = jd["uid"].ToString();
                string pwd = jd["pwd"].ToString();
                // 傳送登入指令
                NetworkLib.instance.SendLogin(uid, pwd);
                
                string fbID = (FB.IsLoggedIn)? FB.UserId : "0";
                PlayerPrefManager.instance.SetLoginInfo(uid, pwd, fbID);
            }
            // 繼承失敗
            else
            {            
                // 關閉帳號選擇畫面
                UIFBConnectController.instance.Close();        

                // 開啟學校選擇畫面
                UISchoolController.instance.Open();

                // 鎖定滑鼠
                MouseLocker.instance.SetMouseLock(false);            
            }
        }
        
        /**================================
	    * <summary>傳送綁定指令</summary>
        * ===============================*/
        public void SendBinding()
        {
            JsonData jd = new JsonData();

            NetworkManager.instance.Send(PackageEnum.AccountBinding, jd);
        }
        
        /**================================
	    * <summary>收到綁定結果</summary>
        * ===============================*/
        void RecvBindingRs(object result)
        {
            Debug.Log(result);
        }
        
        /**================================
	    * <summary>傳送綁定指令</summary>
        * ===============================*/
        public void SendLogin(string uid, string pwd)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["pwd"] = pwd;
            

                Debug.Log(uid);
                Debug.Log(pwd);
            NetworkManager.instance.Send(PackageEnum.login, jd);
        }
        
        /**================================
	    * <summary>收到綁定結果</summary>
        * ===============================*/
        void RecvLoginRs(object result)
        {
            // 鎖定滑鼠
            MouseLocker.instance.SetMouseLock(false);

            JsonData jd = result as JsonData;

            switch(jd["res"].ToString())
            {
                // 登入成功
                case "1":
                    {
                        Character.ins.SyncFromServer(jd["data"]);
                        // 關閉學校選擇
                        UISchoolController.instance.Close();
                        // 關閉帳號連接畫面
                        UIFBConnectController.instance.Close();
                        // 關啟主畫面
                        UIMainController.instance.Open();
                        GameBase.login = true;
                    }
                    break;
            }
        }
        
        /**================================
	    * <summary>傳送角色升級指令</summary>
        * <param name="npcID">角色ID</param>
        * ===============================*/
        public void SendNpcLvUp(string uid, int npcID)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;
            jd["role_id"] = npcID;

            NetworkManager.instance.Send(PackageEnum.roleStoneFull, jd);
        }
        
        /**================================
	    * <summary>收到綁定結果</summary>
        * ===============================*/
        void RecvNpcLvUpRs(object result)
        {
            JsonData jd = result as JsonData;

            switch(jd["res"].ToString())
            {
                // 登入成功
                case "1":
                    {
                        Debug.Log("npcLvUp");
                    }
                    break;
            }
        }

        /**================================
	    * <summary>傳送玩家升級指令</summary>
        * ===============================*/
        public void SendLvUp(string uid)
        {
            JsonData jd = new JsonData();
            jd["uid"] = uid;

            NetworkManager.instance.Send(PackageEnum.lvup, jd);
        }
        
        /**================================
	    * <summary>收到玩家升級結果</summary>
        * ===============================*/
        void RecvLvUpRs(object result)
        {
            JsonData jd = result as JsonData;

            switch(jd["res"].ToString())
            {
                // 登入成功
                case "1":
                    {
                        Debug.Log("LvUp");
                    }
                    break;
            }
        }
    }
}