using UnityEngine;
using System.Collections;

/**=============================================================
 * <summary> 確認視窗選項 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

[RequireComponent (typeof (UIButton))]
public class DialogOption : MonoBehaviour 
{
	public enum OptionType
	{
		OK		=	0x01,
		Cancel 	= 	0x02,
	}
	
	public OptionType type 	= OptionType.OK;
	
	[HideInInspector]
	public DialogRoot root  = null;

	private EventDelegate eventDelegate = new EventDelegate();

	void Awake()
	{                
		root = GetComponentInParent<DialogRoot>();

		eventDelegate.target = root;
		eventDelegate.methodName = "OnChooseOption";
		eventDelegate.parameters[0] = new EventDelegate.Parameter(type);  
	}
	
	/**=============================================
	 * 按鈕按下
	 * ===========================================*/
	void OnClick()
	{
		eventDelegate.Execute();
	}
}
