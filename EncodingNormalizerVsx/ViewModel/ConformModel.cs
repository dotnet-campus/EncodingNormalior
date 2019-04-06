using EncodingNormalior.Model;
using EncodingNormalizerVsx.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EncodingScrutatorFile = EncodingNormalior.Model.EncodingScrutatorFile;
using EncodingScrutatorFolder = EncodingNormalior.Model.EncodingScrutatorFolder;
using IEncodingScrutatorFile = EncodingNormalizerVsx.Model.IEncodingScrutatorFile;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConformModel : NotifyProperty
    {
        public ConformModel()
        {
            Visibility = Visibility.Collapsed;

            InspectCompleted = (s, e) =>
            {
                //输出不规范文件
                PrintConformEncoding();
                ProgressStr = "扫描完成";
            };
        }

        private string _circular;

        private EncodingScrutatorFolder _encodingScrutatorFolder;

        private IncludeFileSetting _includeFile;

        private string _progressStr = "正在扫描……";
        private Visibility _visibility;
        private InspectFileWhiteListSetting _whiteList;
        public EventHandler Closing;

        public EventHandler InspectCompleted;

        /// <summary>
        ///     显示用户可以使用控件
        /// </summary>
        public Visibility Visibility
        {
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
            get { return _visibility; }
        }

        /// <summary>
        ///     不符合编码文件文件夹
        /// </summary>
        public ObservableCollection<EncodingScrutatorFolderFile> EncodingScrutatorFolder { set; get; } =
            new ObservableCollection<EncodingScrutatorFolderFile>();

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

        private Account Account { set; get; }
        private Encoding SitpulationEncoding { set; get; }

        public string ProgressStr
        {
            set
            {
                _progressStr = value;
                OnPropertyChanged();
            }
            get { return _progressStr; }
        }


        /// <summary>
        ///     检查多个文件夹文件编码
        /// </summary>
        /// <param name="folder"></param>
        public void InspectFolderEncoding(List<string> folder)
        {
            EncodingScrutatorProgress progress = new EncodingScrutatorProgress();
            progress.ProgressChanged += Progress_ProgressChanged;

            ParseAccount();
            List<EncodingScrutatorFolder> encodingScrutatorFolder = FolderToVirtualDirectory(folder, progress);
            //};

            new Task(() =>
            {
                foreach (EncodingScrutatorFolder temp in encodingScrutatorFolder)
                {
                    temp.InspectFolderEncoding();
                }
                //通知完成
                progress.Report(null);
            }).Start();
        }

        /// <summary>
        ///     把所有的要检测的文件夹放到虚拟目录
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private List<EncodingScrutatorFolder> FolderToVirtualDirectory(List<string> folder,
            EncodingScrutatorProgress progress)
        {
            List<EncodingScrutatorFolder> encodingScrutatorFolder = new List<EncodingScrutatorFolder>();
            foreach (string temp in folder.Where(temp => !string.IsNullOrEmpty(temp)))
            {
                encodingScrutatorFolder.Add(
                    new EncodingScrutatorFolder(new DirectoryInfo(temp),
                        _whiteList, _includeFile)
                    {
                        Progress = progress,
                        SitpulationEncodingSetting = new SitpulationEncodingSetting
                        {
                            SitpulationEncoding = SitpulationEncoding
                        }
                    });
            }


            _encodingScrutatorFolder = new EncodingScrutatorFolder(new DirectoryInfo(Environment.SystemDirectory))
            {
                Folder = encodingScrutatorFolder
            };

            foreach (EncodingScrutatorFolder temp in encodingScrutatorFolder)
            {
                temp.Parent = _encodingScrutatorFolder;
            }

            return encodingScrutatorFolder;
        }

        /// <summary>
        ///     检查文件夹文件编码
        /// </summary>
        /// <param name="folder"></param>
        public void InspectFolderEncoding(string folder)
        {
            InspectFolderEncoding(new List<string> { folder });
        }

        private void ParseAccount()
        {
            if (Account == null)
            {
                Account = Account.ReadAccount();
            }

            Encoding encoding = Account.ConvertCriterionEncoding();
            SitpulationEncoding = encoding;
            if (_whiteList == null)
            {
                _whiteList =
                    new InspectFileWhiteListSetting(
                        Account.WhiteList.Split('\n').Select(temp => temp.Replace("\r", "")).ToList());
            }
            if (_includeFile == null)
            {
                _includeFile =
                    new IncludeFileSetting(
                        Account.FileInclude.Split('\n')
                            .Select(temp => temp.Replace("\r", ""))
                            .Where(temp => !string.IsNullOrEmpty(temp))
                            .ToList());
            }
        }

        /// <summary>
        ///     输出不规范文件
        /// </summary>
        private int PintnoConformEncodingFile(EncodingScrutatorFolder encodingScrutatorFolder)
        {
            int count = 0;
            for (int i = 0; i < encodingScrutatorFolder.File.Count; i++)
            {
                EncodingScrutatorFile temp = encodingScrutatorFolder.File[i];
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

            count = PintnoConformEncodingFolder(encodingScrutatorFolder, count);
            return count;
        }

        private int PintnoConformEncodingFolder(EncodingScrutatorFolder encodingScrutatorFolder, int count)
        {
            //输出不规范文件夹
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
            if (e == null)
            {
                //完成检测
                InspectCompleted?.Invoke(this, null);
            }
            else
            {
                StringBuilder str = new StringBuilder();
                str.Append(e.File.Name);
                EncodingScrutatorFolder folder = e.Parent;
                while (folder != null)
                {
                    str.Insert(0, folder.Name + "\\");
                    folder = folder.Parent;
                }
                str.Insert(0, "正在扫描\r\n");
                Circular = str.ToString();
            }
        }

        private void PrintConformEncoding()
        {
            StringBuilder str = new StringBuilder();
            str.Append("扫描完成");
            str.Append("\r\n");
            int count = PintnoConformEncodingFile(_encodingScrutatorFolder.Folder);
            str.Append("找到不规范文件" + count);
            str.Append(" 当前编码 " + SitpulationEncoding.EncodingName);
            Visibility = Visibility.Visible;
            PrintConformDispatcherSynchronizationContext(EncodingScrutatorFolderBeCriterion);
            Circular = str.ToString();
            if (count == 0)
            {
                MessageBox.Show("没有发现不规范文件", "编码规范工具");
                PrintConformDispatcherSynchronizationContext(() => { Closing?.Invoke(this, null); });
            }
        }

        private int PintnoConformEncodingFile(List<EncodingScrutatorFolder> encodingScrutatorFolder)
        {
            int count = 0;
            foreach (EncodingScrutatorFolder temp in encodingScrutatorFolder) count += PintnoConformEncodingFile(temp);
            return count;
        }

        private void EncodingScrutatorFolderBeCriterion()
        {
            //foreach (var temp in _encodingScrutatorFolder.Folder)
            //{
            //    var encodingScrutatorFolder = temp;
            //    EncodingScrutatorFolderBeCriterion(encodingScrutatorFolder);
            //}
            EncodingScrutatorFolderBeCriterion(_encodingScrutatorFolder);
        }

        private void EncodingScrutatorFolderBeCriterion(EncodingScrutatorFolder encodingScrutatorFolder)
        {
            foreach (EncodingScrutatorFolder temp in encodingScrutatorFolder.Folder) EncodingScrutatorFolder.Add(new Model.EncodingScrutatorFolder(temp, null));
            foreach (EncodingScrutatorFile temp in encodingScrutatorFolder.File) EncodingScrutatorFolder.Add(new Model.EncodingScrutatorFile(temp, null));
        }

        private void PrintConformDispatcherSynchronizationContext(Action action)
        {
            SynchronizationContext.SetSynchronizationContext(new
                DispatcherSynchronizationContext(Application.Current.Dispatcher));
            SynchronizationContext.Current.Send(obj => { action?.Invoke(); }, null);
        }

        //规范编码之前，确保你已经管理所有代码

        /// <summary>
        ///     规范编码
        /// </summary>
        public void WriteCriterionEncoding()
        {
            EncodingScrutatorProgress progress = new EncodingScrutatorProgress();
            int count = 0;
            progress.WriteSitpulationFileChanged += (s, e) =>
            {
                StringBuilder str = new StringBuilder();
                str.Append(e.GetEncodingScrutatorFileDirectory());
                str.Insert(0, "正在转换编码\r\n");
                Circular = str.ToString();
            };

            progress.ExceptChanged += (s, e) =>
            {
                count++;
                Circular += "\r\n转换出现异常" + e.Message;
            };

            new Task(() =>
            {
                WriteFolderEncoding.WriteSitpulationEncoding(EncodingScrutatorFolder, progress, SitpulationEncoding);
                Circular = "转换完成\r\n ";
                if (count > 0)
                {
                    Circular += "转换失败" + count;
                }
                else
                {
                    MessageBox.Show("转换完成", "编码规范工具");
                    PrintConformDispatcherSynchronizationContext(() => { Closing?.Invoke(this, null); });
                }
                FailWriteSitpulation();
            }).Start();
        }

        /// <summary>
        ///     写入失败文件
        /// </summary>
        private void FailWriteSitpulation()
        {
            SynchronizationContext.SetSynchronizationContext(new
                DispatcherSynchronizationContext(Application.Current.Dispatcher));
            SynchronizationContext.Current.Send(obj =>
            {
                List<IEncodingScrutatorFile> encodingScrutatorFolder = EncodingScrutatorFolder.ToList<IEncodingScrutatorFile>();
                EncodingScrutatorFolder.Clear();
                FailWriteSitpulation(encodingScrutatorFolder);
                foreach (IEncodingScrutatorFile temp in encodingScrutatorFolder) EncodingScrutatorFolder.Add((EncodingScrutatorFolderFile) temp);
            }, null);
        }

        /// <summary>
        ///     显示写入失败的文件或文件夹
        /// </summary>
        /// <param name="encodingScrutatorFolder"></param>
        private void FailWriteSitpulation(List<IEncodingScrutatorFile> encodingScrutatorFolder)
        {
            for (int i = 0; i < encodingScrutatorFolder.Count; i++)
            {
                IEncodingScrutatorFile temp = encodingScrutatorFolder[i];
                if (temp is Model.EncodingScrutatorFile)
                {
                    //如果写入成功，转换编码或不需要转换编码，直接移除
                    if (!temp.Check)
                    {
                        encodingScrutatorFolder.RemoveAt(i);
                        i--;
                    }
                }
                else if (temp is Model.EncodingScrutatorFolder)
                {
                    //如果不需要转换，直接移除
                    if (!temp.Check)
                    {
                        encodingScrutatorFolder.RemoveAt(i);
                        i--;
                    }

                    if (temp.Folder != null)
                    {
                        FailWriteSitpulation(temp.Folder);
                    }
                    //如果没有文件了，直接移除
                    if (temp.Folder == null || temp.Folder.Count == 0)
                    {
                        encodingScrutatorFolder.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}