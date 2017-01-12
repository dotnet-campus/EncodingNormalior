using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using EncodingNormalior.Model;
using Microsoft.VisualStudio.Shell;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConformModel : NotifyProperty
    {
        private Visibility _visibility;

        public Visibility Visibility
        {
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
            get { return _visibility; }
        }
        private string _circular;

        public EncodingScrutatorFolder EncodingScrutatorFolder { set; get; }

        /// <summary>
        ///     通知
        /// </summary>
        public string Circular
        {
            set
            {
                _circular = value;
                OnPropertyChanged();
            }
            get { return _circular; }
        }

        public void InspectFolderEncoding(string folder)
        {
            Account = Account.ReadAccount();
            Progress<EncodingScrutatorFile> progress = new Progress<EncodingScrutatorFile>();
            progress.ProgressChanged += Progress_ProgressChanged;
            Encoding encoding = Account.ConvertCriterionEncoding();
            InspectFileWhiteListSetting whiteList = new InspectFileWhiteListSetting(Account.WhiteList.Split('\n').Select(temp => temp.Replace("\r", "")).ToList());
            IncludeFileSetting includeFile = new IncludeFileSetting(Account.FileInclude.Split('\n').Select(temp => temp.Replace("\r", "")).ToList());
            EncodingScrutatorFolder encodingScrutatorFolder = new EncodingScrutatorFolder(new DirectoryInfo(folder),
               whiteList, includeFile)
            {
                Progress = progress,
                SitpulationEncodingSetting = new SitpulationEncodingSetting()
                {
                    SitpulationEncoding = encoding
                }
            };

            _encodingScrutatorFolder = encodingScrutatorFolder;

            new System.Threading.Tasks.Task(() =>
            {
                encodingScrutatorFolder.InspectFolderEncoding();
            }).Start();
        }

        private EncodingScrutatorFolder _encodingScrutatorFolder;

        /// <summary>
        /// 输出不规范文件
        /// </summary>
        private int PintnoConformEncodingFile(EncodingScrutatorFolder encodingScrutatorFolder)
        {
            Visibility=Visibility.Collapsed;
            int count = 0;
            for (int i = 0; i < encodingScrutatorFolder.File.Count; i++)
            {
                var temp = encodingScrutatorFolder.File[i];
                if (!temp.Ignore)
                {
                    if (encodingScrutatorFolder.SitpulationEncodingSetting.ConformtotheDefaultEncoding(temp.Encoding))
                    {
                        encodingScrutatorFolder.File.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        count++;
                    }
                }
                else
                {
                    encodingScrutatorFolder.File.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < encodingScrutatorFolder.Folder.Count; i++)
            {
                count += PintnoConformEncodingFile(encodingScrutatorFolder.Folder[i]);
                if (encodingScrutatorFolder.Folder[i].Folder.Count == 0 &&
                    encodingScrutatorFolder.Folder[i].File.Count == 0)
                {
                    encodingScrutatorFolder.Folder.RemoveAt(i);
                    i--;
                }
            }
            return count;
        }

        private void Progress_ProgressChanged(object sender, EncodingScrutatorFile e)
        {
            StringBuilder str = new StringBuilder();
            if (e == null)
            {
                str.Append("扫描完成");
                str.Append("\r\n");
                str.Append("找到不规范文件"+ PintnoConformEncodingFile(_encodingScrutatorFolder));
                Visibility=Visibility.Visible;
            }
            else
            {
                //str.Append("正在扫描\r\n");
                str.Append(e.File.Name);
                EncodingScrutatorFolder folder = e.Parent;
                while (folder != null)
                {
                    str.Insert(0, folder.Name + "\\");
                    folder = folder.Parent;
                }
                str.Insert(0, "正在扫描\r\n");
            }
            Circular = str.ToString();
        }

        private Account Account { set; get; }


        //规范编码之前，确保你已经管理所有代码

        /// <summary>
        ///     规范编码
        /// </summary>
        public void WriteCriterionEncoding()
        {
        }
    }
}