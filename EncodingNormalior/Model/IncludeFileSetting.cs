using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EncodingNormalior.Model
{
    /// <summary>
    /// 包含文件要求
    /// </summary>
    public class IncludeFileSetting : ISetting
    {
        public IncludeFileSetting()
        {

        }

        static IncludeFileSetting()
        {
            //TextFileSuffix
            var textFileSuffix = Resource.TextFileSuffix.textFileSuffix.Split('\n');

            TextFileSuffix = new List<string>();
            foreach (var temp in textFileSuffix.Select(temp=> temp.Replace("\r", "").Trim()))
            {
                if (!string.IsNullOrEmpty(temp))
                {
                    TextFileSuffix.Add("*." + temp);
                }
            }
        }

        /// <summary>
        /// 包含的文件的通配符
        /// </summary>
        public List<string> IncludeWildcardFile { set; get; } = new List<string>();

        /// <summary>
        /// 常用文本后缀
        /// </summary>
        public static List<string> TextFileSuffix { set; get; }

        /// <summary>
        /// 获取文件包含规则
        /// </summary>
        /// <returns></returns>
        public List<string> GetaIncludeRegexFile()
        {
            List<string> includeRegexFile = new List<string>();

            if (IncludeWildcardFile == null)
            {
                return includeRegexFile;
            }

            foreach (var temp in IncludeWildcardFile)
            {
                includeRegexFile.Add(GetWildcardRegexString(temp));
            }
            return includeRegexFile;
        }

        public static string GetWildcardRegexString(string wildcardStr)
        {
            Regex replace = new Regex("[.$^{\\[(|)*+?\\\\]");
            string regex = replace.Replace(wildcardStr,
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
                });
            return regex;
        }
    }
}