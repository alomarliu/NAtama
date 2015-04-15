using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;
using System.Text;
using System;

public class CreateAssetBundle : MonoBehaviour 
{

	/** 放各類版本文件的List */
	public static List<Dictionary<string, string>> dicList = new List<Dictionary<string, string>>();

	static string soundExt = 	".sound";
	static string DBFExt = 	    ".dbf";
	static string fontExt = 	".font";
	static string UIExt = 		".ui";
	static string atlasExt = 	".atlas";
	static string spriteExt = 	".sp";
	static string objectExt = 	".ab";
    static string rootDir       = "";

	public static ConfigData configData     = null;
	static AssetBundleConfig config         = null;

	/**=======================
	 * 載入設定檔
	 *======================*/
	public static void LoadABConfig()
	{
		if(config == null)
		{
			UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath("Assets/PackManager/Config/ABConfig.asset", typeof(UnityEngine.Object));
			config = (asset as AssetBundleConfig);
		}

        configData = config.configData;
		UpdateConfig(configData.version);

		LoadFileVersion();
	}
	
	/**=======================
	 * 更新設定檔
	 *======================*/
	public static void UpdateConfig(string verstion)
    {
		AssetDatabase.SaveAssets();
		EditorUtility.SetDirty(config);

        configData.version = verstion;
        rootDir = string.Format("{0}/{1}/", configData.targetDir, verstion);
	}
    
	/**=======================
	 * 載入打包版本檔案
	 *======================*/
	static void LoadFileVersion()
	{ 
		dicList.Clear();
		
		for(int i = 0; i < (int)PackType.MAX; ++i)
		{
			dicList.Add(new Dictionary<string, string>());
		}
		
		string path = "";

		//if(EditorGUIUtility.isProSkin)
			path = configData.targetDir + "/" + configData.versionFileName;
		//else
		//	path = configData.targetDir + "/" + configData.versionFileName + ".txt";
		
		if(!File.Exists(path))
		{
			Debug.LogError("file not exists.");
			return;
		}

		path = "file:///" + System.IO.Path.GetFullPath(path);

		WWW www = new WWW(path);

		if(www.error != null)
		{
			Debug.LogError(www.error);
			return;
		}

		string text = "";
		
		TextAsset tAsset = www.assetBundle.Load("version") as TextAsset;
		text = tAsset.text;

		JsonReader  customReader;
		customReader  = new JsonReader(text);
		
		customReader.AllowComments            = false;
		customReader.AllowSingleQuotedStrings = false;

		JsonData js = JsonMapper.ToObject(customReader);
		Dictionary<string, string> dic;

		for(int i = 0; i < (int)PackType.MAX; ++i)
		{
			text = js[i.ToString()].ToString();	
			dic = JsonMapper.ToObject<Dictionary<string, string>>(text);
			dicList[i] = dic;
			
			foreach(KeyValuePair<string, string> l in dic)
			{
				Debug.Log("key: " + l.Key + ", value: " + l.Value);
			}
		}

		Debug.Log("count: " + js["totalCount"]);

		if(www.assetBundle != null)
			www.assetBundle.Unload(true);
	}
	
	/**=======================
	 * 儲存打包版本檔案
	 *======================*/
	static public void SaveFileVersion()
	{
		// 目錄不存在
		if(!Directory.Exists(rootDir))
			return;

		string txtFileName = "Assets/" + configData.versionFileName + ".txt";	
		JsonData jd = new JsonData ();			

		Dictionary<string, string> dic = null;
		string json = "";
		int totalCount = 0;

		for(int i = 0; i < (int)PackType.MAX; ++i)
		{
			if(i >= dicList.Count)
				break;

			dic = dicList[i];
			json = JsonMapper.ToJson(dic);
			jd[i.ToString()] = json;
			totalCount += dic.Count;
		}
        
		jd["nowVer"] = configData.version;
		jd["totalCount"] = totalCount;
		json = jd.ToJson ();

		File.WriteAllText(txtFileName, json);
		
		AssetDatabase.Refresh();

		TextAsset tAsset = AssetDatabase.LoadAssetAtPath(txtFileName, typeof(TextAsset)) as TextAsset;

        string path = rootDir + configData.versionFileName;

		if(EditorGUIUtility.isProSkin)
			BuildPipeline.BuildAssetBundle(tAsset, null, path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);

		string path2 = "";

		// pro版
		if (EditorGUIUtility.isProSkin) 
		{
			path2 = configData.targetDir + "/"+ configData.versionFileName;
		}
		else 
		{
			path = txtFileName;
			path2 = rootDir + "/" + configData.versionFileName + ".txt";
		}

        AssetDatabase.DeleteAsset(txtFileName);
        
		// 複製
		File.Copy(path, path2, true);

		AssetDatabase.Refresh();
	}
	
	/**=======================
	 * 打包選擇的檔案
	 *======================*/
	public static void PackAssetBunldes()
	{	
		// AssetBundle 的資料夾名稱及副檔名
		string targetDir = "";
		string extensionName = ".assetbundle";
		
		//取得在 Project 視窗中選擇的資源(包含資料夾的子目錄中的資源)
		UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof (UnityEngine.Object), SelectionMode.DeepAssets);
		string path = "";
		PackType type;
		List<string> uiList = new List<string>();

		BuildAssetBundleOptions option = BuildAssetBundleOptions.DeterministicAssetBundle;
		
		foreach(UnityEngine.Object obj in SelectedAsset)
		{			
			path = AssetDatabase.GetAssetPath(obj);

			if(path.Contains("Assets/" + configData.DBFDir))
			{
				targetDir = rootDir + configData.DBFDir;
				extensionName = DBFExt;
				type = PackType.DBF;
			}
			else if(path.Contains(configData.atlasDir))
			{
				targetDir = rootDir + configData.atlasDir;
				extensionName = atlasExt;
				type = PackType.Atlas;
			}
			else if(path.Contains("Assets/" + configData.fontDir))
			{
				targetDir = rootDir + configData.fontDir;
				extensionName = fontExt;
				type = PackType.MyFont;
			}
			else if(path.Contains("Assets/" + configData.UIDir))
			{
				targetDir = rootDir + configData.UIDir;
				extensionName = UIExt;
				type = PackType.UI;
				uiList.Add(AssetDatabase.AssetPathToGUID(path));
				continue;
			}
			else if(path.Contains("Assets/" + configData.soundDir))
			{
				targetDir = rootDir + configData.soundDir;
				extensionName = soundExt;
				type = PackType.Sound;
			}
			else 
				continue;
			
			//建立存放 AssetBundle 的資料夾
			if(!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
			// AssetBundle 儲存檔案路徑
			string targetPath = targetDir + Path.DirectorySeparatorChar + obj.name + extensionName;
			
			if(File.Exists(targetPath)) File.Delete(targetPath);
			
			if(!(obj is UnityEngine.Object) && !(obj is Texture2D) && !(obj is Material)) continue;

			//建立 AssetBundle
			if(BuildPipeline.BuildAssetBundle(obj, null, targetPath, option,BuildTarget.Android))
			{				
				Debug.Log(obj.name + " 建立完成");		

				path = Path.ChangeExtension(path, extensionName);
				UpdateVersionList(type, path, configData.version);
			}
			else
			{				
				Debug.Log(obj.name + " 建立失敗");
			}
		}

		// 打包選擇的UI
		if(uiList.Count > 0)
			PackUIAB(uiList.ToArray());
		
		AssetDatabase.Refresh();
	}
	
	/**=======================
	 * 打包DBF
	 *======================*/
	static public void PackDBFAB()
	{
		string[] paths = {"Assets/" + configData.DBFDir};
		string[] guids = AssetDatabase.FindAssets("t:TextAsset", paths);
		string path;
		string targetDir = rootDir + configData.DBFDir;
		// AssetBundle 儲存檔案路徑
		string targetPath = "";
        string extensionName = DBFExt;
		TextAsset asset;
		List<TextAsset> list = new List<TextAsset>();
		
		//建立存放 AssetBundle 的資料夾
		if(!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
		
		foreach(string id in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(id);
			asset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			
			list.Add(asset);
		}
		
		targetPath = targetDir + Path.DirectorySeparatorChar + "GameData" + extensionName;
		
		if(File.Exists(targetPath)) File.Delete(targetPath);
		
		BuildPipeline.BuildAssetBundle(null, list.ToArray(), targetPath, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
		
		string fileName = "Assets/"  + configData.DBFDir + "/GameData" + extensionName;
		UpdateVersionList(PackType.DBF, fileName, configData.version);
		
		AssetDatabase.Refresh();
	}
	
	/**=======================
	 * 打包音樂
	 *======================*/
	static public void PackSoundAB()
	{
		string[] paths = {"Assets/" + configData.soundDir};
		string[] guids = AssetDatabase.FindAssets("t:AudioClip", paths);
		string path;
		string targetDir = rootDir + configData.soundDir;
		// AssetBundle 儲存檔案路徑
		string targetPath = "";
		AudioClip asset;
		
		//建立存放 AssetBundle 的資料夾
		if(!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
		
		foreach(string id in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(id);
			asset = AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip;
			
			targetPath = targetDir + Path.DirectorySeparatorChar + asset.name + soundExt;
			
			BuildPipeline.BuildAssetBundle(asset, null, targetPath, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
			
			path = Path.ChangeExtension(path, soundExt);
			UpdateVersionList(PackType.Sound, path, configData.version);
		}

		AssetDatabase.Refresh();
	}
		
	/**=======================
	 * 打包圖集
	 *======================*/
	static public void PackAtlasAB()
	{
		string[] paths = {"Assets/" + configData.atlasDir};
		string[] guids = AssetDatabase.FindAssets("t:GameObject", paths);
		string path;
		string targetDir = "Assets/" + configData.atlasDir;
		// AssetBundle 儲存檔案路徑
		string targetPath = "";
		GameObject asset;
		BuildAssetBundleOptions option = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle;

		//建立存放 AssetBundle 的資料夾
		if(!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

		foreach(string id in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(id);
			asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			
			targetPath = targetDir + Path.DirectorySeparatorChar + asset.name + atlasExt;
			
			BuildPipeline.BuildAssetBundle(asset, null, targetPath, option, BuildTarget.Android);

			path = Path.ChangeExtension(path, atlasExt);
			UpdateVersionList(PackType.Atlas, path, configData.version);
		}
		
		AssetDatabase.Refresh();
	}
	
	/**=======================
	 * 打包字型
	 * <param> 是否依賴 </param>
	 *======================*/
	static public int PackFontAB(bool dependencies)
	{
		string[] paths = {"Assets/" + configData.fontDir};
		string[] guids = AssetDatabase.FindAssets("t:GameObject", paths);
		string path;
		string targetDir = rootDir + configData.fontDir;
		// AssetBundle 儲存檔案路徑
		string targetPath = "";
		GameObject asset;
		BuildAssetBundleOptions option = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle;
		
		//建立存放 AssetBundle 的資料夾
		if(!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
		
		foreach(string id in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(id);
			asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			
			targetPath = targetDir + Path.DirectorySeparatorChar + asset.name + fontExt;

            if (dependencies)
                BuildPipeline.PushAssetDependencies();

			BuildPipeline.BuildAssetBundle(asset, null, targetPath, option, BuildTarget.Android);

            if (!dependencies)
            {
                path = Path.ChangeExtension(path, fontExt);
                UpdateVersionList(PackType.MyFont, path, configData.version);
            }
		}

        AssetDatabase.Refresh();

		return guids.Length;
	}
	
	/**=======================
	 * 打包UI
	 *======================*/	
	public static void PackUIAB(string[] guids)
	{		
		string[] paths = {"Assets/" + configData.UIDir};

		if(guids == null)
			guids = AssetDatabase.FindAssets("t:GameObject", paths);

		string path;
		//UnityEngine.Object[] assets;
		UnityEngine.Object asset;
		string targetDir = rootDir + configData.UIDir;
		// AssetBundle 儲存檔案路徑
		string targetPath = "";
		string extensionName = ".assetbundle";
		int count = 0;
		
		// 打包字型
		int fontCount = PackFontAB(true);
		
		//建立存放 AssetBundle 的資料夾
		if(!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

		List<string> alreayPack = new List<string>();
		PackType type;
		string tempP = "";

		foreach(string id in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(id);
			paths = AssetDatabase.GetDependencies(new string[]{path});
			
			count = 0;
			
			// 打包依賴的音效及圖集
			foreach(string p in paths)
			{	
				if(p != path && (p.Contains("Assets/" + configData.soundDir) || p.Contains("Assets/" + configData.atlasDir)))
				{
					if(alreayPack.Contains(p))
						continue;
					
					// 加入已打包清單
					alreayPack.Add(p);

					asset = AssetDatabase.LoadMainAssetAtPath(p);
					
					if(asset is AudioClip)
					{
						targetPath = rootDir + configData.soundDir;
						type = PackType.Sound;
						extensionName = soundExt;
					}
					else if(asset is GameObject)
					{
						targetPath = rootDir + configData.atlasDir;
						type = PackType.Atlas;
						extensionName = atlasExt;
					}
					else 
						continue;
					
					//建立存放 AssetBundle 的資料夾
					if(!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
					
					targetPath += Path.DirectorySeparatorChar + asset.name + extensionName;

					if(File.Exists(targetPath)) 
						File.Delete(targetPath);

					count++;
					
					BuildPipeline.PushAssetDependencies();
					BuildPipeline.BuildAssetBundle(asset, null, targetPath, BuildAssetBundleOptions.CollectDependencies | 
                                                                            BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);	

					// 更換副檔名
					tempP = Path.ChangeExtension(p, extensionName);
					UpdateVersionList(type, tempP, configData.version);
				}
			}
			
			asset = AssetDatabase.LoadMainAssetAtPath(path);
			targetPath = targetDir + Path.DirectorySeparatorChar + asset.name + UIExt;
			
			if(File.Exists(targetPath)) File.Delete(targetPath);
			
			BuildPipeline.PushAssetDependencies();
			BuildPipeline.BuildAssetBundle(asset, null, targetPath, BuildAssetBundleOptions.CollectDependencies | 
                                                                    BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);	
		    BuildPipeline.PopAssetDependencies();

			// 更換副檔名
			path = Path.ChangeExtension(path, UIExt);
			UpdateVersionList(PackType.UI, path, configData.version);
		}
		
		for(int i = 0; i < count+fontCount; ++i)
		{
			BuildPipeline.PopAssetDependencies();
		}
		
		AssetDatabase.Refresh();
	}

    
	/**=======================
	 * 打包AttachingBuilding
	 *======================*/	
	public static void PackAttachingBuildingAB(string[] guids)
	{		
		string[] paths = {"Assets/" + configData.AttachBuildingDir};

		if(guids == null)
			guids = AssetDatabase.FindAssets("t:GameObject", paths);

		string path;
		UnityEngine.Object asset;
		string targetDir = rootDir + configData.AttachBuildingDir;
		// AssetBundle 儲存檔案路徑
		string targetPath = "";
		string extensionName = ".assetbundle";
		int count = 0;
		
		//建立存放 AssetBundle 的資料夾
		if(!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

		List<string> alreayPack = new List<string>();
		PackType type;
		string tempP = "";

		foreach(string id in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(id);
			paths = AssetDatabase.GetDependencies(new string[]{path});
			
			count = 0;
			
			// 打包依賴的音效及圖集
			foreach(string p in paths)
			{	
				if(p != path && p.Contains("Assets/" + configData.SpriteDir))
				{
					if(alreayPack.Contains(p))
						continue;
					
					// 加入已打包清單
					alreayPack.Add(p);

					asset = AssetDatabase.LoadMainAssetAtPath(p);
					
					if(asset is Texture2D)
					{
						targetPath =  rootDir + configData.SpriteDir;
						type = PackType.Sprite;
						extensionName = spriteExt;
					}
					else 
						continue;
					
					//建立存放 AssetBundle 的資料夾
					if(!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
					
					targetPath += Path.DirectorySeparatorChar + asset.name + extensionName;

					if(File.Exists(targetPath)) 
						File.Delete(targetPath);

					count++;
					
					BuildPipeline.PushAssetDependencies();
					BuildPipeline.BuildAssetBundle(asset, null, targetPath, BuildAssetBundleOptions.CollectDependencies | 
                                                                            BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);	

					// 更換副檔名
					tempP = Path.ChangeExtension(p, extensionName);
					UpdateVersionList(type, tempP, configData.version);
				}
			}
			
			asset = AssetDatabase.LoadMainAssetAtPath(path);
			targetPath = targetDir + Path.DirectorySeparatorChar + asset.name + objectExt;
			
			if(File.Exists(targetPath)) File.Delete(targetPath);
			
			BuildPipeline.PushAssetDependencies();
			BuildPipeline.BuildAssetBundle(asset, null, targetPath, BuildAssetBundleOptions.CollectDependencies | 
                                                                    BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);	
		    BuildPipeline.PopAssetDependencies();

			// 更換副檔名
			path = Path.ChangeExtension(path, objectExt);
			UpdateVersionList(PackType.AttachBuilding, path, configData.version);
		}
		
		for(int i = 0; i < count; ++i)
		{
			BuildPipeline.PopAssetDependencies();
		}
		
		AssetDatabase.Refresh();
	}

	/**=======================
	 * 更新版本列表
	 *======================*/
	static void UpdateVersionList(PackType type, string filePath, string version)
	{
		if((int)type >= dicList.Count)
			return;

		Dictionary<string, string> dic;
		dic = dicList[(int)type];

		if(dic.ContainsKey(filePath))
			dic[filePath] = version;
		else
			dic.Add(filePath, version);
	}
}
