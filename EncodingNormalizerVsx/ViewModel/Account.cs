using System;
using System.IO;
using System.Linq;
using System.Text;
using EncodingNormalior.Model;
using Newtonsoft.Json;

namespace EncodingNormalizerVsx.ViewModel
{
    /// <summary>
    ///     用户设置
    /// </summary>
    public class Account : NotifyProperty
    {
        public Encoding ConvertCriterionEncoding()
        {
            switch (CriterionEncoding)
            {
                case CriterionEncoding.UTF8:
                    return Encoding.UTF8;
                    break;
                case CriterionEncoding.GBK:
                    return Encoding.GetEncoding("gbk");
                    break;
                case CriterionEncoding.Unicode:
                    return Encoding.Unicode;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 保存用户设置文件夹
        /// </summary>
        [JsonIgnore]
        private static readonly string _folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EncodingNormalizer\\";

        /// <summary>
        /// 保存用户设置文件
        /// </summary>
        [JsonIgnore]
        private static readonly string _file = _folder + "Account.json";


        private CriterionEncoding _criterionEncoding;

        private string _fileInclude;


        private string _whiteList;
        /// <summary>
        /// 包含的文件
        /// </summary>
        public string FileInclude
        {
            set
            {
                _fileInclude = value;
                OnPropertyChanged();
            }
            get { return _fileInclude; }
        }
        /// <summary>
        /// 不包含文件
        /// </summary>
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


        /// <summary>
        ///     读取设置
        /// </summary>
        public static Account ReadAccount()
        {
            //var folder = _folder;
            var file = _file;
            Account account;
            try
            {
                var str = "";
                using (var stream = File.OpenText(file))
                {
                    str = stream.ReadToEnd();
                }
                //ildasm Newtonsoft.Json.dll /out=Newtonsoft.Json.il
                //ilasm Newtonsoft.Json.il /dll /resource=Newtonsoft.Json.res /key=Key.snk /optimize

                account = JsonConvert.DeserializeObject<Account>(str);
            }
            catch (Exception)
            {
                //默认配置
                var whiteList = EncodingNormalior.Resource.TextFileSuffix.WhiteList;
                var fileInclude = IncludeFileSetting.TextFileSuffix.Aggregate("", (current, temp) => current + (temp + "\r\n"));
                //EncodingNormalior.Resource.TextFileSuffix.textFileSuffix;
                account = new Account()
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
            return account;
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

            var str = JsonConvert.SerializeObject(this);

            using (var stream = File.CreateText(file))
            {
                stream.Write(str);
            }

            return true;
        }
    }

    public enum CriterionEncoding
    {
        UTF8,
        GBK,
        Unicode,
    }
}