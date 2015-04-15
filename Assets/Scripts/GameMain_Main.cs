using LitJson;
using NLNetwork;
using System.Collections;
using UnityEngine;

public class GameMain_Main : MonoBehaviour 
{    
    /** singleton */
    private static GameMain_Main _instance = null;

    public string lastResponse = "";

    public static GameMain_Main instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
            _instance = this;

        NetworkManager.instance.server_url = GameBase.php;

        // FPS
        Application.targetFrameRate = 30;
    }

    void Start()
    {
        if (!GameBase.login)
        {
            JsonData jd = PlayerPrefManager.instance.GetLoginInfo();

            if (jd.Keys.Contains("UID"))
            {
                string uid = jd["UID"].ToString();
                string pwd = jd["PWD"].ToString();
                // 傳送登入封包
                NetworkLib.instance.SendLogin(uid, pwd);
            }
            else
                UIFBConnectController.instance.Open();
        }
        else
        {
            UIMainController.instance.Open();
        }
    }

	void Destroy()
	{
        Resources.UnloadUnusedAssets();
	}    
}
