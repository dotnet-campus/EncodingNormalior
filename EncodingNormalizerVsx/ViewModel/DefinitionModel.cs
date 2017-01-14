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
        private static readonly string _folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EncodingNormalizer\\";
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
            }

            OptionCriterionEncoding = new List<string>();
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
        private void ReadAccount()
        {
            Account = Account.ReadAccount();
        }


        /// <summary>
        ///     写入用户设置
        /// </summary>
        public bool WriteAccount()
        {
            //todo: 校验Account
            ViewModel.CriterionEncoding criterionEncoding;
            /*Account.CriterionEncoding =*/
            if (ViewModel.CriterionEncoding.TryParse(CriterionEncoding, out criterionEncoding))
            {
                Account.CriterionEncoding = criterionEncoding;
            }
            
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
    }
}