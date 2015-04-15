using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFrameWork;

/**=============================================================
 * <summary> 確認視窗Root </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

[RequireComponent (typeof (UIController))]
public class DialogRoot : MonoBehaviour 
{
	private UIController _mainUI;

	[HideInInspector]
	public EventDelegate eventDelegate			= null;
	
	[HideInInspector]
	public DialogOption.OptionType _option		= DialogOption.OptionType.OK;
	[HideInInspector]
	public object[] _params						= null;

	void Awake()
	{
		_mainUI = GetComponent<UIController>();
	}

    void Start()
    {        
		int layer = LayerMask.NameToLayer("Dialog");
		NGUITools.SetLayer(gameObject, layer);
    }
	
	/**=============================================
	 * showDialog
	 * ===========================================*/
	public void SetCallBackInfo(string method, MonoBehaviour target, params object[] param)
	{		
		eventDelegate = new EventDelegate(target, method);

		_params = param;
	}
	
	/**=============================================
	 * 當項目選擇之後
	 * ===========================================*/
	void OnChooseOption(DialogOption.OptionType type)
	{   
		_mainUI.Close();

		_option = type;

        if (null != eventDelegate)
        {
            if (eventDelegate.parameters.Length > 0)
            {
                eventDelegate.parameters[0] = new EventDelegate.Parameter(_option);

                int idx = 1;

                if (_params != null)
                {
                    foreach (object obj in _params)
                    {                   
                        eventDelegate.parameters[idx++] = new EventDelegate.Parameter(obj);
                    }
                }
            }

            eventDelegate.Execute();
        }        

        Reset();
	}

    public void Reset()
    {
		_option = DialogOption.OptionType.OK;
		_params = null;
        eventDelegate = null;        
    }
}
