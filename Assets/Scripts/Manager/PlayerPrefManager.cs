using UnityEngine;
using System.Collections;
using LitJson;

public class PlayerPrefManager : MonoBehaviour 
{    
    /** singleton */
	private static PlayerPrefManager _instance   = 	null;
	
    public enum SettingsName { music = 0, sound, vibrate, conversation } 

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
	 * <summary> 取得每日石頭(讓玩家挑戰用的) </summary>
	 *===============================*/
    public JsonData GetEveryDayNpc()
    {
        string json = PlayerPrefs.GetString("npcInfo", "");

        JsonData jd = JsonMapper.ToObject(json);
        
        return jd;
    }


    /**================================
     * <summary> 保存設定狀態,如果輸入的矩陣長度不是4不執行 </summary>
     *===============================*/
    public void SetSettings(bool[] settings)
    {
        if (settings.Length == 4)
        {
            JsonData jd = new JsonData();
            jd["Music"] = settings[(int)SettingsName.music];
            jd["Sound"] = settings[(int)SettingsName.sound];
            jd["Vibrate"] = settings[(int)SettingsName.vibrate];
            jd["Conversation"] = settings[(int)SettingsName.conversation];

            string json = jd.ToJson();

            PlayerPrefs.SetString("settings", json);
        }
    }

    /**================================
     * <summary> 讀取設定狀態 </summary>
     *===============================*/
    public JsonData GetSetting()
    {
        string json = PlayerPrefs.GetString("settings", "True");

        JsonData jd = JsonMapper.ToObject(json);

        return jd;
    }

    /**================================
     * <summary> 保存音量設定 </summary>
     *===============================*/

    public void SetVolume(float[] volume)
    {
        PlayerPrefs.SetFloat("volMusic", volume[0]);
        PlayerPrefs.SetFloat("volSound", volume[1]);
    }

    /**================================
     * <summary> 讀取音量設定 </summary>
     *===============================*/

    public float[] GetVolume()
    {
        float[] vol = new float[2];

        vol[0] = PlayerPrefs.GetFloat("volMusic", 1);
        vol[1] = PlayerPrefs.GetFloat("volSound", 1);

        return vol;
    }
}
