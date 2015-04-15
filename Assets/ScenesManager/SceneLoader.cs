using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour 
{		
	public static SceneLoader ins;
	
	public Scenes own;
	
	void Awake()
	{
		if(ins == null)
		{			
			ins = this;
			GameObject.DontDestroyOnLoad(gameObject);
			SceneManager.ins.OwnSecne = (int)this.own;
			
		}
		else if(ins != this)
		{			
			Destroy(gameObject);
		}
	}
	
	public void LoadLevel(Scenes target)
	{		
		SceneManager.ins.LoadScene((int)target);
	}
}