using UnityEngine;
using System;

public class ToolLib
{
    /**==================================
     * <summary>檢查列舉值是否正確</summary>
     * ================================*/
    public static bool IsValidEnumValue<T>(T value)
    {
        System.Collections.IEnumerator myEnumerator = Enum.GetValues(typeof(T)).GetEnumerator();

        myEnumerator.Reset();

        while (myEnumerator.MoveNext())
        {
            if (myEnumerator.Current.ToString() == value.ToString())
                return true;
        }

        return false;
    }

	/**=============================================
	 * 將陣列打亂
	 * ===========================================*/
	public static void Shuffle<T>(T[] array)
	{
		int count = array.Length;
		T temp;
		int rand;

		for (int i = 0; i < count; ++i) 
		{
			rand = UnityEngine.Random.Range(0, count);
			temp = array[i];
			array[i] = array[rand];
			array[rand] = temp;
		}
	}
}
