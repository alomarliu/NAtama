using UnityEngine;
using System.Collections;

public class WarningManager : MonoBehaviour 
{    
    /** singleton */
	private static WarningManager 	_instance   = 	null;
    GameObject _prefab = null;
	
	void Awake()
    {        
		if(_instance == null)
		{			
			_instance = this;		
		}
		else if(_instance != this)
		{			
			Destroy(gameObject);
		}
    }

	/**=============================================
	 * instance getter
	 * ===========================================*/
	public static WarningManager instance
	{	
		get
		{
			return _instance;
		}
	}

    void Start()
    {
        _prefab = Resources.Load<GameObject>("WarningLabel");

    }
    public void ShowMessage(Vector3 pos, string text)
    {

        GameObject label = NGUITools.AddChild(gameObject, _prefab);
        label.GetComponent<UILabel>().text = text;
        label.transform.position = pos;
        
        TweenPosition tween = TweenPosition.Begin(label, 1f, pos, true);
        pos.y += 0.2f;
        tween.to = pos;
        tween.AddOnFinished(DestroyObject);
    }
    
    void DestroyObject()
    {
        Destroy(TweenPosition.current.gameObject);
    }

    void OnDestroy()
    {
        _prefab = null;
        Resources.UnloadUnusedAssets();
    }
}
