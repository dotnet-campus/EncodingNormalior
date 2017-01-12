using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConformModel : NotifyProperty
    {
        public ConformModel()
        {

        }

        //规范编码之前，确保你已经管理所有代码

        /// <summary>
        /// 规范编码
        /// </summary>
        public void WriteCriterionEncoding()
        {

        }

        private string _circular;
        /// <summary>
        /// 通知
        /// </summary>
        public string Circular
        {
            set
            {
                _circular = value;
                OnPropertyChanged();
            }
            get
            {
                return _circular;
            }
        }
    }
}
