using System.Collections.Generic;
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
        //private List<string> _includeRegexFile;
        private List<Regex> _includeRegexFile;

        public EncodingScrutatorFolder(DirectoryInfo folder)
            : this(folder, new InspectFileWhiteListSetting(InspectFileWhiteListSetting.DefaultWhiteList),
                new IncludeFileSetting(IncludeFileSetting.TextFileSuffix))
        {
        }

        public EncodingScrutatorFolder(DirectoryInfo folder, InspectFileWhiteListSetting inspectFileWhiteListSetting,
            IncludeFileSetting includeFileSetting)
        {
            FaceFolder = folder;
            Name = folder.Name;
            InspectFileWhiteListSetting = inspectFileWhiteListSetting;
            IncludeFileSetting = includeFileSetting;
        }

        /// <summary>
        ///     设置或获取文件检测白名单
        /// </summary>
        public InspectFileWhiteListSetting InspectFileWhiteListSetting { set; get; }

        /// <summary>
        ///     获取文件夹名
        /// </summary>
        public string Name { get; }

        public DirectoryInfo Parent { set; get; }

        /// <summary>
        ///     文件夹是否被忽略
        /// </summary>
        public bool Ignore { set; get; }

        /// <summary>
        ///     当前文件夹
        /// </summary>
        private DirectoryInfo FaceFolder { get; }

        /// <summary>
        ///     设置或获取编码规范
        /// </summary>
        public SitpulationEncodingSetting SitpulationEncodingSetting { set; get; } =
            SitpulationEncodingSetting.DefaultSitpulationEncodingSetting;

        /// <summary>
        ///     设置或获取要包含文件
        /// </summary>
        public IncludeFileSetting IncludeFileSetting { set; get; }

        /// <summary>
        ///     设置或获取文件夹包含文件夹
        /// </summary>
        public List<EncodingScrutatorFolder> Folder { set; get; } = new List<EncodingScrutatorFolder>();

        /// <summary>
        ///     文件夹的文件和文件编码
        /// </summary>
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
        ///     获取当前目录下的文件夹递归文件编码
        /// </summary>
        private void InspectFaceFolderEncoding()
        {
            foreach (var temp in FaceFolder.GetDirectories())
            {
                var folder = new EncodingScrutatorFolder(temp, InspectFileWhiteListSetting, IncludeFileSetting)
                {
                    //IncludeFileSetting = IncludeFileSetting,
                    //InspectFileWhiteListSetting = InspectFileWhiteListSetting,
                    SitpulationEncodingSetting = SitpulationEncodingSetting,
                    _includeRegexFile = _includeRegexFile,
                    Parent = FaceFolder
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

        /// <summary>
        ///     获取所有文件编码
        /// </summary>
        private void InspectFileEncoding()
        {
            foreach (var temp in FaceFolder.GetFiles())
                //.Select(temp=>new EncodingScrutatorFile(temp)))//.Where(PredicateInclude))
            {
                var file = new EncodingScrutatorFile(temp)
                {
                    Parent = FaceFolder
                };
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

        /// <summary>
        ///     获取包含文件的规则
        /// </summary>
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
        ///     是否要包含文件
        /// </summary>
        /// <param name="file"></param>
        private bool PredicateInclude(FileInfo file)
        {
            //return _includeRegexFile.Select(temp => new Regex(temp)).Any(regex => regex.IsMatch(file.Name));
            return _includeRegexFile.Any(temp => temp.IsMatch(file.Name)) &&
                   !InspectFileWhiteListSetting.FileRegexWhiteList.Any(temp => temp.IsMatch(file.Name));
        }
    }
}