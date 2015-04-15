using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
 
class NLEncrypt
{
    public static string myKey  = "1234567812345678";
    public static string myIV   = "1234567812345678";

    public  static  string  Encrypt( string  toEncrypt, string  key, string  iv)
    {
        byte [] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        byte [] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
        byte [] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
        string result = "";


        using (RijndaelManaged rDel = new RijndaelManaged())
        {
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.BlockSize = 128;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
 
        return result;
    }
 
    public  static  string  Decrypt( string  toDecrypt, string  key, string  iv)
    {
        byte [] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        byte [] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
        byte [] toEncryptArray = Convert.FromBase64String(toDecrypt);
        string result = "";

        using (RijndaelManaged rDel = new RijndaelManaged())
        {
            rDel.BlockSize = 128;
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            result = UTF8Encoding.UTF8.GetString(resultArray);
        }
 
        return result;
    }
}