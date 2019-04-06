using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using EncodingUtf8AndGBKDifferentiater;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConvertFileEncodingModel : INotifyPropertyChanged
    {
        private FileInfo _file;
        private string _encoding;
        private string _convertEncoding;

        public List<string> OptionEncoding { get; } = new List<string>()
        {
            "Utf8", "GBK"
        };

        public string Encoding
        {
            set
            {
                if (value == _encoding) return;
                _encoding = value;
                OnPropertyChanged();
            }
            get => _encoding;
        }

        public string ConvertEncoding
        {
            get
            {
                if (string.IsNullOrEmpty(_convertEncoding))
                {
                    _convertEncoding = OptionEncoding[0];
                }

                return _convertEncoding;
            }
            set
            {
                if (value == _convertEncoding) return;
                _convertEncoding = value;
                OnPropertyChanged();
            }
        }

        public FileInfo File
        {
            get => _file;
            set
            {
                _file = value;

                DifferentiateEncoding();
            }
        }

        private void DifferentiateEncoding()
        {
            var (encoding, confidenceCount) = EncodingDifferentiater.DifferentiateEncoding(File);

            if (encoding.BodyName == System.Text.Encoding.UTF8.BodyName)
            {
                Encoding = OptionEncoding[0];
            }
            else if (encoding.BodyName == System.Text.Encoding.GetEncoding("GBK").BodyName)
            {
                Encoding = OptionEncoding[1];
            }
            else if (encoding.BodyName == System.Text.Encoding.ASCII.BodyName)
            {
                Encoding = "ASCII";
            }
            else
            {
                Encoding = encoding.EncodingName;
            }
        }

        private Encoding GetEncoding(string encoding)
        {
            switch (encoding)
            {
                case "Utf8": return System.Text.Encoding.UTF8;
                case "GBK": return System.Text.Encoding.GetEncoding("GBK");
                default: return System.Text.Encoding.GetEncoding(encoding);
            }
        }

        public void ConvertFile()
        {
            string str;
            using (var stream = new StreamReader(File.OpenRead(), GetEncoding(Encoding)))
            {
                str = stream.ReadToEnd();
            }

            using (var stream = new StreamWriter(File.Open(FileMode.Create), GetEncoding(ConvertEncoding)))
            {
                stream.Write(str);
            }

            MessageBox.Show("转换完成");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}