using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class DataLoading : MonoBehaviour 
{
	[SerializeField]
	UIProgressBar progress = null;
	[SerializeField]
	UIProgressBar eachProgress = null;

    /// <summary>狀態</summary>
	[SerializeField]
	UILabel _lbStatus = null;
	[SerializeField]
	UILabel _lbDownloadFile = null;

    string _oldVer = "";
    string _newVer = "";

	// Use this for initialization
	IEnumerator Start () 
	{
        LoadingManager.instance.SetLoading(true);

        _lbStatus.text = "Checking...";
		//string path = "file://" + System.IO.Path.GetFullPath("Assets/PackFolder/version");
		string path = GameBase.versionFileUrl + "?ver=" + GameBase.version;
        
		// 下載版本檔案
		WWW www = new WWW(path);
		
		yield return www;		
        
		if(www.error != null)
		{
			Debug.LogError(www.error);
			yield break;
		}
			
		TextAsset tAsset = www.assetBundle.mainAsset as TextAsset;

		JsonReader customReader;
		customReader  = new JsonReader(tAsset.text);		
		customReader.AllowComments            = false;
		customReader.AllowSingleQuotedStrings = false;
		
		JsonData js = JsonMapper.ToObject(customReader);
        _oldVer = PlayerPrefManager.instance.GetVersion();

        // 版號不同才下載
        if (js["nowVer"].ToString() != _oldVer)
        {
            _newVer = js["nowVer"].ToString();
            _lbStatus.text = "Downloading...";
            yield return StartCoroutine(DownLoadFile(tAsset.text));
        }

        progress.value = 1;
        eachProgress.value = 1;
        
        _lbStatus.text = "UnPacking...";
        // 載入
        yield return StartCoroutine(LoadAssetsBundle(tAsset.text));
        
        // 寫入使用者設定
        PlayerPrefManager.instance.SetVersion(GameBase.version);        
        
        LoadingManager.instance.SetLoading(false);

		// 切換到下一個場景
		SceneLoader.ins.LoadLevel(Scenes.Main);
	}
    
	/**=============================================
	 * <summary>下載</summary>
	 * ===========================================*/
	IEnumerator DownLoadFile(string json)
    {
		JsonReader  customReader;
		customReader  = new JsonReader(json);
		
		customReader.AllowComments            = false;
		customReader.AllowSingleQuotedStrings = false;
		
		JsonData js = JsonMapper.ToObject(customReader);
		Dictionary<string, string > dic = new Dictionary<string, string>();
        Dictionary<string, string> newDic = null;
        
		string text = "";
		string path = "";
		WWW www;
        int totalCount = 0;
		int downloadCount = 0;
		int version = 0;
		string[] strVer = (_oldVer.Split('v'))[1].Split('.');
        // 遊戲的目前版號
        int nowVer = int.Parse(strVer[0] + strVer[1] + strVer[2]);

        List<Dictionary<string, string>> downList = new List<Dictionary<string, string>>();
        
        // 檔案存放路徑
        string filePath = "";
        string dirName = "";
        FileStream fs = null;

        // 先挑出較新版號的檔案
		for(int i = 0; i < (int)PackType.MAX; ++i)
		{
			text = js[i.ToString()].ToString();	
			dic = JsonMapper.ToObject<Dictionary<string, string>>(text);
            newDic = new Dictionary<string, string>();
			
			foreach(KeyValuePair<string, string> l in dic)
			{
				path = GameBase.rootUrl + "/"+l.Key + "?ver=" + _newVer;
                Debug.Log(path);

				strVer = (l.Value.Split('v'))[1].Split('.');
				version = int.Parse(strVer[0] + strVer[1] + strVer[2]);
                // 檔案存放路徑
                filePath = Application.persistentDataPath + "/" + l.Key;
				
                // 檔案版號比目前版號新或檔案不存在, 加入下載清單
                if(nowVer < version || !File.Exists(filePath))
                {
                    newDic.Add(l.Key, l.Value);
                    totalCount++;
                }
            }

            downList.Add(newDic);
        }

        // 開始下載
        for (int i = 0; i < downList.Count; ++i)
        {
            dic = downList[i];
            
            foreach (KeyValuePair<string, string> l in dic)
            {
                path = GameBase.rootUrl + "/" + l.Key + "?ver=" + _newVer;

               // Debug.Log(path);
                strVer = (l.Value.Split('v'))[1].Split('.');
                version = int.Parse(strVer[0] + strVer[1] + strVer[2]);
                //Debug.Log(version);
                www = new WWW(path);
                _lbDownloadFile.text = path;
                while(www.progress < 1f)
                {
                    eachProgress.value = www.progress;
                    yield return new WaitForSeconds( .1f );
                }

                yield return www;

                downloadCount++;                
                progress.value = Mathf.Clamp01((float)downloadCount / totalCount);

                // 檔案存放路徑
                filePath = Application.persistentDataPath + "/" + l.Key;
                // 目錄名
			    dirName = Path.GetDirectoryName(filePath);
                // 目錄不存在
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                // 若檔案存在 , 先刪除
                if (File.Exists(filePath))
                    File.Delete(filePath);

                Debug.Log(filePath);
                fs = File.Create(filePath); //path為你想保存文件的路徑。
                fs.Write(www.bytes, 0, www.bytes.Length); 
                fs.Close();

                www.assetBundle.Unload(true);
            }
        }

        _lbDownloadFile.text = "";
    }
    
	/**=============================================
	 * <summary>載入</summary>
	 * ===========================================*/
    IEnumerator LoadAssetsBundle(string json)
	{
		JsonReader  customReader;
		customReader  = new JsonReader(json);		
		customReader.AllowComments            = false;
		customReader.AllowSingleQuotedStrings = false;
		
		JsonData js = JsonMapper.ToObject(customReader);
		Dictionary<string, string > dic = new Dictionary<string, string>();

		string text = "";
		string path = "";

		for(int i = 0; i < (int)PackType.MAX; ++i)
		{
			text = js[i.ToString()].ToString();	
			dic = JsonMapper.ToObject<Dictionary<string, string>>(text);
			
			foreach(KeyValuePair<string, string> l in dic)
			{
                // 檔案存放路徑
                path = Application.persistentDataPath + "/" + l.Key;
                
                if (!File.Exists(path))
                    continue;

                yield return StartCoroutine(CallDownload(path, (PackType)i, l.Key));
			}
		}
	}

    IEnumerator CallDownload(string path, PackType type, string key)
    {        
        WWW www = new WWW("file:///" + path);
        yield return www;

        // 有 error
        if (!string.IsNullOrEmpty(www.error))
            yield break;

        // 從檔案建立
        AssetBundle ab = www.assetBundle;
        
		switch(type)
		{
		case PackType.UI:
		case PackType.Atlas:
		case PackType.MyFont:
		case PackType.Sound:
        case PackType.AttachBuilding:
        case PackType.Sprite:
			{
                if(key == "Assets/Atlas/Atlas_Battle.atlas")
                    Debug.Log(key);
				AssetsManager.instance.AddAsset(key, ab.mainAsset);
			}
			break;
		case PackType.DBF:
			DBFManager.UnZipData(ab);
			ab.Unload(false);
			break;
		default:
			ab.Unload(false);
			break;
		}
    }
}
