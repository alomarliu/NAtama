using UnityEngine;
using System.Collections;

public class LoadingManager : MonoBehaviour 
{    
    /** singleton */
	private static LoadingManager 	_instance   = 	null;
	
	void Awake()
    {        
		if(_instance == null)
		{			
			_instance = this;		        
            NGUITools.SetActiveSelf(gameObject, false);
		}
		else if(_instance != this)
		{			
			Destroy(gameObject);
		}
    }

	/**=============================================
	 * instance getter
	 * ===========================================*/
	public static LoadingManager instance
	{	
		get
		{
			return _instance;
		}
	}
    
    /**========================================
    * <summary></summary>
    * ======================================*/
    public void SetLoading(bool loading)
    {
        NGUITools.SetActiveSelf(gameObject, loading);

        // loading時滑鼠鎖定
        MouseLocker.instance.SetMouseLock(loading);
    }
}
