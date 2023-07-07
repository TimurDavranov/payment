using System.Security.Cryptography;
using System.Text;

namespace Common.Extentions;

public static class StringExtentions
{
    public static bool IsNull(this string value)
    {
        if (value == null)
            return false;

        if (value == "")
            return false;

        if (value == " ")
            return false;

        return true;
    }

    public static string ToMd5Hash(this string str, string numericFormat = "x2")
    {
        var MD5Code = new MD5CryptoServiceProvider();
        byte[] byteStr = Encoding.UTF8.GetBytes(str);
        byteStr = MD5Code.ComputeHash(byteStr);
        StringBuilder sb = new StringBuilder();
        foreach (byte ba in byteStr)
        {
            sb.Append(ba.ToString(numericFormat).ToLower());
        }
        return sb.ToString();
    }
}