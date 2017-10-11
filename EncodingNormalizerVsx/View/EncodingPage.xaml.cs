using System;
using System.Collections.Generic;
using System.IO;
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
using EncodingNormalior.Model;

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
            DataContext = this;
        }

        public EventHandler Closing;
        private string _file;

        public List<string> FileEncoding { get; set; } = new List<string>()
        {
            "utf-8",
            "GBK"
        };

        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                FileChange();
            }
        }

        public static readonly DependencyProperty QrvudejxqDwtjrfkhProperty = DependencyProperty.Register(
            "QrvudejxqDwtjrfkh", typeof(string), typeof(EncodingPage), new PropertyMetadata(default(string)));

        public string QrvudejxqDwtjrfkh
        {
            get { return (string) GetValue(QrvudejxqDwtjrfkhProperty); }
            set { SetValue(QrvudejxqDwtjrfkhProperty, value); }
        }

        public static readonly DependencyProperty MforpsuKcyufbjlpProperty = DependencyProperty.Register(
            "MforpsuKcyufbjlp", typeof(string), typeof(EncodingPage), new PropertyMetadata(default(string)));

        public string MforpsuKcyufbjlp
        {
            get { return (string) GetValue(MforpsuKcyufbjlpProperty); }
            set { SetValue(MforpsuKcyufbjlpProperty, value); }
        }

        private void FileChange()
        {
            var encodingScrutator = new EncodingScrutator(new EncodingScrutatorFile(new FileInfo(File)));
            var encod = encodingScrutator.InspectFileEncoding().Encoding;
            if (Equals(encod, Encoding.GetEncoding("gbk")))
            {
                QrvudejxqDwtjrfkh = FileEncoding.First(temp => temp == "GBK");
            }
            else if (Equals(encod, Encoding.UTF8))
            {
                QrvudejxqDwtjrfkh = FileEncoding.First(temp => temp == "utf-8");
            }

            MforpsuKcyufbjlp = FileEncoding.First(temp => temp == "utf-8");
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            Encoding encod = Encoding.UTF8;
            if (QrvudejxqDwtjrfkh.Equals("GBK"))
            {
                encod = Encoding.GetEncoding("gbk");
            }
            else if (QrvudejxqDwtjrfkh.Equals("utf-8"))
            {
                encod = Encoding.UTF8;
            }

            string str;

            using (var stream = new StreamReader(new FileStream(File, FileMode.Open), encod))
            {
                str = stream.ReadToEnd();
            }

            if (MforpsuKcyufbjlp.Equals("GBK"))
            {
                encod = Encoding.GetEncoding("gbk");
            }
            else if (MforpsuKcyufbjlp.Equals("utf-8"))
            {
                encod = Encoding.UTF8;
            }

            using (var stream = new StreamWriter(System.IO.File.OpenWrite(File), encod))
            {
                stream.Write(str);
            }

            Closing?.Invoke(this, null);
        }
    }
}
