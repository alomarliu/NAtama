using UnityEngine;
using System.Collections;

public class TimeCircle : MonoBehaviour 
{
    /// <summary>總時間</summary>
    [SerializeField]
    int _totalTime = 10;

    float _startTime = 0;
    
    /// <summary>初始圓</summary>
    [SerializeField]
    UISprite _originCircle = null;
    /// <summary>1P先答後要顯示的圓</summary>
    [SerializeField]
    UISprite _1pAnsCircle = null;
    /// <summary>2P先答後要顯示的圓</summary>
    [SerializeField]
    UISprite _2pAnsCircle = null;
    
    /// <summary>時間</summary>
    [SerializeField]
    UILabel _lbTime = null;

    bool _hasAns = false;
    
    void Start()
    {
        _originCircle.fillAmount = 0;
        _1pAnsCircle.fillAmount = 0;
        _2pAnsCircle.fillAmount = 0;
    }

	/**================================
	 * <summary> 初始 </summary>
	 *===============================*/
    public void Init(int time)
    {
        enabled = false;
        _totalTime = time;
        _originCircle.fillAmount = 0;
        _1pAnsCircle.fillAmount = 0;
        _2pAnsCircle.fillAmount = 0;
        _hasAns = false;
        
        NGUITools.SetActiveSelf(_1pAnsCircle.gameObject, false);
        NGUITools.SetActiveSelf(_2pAnsCircle.gameObject, false);
    }

    public void Begin()
    {        
        _startTime = Time.time;
        enabled = true;
    }
    
	/**================================
	 * <summary> 回答 </summary>
	 *===============================*/
    public void Answer(int side)
    {
        if(side == 0)
            NGUITools.SetActiveSelf(_1pAnsCircle.gameObject, true);
        else
            NGUITools.SetActiveSelf(_2pAnsCircle.gameObject, true);
        
        _hasAns = true;
    }
    
	/**================================
	 * <summary> 結束 </summary>
	 *===============================*/
    public void Stop()
    {
        enabled = false;
    }

	/**================================
	 * <summary> 更新 </summary>
	 *===============================*/
    public void FixedUpdate()
    {
        if (_startTime == 0)
            return;

        float passTime = Time.time - _startTime;
        float rate = passTime / _totalTime;
        
        // 無人回答的情況才更新 origin
        if(!_hasAns)
            _originCircle.fillAmount = rate;        

        _1pAnsCircle.fillAmount = rate;
        _2pAnsCircle.fillAmount = rate;
        _lbTime.text = (_totalTime - Mathf.FloorToInt(passTime)).ToString();
    }
}
