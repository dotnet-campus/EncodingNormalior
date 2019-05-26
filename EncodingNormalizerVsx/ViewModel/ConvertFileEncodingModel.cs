using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using EncodingUtf8AndGBKDifferentiater;

namespace EncodingNormalizerVsx.ViewModel
{
    public class ConvertFileEncodingModel : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public ConvertFileEncodingModel()
        {
            var optionEncoding = new List<Encoding>
            {
                Encoding.UTF8,
                Encoding.GetEncoding("GBK"),
                Encoding.GetEncoding("big5"),
                Encoding.GetEncoding("utf-16"),
                Encoding.BigEndianUnicode,
                Encoding.UTF32
            };

            foreach (var temp in Encoding.GetEncodings().Select(temp => temp.GetEncoding()))
            {
                if (optionEncoding.All(encoding => encoding.EncodingName != temp.EncodingName))
                {
                    optionEncoding.Add(temp);
                }
            }

            OptionEncoding = optionEncoding;
        }

        public List<Encoding> OptionEncoding { get; }

        public Encoding Encoding
        {
            set
            {
                if (value == _encoding) return;
                _encoding = value;
                OnPropertyChanged();
            }
            get => _encoding;
        }

        public Encoding ConvertEncoding
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
                string str;
                using (var stream = new StreamReader(File.OpenRead(), Encoding))
                {
                    str = stream.ReadToEnd();
                }

                using (var stream = new StreamWriter(File.Open(FileMode.Create), ConvertEncoding))
                {
                    stream.Write(str);
                }

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
        private Encoding _convertEncoding;
        private Encoding _encoding;
        private FileInfo _file;
        private string _trace;

        private void DifferentiateEncoding()
        {
            var (encoding, confidenceCount) = EncodingDifferentiater.DifferentiateEncoding(File);

            if (encoding.BodyName == Encoding.UTF8.BodyName)
            {
                Encoding = OptionEncoding[0];
            }
            else if (encoding.BodyName == Encoding.GetEncoding("GBK").BodyName)
            {
                Encoding = OptionEncoding[1];
            }
            else if (encoding.BodyName == Encoding.ASCII.BodyName)
            {
                Encoding = Encoding.ASCII;
            }
            else
            {
                Encoding = encoding;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}