using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EncodingNormalizerVsx.ViewModel;

namespace EncodingNormalizerVsx.View
{
    /// <summary>
    /// ConformPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConformPage : UserControl
    {
        public ConformPage()
        {
            ViewModel = new ConformModel();
            InitializeComponent();

        }

        public void InspectFolderEncoding()
        {
            ViewModel.InspectFolderEncoding(SolutionFolder);
        }

        public EventHandler Closing;

        /// <summary>
        /// 项目文件夹
        /// </summary>
        public string SolutionFolder { set; get; }

        public ConformModel ViewModel { set; get; }
    }
}
