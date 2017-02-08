using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     包含文件配置
    ///     通过输入文件列表，保存在字符串数组，通过 <see cref="GetaIncludeRegexFile" /> 函数得到正则，进行匹配文件。
    /// </summary>
    public class IncludeFileSetting : ISetting
    {
        static IncludeFileSetting()
        {
            //TextFileSuffix
            //读配置

            ReadFileSuffix();

            //获取用户自定义的包括文件，默认在应用同目录是全局配置

            string file = EncodingScrutatorFileStorage.InlcudeFile;
            if (File.Exists(file))
            {
                foreach (var temp in ReadIncludeFile(file))
                {
                    TextFileSuffix.Add(temp);
                }
            }
        }

        public IncludeFileSetting(FileInfo file)
        {
            IncludeWildcardFile = ReadIncludeFile(file.FullName);
        }

        public IncludeFileSetting(List<string> includeWildcardFile)
        {
            IncludeWildcardFile = includeWildcardFile;
        }

        /// <summary>
        ///     包含的文件的通配符
        /// </summary>
        public List<string> IncludeWildcardFile { set; get; } = new List<string>();

        /// <summary>
        ///     常用文本后缀
        /// </summary>
        public static List<string> TextFileSuffix { set; get; }

        /// <summary>
        ///     获取文件包含正则规则
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
                if (!string.IsNullOrEmpty(temp)) includeRegexFile.Add(GetWildcardRegexString(temp));
            }
            return includeRegexFile;
        }

        private static List<string> ReadIncludeFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException("文件" + file + "不存在");
            }
            List<string> includeFile = new List<string>();
            using (StreamReader stream = new StreamReader(new FileStream(file, FileMode.Open)))
            {
                //foreach (var temp in stream.ReadToEnd().Split('\n').Select(temp => temp.Replace("\r", "")))
                //{
                //    if (!string.IsNullOrEmpty(temp))
                //    {
                //        includeFile.Add(temp);
                //    }
                //}
                includeFile.AddRange(
                    stream.ReadToEnd()
                        .Split('\n')
                        .Select(temp => temp.Replace("\r", ""))
                        .Where(temp => !string.IsNullOrEmpty(temp)));
            }
            return includeFile;
        }

        /// <summary>
        ///     常用文件配置
        /// </summary>
        private static void ReadFileSuffix()
        {
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
        }

        private static string GetWildcardRegexString(string wildcardStr)
        {
            Regex replace = new Regex("[.$^{\\[(|)*+?\\\\]");
            string regex = replace.Replace(wildcardStr,
                delegate(Match m)
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