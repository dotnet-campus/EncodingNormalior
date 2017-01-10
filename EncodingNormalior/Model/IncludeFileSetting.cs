using System.Collections.Generic;
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

        /// <summary>
        /// 包含的文件的通配符
        /// </summary>
        public List<string> IncludeWildcardFile { set; get; }=new List<string>();

        public List<string> GetaIncludeRegexFile()
        {
            List<string> includeRegexFile=new List<string>();

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

        static string GetWildcardRegexString(string wildcardStr)
        {
            Regex replace = new Regex("[.$^{\\[(|)*+?\\\\]");
            string regex= replace.Replace(wildcardStr,
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