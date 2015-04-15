using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PackPanel : EditorWindow 
{	
	[MenuItem("PackManager/Control Panel", false, 0)]
	static void ModelCreater()
	{
		EditorWindow.GetWindow(typeof(PackPanel));
	}

	string version = "";

	void OnEnable()
	{
		CreateAssetBundle.LoadABConfig();

		version = CreateAssetBundle.configData.version;
	}	

	void OnDisable()
	{
		CreateAssetBundle.SaveFileVersion();
	}

	void OnGUI()
	{
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("Version", GUILayout.MaxWidth(140));
			version = GUILayout.TextField(version);
			if(GUILayout.Button("Update"))
			{
				CreateAssetBundle.UpdateConfig(version);
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("LoadFileVersion", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Load"))
			{
				CreateAssetBundle.LoadABConfig();
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("PackFont", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Pack"))
			{
				CreateAssetBundle.PackFontAB(false);
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("PackDBF", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Pack"))
			{
				CreateAssetBundle.PackDBFAB();
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("PackSound", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Pack"))
			{
				CreateAssetBundle.PackSoundAB();
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("PackAtlas", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Pack"))
			{
				CreateAssetBundle.PackAtlasAB();
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("PackUI", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Pack"))
			{
				CreateAssetBundle.PackUIAB(null);
			}
		}
		GUILayout.EndHorizontal();
        
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("PackAttachBuilding", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Pack"))
			{
				CreateAssetBundle.PackAttachingBuildingAB(null);
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("PackSelect", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Pack"))
			{
				CreateAssetBundle.PackAssetBunldes();
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("SaveVersion", GUILayout.MaxWidth(140));
			if(GUILayout.Button("Save"))
			{
				CreateAssetBundle.SaveFileVersion();
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}
}
