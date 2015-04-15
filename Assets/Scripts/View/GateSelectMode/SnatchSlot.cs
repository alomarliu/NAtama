using Model.DBF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnatchSlot : MonoBehaviour 
{
    SnatchStageLib _data = null;
    
	/**================================
	 * <summary> 設定資料 </summary>
	 *===============================*/
    public void SetData(SnatchStageLib data)
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
        t.FindChild("lbName").GetComponent<UILabel>().text = _data.NAME;
        t.FindChild("lbSkill").GetComponent<UILabel>().text = _data.Skill.Name;
        t.FindChild("lbPower").GetComponent<UILabel>().text = "威力: +"+_data.BaseATK.ToString();

        Character.NpcInfo npcInfo = Character.GetNpc(_data.GUID);

        UILabel lbLv = t.FindChild("lbLv").GetComponent<UILabel>();
        lbLv.color = new Color(1, 1, 1, 1);
        lbLv.text = (null == npcInfo) ? "[FF0000]未解鎖[-]" : "[344334]Lv" + npcInfo.lv+"[-]";

        UpdateStone();
    }
    
	/**================================
	 * <summary> 更新石頭 </summary>
	 *===============================*/
    void UpdateStone()
    {
        int npcID = _data.GUID;        

        SnatchStageLib obj = DBFManager.snatchStageLib.Data(npcID) as SnatchStageLib;

        List<Transform> childList = transform.FindChild("Stone").GetComponent<UIGrid>().GetChildList();
        Transform t = null;
        UISprite sprite = null;
        ItemBaseData itemObj = null;
        long itemCount = 0;

        for(int i = 0; i < childList.Count; ++i)
        {
            t = childList[i];

            NGUITools.SetActiveSelf(t.gameObject, obj != null && obj.Require[i] != -1);

            if(obj != null && obj.Require[i] != -1)
            {
                sprite = t.FindChild("uiPic").GetComponent<UISprite>();
                itemObj = DBFManager.itemObjLib.Data(obj.Require[i]) as ItemBaseData;
                itemCount = Character.GetItemCount(obj.Require[i]);
                NGUITools.SetActiveSelf(sprite.gameObject, itemObj != null && itemCount > 0);
                
                if(itemObj != null)
                {
                    sprite.spriteName = itemObj.Act.Split('/')[1];
                }
            }
        }
    }
}
