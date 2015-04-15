using System.Collections;
using UnityEngine;
using UIFrameWork;
using System;

/**=============================================================
 * <summary> 確認視窗 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

public class UIDialogController : UIController
{
    [Flags]
    public enum DialogType
    {
        OK          = 0x01,
        OKCancel    = 0x02,
    }

	private static UIDialogController _instance = null;
    
	/** 內容 */
    [SerializeField]
	UILabel _lbContent					        = null;
	/** 提示 */
    [SerializeField]
	UILabel _lbHint					            = null;
	private DialogRoot _dialog					= null;

	/**=============================================
	 * 取得 Singleton
	 * ===========================================*/
	public static UIDialogController instance
	{
		get
		{
			if(_instance == null)
				_instance = UIManager.instance.GetUI<UIDialogController>();

			return _instance;
		}
	}
	
	/**=============================================
	 * <summary>開啟</summary>
	 * <param name="values">values[0]: type </param>
	 * <param name="values">values[1]: content </param>
	 * <param name="values">values[2]: callBack </param>
	 * <param name="values">values[3]: callBackTarget </param>
	 * <param name="values">values[4]: callBackParams </param>
	 * ===========================================*/
	override public void Open(params object[] values)
	{		
		if(visible)
			return;

		base.Open();

        DialogType type = (DialogType)values[0];

        IEnumerator itor = Enum.GetValues(typeof(DialogType)).GetEnumerator();
        Transform t;

        while(itor.MoveNext())
        {
            t = transform.FindChild("Container/"+itor.Current.ToString());
            
            if (t != null)
                NGUITools.SetActiveSelf(t.gameObject, ((DialogType)itor.Current == type));
        }        
        
        if (values[0] == null || values[1] == null)
            return;

        // 依參數數量做不同處理
        if (values.Length <= 2)
            _dialog.Reset();
        else
        {
            _dialog.SetCallBackInfo(values[2].ToString(),
                                    values[3] as MonoBehaviour,
                                    (values.Length >= 5) ? values[4] : null);
        }

		_lbContent.text = values[1].ToString();

		string[] mask = {"Dialog"};
		MouseLocker.instance.SetMouseLock(true, mask);

        _lbHint.text = "";
	}

	override public void Close()
	{				
		if(!visible)
			return;
        
        // 解除滑鼠鎖定
		MouseLocker.instance.SetMouseLock(false);
		base.Close();
	}

	void Awake()
	{
		_dialog = GetComponent<DialogRoot>();
	}
}
