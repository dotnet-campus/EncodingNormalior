using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using EncodingNormalizerVsx.Model;
using EncodingUtf8AndGBKDifferentiater;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConvertFileEncodingModel : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public ConvertFileEncodingModel()
        {
            var optionEncoding = new List<IEncodingConverter>
            {
                new EncodingConverter(System.Text.Encoding.UTF8),
                new Utf8WithoutBomEncodingConverter(),
                new EncodingConverter(System.Text.Encoding.GetEncoding("GBK")),
                new EncodingConverter(System.Text.Encoding.GetEncoding("big5")),
                new EncodingConverter(System.Text.Encoding.GetEncoding("utf-16")),
                new EncodingConverter(System.Text.Encoding.BigEndianUnicode),
                new EncodingConverter(System.Text.Encoding.UTF32)
            };

            foreach (var temp in System.Text.Encoding.GetEncodings().Select(temp => temp.GetEncoding()))
            {
                if (optionEncoding.All(encoding => encoding.EncodingName != temp.EncodingName))
                {
                    optionEncoding.Add(new EncodingConverter(temp));
                }
            }

            OptionEncoding = optionEncoding;
        }

        public List<IEncodingConverter> OptionEncoding { get; }

        public IEncodingConverter Encoding
        {
            set
            {
                if (value == _encoding) return;
                _encoding = value;
                OnPropertyChanged();
            }
            get => _encoding;
        }

        public IEncodingConverter ConvertEncoding
        {
            get
            {
                if (_convertEncoding is null)
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

        public string Trace
        {
            set
            {
                _trace = value;
                OnPropertyChanged();
            }
            get => _trace;
        }

        public bool ConvertFile()
        {
            try
            {
                string str = Encoding.Read(File);

                ConvertEncoding.Storage(str, File);

                Trace = "Success";
            }
            catch (Exception e)
            {
                Trace = e.ToString();
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private IEncodingConverter _convertEncoding;
        private IEncodingConverter _encoding;
        private FileInfo _file;
        private string _trace;

        private void DifferentiateEncoding()
        {
            var (encoding, _) = EncodingDifferentiater.DifferentiateEncoding(File);

            if (encoding.BodyName == System.Text.Encoding.UTF8.BodyName)
            {
                SetEncoding(System.Text.Encoding.UTF8);
                return;
            }

            // 其实这些判断是多余的代码，只需要调用 SetEncoding 传入编码名就可以
            var gbk = System.Text.Encoding.GetEncoding("GBK");
            if (encoding.BodyName == gbk.BodyName)
            {
                SetEncoding(gbk);
            }
            else if (encoding.BodyName == System.Text.Encoding.ASCII.BodyName)
            {
                SetEncoding(System.Text.Encoding.ASCII);
            }
            else
            {
                SetEncoding(encoding);
            }
        }

        private void SetEncoding(Encoding encoding)
        {
            Encoding = OptionEncoding.FirstOrDefault(temp => temp.EncodingName == encoding.EncodingName);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}