using UnityEngine;
using System;
using System.Collections;

public class ServerTime : MonoBehaviour 
{
    public static long time = 0;
    static long _lastServerTime = 0;
    static float _beginTime = 0;
	
    /** singleton */
	private static ServerTime 	_instance   = 	null;
    
	public static ServerTime ins
	{
		get
		{
			return _instance;
		}
	}

	void Awake()
    {        
		if(_instance == null)
		{			
			_instance = this;
			GameObject.DontDestroyOnLoad(gameObject);	

            _lastServerTime = time = DateTime.UtcNow.Ticks;
            _beginTime = RealTime.time;		
		}
		else if(_instance != this)
		{			
			Destroy(gameObject);
		}
    }

    void FixedUpdate()
    {
        float timePass = RealTime.time - _beginTime;
        time = _lastServerTime + (long)(timePass * TimeSpan.TicksPerSecond);
    }

    public void SetServerTime(int seconds)
    {
        _lastServerTime = time = ServerTimeToTick(seconds);
        _beginTime = RealTime.time;
    }
    
	/**================================
	 * <summary> 
     * 將SERVER的秒轉換為Tick 
     * </summary>
	 *===============================*/
    public long ServerTimeToTick(int seconds)
    {
        DateTime dt = new DateTime().AddYears(1969);
        long baseTick = dt.Ticks;
        long t = seconds * TimeSpan.TicksPerSecond + baseTick;

        return t;
    }
}
