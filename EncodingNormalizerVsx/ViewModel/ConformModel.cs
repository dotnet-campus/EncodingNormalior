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
using EncodingNormalior.Model;
using EncodingNormalizerVsx.Model;
using EncodingScrutatorFile = EncodingNormalior.Model.EncodingScrutatorFile;
using EncodingScrutatorFolder = EncodingNormalior.Model.EncodingScrutatorFolder;
using IEncodingScrutatorFile = EncodingNormalizerVsx.Model.IEncodingScrutatorFile;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConformModel : NotifyProperty
    {
        private string _circular;

        //private List<EncodingScrutatorFolder> _encodingScrutatorFolder;
        private EncodingScrutatorFolder _encodingScrutatorFolder;

        private IncludeFileSetting _includeFile;
        private Visibility _visibility;
        private InspectFileWhiteListSetting _whiteList;
        public EventHandler Closing;

        public ConformModel()
        {
            Visibility = Visibility.Collapsed;
        }

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

        /// <summary>
        ///     检查多个文件夹文件编码
        /// </summary>
        /// <param name="folder"></param>
        public void InspectFolderEncoding(List<string> folder)
        {
            var progress = new EncodingScrutatorProgress();
            progress.ProgressChanged += Progress_ProgressChanged;
            ParseAccount();
            var encodingScrutatorFolder = new List<EncodingScrutatorFolder>();
            foreach (var temp in folder)
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


            _encodingScrutatorFolder //= encodingScrutatorFolder;
                //=new List<EncodingScrutatorFolder>()
                // {
                = new EncodingScrutatorFolder(new DirectoryInfo(Environment.SystemDirectory))
                {
                    Folder = encodingScrutatorFolder
                };

            foreach (var temp in encodingScrutatorFolder)
                temp.Parent = _encodingScrutatorFolder;
            //};

            new Task(() =>
            {
                foreach (var temp in encodingScrutatorFolder)
                    temp.InspectFolderEncoding();
                progress.Report(null);
            }).Start();
        }

        /// <summary>
        ///     检查文件夹文件编码
        /// </summary>
        /// <param name="folder"></param>
        public void InspectFolderEncoding(string folder)
        {
            InspectFolderEncoding(new List<string> {folder});
        }

        private void ParseAccount()
        {
            if (Account == null)
                Account = Account.ReadAccount();

            var encoding = Account.ConvertCriterionEncoding();
            SitpulationEncoding = encoding;
            if (_whiteList == null)
                _whiteList =
                    new InspectFileWhiteListSetting(
                        Account.WhiteList.Split('\n').Select(temp => temp.Replace("\r", "")).ToList());
            if (_includeFile == null)
                _includeFile =
                    new IncludeFileSetting(
                        Account.FileInclude.Split('\n')
                            .Select(temp => temp.Replace("\r", ""))
                            .Where(temp => !string.IsNullOrEmpty(temp))
                            .ToList());
        }

        /// <summary>
        ///     输出不规范文件
        /// </summary>
        private int PintnoConformEncodingFile(EncodingScrutatorFolder encodingScrutatorFolder)
        {
            var count = 0;
            for (var i = 0; i < encodingScrutatorFolder.File.Count; i++)
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

            for (var i = 0; i < encodingScrutatorFolder.Folder.Count; i++)
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
                PrintConformEncoding();
            }
            else
            {
                var str = new StringBuilder();
                str.Append(e.File.Name);
                var folder = e.Parent;
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
            var str = new StringBuilder();
            str.Append("扫描完成");
            str.Append("\r\n");
            var count = PintnoConformEncodingFile(_encodingScrutatorFolder.Folder);
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
            var count = 0;
            foreach (var temp in encodingScrutatorFolder)
                count += PintnoConformEncodingFile(temp);
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
            foreach (var temp in encodingScrutatorFolder.Folder)
                EncodingScrutatorFolder.Add(new Model.EncodingScrutatorFolder(temp, null));
            foreach (var temp in encodingScrutatorFolder.File)
                EncodingScrutatorFolder.Add(new Model.EncodingScrutatorFile(temp, null));
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
            var progress = new EncodingScrutatorProgress();
            var count = 0;
            progress.WriteSitpulationFileChanged += (s, e) =>
            {
                var str = new StringBuilder();
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
                new WriteFolderEncoding().
                    WriteSitpulationEncoding(EncodingScrutatorFolder, progress, SitpulationEncoding);
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
                var encodingScrutatorFolder = EncodingScrutatorFolder.ToList<IEncodingScrutatorFile>();
                EncodingScrutatorFolder.Clear();
                FailWriteSitpulation(encodingScrutatorFolder);
                foreach (var temp in encodingScrutatorFolder)
                    EncodingScrutatorFolder.Add((EncodingScrutatorFolderFile) temp);
            }, null);
        }

        private void FailWriteSitpulation(List<IEncodingScrutatorFile> encodingScrutatorFolder)
        {
            for (var i = 0; i < encodingScrutatorFolder.Count; i++)
            {
                var temp = encodingScrutatorFolder[i];
                if (temp is Model.EncodingScrutatorFile)
                {
                    if (!temp.Check)
                    {
                        encodingScrutatorFolder.RemoveAt(i);
                        i--;
                    }
                }
                else if (temp is Model.EncodingScrutatorFolder)
                {
                    if (!temp.Check)
                    {
                        encodingScrutatorFolder.RemoveAt(i);
                        i--;
                    }
                    if (temp.Folder != null)
                        FailWriteSitpulation(temp.Folder);
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