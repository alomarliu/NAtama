using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLNetwork
{
    public class NetworkManager : MonoBehaviour
    {
        public string server_url = "http://ec2-54-241-201-14.us-west-1.compute.amazonaws.com/life_adventure_ccb/clinetTest.php";
        
	    public static NetworkManager instance;

        public delegate void CallBack(object result);
        Dictionary<PackageEnum, CallBack> _callbacks = new Dictionary<PackageEnum, CallBack>();

	    void Awake()
	    {
		    if(instance == null)
		    {			
			    instance = this;
			    GameObject.DontDestroyOnLoad(gameObject);

                // 初始化
                NetworkLib.instance.Init();			
		    }
		    else if(instance != this)
		    {			
			    Destroy(gameObject);
		    }
	    }	        
        
        /**================================
	    * <summary>註冊回呼函式</summary>
        * ===============================*/
        public void RegisterCallBack(PackageEnum comm, CallBack e)
        {
            // 已存在
            if (_callbacks.ContainsKey(comm))
                return;

            _callbacks[comm] = e;

        }
        
        /**================================
	    * <summary>提供外部呼叫的傳送函式</summary>
        * ===============================*/
        public void Send(PackageEnum comm, JsonData data)
        {
            StartCoroutine(SendCommand(comm, data));
        }

        /**================================
	    * <summary>送出指令</summary>
        * ===============================*/
        private IEnumerator SendCommand(PackageEnum comm, JsonData data)
        {
            if (null == data)
                yield break;

            LoadingManager.instance.SetLoading(true);

            // API 名稱
            data["API"] = comm.ToString();

            // 加密
            String encryptData = NLEncrypt.Encrypt(data.ToJson() , NLEncrypt.myKey , NLEncrypt.myIV);

            WWWForm form = new WWWForm();            
            form.AddField("command", encryptData);

            WWW postRequest = new WWW(server_url, form);

            yield return postRequest;
            
            LoadingManager.instance.SetLoading(false);

            Debug.Log(postRequest.text);
            if (!string.IsNullOrEmpty(postRequest.error))
            {
                Error(postRequest.error);
            }
            else
            {
                JsonData d = JsonMapper.ToObject(postRequest.text);

                if (int.Parse(d["res"].ToString()) <= 1)
                    OK(comm, d);
                else
                {
                    switch (d["res"].ToString())
                    {
                        case "2":
                            Debug.LogError("Invalid API");
                            break;
                        case "3":
                            Debug.LogError("Params error");
                            break;
                        case "4":
                            Debug.LogError("Server Maintain");
                            break;
                    }
                }
            }
        }

        /**================================
	    * <summary>正確處理</summary>
        * ===============================*/
        private void OK(PackageEnum comm, JsonData data)
        {
            if (!_callbacks.ContainsKey(comm))
                return;
            
            _callbacks[comm](data);
        }
        
        /**================================
	    * <summary>錯誤處理</summary>
        * ===============================*/
        void Error(string err)
        {            
            Debug.LogWarning(err);
        }
    }
}
