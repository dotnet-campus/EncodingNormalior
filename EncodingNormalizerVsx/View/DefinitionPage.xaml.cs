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
    /// DefinitionPage.xaml 的交互逻辑
    /// </summary>
    public partial class DefinitionPage : System.Windows.Controls.UserControl
    {
        public DefinitionPage()
        {
            ViewModel = new DefinitionModel();
            InitializeComponent();
            //ViewModel = (DefinitionModel) DataContext;
        }

        public DefinitionModel ViewModel { set; get; }

        private void WriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //保存数据
            if (ViewModel.WriteAccount())
            {
                Closing?.Invoke(this, null);
            }
            
        }

        /// <summary>
        /// 通知窗口关闭
        /// </summary>
        public EventHandler Closing;

        private void AbandonButton_OnClick(object sender, RoutedEventArgs e)
        {
            Closing?.Invoke(this,null);
        }
    }
}
