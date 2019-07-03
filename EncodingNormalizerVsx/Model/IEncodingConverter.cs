using System.IO;

namespace EncodingNormalizerVsx.Model
{
    public interface IEncodingConverter
    {
        string EncodingName { get; }
        string Read(FileInfo file);
        void Storage(string str, FileInfo file);
    }
}