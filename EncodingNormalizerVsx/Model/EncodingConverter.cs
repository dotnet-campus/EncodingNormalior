using System.IO;
using System.Text;

namespace EncodingNormalizerVsx.Model
{
    public class EncodingConverter : IEncodingConverter
    {
        /// <inheritdoc />
        public EncodingConverter(Encoding encoding)
        {
            Encoding = encoding;
        }

        public string Read(FileInfo file)
        {
            using (var stream = new StreamReader(file.OpenRead(), Encoding))
            {
                return stream.ReadToEnd();
            }
        }

        public void Storage(string str, FileInfo file)
        {
            using (var stream = new StreamWriter(file.Open(FileMode.Create, FileAccess.Write), Encoding))
            {
                stream.Write(str);
            }
        }

        public Encoding Encoding { get; }

        public virtual string EncodingName => Encoding.EncodingName;
    }
}