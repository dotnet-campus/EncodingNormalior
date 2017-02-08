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
            Regex replace = //new Regex("[.$^{\\[(|)*+?\\\\]");
                _regex;
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

        private static Regex _regex = new Regex("[.$^{\\[(|)*+?\\\\]", RegexOptions.Compiled);

        /// <summary>
        /// 获取通配符的正则
        /// </summary>
        /// <param name="wildcarStr"></param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static Regex GetWildcardRegex(string wildcarStr, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return new Regex(GetWildcardRegexString(wildcarStr));
            }
            return new Regex(GetWildcardRegexString(wildcarStr), RegexOptions.IgnoreCase);
        }
    }
}