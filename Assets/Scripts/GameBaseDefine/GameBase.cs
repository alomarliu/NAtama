using System;
using System.Collections;

public sealed class GameBase
{    
	/// 版本號.
	public static string    version			    = "v0.001.0000";

	/** 設定檔的路徑 */
#if CCB
	public static string    configUrl 	        = "https://s3-us-west-1.amazonaws.com/spiritwork-mws/NiceAtama/config.xml";
#else
	public static string    configUrl 	        = "https://s3-us-west-1.amazonaws.com/spiritwork-mws/NiceAtama/config.xml";
#endif

	/** 版本列表的路徑 */
	public static string    versionFileUrl 	    = "https://d3arbmdw1iawyq.cloudfront.net/NiceAtama/version";
	//public static string    versionFilePath 	= "https://mws.spiritworkgame.com/NiceAtama/version";
	/** 主路徑 */
	public static string    rootUrl			    = "https://d3arbmdw1iawyq.cloudfront.net/NiceAtama";    
	//public static string    rootUrl			= "https://mws.spiritworkgame.com/NiceAtama";    
    
	/// <summary>php網址</summary>
    public static string    php                 = "http://d3hfu111t6hek2.cloudfront.net/GoldenBrain/gateway.php";

    public static bool      login               = false;

    // 特定道具編號
    public const int ITEM_COIN = 200001;
    public const int ITEM_GOLD = 200002;
    public const int ITEM_EXP  = 200003;
}