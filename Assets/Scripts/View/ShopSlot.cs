using Model.DBF;
using NLNetwork;
using System.Collections;
using UnityEngine;

public class ShopSlot : MonoBehaviour 
{    
    ShopLib _data = null;
    // 付費
    [SerializeField]
    UILabel _lbPay = null;
    // 獲得道具
    [SerializeField]
    UILabel _lbGetItem = null;
    [SerializeField]
    UISprite _uiIcon = null;
    [SerializeField]
    UISprite _uiOnSale = null;
     
	/**================================
	 * <summary> 設定資料 </summary>
	 *===============================*/
    public void SetData(ShopLib data)
    {
        _data = data;

        UpdateUI();
    }
    
	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    public void UpdateUI()
    {
        _lbPay.text = "NT$ " + _data.Pay.ToString();

        ItemBaseData itemObj = DBFManager.itemObjLib.Data(_data.ItemID) as ItemBaseData;

        if(null != itemObj)
        {            
            _lbGetItem.text = itemObj.Name + " " + _data.Value.ToString();
            _uiIcon.spriteName = itemObj.Act;
            NGUITools.MakePixelPerfect(_uiIcon.transform);
        }
        
        NGUITools.SetActiveSelf(_uiOnSale.gameObject, _data.OnSale == 1);
    }
    
    void OnClick()
    {
        if (null == _data)
            return;

        UIDialogController.instance.Open(UIDialogController.DialogType.OKCancel,
                                         "確定要購買嗎?", "OnConfirm", this);
    }
    
	/**================================
	 * <summary> 確定學習 </summary>
	 *===============================*/
    void OnConfirm(DialogOption.OptionType option, params object[] param)
    {
        if (option != DialogOption.OptionType.OK)
            return;

        // 不可再加了
        if (!Character.CheckAddItem(_data.ItemID, _data.Value))
            return;

        // 給道具
        Character.GiveItem(_data.ItemID, _data.Value);

        NetworkLib.instance.SendShopBuy(Character.ins.uid, _data.GUID, "", "");

        UIShopController.instance.CloseTween();
    }
}
