using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneNames : ScriptableObject 
{	
#if UNITY_EDITOR
	[MenuItem("Custom Editor/SceneNameAsset")]
	static void CreateDataAsset()
	{
		
		//資料 Asset 路徑
		string holderAssetPath = "Assets/Resources/";
		
		if(!Directory.Exists(holderAssetPath)) Directory.CreateDirectory(holderAssetPath);
		
		//建立實體
		SceneNames holder = ScriptableObject.CreateInstance<SceneNames> ();
		
		//使用 holder 建立名為 SceneNameSettings.asset 的資源
		AssetDatabase.CreateAsset(holder, holderAssetPath + "SceneNameSettings.asset");
	}
#endif
	/// 初始場景
	public Scenes initScene;
	/// 場景資訊
	public SceneNameHolder[] scenes;
}

[System.Serializable]
public class SceneNameHolder 
{	
	/// 場景名稱
	public Scenes own;
	/// loading的場景名稱
	public LoadingScenes loading;
	/// 是否將loading場景直接掛在目前場景上
	public bool isAdditiveLoading;
}