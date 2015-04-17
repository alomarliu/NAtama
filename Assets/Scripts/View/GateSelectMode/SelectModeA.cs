using LitJson;
using Model.DBF;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

public class SelectModeA : MonoBehaviour , IGateSelectMode
{    
    object[] _data = {};
    /// <summary> grid </summary>
    [SerializeField]
    UIGrid _dataGrid = null;
    UIMyWrapContent _wrap = null;
    [SerializeField]
    UIScrollView _scrollView = null;
    
    [SerializeField]
    UILabel _challengeCount = null;

    UIController _uiMain = null;
    
    void Awake()
    {                
        _wrap = _dataGrid.GetComponent<UIMyWrapContent>();
        _wrap.onInitializeItem += ItemChange;

        _uiMain = GetComponentInParent<UIController>();
    }

    void OnDestroy()
    {        
        _wrap.onInitializeItem -= ItemChange;
    }

    void Start()
    {
    }

	/**================================
	 * <summary> 更新畫面 </summary>
	 *===============================*/
    public void UpdateUI()
    {        
        _data = DBFManager.challengeStageLib.GetList<ChallengeStageLib>();

        _wrap.maxIndex = Mathf.Max(4, _data.Length-1);

        List<Transform> childList = _dataGrid.GetChildList();
        Transform t = null;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];

            if(i < _data.Length)
            {                
                UpdateButton(t, i);
            }
            else
            {
                NGUITools.SetActiveSelf(t.gameObject, false);
            }
        }

        _dataGrid.Reposition();
        _scrollView.ResetPosition();

        _challengeCount.text = Character.ins.challengeCount.ToString();
    }    

	/**================================
	 * <summary> 關卡選擇 </summary>
	 *===============================*/
    public void OnGateSelect(string gate = "", int npc = 0)
    {
        Character.ins.winStone = -1;
        int gateID = int.Parse(gate);
        ChallengeStageLib obj = DBFManager.challengeStageLib.Data(gateID) as ChallengeStageLib;

        if (null == obj)
            return;

        //ItemBaseData itemObj = DBFManager.itemObjLib.Data(obj.RequireItem.ItemID) as ItemBaseData;
        ItemBaseData itemObj = DBFManager.itemObjLib.Data(GameBase.ITEM_COIN) as ItemBaseData;

        if (null == itemObj)
            return;        

        Enum_BattleMode mode = Enum_BattleMode.ModeA;

        // 踢館模式
        if (gateID == Character.ins.schoolLv+1)
        {
            mode = Enum_BattleMode.ModeC;
            npc = obj.BOSS;
        }
        else if (gateID <= Character.ins.schoolLv)
        {
            mode = (npc > 0) ? Enum_BattleMode.ModeB : Enum_BattleMode.ModeA;

            // 隨便挑一隻角色
            if (npc == 0)
                npc = Random.Range(0, DBFManager.snatchStageLib.Count)+1;
        }

        string tempstr = "";
        int payItemId = -1;
        int payValue = 0;

        switch(mode)
        {
            case Enum_BattleMode.ModeB:
                {
                    if(!Character.ins.CheckChallenge())
                        tempstr = "挑戰次數不足!";
                }
                break;
            case Enum_BattleMode.ModeA:
            case Enum_BattleMode.ModeC:
                {
                    payItemId = itemObj.GUID;
                    payValue = obj.RequireItem.Value;
                    // 扣道具失敗
                    if(!Character.CheckDelItem(itemObj.GUID, obj.RequireItem.Value))
                        tempstr = string.Format("{0}不足", itemObj.Name);
                }
                break;
        }

        if (!string.IsNullOrEmpty(tempstr))
        {
            UIDialogController.instance.Open(UIDialogController.DialogType.OK, tempstr);
            return;
        }
        
        ChallengeNpcLib npcObj = DBFManager.challengeNpcLib.Data(npc) as ChallengeNpcLib;

        switch (mode)
        {
            case Enum_BattleMode.ModeA:
                tempstr = string.Format("出擊將花費{0} {1}元, 確定嗎?", itemObj.Name, obj.RequireItem.Value);
                break;
            case Enum_BattleMode.ModeB:
                {

                    if (null != npcObj)
                        Character.ins.winStone = npcObj.ItemID;

                    tempstr = string.Format("確定挑戰{0}學長?", npcObj.Name);
                }
                break;
            case Enum_BattleMode.ModeC:
                {
                    tempstr = string.Format("確定對第{0}關{1}號學長踢館?", gateID, npc);
                }
                break;
        }
        
        UIDialogController.instance.Open(UIDialogController.DialogType.OKCancel,
                                         tempstr, "OnConfirm", _uiMain, 
                                         new object[]{gateID, npc, payItemId, payValue, mode});
    }
    
	/**================================
	 * <summary> 確認 </summary>
	 *===============================*/
    public void OnConfirm(DialogOption.OptionType option, params object[] param)
    {
        if (option != DialogOption.OptionType.OK)
            return;
        
        int gateID = (int)param[0];
        int npcID = (int)param[1];
        int itemID = (int)param[2];
        int itemCount = (int)param[3];
        Enum_BattleMode mode = (Enum_BattleMode)param[4];

        UIFindTargetController.instance.Open(itemID, itemCount);
        // 設定對戰資訊
        PlayerPrefManager.instance.SetBattleInfo(mode, gateID, npcID);
    }
    
	/**================================
	 * <summary> Wrap 項目有改變 </summary>
	 *===============================*/
    private void ItemChange(GameObject go, int wrapIndex, int realIndex)
    {
        if (_data.Length <= 0)
            return;
        
        if (go.activeInHierarchy)
        {
            UpdateButton(go.transform, realIndex);
        }
    }
    
	/**================================
	 * <summary> 更新按鈕資訊 </summary>
	 *===============================*/
    void UpdateButton(Transform t, int idx)
    {
        if (idx < 0 || idx >= _data.Length)
            return;        

        ChallengeStageLib obj = (ChallengeStageLib)_data[idx];
        t.gameObject.name = obj.GUID.ToString();
        t.FindChild("lbName").GetComponent<UILabel>().text = obj.NAME;
        t.FindChild("lbTime/lbTimeVal").GetComponent<UILabel>().text = obj.TimeLimit.ToString();
        t.FindChild("lbCost/lbCostVal").GetComponent<UILabel>().text = obj.RequireItem.Value.ToString();
        t.FindChild("uiNPC/lbMode").GetComponent<UILabel>().text = (idx < Character.ins.schoolLv) ? "挑戰" : "踢館";
        NGUITools.SetActiveSelf(t.FindChild("uiKick").gameObject, idx == Character.ins.schoolLv);
        NGUITools.SetActiveSelf(t.FindChild("uiStone").gameObject, idx < Character.ins.schoolLv);
        NGUITools.SetActiveSelf(t.FindChild("lbClickme").gameObject, idx < Character.ins.schoolLv);

        Transform tNpc = t.FindChild("uiNPC");
        NGUITools.SetActiveSelf(tNpc.gameObject, idx <= Character.ins.schoolLv);
        //tNpc.GetComponent<UIButton>().isEnabled = idx < Character.ins.schoolLv;
        
        // 超過學校解鎖等級的不能點
        t.GetComponent<UIButton>().isEnabled = idx <= Character.ins.schoolLv;
        
        // 踢館資訊
        if(idx == Character.ins.schoolLv)
        {
            ChallengeNpcLib npcObj = DBFManager.challengeNpcLib.Data(obj.BOSS) as ChallengeNpcLib;

            if (npcObj != null)
            {
                UISprite uiSprite = tNpc.GetComponent<UISprite>();
                // 更換NPC圖片
                uiSprite.spriteName = npcObj.Pic;
                uiSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
                uiSprite.MakePixelPerfect();
                uiSprite.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
                uiSprite.SetDimensions(180, 200);
            }
        }
        else if(idx < Character.ins.everyDayNpcs.Length)
        {
            ChallengeNpcLib npcObj = DBFManager.challengeNpcLib.Data(Character.ins.everyDayNpcs[idx]) as ChallengeNpcLib;

            if(npcObj != null)
            {
                UISprite uiSprite = tNpc.GetComponent<UISprite>();
                // 更換NPC圖片
                uiSprite.spriteName = npcObj.Pic;
                uiSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
                uiSprite.MakePixelPerfect();
                uiSprite.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
                uiSprite.SetDimensions(180, 200);

                ItemBaseData itemObj = DBFManager.itemObjLib.Data(npcObj.ItemID) as ItemBaseData;

                if (itemObj != null)
                    t.FindChild("uiStone").GetComponent<UISprite>().spriteName = itemObj.Act.Split('/')[1];
            }
        }
    }
}
