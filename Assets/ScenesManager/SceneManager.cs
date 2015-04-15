using UnityEngine;
using System.Collections;

/**=============================================================
 * <summary> 場景管理器 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class SceneManager : MonoBehaviour 
{
	private enum Status
	{		
		None,
		Prepare,
		Start,
		Loading,
		Complete
	}
	
	public static SceneManager ins;
	
	public SceneNames sceneNames;
	
	private Status _status;
	private AsyncOperation _loadOperation;
	private int own;
	
	public float Progress
	{
		get
		{
			if(this._loadOperation == null)
			{
				return 0;			
			}
			else
			{
				return this._loadOperation.progress;
			}
		}
	}
	
	public int OwnSecne
	{
		set
		{
			this.own  = Mathf.Clamp(value , 0 , this.sceneNames.scenes.Length - 1);
		}
	}

	void Awake()
	{		
		if(ins == null)
		{			
			ins = this;
			GameObject.DontDestroyOnLoad(gameObject);
			
			if(this.sceneNames.initScene != Scenes.None)
			{
				Application.LoadLevel(this.sceneNames.initScene.ToString());
			}
			
		}
		else if(ins != this)
		{			
			Destroy(gameObject);
		}
	}
	
	public void LoadScene(int idx)
	{	
		if(this._status != Status.None && this._status != Status.Complete) return;
		
		if(idx >= this.sceneNames.scenes.Length) return;

		StartCoroutine(this.AsyncLoadScene(this.sceneNames.scenes[idx]));
	}
	
	public void LoadOwnScene()
	{		
		this.LoadScene(this.own);
	}
	
	public void StartLoadTargetScene()
	{		
		this._status = Status.Start;
		this._loadOperation = null;
	}
	
	private IEnumerator AsyncLoadScene(SceneNameHolder sceneName)
	{		
		yield return StartCoroutine(this.LoadLoadingScene(sceneName));
		
		yield return StartCoroutine(this.LoadTargetScene(sceneName));
	}
	
	private IEnumerator LoadLoadingScene(SceneNameHolder sceneName)
	{		
		if(sceneName.loading == LoadingScenes.None) yield break;
		
		this._status = Status.Prepare;
		
		this._loadOperation = sceneName.isAdditiveLoading ? Application.LoadLevelAdditiveAsync(sceneName.loading.ToString()) : Application.LoadLevelAsync(sceneName.loading.ToString());
		
		yield return this._loadOperation;
		
		while(this._status == Status.Prepare)
		{			
			yield return null;
		}
	}
	
	private IEnumerator LoadTargetScene(SceneNameHolder sceneName)
	{		
		this._status = Status.Loading;
		
		if(sceneName.own == Scenes.None)
		{			
			this._status = Status.None;
			yield break;
		}

        Application.LoadLevel(sceneName.own.ToString());
		this._status = Status.Complete;
        /*
		this._loadOperation = Application.LoadLevelAsync(sceneName.own.ToString());
		_loadOperation.allowSceneActivation = false ;

		while(!_loadOperation.isDone && _loadOperation.progress < 0.9f) 
		{
			yield return null ;
		}
        
		this._status = Status.Complete;
		//4.等待完成，設置allowSceneActivati​​on為true，開始跳轉
		Debug.Log( "loading Complete!" );
		_loadOperation.allowSceneActivation = true ;
         * */

	}
}