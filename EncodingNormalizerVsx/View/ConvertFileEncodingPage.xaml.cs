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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EncodingNormalizerVsx.View
{
    /// <summary>
    /// ConvertFileEncodingPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConvertFileEncodingPage : Window
    {
        public ConvertFileEncodingPage(FileInfo file)
        {
            // 这里不使用 file.Exists 的原因在于，如果直接判断拿到的是在创建 FileInfo 时的缓存的值
            // 如果一开始创建 FileInfo 不存在这个文件，然后后续才创建文件
            // 因为在 FileInfo 创建的时候就会获取这个文件的值，拿到了文件不存在，写入 Exists 缓存
            // 那么通过 file.Exists 拿到的都是文件不存在，即使后续创建了文件
            // 或反过来，在创建 FileInfo 的时候存在文件，然后删除文件，拿到 file.Exists 都是存在
            // 在调用 file.Refresh(); 才会更新当前文件是否存在，但是这个方法需要更新太多属性，所以就直接使用静态方法判断文件存在
            if (!File.Exists(file.FullName))
            {
                throw new ArgumentException($"文件{file}找不到");
            }

            ViewModel = new ConvertFileEncodingModel()
            {
                File = file
            };

            DataContext = ViewModel;

            InitializeComponent();

            CurrentFile.Text = "CurrentFile: " + ViewModel.File.FullName;
        }

        public ConvertFileEncodingModel ViewModel { get; }

        private void ConvertFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ConvertFile())
            {
                var storyboard = (Storyboard) FindResource("SuccessStoryboard");
                storyboard.Completed += async (o, args) =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Close();
                };
                storyboard.Begin();
            }
            else
            {
                var storyboard = (Storyboard) FindResource("FailStoryboard");
                storyboard.Begin();
            }
        }
    }
}