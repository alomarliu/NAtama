using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.DBF;

/**=============================================================
 * <summary> 問題系統 </summary>
 * 
 * <author>Neymar Liu</author>
 * <date>$time$</date>
 * 
 * Copyright (c) 2014 All Rights Reserved
 =============================================================*/

namespace GamePlay
{
    public class QuestionSys
    {
        public int questionID = -1;

        /** singleton */
        private static QuestionSys _instance = null;
        private static bool _allowInstance = false;

        /**=============================================
         * 建構子
         * ===========================================*/
        QuestionSys()
        {
            if (!_allowInstance)
                throw new Exception("not allow instance.");
        }

        /**=============================================
         * instance getter
         * ===========================================*/
        public static QuestionSys instance
        {
            get
            {
                _allowInstance = true;

                if (_instance == null)
                    _instance = new QuestionSys();

                _allowInstance = false;

                return _instance;
            }
        }

        /**========================
        * <summary>隨機挑題目</summary>
        * =======================*/
        public int RandQuestion(QuestionLib.QuestionType type, int catalog, int difficult)
        {
            return RandQuestion(type, catalog, difficult, difficult);
        }

        /**========================
        * <summary>隨機挑題目</summary>
        * =======================*/
        public int RandQuestion(QuestionLib.QuestionType type, int catalog, int minDifficult, int maxDifficult)
        {
            List<int> list = new List<int>();
            int questID = -1;
            QuestionLib obj = null;

            for (int x = minDifficult; x <= maxDifficult; ++x)
            {
                for (int i = 1; i <= 999; ++i)
                {
                    questID = int.Parse("1" + catalog.ToString().PadLeft(2, '0') + x) * 1000 + i;
                    obj = DBFManager.questionList.Data(questID) as QuestionLib;

                    if (null != obj)
                    {
                        list.Add(obj.GUID);
                    }
                }
            }

            if (list.Count <= 0)
                return -1;

            int idx = UnityEngine.Random.Range(0, list.Count); ;
            
            return questionID = list[idx];
        }
        
        /**========================
        * <summary>取得正確答案</summary>
        * =======================*/
        public string GetAns()
        {            
            if (-1 == questionID)
                return "";

            QuestionLib obj = DBFManager.questionList.Data(questionID) as QuestionLib;

            if (null == obj)
                return "";

            string correctAns = "";
            
                correctAns = obj.Option1;

            switch((QuestionLib.QuestionType)obj.Type)
            {
            // 是非題
            case QuestionLib.QuestionType.YorN:
                correctAns = obj.Option1;
                break;
            // 選擇題
            case QuestionLib.QuestionType.Choose:
                correctAns = obj.Option1;
                break;
            // 排序題
            case QuestionLib.QuestionType.Sort:
                correctAns = string.Format("{0},{1},{2},{3}", obj.Option1, obj.Option2, obj.Option3, obj.Option4);
                break;
            }
            
            return correctAns;
        }

        /**========================
        * <summary>回答</summary>
        * =======================*/
        public bool Answer(string ans)
        {
            string correctAns = GetAns();

            if (string.IsNullOrEmpty(correctAns))
                return false;
            
            return ans == correctAns;
        }
    }
}