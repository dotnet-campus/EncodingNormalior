using System;
using System.Collections.Generic;
using System.IO;
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

        public static List<string> ReadIncludeFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException("文件" + file + "不存在");
            }
            List<string> includeFile = new List<string>();
            using (StreamReader stream = new StreamReader(new FileStream(file, FileMode.Open)))
            {
                foreach (var temp in stream.ReadToEnd().Split('\n'))
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        includeFile.Add(temp);
                    }
                }
            }
            return includeFile;
        }

        static IncludeFileSetting()
        {
            //TextFileSuffix
            //读配置
            string file = "textFileSuffix.txt";
            string[] textFileSuffix;
            if (!File.Exists(file))
            {
                textFileSuffix = Resource.TextFileSuffix.textFileSuffix.Split('\n');
            }
            else
            {
                using (StreamReader stream = new StreamReader(new FileStream(file, FileMode.Open)))
                {
                    textFileSuffix = stream.ReadToEnd().Split('\n');
                }
            }

            TextFileSuffix = new List<string>();
            foreach (var temp in textFileSuffix.Select(temp => temp.Replace("\r", "").Trim()))
            {
                if (!string.IsNullOrEmpty(temp))
                {
                    TextFileSuffix.Add("*." + temp);
                }
            }


            file = EncodingScrutatorFileStorage.InlcudeFile;
            if (File.Exists(file))
            {
                foreach (var temp in ReadIncludeFile(file))
                {
                    TextFileSuffix.Add(temp);
                }
            }
        }

        public IncludeFileSetting(List<string> includeWildcardFile)
        {
            IncludeWildcardFile = includeWildcardFile;
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
                if(!string.IsNullOrEmpty(temp))
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