using UnityEngine;
using System.Collections;
using LitJson;

public class PlayerPrefManager : MonoBehaviour 
{    
    /** singleton */
	private static PlayerPrefManager _instance   = 	null;
	
	void Awake()
    {        
		if(_instance == null)
		{			
			_instance = this;
			GameObject.DontDestroyOnLoad(gameObject);			
		}
		else if(_instance != this)
		{			
			Destroy(gameObject);
		}
    }

	/**=============================================
	 * instance getter
	 * ===========================================*/
	public static PlayerPrefManager instance
	{	
		get
		{
			return _instance;
		}
	}
    
	/**================================
	* <summary> 設定對戰模式 </summary>
    * <param name="stageID">關卡ID</param>
    * <param name="npcID">npcID</param>
	*===============================*/
    public void SetBattleInfo(Enum_BattleMode mode, int stageID, int npcID)
    {        
        JsonData jd = new JsonData();
        jd["mode"] = (int)mode;
        jd["stageID"] = stageID;
        jd["npcID"] = npcID;

        string json = jd.ToJson();

        PlayerPrefs.SetString("BattleInfo", json);
    }
        
	/**================================
	 * <summary> 取得對戰模式 </summary>
	 *===============================*/
    public JsonData GetBattleInfo()
    {
        string json = PlayerPrefs.GetString("BattleInfo", "");
        
        JsonData jd = JsonMapper.ToObject(json);

        return jd;
    }
    
	/**================================
	 * <summary> 設定版號 </summary>
	 *===============================*/
    public void SetVersion(string  ver)
    {
        PlayerPrefs.SetString("version", ver);        
    }
        
	/**================================
	 * <summary> 取得版號 </summary>
	 *===============================*/
    public string GetVersion()
    {
        return PlayerPrefs.GetString("version", "v0.001.0000");
    }
            
	/**================================
	 * <summary> 取得首次登入 </summary>
	 *===============================*/
    public JsonData GetLoginInfo()
    {
        string json = PlayerPrefs.GetString("loginInfo", "");
        
        JsonData jd = JsonMapper.ToObject(json);

        return jd;
    }
    
	/**================================
	 * <summary> 首次登入 </summary>
	 *===============================*/
    public void SetLoginInfo(string uid, string pwd, string fbID = "0")
    {
        JsonData jd = new JsonData();
        jd["UID"] = uid;
        jd["PWD"] = pwd;
        jd["FBID"] = fbID;

        string json = jd.ToJson();

        PlayerPrefs.SetString("loginInfo", json);
    }    
    
	/**================================
	 * <summary> 取得首次登入 </summary>
	 *===============================*/
    public string GetPlayerName()
    {
        string json = PlayerPrefs.GetString("name", "參賽者");

        return json;
    }
    
	/**================================
	 * <summary> 首次登入 </summary>
	 *===============================*/
    public void SetPlayerName(string n)
    {
        PlayerPrefs.SetString("name", n);
    }    
    
	/**================================
	 * <summary> 設定每日石頭(讓玩家挑戰用的) </summary>
	 *===============================*/
    public void SetEveryDayNPC(int[] npcs)
    {        
        JsonData jd = new JsonData();
        jd["Npcs"] = JsonMapper.ToJson(npcs);
        jd["time"] = ServerTime.time;

        string json = jd.ToJson();

        PlayerPrefs.SetString("npcInfo", json);
    }

	/**================================
	 * <summary> 設定每日石頭(讓玩家挑戰用的) </summary>
	 *===============================*/
    public JsonData GetEveryDayNpc()
    {
        string json = PlayerPrefs.GetString("npcInfo", "");

        JsonData jd = JsonMapper.ToObject(json);
        
        return jd;
    }
}
