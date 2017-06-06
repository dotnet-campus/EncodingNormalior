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

namespace EncodingNormalizerVsx.View
{
    /// <summary>
    /// EncodingPage.xaml 的交互逻辑
    /// </summary>
    public partial class EncodingPage : UserControl
    {
        public EncodingPage()
        {
            InitializeComponent();
        }

        public EventHandler Closing;

        public string File { get; set; }
    }
}
