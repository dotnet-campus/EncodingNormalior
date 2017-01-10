﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     包括文件夹和文件
    /// </summary>
    public class EncodingScrutatorFolder
    {
        /// <summary>
        ///     设置或获取文件检测白名单
        /// </summary>
        public InspectFileWhiteListSetting InspectFileWhiteListSetting = new InspectFileWhiteListSetting(new List<string>()
        {
            "bin\\","*.png"
        });



        public EncodingScrutatorFolder(DirectoryInfo folder)
        {
            FaceFolder = folder;
            Name = folder.Name;
        }

        public string Name { set; get; }

        public bool Ignore { set; get; }
        /// <summary>
        ///     当前文件夹
        /// </summary>
        private DirectoryInfo FaceFolder { get; }

        /// <summary>
        ///     设置或获取编码规范
        /// </summary>
        public ISetting SitpulationEncodingSetting { set; get; } =
            Model.SitpulationEncodingSetting.DefaultSitpulationEncodingSetting;
        /// <summary>
        /// 设置或获取要包含文件
        /// </summary>
        public IncludeFileSetting IncludeFileSetting { set; get; } = new IncludeFileSetting()
        {
            IncludeWildcardFile = new List<string>() { "*" }
        };

        public List<EncodingScrutatorFolder> Folder { set; get; } = new List<EncodingScrutatorFolder>();

        public List<EncodingScrutatorFile> File { set; get; }
            = new List<EncodingScrutatorFile>();

        /// <summary>
        ///     获取文件夹所有文件的编码
        /// </summary>
        public void InspectFolderEncoding()
        {
            //递归获取所有的文件夹
            if (!FaceFolder.Exists)
            {
                return;
            }
            GetaIncludeRegexFile();
            InspectFileEncoding();

            InspectFaceFolderEncoding();
        }
        /// <summary>
        /// 获取当前目录下的文件夹递归文件编码
        /// </summary>
        private void InspectFaceFolderEncoding()
        {
            foreach (var temp in FaceFolder.GetDirectories())
            {
                var folder = new EncodingScrutatorFolder(temp)
                {
                    IncludeFileSetting = IncludeFileSetting,
                    InspectFileWhiteListSetting = InspectFileWhiteListSetting,
                    SitpulationEncodingSetting = SitpulationEncodingSetting,
                    _includeRegexFile = _includeRegexFile,
                };

                Folder.Add(folder);
                //不包含
                if (InspectFileWhiteListSetting.FolderWhiteList.Any(t => string.Equals(t, temp.Name)))
                {
                    folder.Ignore = true;
                }
                if (!folder.Ignore)
                {
                    //递归
                    folder.InspectFolderEncoding();
                }
            }
        }

        private void InspectFileEncoding()
        {
            foreach (var temp in FaceFolder.GetFiles())//.Select(temp=>new EncodingScrutatorFile(temp)))//.Where(PredicateInclude))
            {
                var file = new EncodingScrutatorFile(temp);
                File.Add(file);
                //文件是否包含
                if (!PredicateInclude(temp))
                {
                    file.Ignore = true;
                }


                if (!file.Ignore)
                {
                    //是否是文本

                    new EncodingScrutator(file).InspectFileEncoding();
                }
            }
        }

        private void GetaIncludeRegexFile()
        {
            if (_includeRegexFile == null)
            {
                //_includeRegexFile = IncludeFileSetting.GetaIncludeRegexFile();
                _includeRegexFile = new List<Regex>();
                foreach (var temp in IncludeFileSetting.GetaIncludeRegexFile())
                {
                    _includeRegexFile.Add(new Regex(temp));
                }
            }
        }


        /// <summary>
        /// 是否要包含文件
        /// </summary>
        /// <param name="file"></param>
        private bool PredicateInclude(FileInfo file)
        {
            //return _includeRegexFile.Select(temp => new Regex(temp)).Any(regex => regex.IsMatch(file.Name));
            return _includeRegexFile.Any(temp => temp.IsMatch(file.Name))  && 
                   !InspectFileWhiteListSetting.FileRegexWhiteList.Any(temp=>temp.IsMatch(file.Name));
        }

        //private List<string> _includeRegexFile;
        private List<Regex> _includeRegexFile;
    }
}