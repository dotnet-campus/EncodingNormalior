using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EncodingNormalior.Model;

namespace EncodingNormalizerVsx.ViewModel
{
    /// <summary>
    ///     定义用户设置
    /// </summary>
    public class DefinitionModel : NotifyProperty
    {
        /// <summary>
        ///     保存用户设置文件夹
        /// </summary>
        private static readonly string _folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                                 "\\EncodingNormalizer\\";

        /// <summary>
        ///     保存用户设置文件
        /// </summary>
        private static readonly string _file = _folder + "Account.json";

        private Account _account;

        private string _criterionEncoding;

        public DefinitionModel()
        {
            //一定会读
            if (Account == null)
                ReadAccount();

            OptionCriterionEncoding = new List<string>();
            foreach (var temp in Enum.GetNames(typeof(CriterionEncoding)))
                OptionCriterionEncoding.Add(temp);

            //获取之前的编码
            CriterionEncoding = OptionCriterionEncoding.First(temp => temp.Equals(Account.CriterionEncoding.ToString()));
        }

        /// <summary>
        ///     可选的编码
        /// </summary>
        public List<string> OptionCriterionEncoding { set; get; }

        public string CriterionEncoding
        {
            set
            {
                _criterionEncoding = value;
                OnPropertyChanged();
            }
            get { return _criterionEncoding; }
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
        private void ReadAccount()
        {
            Account = Account.ReadAccount();
        }


        /// <summary>
        ///     写入用户设置
        /// </summary>
        public bool WriteAccount()
        {
            CriterionEncoding criterionEncoding;
            /*Account.CriterionEncoding =*/
            if (Enum.TryParse(CriterionEncoding, out criterionEncoding))
                Account.CriterionEncoding = criterionEncoding;
            //Account.FileInclude

            //检查白名单
            //if (!)
            //{
            //    MessageBox.Show("不支持指定文件夹中的文件", "白名单格式错误");
            //    return false;
            //}
            if (!ConformWhiteList())
                return false;

            if (!ConformFileInclude())
                return false;

            try
            {
                Account.WriteAccount();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool ConformFileInclude()
        {
            try
            {
                var illegalFile = new List<string>
                {
                    "\\",
                    "/",
                    ":",
                    "\"",
                    "?",
                    "<",
                    ">",
                    "|"
                };
                foreach (var temp in Account.FileInclude.Split('\n').Select(temp => temp.Replace("\r", "")).ToList())
                    if (illegalFile.Any(temp.Contains))
                    {
                        MessageBox.Show("出现文件不能包含字符 \r\n第一处错误在 " + temp, "包含文件格式错误");
                        return false;
                    }
                //    var includeFileSetting = new IncludeFileSetting(
                //        Account.FileInclude.Split('\n').Select(temp => temp.Replace("\r", "")).ToList());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "包含文件格式错误");
                return false;
            }
            return true;
        }

        private bool ConformWhiteList()
        {
            try
            {
                var inspectFileWhiteListSetting =
                    new InspectFileWhiteListSetting(
                        new List<string>(Account.WhiteList.Split('\n').Select(temp => temp.Replace("\r", "")).ToList()));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "白名单格式错误");
                return false;
            }
            return true;
        }
    }
}