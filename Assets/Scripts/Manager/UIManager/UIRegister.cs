using UnityEngine;
using System.Collections;
using UIFrameWork;

/**=============================================================
 * <summary> UI註冊器 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIRegister : MonoBehaviour 
{
	[SerializeField]
	private bool _register = true;

	public Enum_UI[] uiIDList;

	public void Awake()
	{
		if(!_register)
			return;

		int iLen = uiIDList.Length;

		for(int i = 0; i < iLen; ++i)
		{
			UIManager.instance.Register(uiIDList[i]);
		}
	}
}
