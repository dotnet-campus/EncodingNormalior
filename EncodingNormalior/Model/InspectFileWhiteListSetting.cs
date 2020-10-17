using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EncodingNormalior.Annotations;

namespace EncodingNormalior.Model
{
    ///// <summary>
    ///// 后缀
    ///// </summary>
    //public class Suffix
    //{
    //    public Suffix()
    //    {

    //    }

    //    /// <summary>
    //    /// 图片后缀
    //    /// </summary>
    //    public List<string> ImageSuffix { set; get; } = new List<string>()
    //    {
    //        "*.bmp","*.jpg","*.gif","*.png","*.dxf","*.cdr"
    //    };

    //    /// <summary>
    //    /// 压缩文件后缀
    //    /// </summary>
    //    public List<string> CompressFileSuffix { set; get; }=new List<string>()
    //    {
    //        "*.zip","*.rar","*.7z",
    //        //"*.",
    //    };

    //}

    /// <summary>
    ///     文件检测白名单设置
    /// </summary>
    public class InspectFileWhiteListSetting : ISetting
    {
        static InspectFileWhiteListSetting()
        {
            DefaultWhiteList = new List<string>();
            var file = EncodingScrutatorFileStorage.WhiteListFile;
            IEnumerable<string> fileSuffix = ReadWhiteList(file);

            foreach (var temp in fileSuffix)
            {
                DefaultWhiteList.Add(temp);
            }
        }

        public InspectFileWhiteListSetting([NotNull] List<string> whiteList)
        {
            foreach (var temp in whiteList.Where(temp=>!string.IsNullOrEmpty(temp)))
            {
                Parse(temp);
            }
        }

        private static Regex _folderRegex = new Regex("\\w+[\\\\|/]");

        public static List<string> DefaultWhiteList { set; get; }

        public IReadOnlyList<string> FileWhiteList { get; } = new List<string>();
        public IReadOnlyList<string> FolderWhiteList { get; } = new List<string>();
        public IReadOnlyList<Regex> FileRegexWhiteList { get; } = new List<Regex>();

        /// <summary>
        ///     获取文件配置白名单
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns>白名单</returns>
        public static InspectFileWhiteListSetting ReadWhiteListSetting(string file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException("文件不存在" + file);
            }
            var whiteList = new List<string>();
            using (StreamReader stream = new StreamReader(
                new FileStream(file, FileMode.Open)))
            {
                whiteList.AddRange(stream.ReadToEnd().Split('\n').Select(temp => temp.Replace("\r", "").Trim()));
            }

            InspectFileWhiteListSetting inspectFileWhiteListSetting = new InspectFileWhiteListSetting(whiteList);
            return inspectFileWhiteListSetting;
        }

        private static IEnumerable<string> ReadWhiteList(string file)
        {
            string[] whiteList;
            if (File.Exists(file))
            {
                using (StreamReader stream = new StreamReader(
                    new FileStream(file, FileMode.Open)))
                {
                    whiteList = stream.ReadToEnd().Split('\n');
                }
            }
            else
            {
                whiteList = EncodingNormaliorContext.WhiteList.Split('\n');
            }

            var fileSuffix = whiteList.Select(temp => temp.Replace("\r", "").Trim());
            return fileSuffix;
        }

        public void Add(string whiteList)
        {
            Parse(whiteList);
        }

        public void Remove(string whiteList)
        {
            var folderWhiteList = ((List<string>)FolderWhiteList);

            Remove(whiteList, folderWhiteList);
            folderWhiteList = (List<string>)FileWhiteList;
            Remove(whiteList, folderWhiteList);
        }

        private static void Remove(string whiteList, List<string> folderWhiteList)
        {
            for (int i = 0; i < folderWhiteList.Count; i++)
            {
                if (string.Equals(folderWhiteList[i], whiteList))
                {
                    folderWhiteList.RemoveAt(i);
                }
            }
        }

        public void Add(List<string> whiteList)
        {
            foreach (var temp in whiteList)
            {
                Parse(temp);
            }
        }

        private void Parse(string whiteList)
        {
            _folderRegex = new Regex("\\w+[\\\\|/]$");

            if (_folderRegex.IsMatch(whiteList))
            {
                ((List<string>)FolderWhiteList).Add(whiteList.Substring(0, whiteList.Length - 1));
            }
            else
            {
                if (whiteList.Contains("\\") || whiteList.Contains("/"))
                {
                    throw new ArgumentException("不支持指定文件夹中的文件\r\n" + whiteList + " 错误");
                }
                ((List<string>)FileWhiteList).Add(whiteList);
                ((List<Regex>)FileRegexWhiteList).Add(new Regex(GetWildcardRegexString(whiteList),
                    RegexOptions.IgnoreCase));
            }
        }

        /// <summary>
        ///     判断白名单是否有效
        /// </summary>
        /// <param name="whiteList"></param>
        public bool ConformWhiteList(string whiteList)
        {
            try
            {
                foreach (var temp in whiteList.Split('\n').Select(temp => temp.Replace("\r", "")).ToList())
                {
                    Parse(temp);
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        ///// <summary>
        /////     设置或获取白名单
        ///// </summary>
        //public List<string> WhiteList { set; get; } = new List<string>();
        //忽略文件夹      文件夹\
        //忽略文件        文件
        //忽略后缀        *.后缀

        private static string GetWildcardRegexString(string wildcardStr)
        {
            //Regex replace = new Regex("[.$^{\\[(|)*+?\\\\]");
            //return replace.Replace(wildcardStr,
            //    delegate (Match m)
            //    {
            //        switch (m.Value)
            //        {
            //            case "?":
            //                return ".?";
            //            case "*":
            //                return ".*";
            //            default:
            //                return "\\" + m.Value;
            //        }
            //    }) + "$";
            return WildcardRegexString.GetWildcardRegexString(wildcardStr);
        }
    }
}