using LitJson;
using System.Collections;
using UnityEngine;

public class GameMain_Battle : MonoBehaviour 
{
    public void Start()
    {
        JsonData jd = PlayerPrefManager.instance.GetBattleInfo();
        int mode = int.Parse(jd["mode"].ToString());
        int gateID = int.Parse(jd["stageID"].ToString());
        int npcID = int.Parse(jd["npcID"].ToString());

        // 開啟模式1
        UIBattleController.instance.Open(mode, gateID, npcID);
    }    
}
