using System;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;

public class GameMain_Start : MonoBehaviour 
{
    bool _checking = false;

    void Start()
    {
        GameBase.version = PlayerPrefManager.instance.GetVersion();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(DownloadVerFile());
        }
    }

    IEnumerator DownloadVerFile()
    {
        _checking = true;

		// 下載設定檔
		WWW www = new WWW(GameBase.configUrl);
		
		yield return www;		
        
		if(www.error != null)
		{
			Debug.LogError(www.error);
            _checking = false;
			yield break;
		}
        
        XmlDocument xd = new XmlDocument();
        StringReader reader = new StringReader(www.text);
        reader.Read(); // skip BOM
        xd.Load(reader);

        string ver = xd.SelectSingleNode("data/common/version").InnerText;
        GameBase.versionFileUrl = xd.SelectSingleNode("data/connect/verFile").InnerText;
        GameBase.php = xd.SelectSingleNode("data/connect/php").InnerText;
        GameBase.rootUrl = xd.SelectSingleNode("data/connect/url").InnerText;       

#if UNITY_EDITOR && !DOWNLOAD 
	    SceneLoader.ins.LoadLevel(Scenes.Main);
#else
        //版本沒變
        //if(GameBase.version == ver)
	        //SceneLoader.ins.LoadLevel(Scenes.Main);
        //else
	        SceneLoader.ins.LoadLevel(Scenes.DataLoading);
#endif        

        GameBase.version = ver;
    }

	void Destroy()
	{        
	}
}
