using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class AssetBundleConfig : ScriptableObject
{	
	[MenuItem("PackManager/Create ABConfig")]
	static void CreateABConfig()
	{		
		//資料 Asset 路徑
		string holderAssetPath = "Assets/PackManager/Config/";


		if(!Directory.Exists(holderAssetPath)) Directory.CreateDirectory(holderAssetPath);
		
		//建立實體
		AssetBundleConfig holder = ScriptableObject.CreateInstance<AssetBundleConfig> ();
		
		//使用 holder 建立名為 PunishmentData.asset 的資源
		AssetDatabase.CreateAsset(holder, holderAssetPath + "ABConfig.asset");
	}

	public ConfigData configData;
}

[System.Serializable]
public class ConfigData
{
	public string version			= "v0.001.0001";

	public string targetDir			= "PackFolder";
	public string versionFileName	= "version";
	/** 要打包的DBF路徑 */
	public string DBFDir 			= "DBF";

	/** 要打包的字型路徑 */
	public string fontDir 			= "Fonts";

	/** 要打包的音樂路徑 */
	public string soundDir			= "Sounds";
	
	/** 要打包的貼圖集路徑 */
	public string atlasDir	 		= "Atlas";
	
	/** 要打包的介面路徑 */
	public string UIDir				= "UIPrefabs";
    
	/** 要打包的Sprite路徑 */
	public string SpriteDir 		= "Sprites";

	/** 要打包的路徑 */
	public string AttachBuildingDir	= "AttachBuildings";

    public ConfigData Clone()
    {
        return this.MemberwiseClone() as ConfigData;
    }
}
