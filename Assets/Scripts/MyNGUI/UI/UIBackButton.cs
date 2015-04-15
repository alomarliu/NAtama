using UnityEngine;
using System.Collections;
using UIFrameWork;

public class UIBackButton : MonoBehaviour 
{
    // 要退回的畫面
    [SerializeField]
    Enum_UI _backUI = Enum_UI.UIMain;
    
    [SerializeField]
    bool _tween = false;

    UIController _ui = null;

    void Awake()
    {
        _ui = GetComponentInParent<UIController>();
    }

	/**================================
	 * <summary> 按下滑鼠 </summary>
	 *===============================*/
    public void OnClick()
    {
        // 滑鼠被鎖定中
        if (MouseLocker.instance.mouseLock)
            return;

        if (_tween)
        {
			UIPlayTween pTween = _ui.GetComponent<UIPlayTween>();
						
			EventDelegate ed = new EventDelegate(this, "TweenComplete");
			ed.oneShot = true;
			pTween.onFinished.Add(ed); 

            _ui.CloseTween();
        }
        else
        {
            _ui.Close();
            UIManager.instance.Open(_backUI);
        }
    }

    public void TweenComplete()
    {
        // 是倒回
        if (UITweener.current.direction == AnimationOrTween.Direction.Reverse)
        {
            _ui.Close();
            UIManager.instance.Open(_backUI);
        }
    }
}
