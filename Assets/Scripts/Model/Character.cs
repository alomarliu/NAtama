using LitJson;
using Model.DBF;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : MonoBehaviour 
{    
	/** singleton */
	private static Character 	_instance  		= 	null;
    
    /// <summary>玩家名字</summary>
    public string playName = "司徒子豪";
    public string uid = "";
    /// <summary>等級</summary>
    public int lv = 1;
    /// <summary>經驗值</summary>
    private uint _exp = 0;
    /// <summary>經驗值</summary>
    private uint _maxExp = 50;
    
    /// <summary>經驗值百分比</summary>
    public float expPer = 0f;
    
    /// <summary>選取的角色</summary>
    public int charID = 1;
    
    /// <summary>總回合數</summary>
    public int totalRound = 0;
    /// <summary>贏的回合數</summary>
    public int winRound = 0;
    /// <summary>生命力</summary>
    public int hp = 0;
    /// <summary>學校解鎖等級</summary>
    public int schoolLv = 3;
    /// <summary>勝率</summary>
    public int winRate = 0;    
    /// <summary>累積獎金</summary>
    public int totalBonus = 0;    
    /// <summary>答對題數</summary>
    public int correctCount = 0;
    /// <summary>學校ID</summary>
    private int _schoolID = 0;
    /// <summary>學校名稱</summary>
    [SerializeField]
    private string _schoolName = "";
    
    /// <summary>每日可挑戰次數</summary>
    public int challengeCount = 5;
    
    /// <summary>金幣</summary>
    public uint coin = 0;
    /// <summary>寶石</summary>
    public uint gold = 0;
    
    /// <summary>奪到的石頭</summary>
    public int winStone = -1;
    public int loseStone = -1;

    public int[] everyDayNpcs = { };

    long _lastGetPayTick = 0;
    long _baseTick = 0;
    
    /// <summary>初始能力</summary>
    public Ability abilityB = new Ability();
    /// <summary>裝備BUFF能力</summary>
    public Ability abilityI = new Ability();
    /// <summary>最終能力</summary>
    public Ability abilityF = new Ability();

	public static Character ins
	{
		get
		{
			return _instance;
		}
	}
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            GameObject.DontDestroyOnLoad(gameObject);

            // C# 是從0001年起算, 將時間位移到1970年
            DateTime dt = new DateTime().AddYears(1969);
            _baseTick = dt.Ticks;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

	/**================================
	 * <summary> 
     * schoolName getter 
     * </summary>
	 *===============================*/
    public string schoolName
    {
        get
        {
            return _schoolName;
        }
    }

	/**================================
	 * <summary> 
     * schoolID getter setter 
     * </summary>
	 *===============================*/
    public int schoolID
    {
        get
        {
            return _schoolID;
        }
        set
        {
            SchoolLib obj = DBFManager.schoolLib.Data(value) as SchoolLib;

            if(null == obj)
                return;

            _schoolID = value;
            _schoolName = obj.Name;
        }
    }

	/**================================
	 * <summary> 同步server資料 </summary>
	 *===============================*/
    public void SyncFromServer(JsonData jd)
    {
        if (!jd.Keys.Contains("accountData"))
            return;

        PlayerPrefs.DeleteKey("npcInfo");
        // server時間
        ServerTime.ins.SetServerTime(Convert.ToInt32(jd["system_time"].ToString()));

        // 亂數種子
        UnityEngine.Random.seed = Convert.ToInt32(jd["system_time"].ToString());

        // 隨機產生石頭列表
        RandEveryDayStone();

        playName = PlayerPrefManager.instance.GetPlayerName();

        JsonData accountJD = jd["accountData"];

        uid = accountJD["uid"].ToString();
        coin = Convert.ToUInt32(accountJD["coin"].ToString());
        gold = Convert.ToUInt32(accountJD["gold"].ToString());        
        lv = Convert.ToInt32(accountJD["lv"].ToString());
        _exp = Convert.ToUInt32(accountJD["exp"].ToString());

        RoleConfigLib roleObj = DBFManager.roleConfigLib.Data(lv) as RoleConfigLib;
        // 取得最大經驗值
        if (null != roleObj)
        {
            _maxExp = Convert.ToUInt32(roleObj.Exp);
            hp = roleObj.HP;
        }

        expPer = (float)_exp / _maxExp;
        schoolID = Convert.ToInt32(accountJD["school_id"].ToString());
        schoolLv = Convert.ToInt32(accountJD["stage_count"].ToString());
        SetLastGetPayTick(Convert.ToInt64(accountJD["bank_update"].ToString()), true);

        // Buff處理
        JsonData buffAry = JsonMapper.ToObject<JsonData>(accountJD["buff_box"].ToJson());
        //JsonData buffAry = JsonMapper.ToObject<JsonData>("[{\"id\":4,\"lv\":1},{\"id\":1,\"lv\":13},{\"id\":2,\"lv\":1}]");
        //JsonData buffAry = JsonMapper.ToObject<JsonData>("[{\"id\":4,\"lv\":1}]");
        int id = 0;
        int level = 0;
        for (int i = 0; i < buffAry.Count; ++i)
        {
            id = int.Parse(buffAry[i]["id"].ToString());
            level = int.Parse(buffAry[i]["lv"].ToString());
            _buffDic.Add(id, new BuffInfo() { buffID = id * 100 + level, lv = level });
        }

        // 學習的ID
        learningBuffID = Convert.ToInt32(accountJD["buff_learning"].ToString());
        // 設定上次開始學習時間
        SetLastLearnBuffTick(Convert.ToInt64(accountJD["buff_learning_time"].ToString()), true);
       // learningBuffID = 1;
       // _lastLearnBuffTick = ServerTime.time;
            
        // 石頭處理
        object[] stoneAry = JsonMapper.ToObject<object[]>(accountJD["role_stone"].ToJson());
        for (int i = 0; i < stoneAry.Length; ++i )
        {
            GiveItem(int.Parse(stoneAry[i].ToString()), 1);
        }

        // 所有角色
        JsonData roleAry = JsonMapper.ToObject<JsonData>(jd["userRole"].ToJson());
        NpcInfo npcInfo = null;
        for (int i = 0; i < roleAry.Count; ++i )
        {
            npcInfo = new NpcInfo() { roleID = int.Parse(roleAry[i]["role_id"].ToString()),
                                      lv = int.Parse(roleAry[i]["lv"].ToString())};

            // 加入
            if(!_npcDic.ContainsKey(npcInfo.roleID))
                _npcDic.Add(npcInfo.roleID, npcInfo);
        }

        NpcLevelUP();

        // 計算能力
        Calculate();
    }

    /**================================
     * <summary> 隨機產生石頭列表 </summary>
     *===============================*/
    private void RandEveryDayStone()
    {
        JsonData jd = PlayerPrefManager.instance.GetEveryDayNpc();

        long time = 0;
        bool rand = false;
        List<int> list = new List<int>();

        if (jd.Keys.Contains("time"))
        {
            time = Convert.ToInt64(jd["time"].ToString());

            DateTime serverD = new DateTime(ServerTime.time);
            DateTime lastD = new DateTime(time);

            if (serverD.Day != lastD.Day)
                rand = true;
        }
        else
            rand = true;

        if(rand)
        {
            ChallengeStageLib obj = null;
            int npcID = -1;

            while(DBFManager.challengeStageLib.MoveNext())
            {
                obj = DBFManager.challengeStageLib.Current as ChallengeStageLib;

                npcID = RandStageNpc(obj.GUID);

                if (npcID > 0)
                    list.Add(npcID);
            }

            everyDayNpcs = list.ToArray();
            // 寫入暫存
            PlayerPrefManager.instance.SetEveryDayNPC(everyDayNpcs);
        }
    }

    public int RandStageNpc(int stageID)
    {        
        ChallengeStageLib obj = DBFManager.challengeStageLib.Data(stageID) as ChallengeStageLib;

        if (obj == null)
            return -1;

        int npcID = -1;
        int sum = 0;
        int num = UnityEngine.Random.Range(0, 199) + 1;

        for(int i = 0; i < obj.Appear.Length; ++i)
        {
            sum += obj.Appear[i].Weight;

            if(num <= sum)
            {
                npcID = obj.Appear[i].NpcID;
                break;
            }
        }

        // 隨便給的, 因為企劃表還沒填
        if (npcID == -1)
            npcID = obj.Appear[0].NpcID;

        return npcID;
    }
    
	/**================================
	 * <summary> 是否可挑戰 </summary>
	 *===============================*/
    public bool CheckChallenge()
    {
        if (challengeCount <= 0)
            return false;

        return true;
    }
    
	/**================================
	 * <summary> 
     * 最後領薪水的時間 
     * </summary>
	 *===============================*/
    public long GetLastPayTick()
    {
        return _lastGetPayTick;
    }

	/**================================
	 * <summary> 
     * 設定最後領薪水的時間 
     * </summary>
	 *===============================*/
    public void SetLastGetPayTick(long value, bool fromServer = false)
    {        
        _lastGetPayTick = value;

        if (fromServer)
        { 
            _lastGetPayTick *= TimeSpan.TicksPerSecond;
            _lastGetPayTick += _baseTick;
        }
    }
    
	/**================================
	 * <summary> 
     * 加exp 
     * </summary>
	 *===============================*/
    public void AddExp(int val)
    {
        ConfigLib configObj = DBFManager.configLib.Data(1) as ConfigLib;

        if (null == configObj || lv >= configObj.MaxLv)
            return;

        _exp += Convert.ToUInt32(val);

        expPer = (float)_exp / _maxExp;
    }
    
	/**================================
	 * <summary> 
     * 升級 
     * </summary>
	 *===============================*/
    public bool LevelUp()
    {
        // 經驗值未滿足
        if (_exp < _maxExp)
            return false;

        RoleConfigLib roleObj = null;

        if (_exp >= _maxExp)
        {
            lv++;            
            _exp -= _maxExp;

            roleObj = DBFManager.roleConfigLib.Data(lv) as RoleConfigLib;
            
            // 取得最大經驗值
            if (null != roleObj)
            {
                _maxExp = Convert.ToUInt32(roleObj.Exp);
                _maxExp = Math.Max(1, _maxExp);
            }
            else
            {
                // 取樣版最後一筆
                roleObj = DBFManager.roleConfigLib.Last as RoleConfigLib;                
                _maxExp = Convert.ToUInt32(roleObj.Exp);
            }
            
            expPer = (float)_exp / _maxExp;            
            hp = roleObj.HP;

            // 解鎖Buff
            UnLockBuff();

            return true;
        }

        return false;
    }

    public uint exp
    {
        get
        {
            return _exp;
        }
    }
    
	/**================================
	 * <summary> 
     * 檢查學力可否升級
     * </summary>
	 *===============================*/
    public bool CheckSchoolLvUp()
    {
        int count = DBFManager.challengeStageLib.Count;

        return (schoolLv < count);
    }
    
	/**================================
	 * <summary> 
     * 學力升級
     * </summary>
	 *===============================*/
    public void SchoolLvUp()
    {
        if (!CheckSchoolLvUp())
            return;

        schoolLv++;
    }
    
	/**================================
	 * <summary> 計算能力 </summary>
	 *===============================*/
    public void Calculate()
    {
        // 計算角色BUFF能力
        CalculateI();
        // 計算最終能力
        CalculateF();
    }
    
	/**================================
	 * <summary> 計算角色BUFF能力 </summary>
	 *===============================*/
    private void CalculateI()
    {
        // 威力加成
        BuffInfo buffInfo = GetBuff((int)BuffTabelLib.BuffType.Type1);
        BuffEffectLib obj = null;

        if(null != buffInfo)
        {
            obj = DBFManager.buffEffectLib.Data(buffInfo.buffID) as BuffEffectLib;

            if(null != obj)
                abilityI.PowerPer = obj.BuffValue;
        }

        // 連擊金幣加成
        buffInfo = GetBuff((int)BuffTabelLib.BuffType.Type2);

        if(null != buffInfo)
        {
            obj = DBFManager.buffEffectLib.Data(buffInfo.buffID) as BuffEffectLib;

            if(null != obj)
                abilityI.BonusPer = obj.BuffValue;
        }

        // 經驗值加成
        buffInfo = GetBuff((int)BuffTabelLib.BuffType.Type3);

        if(null != buffInfo)
        {
            obj = DBFManager.buffEffectLib.Data(buffInfo.buffID) as BuffEffectLib;

            if(null != obj)
                abilityI.ExpPer = obj.BuffValue;
        }
    }

	/**================================
	 * <summary> 計算最終能力 </summary>
	 *===============================*/
    private void CalculateF()
    {
        abilityF.PowerPer = abilityB.PowerPer + abilityI.PowerPer;
        abilityF.BonusPer = abilityB.BonusPer + abilityI.BonusPer;
        abilityF.ExpPer = abilityB.ExpPer + abilityI.ExpPer;
    }
    
}
