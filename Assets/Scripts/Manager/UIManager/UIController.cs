using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFrameWork
{
	public class UIController : MonoBehaviour, IUIInterface
	{
		public Enum_UI uiKey;

		private bool 		_visible	= false;
		private bool 		_isTweening	= false;
				
		/**=============================================
		 * visible getter & setter
		 * ===========================================*/
		public bool visible
		{
			get { return _visible; }
			set { _visible = value; }
		}
		
		/**=============================================
		 * 初始化
		 * ===========================================*/
		public void Initialize()
		{		
			Close();
		}
		
		/**=============================================
		 * 開啟
		 * ===========================================*/
		public virtual void Open(params object[] values)
		{
			if(_visible)
				return;

			if(!gameObject.activeInHierarchy)
				NGUITools.SetActive (gameObject, true);

			_visible = true;
			_isTweening = false;
		}
				
		/**=============================================
		 * 播放開啟動態
		 * ===========================================*/
		public virtual void OpenTween(params object[] values)
		{
			if(_visible)
				return;
			
			// 正在播放
			if(_isTweening)
				return;

			//if(!gameObject.activeInHierarchy)
			//	NGUITools.SetActive (gameObject, true);

            Open(values);

			UIPlayTween pTween = gameObject.GetComponent<UIPlayTween>();

            if (null == pTween)
                return;
            
            pTween.resetOnPlay = true;
			pTween.Play(true);
			
            /*
			EventDelegate ed = new EventDelegate(this, "Open");
            ed.parameters[0] = new EventDelegate.Parameter(values);
			ed.oneShot = true;
			pTween.onFinished.Add(ed); 
             * */

			//_isTweening = true;
		}

		/**=============================================
		 * 關閉
		 * ===========================================*/
		public virtual void Close()
		{
			if(!_visible)
				return;
			
			if (gameObject.activeInHierarchy)				
				NGUITools.SetActive(gameObject, false);
			
			_visible = false;
			_isTweening = false;
		}
		
		/**=============================================
		 * 播放關閉動態
		 * ===========================================*/
		public virtual void CloseTween()
		{
			if(!_visible)
				return;
			
			// 正在播放
			if(_isTweening)
				return;

			UIPlayTween pTween = gameObject.GetComponent<UIPlayTween>();            
            pTween.resetOnPlay = true;
			pTween.Play(false);
						
			EventDelegate ed = new EventDelegate(this, "Close");
			ed.oneShot = true;
			pTween.onFinished.Add(ed); 

			_isTweening = true;
		}

		void Destroy()
		{
			//panelList.Clear();
		}        
	}
}
