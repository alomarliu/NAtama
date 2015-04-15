using UnityEngine;
using System.Collections;

// 打包類型
public enum PackType
{
	DBF	= 0x00,
	Sound,
	MyFont,
	Atlas,
	UI,

    /// <summary>貼圖</summary>
    Sprite ,
    /// <summary>建築</summary>
    AttachBuilding,
    
	MAX,
}
