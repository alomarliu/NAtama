using UnityEngine;
using System.Collections;

public class MouseLocker : MonoBehaviour 
{
    bool _mouseLock = false;
    
    /** singleton */
	private static MouseLocker 	_instance   = 	null;
	
	void Awake()
    {        
		if(_instance == null)
		{			
			_instance = this;
			GameObject.DontDestroyOnLoad(gameObject);			
		}
		else if(_instance != this)
		{			
			Destroy(gameObject);
		}
    }

	/**=============================================
	 * instance getter
	 * ===========================================*/
	public static MouseLocker instance
	{	
		get
		{
			return _instance;
		}
	}

    public bool mouseLock
    {
        get
        {
            return _mouseLock;
        }
    }

	/**=============================================
	 * 鎖定滑鼠
	 * ===========================================*/
	public void SetMouseLock(bool value)
	{
		string[] s = {};
		SetMouseLock(value, s);
	}

	/**=============================================
	 * 鎖定滑鼠 
	 * <param> except 除了.. </param>
	 * ===========================================*/
	public void SetMouseLock(bool value, string[] exceptLayer)
	{
		Camera cam;
		int count = Camera.allCamerasCount;
		UICamera uiCam;
		LayerMask mask = 0 | LayerMask.GetMask(exceptLayer);

		for (int i = 0; i < count; ++i) 
		{
			cam = Camera.allCameras[i];
			uiCam = cam.GetComponent<UICamera>();

			// NGUI 的 Camera
			if(uiCam)
			{
				// NGUI 預設為allLayer
				uiCam.eventReceiverMask = (value)? (int)mask : Physics2D.AllLayers;
			}
			else
			{
				cam.eventMask = (value)? 0 : Physics2D.AllLayers;
			}
		}

		_mouseLock = value;
	}
}
