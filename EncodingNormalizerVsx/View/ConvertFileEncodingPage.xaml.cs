using EncodingNormalizerVsx.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EncodingNormalizerVsx.View
{
    /// <summary>
    /// ConvertFileEncodingPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConvertFileEncodingPage : Window
    {
        public ConvertFileEncodingPage(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                throw new ArgumentException($"文件{file}找不到");
            }

            ViewModel = new ConvertFileEncodingModel()
            {
                File = new FileInfo(file)
            };

            DataContext = ViewModel;

            InitializeComponent();

            CurrentFile.Text = "CurrentFile:" + ViewModel.File.FullName;
        }

        public ConvertFileEncodingModel ViewModel { get; }

        private void ConvertFile_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ConvertFile();
        }
    }
}