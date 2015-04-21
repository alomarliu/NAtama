using UnityEngine;
using System.Collections;
using System;

namespace NLNetwork
{
    public partial class NetworkLib
    {
	    /** singleton */
	    public  static NetworkLib 	_instance  		= null;
	    private static bool			_allowInstance	= false;

	    /**=============================================
	     * instance getter
	     * ===========================================*/
	    public static NetworkLib instance
	    {	
		    get
		    {
			    _allowInstance = true;

			    if(_instance == null)
				    _instance = new NetworkLib();
			
			    _allowInstance = false;

			    return _instance;
		    }
	    }

        /**================================
	    * <summary>建構式</summary>
        * ===============================*/
        public NetworkLib()
        {
		    if(!_allowInstance)
			    throw new Exception("not allow instance.");
        }
        
        /**================================
	    * <summary>初始化</summary>
        * ===============================*/
        public void Init()
        {
            RegisterCharacter();
            RegisterItem();
            RegisterStage();
            RegisterBuff();
            RegisterShop();
        }
    }
}
