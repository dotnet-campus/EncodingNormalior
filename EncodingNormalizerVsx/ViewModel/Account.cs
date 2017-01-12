using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EncodingNormalior.Model;
using Newtonsoft.Json;

namespace EncodingNormalizerVsx.ViewModel
{
    /// <summary>
    ///     定义用户设置
    /// </summary>
    public class DefinitionModel : NotifyProperty
    {
        /// <summary>
        /// 保存用户设置文件夹
        /// </summary>
        private static readonly string _folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            /// <summary>
            /// 保存用户设置文件
            /// </summary>
        private static readonly string _file = _folder + "Account.json";

        private Account _account;

        public DefinitionModel()
        {
            //一定会读
            if (Account == null)
            {
                ReadAccount();
                /*
                 * “System.IO.FileLoadException”类型的异常在 EncodingNormalizerVsx.dll 中发生，但未在用户代码中进行处理

                   其他信息: 未能加载文件或程序集“EncodingNormalior, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null”或它的某一个依赖项。需要强名称程序集。 (异常来自 HRESULT:0x80131044)
                 */
            }

            OptionCriterionEncoding =new List<string>();
            foreach (var temp in Enum.GetNames(typeof(CriterionEncoding)))
            {
                OptionCriterionEncoding.Add(temp);
            }

            //获取之前的编码
            CriterionEncoding = OptionCriterionEncoding.First(temp => temp.Equals(Account.CriterionEncoding.ToString()));
        }

        /// <summary>
        /// 可选的编码
        /// </summary>
        public List<string> OptionCriterionEncoding { set; get; }

        private string _criterionEncoding;

        public string CriterionEncoding
        {
            set
            {
                _criterionEncoding = value;
                OnPropertyChanged();
            }
            get
            {
                return _criterionEncoding;
            }
        }
        /// <summary>
        ///     用户设置
        /// </summary>
        public Account Account
        {
            set
            {
                _account = value;
                OnPropertyChanged();
            }
            get { return _account; }
        }

        /// <summary>
        ///     读取设置
        /// </summary>
        public void ReadAccount()
        {
            var folder = _folder;
            var file = _file;

            try
            {
                var str = "";
                using (var stream = File.OpenText(file))
                {
                    str = stream.ReadToEnd();
                }
                //ildasm Newtonsoft.Json.dll /out=Newtonsoft.Json.il
                //ilasm Newtonsoft.Json.il /dll /resource=Newtonsoft.Json.res /key=Key.snk /optimize

                Account = JsonConvert.DeserializeObject<Account>(str);
            }
            catch (Exception)
            {
                //默认配置
                var whiteList = EncodingNormalior.Resource.TextFileSuffix.WhiteList;
                var fileInclude = EncodingNormalior.Resource.TextFileSuffix.textFileSuffix;
                Account=new Account()
                {
                    CriterionEncoding = ViewModel.CriterionEncoding.UTF8,
                    FileInclude = fileInclude,
                    WhiteList = whiteList
                };
                //foreach (var temp in InspectFileWhiteListSetting.DefaultWhiteList)
                //{
                //    whiteList += temp + "\r\n";
                //}
            }
            //var folder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            //             "\\EncodingNormalizer\\";
            //var file = folder + "Account.json";
            //if (!Directory.Exists(folder))
            //{
            //    Directory.CreateDirectory(folder);
            //}

            //using (StreamWriter stream = File.CreateText(file))
            //{
            //    stream.Write("EncodingNormalizer");
            //}

            //using (StreamReader stream = File.OpenText(file))
            //{
            //    string str = stream.ReadToEnd();
            //}
        }


        /// <summary>
        ///     写入用户设置
        /// </summary>
        public bool WriteAccount()
        {
            //校验Account

            var folder = _folder;
            var file = _file;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var str = JsonConvert.SerializeObject(Account);

            using (var stream = File.CreateText(file))
            {
                stream.Write(str);
            }

            return true;
        }
    }

    /// <summary>
    ///     用户设置
    /// </summary>
    public class Account : NotifyProperty
    {
        private CriterionEncoding _criterionEncoding;

        private string _fileInclude;


        private string _whiteList;

        public string FileInclude
        {
            set
            {
                _fileInclude = value;
                OnPropertyChanged();
            }
            get { return _fileInclude; }
        }

        public string WhiteList
        {
            set
            {
                _whiteList = value;
                OnPropertyChanged();
            }
            get { return _whiteList; }
        }

        public CriterionEncoding CriterionEncoding
        {
            set
            {
                _criterionEncoding = value;
                OnPropertyChanged();
            }
            get { return _criterionEncoding; }
        }
    }

    public enum CriterionEncoding
    {
        UTF8,
        GBK
    }
}