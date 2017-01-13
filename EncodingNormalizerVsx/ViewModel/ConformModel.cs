using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using EncodingNormalior.Model;
using EncodingNormalizerVsx.Model;
using EncodingScrutatorFile = EncodingNormalior.Model.EncodingScrutatorFile;
using EncodingScrutatorFolder = EncodingNormalior.Model.EncodingScrutatorFolder;
using IEncodingScrutatorFile = EncodingNormalizerVsx.Model.IEncodingScrutatorFile;

//using Microsoft.VisualStudio.Shell;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConformModel : NotifyProperty
    {
        public ConformModel()
        {
            Visibility = Visibility.Collapsed;
        }

        private string _circular;

        private EncodingScrutatorFolder _encodingScrutatorFolder;
        private Visibility _visibility;
        private InspectFileWhiteListSetting _whiteList;
        private IncludeFileSetting _includeFile;

        public Visibility Visibility
        {
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
            get { return _visibility; }
        }

        public ObservableCollection<Model.EncodingScrutatorFolderFile> EncodingScrutatorFolder { set; get; } = new ObservableCollection<EncodingScrutatorFolderFile>();

        //public EncodingScrutatorFolder EncodingScrutatorFolder
        //{
        //    set
        //    {
        //        _encodingScrutatorFolder = value;
        //        OnPropertyChanged();
        //    }
        //    get { return _encodingScrutatorFolder; }
        //}

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

        public void InspectFolderEncoding(List<string> folder)
        {
            foreach (var temp in folder)
            {
                InspectFolderEncoding(temp);
            }
        }

        public void InspectFolderEncoding(string folder)
        {
            if (Account == null)
            {
                Account = Account.ReadAccount();
            }

            var progress = new EncodingScrutatorProgress();
            progress.ProgressChanged += Progress_ProgressChanged;
            var encoding = Account.ConvertCriterionEncoding();
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
                    new IncludeFileSetting(Account.FileInclude.Split('\n').Select(temp => temp.Replace("\r", "")).ToList());
            }
            var encodingScrutatorFolder = new EncodingScrutatorFolder(new DirectoryInfo(folder),
                _whiteList, _includeFile)
            {
                Progress = progress,
                SitpulationEncodingSetting = new SitpulationEncodingSetting
                {
                    SitpulationEncoding = encoding
                }
            };

            if (_encodingScrutatorFolder == null)
            {
                _encodingScrutatorFolder = encodingScrutatorFolder;
            }
            else
            {
                _encodingScrutatorFolder.Folder.Add(encodingScrutatorFolder);
            }

            new Task(() =>
            {
                encodingScrutatorFolder.InspectFolderEncoding();
                progress.Report(null);
            }).Start();
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
            var str = new StringBuilder();
            if (e == null)
            {
                str.Append("扫描完成");
                str.Append("\r\n");
                str.Append("找到不规范文件" + PintnoConformEncodingFile(_encodingScrutatorFolder));
                str.Append(" 当前编码 " +
                           _encodingScrutatorFolder.SitpulationEncodingSetting.SitpulationEncoding.EncodingName);
                Visibility = Visibility.Visible;
                // EncodingScrutatorFolder = _encodingScrutatorFolder;

                SynchronizationContext.SetSynchronizationContext(new
   DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Send(obj =>
                {
                    foreach (var temp in _encodingScrutatorFolder.Folder)
                    {
                        EncodingScrutatorFolder.Add(new Model.EncodingScrutatorFolder(temp, null));
                    }
                    foreach (var temp in _encodingScrutatorFolder.File)
                    {
                        EncodingScrutatorFolder.Add(new Model.EncodingScrutatorFile(temp, null));
                    }
                }, null);
            }
            else
            {
                //str.Append("正在扫描\r\n");
                str.Append(e.File.Name);
                var folder = e.Parent;
                while (folder != null)
                {
                    str.Insert(0, folder.Name + "\\");
                    folder = folder.Parent;
                }
                str.Insert(0, "正在扫描\r\n");
            }
            Circular = str.ToString();
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
                var folder = e.Parent;
                str.Append(e.File.Name);
                while (folder != null)
                {
                    str.Insert(0, folder.Name + "\\");
                    folder = folder.Parent;
                }
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
                WriteSitpulationEncoding(EncodingScrutatorFolder, progress, SitpulationEncoding);
                Circular = "转换完成\r\n ";
                if (count > 0)
                {
                    Circular += "转换失败" + count;
                }
                FailWriteSitpulation();
            }).Start();
        }

        /// <summary>
        /// 写入失败文件
        /// </summary>
        private void FailWriteSitpulation()
        {
            SynchronizationContext.SetSynchronizationContext(new
DispatcherSynchronizationContext(Application.Current.Dispatcher));
            SynchronizationContext.Current.Send(obj =>
            {
                var encodingScrutatorFolder = EncodingScrutatorFolder.ToList<Model.IEncodingScrutatorFile>();
                EncodingScrutatorFolder.Clear();
                FailWriteSitpulation(encodingScrutatorFolder);
                foreach (var temp in encodingScrutatorFolder)
                {
                    EncodingScrutatorFolder.Add((EncodingScrutatorFolderFile)temp);
                }
                // FailWriteSitpulation(encodingScrutatorFolder);
                //for (int i = 0; i < encodingScrutatorFolder.Count; i++)
                //{
                //    var temp = encodingScrutatorFolder[i];
                //    if (temp is Model.EncodingScrutatorFile)
                //    {
                //        if (temp.Check)
                //        {
                //            encodingScrutatorFolder.RemoveAt(i);
                //            i--;
                //        }
                //    }
                //    else if(temp is Model.EncodingScrutatorFolder)
                //    {
                //        FailWriteSitpulation(temp);
                //    }
                //}
            }, null);
        }

        private void FailWriteSitpulation(List<IEncodingScrutatorFile> encodingScrutatorFolder)
        {
            //var encodingScrutatorFolder = folder.Folder;
            for (int i = 0; i < encodingScrutatorFolder.Count; i++)
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
                    {
                        FailWriteSitpulation(temp.Folder);
                    }
                    if (temp.Folder == null || temp.Folder.Count == 0)
                    {
                        encodingScrutatorFolder.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        //private void FailWriteSitpulation(IEncodingScrutatorFile folder)
        //{
        //    var encodingScrutatorFolder = folder.Folder;
        //    for (int i = 0; i < encodingScrutatorFolder.Count; i++)
        //    {
        //        var temp = encodingScrutatorFolder[i];
        //        if (temp is Model.EncodingScrutatorFile)
        //        {
        //            if (temp.Check)
        //            {
        //                encodingScrutatorFolder.RemoveAt(i);
        //                i--;
        //            }
        //        }
        //        else if (temp is Model.EncodingScrutatorFolder)
        //        {
        //            FailWriteSitpulation(temp);
        //        }
        //    }
        //}


        /// <summary>
        /// 把所有的文件写编码规范
        /// </summary>
        private void WriteSitpulationEncoding(IEnumerable<IEncodingScrutatorFile> encodingScrutatorFolder, EncodingScrutatorProgress progress, Encoding encoding)
        {
            foreach (var temp in encodingScrutatorFolder.Where(temp => temp.Check))
            {
                if (temp is Model.EncodingScrutatorFile)
                {
                    var file = (Model.EncodingScrutatorFile)temp;
                    progress.ReportWriteSitpulationFile(file);
                    try
                    {
                        file.WriteSitpulationEncoding(encoding);
                        file.Check = false;
                    }
                    catch (Exception e)
                    {
                        progress.ReportExcept(e);
                    }
                }
                else if (temp is Model.EncodingScrutatorFolder)
                {
                    WriteSitpulationEncoding(((Model.EncodingScrutatorFolder)temp).Folder, progress, encoding);
                }
            }
        }
    }


}