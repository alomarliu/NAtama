using UnityEngine;
using System.Collections;

public class UISetLabel : MonoBehaviour 
{
    UILabel _label = null;

    void Awake()
    {
        _label = GetComponent<UILabel>();
    }

    public void SetCurrentSelection()
    {
        if(null == _label)
            return;
        
		if (UIMyPopupList.current != null)
		{
			_label.text = UIMyPopupList.current.isLocalized ?
				Localization.Get(UIMyPopupList.current.value) :
				UIMyPopupList.current.value;
		}
    }
}
