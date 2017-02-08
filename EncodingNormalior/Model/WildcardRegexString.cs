using System.Text.RegularExpressions;

namespace EncodingNormalior.Model
{
    public static class WildcardRegexString
    {
        /// <summary>
        /// 通配符转正则
        /// </summary>
        /// <param name="wildcardStr"></param>
        /// <returns></returns>
        public static string GetWildcardRegexString(string wildcardStr)
        {
            Regex replace = new Regex("[.$^{\\[(|)*+?\\\\]");
            return replace.Replace(wildcardStr,
                       delegate (Match m)
                       {
                           switch (m.Value)
                           {
                               case "?":
                                   return ".?";
                               case "*":
                                   return ".*";
                               default:
                                   return "\\" + m.Value;
                           }
                       }) + "$";
        }
    }
}