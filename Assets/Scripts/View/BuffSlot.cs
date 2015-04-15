using Model.DBF;
using NLNetwork;
using System;
using System.Collections;
using UnityEngine;

public class BuffSlot : MonoBehaviour 
{    
    BuffTabelLib _data = null;

    [SerializeField]
    GameObject _notMaxLv = null;
    [SerializeField]
    GameObject _maxLv = null;
    [SerializeField]
    UILabel _lbCoin = null;
    [SerializeField]
    UILabel _lbGold = null;
    [SerializeField]
    UILabel _lbTime = null;
    [SerializeField]
    UILabel _lbUnLockLv = null;

	/**================================
	 * <summary> 設定資料 </summary>
	 *===============================*/
    public void SetData(BuffTabelLib data)
    {
        _data = data;

        UpdateUI();
    }
    
	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    public void UpdateUI()
    {
        Transform t = transform;
        Character.BuffInfo info = Character.GetBuff(_data.GUID);
        int nowBuffID = (info == null)? _data.GUID * 100 : info.buffID;
        int nextBuffID = (info == null)? _data.GUID * 100 + 1 : info.buffID+1;
        BuffEffectLib nowObj = DBFManager.buffEffectLib.Data(nowBuffID) as BuffEffectLib;
        BuffEffectLib nextObj = DBFManager.buffEffectLib.Data(nextBuffID) as BuffEffectLib;

        // 是否顯示最大等級
        NGUITools.SetActiveSelf(_maxLv, null == nextObj);
        // 是否顯示未達最大等級元件
        NGUITools.SetActiveSelf(_notMaxLv, null != nextObj);

        string tempStr = "";

        if(null != nextObj)
        {
            t.FindChild("lbBuffName").GetComponent<UILabel>().text = _data.Name + ((info != null)? " Lv" + nextObj.Lv : "");

            switch(_data.GUID)
            {
                case 4:
                    tempStr = string.Format(_data.Tip, nextObj.Coin, nextObj.SaveTime);
                    break;
                default:
                    tempStr = string.Format(_data.Tip, nextObj.BuffValue);
                    break;
            }

            t.FindChild("NotMaxLv/lbBuffTip").GetComponent<UILabel>().text = tempStr;

            _lbCoin.text = nowObj.Require[0].Value.ToString();
            _lbGold.text = nowObj.Require[1].Value.ToString();

            TimeSpan time = new TimeSpan(0, nowObj.Time, 0);
            _lbTime.text = time.ToString();
            
        }

        NGUITools.SetActiveSelf(t.FindChild("uiLock").gameObject, null == info);

        BuffTabelLib buffTableObj = DBFManager.buffTabelLib.Data(_data.GUID) as BuffTabelLib;
        _lbUnLockLv.text = "解鎖等級 Lv" + ((null != buffTableObj) ? buffTableObj.Lv.ToString() : "");
    }

    void OnClick()
    {
        // 檢查可否學該BUFF
        if (!Character.CheckLearnBuff(_data.GUID))
            return;
        
        Character.BuffInfo info = Character.GetBuff(_data.GUID);
        int buffID = (info == null)? -1 : info.buffID;
        BuffEffectLib nowObj = DBFManager.buffEffectLib.Data(buffID) as BuffEffectLib;
        int[] require = { 0, 0 };
        bool needBuy = false;
        
        for(int i = 0; i < nowObj.Require.Length; ++i)
        {
            // 檢查道具是否足夠
            if(nowObj.Require[i].ItemID > 0 && 
              !Character.CheckDelItem(nowObj.Require[i].ItemID, nowObj.Require[i].Value))
            {
                require[i] = nowObj.Require[i].Value;
                needBuy = true;
            }
        }             

        if(needBuy)
        {
            // 開啟金流介面
            UICashFlowDialogController.instance.Open("鍛練費不足", require[0], require[1]);
            return;
        }

        UIDialogController.instance.Open(UIDialogController.DialogType.OKCancel,
                                         "確定要鍛鍊嗎?", "OnConfirm", this, 
                                         new object[]{ buffID });
    }
    
	/**================================
	 * <summary> 確定學習 </summary>
	 *===============================*/
    void OnConfirm(DialogOption.OptionType option, params object[] param)
    {
        if (option != DialogOption.OptionType.OK)
            return;

        int nowBuffID = (int)param[0];

        BuffEffectLib nowObj = DBFManager.buffEffectLib.Data(nowBuffID) as BuffEffectLib;

        if (null == nowObj)
            return;
        
        for(int i = 0; i < nowObj.Require.Length; ++i)
        {
            // 扣道具
            if(nowObj.Require[i].ItemID != -1)
                Character.DelItem(nowObj.Require[i].ItemID, nowObj.Require[i].Value);
        }        

        Character.learningBuffID = nowBuffID;
        // 設定開始學的時間
        Character.SetLastLearnBuffTick(ServerTime.time, false);
        // 按下退回
        UIBuffController.instance.backBtn.OnClick();

        // 傳送開始學的指令
        NetworkLib.instance.SendBuffLvUp(Character.ins.uid, _data.GUID);      
    }
}
