using System.IO;
using System.Text;

namespace EncodingNormalizerVsx.Model
{
    public class EncodingScrutatorFile : EncodingScrutatorFolderFile
    {
        public Encoding Encoding { set; get; }

        public EncodingScrutatorFile(EncodingNormalior.Model.EncodingScrutatorFile encodingScrutatorFile,
            EncodingScrutatorFolder parent) : base(encodingScrutatorFile.Name, parent)
        {
            File = encodingScrutatorFile.File;
            Encoding = encodingScrutatorFile.Encoding;
            //_encoding = encodingScrutatorFile.Encoding.EncodingName;
        }

        //private string _encoding;

        public FileInfo File { set; get; }
        ///// <summary>
        ///// 重写文件编码
        ///// </summary>
        ///// <param name="encoding"></param>
        //public void WriteSitpulationEncoding(Encoding encoding)
        //{
        //    if (_encoding == System.Text.Encoding.ASCII.EncodingName)
        //    {
        //        if (Equals(encoding, System.Text.Encoding.UTF8) ||
        //            Equals(encoding, System.Text.Encoding.GetEncoding("GBK")) ||
        //            Equals(encoding, System.Text.Encoding.ASCII)) //对 ASCII不写入
        //        {
        //            return;
        //        }
        //    }
        //    string str;
        //    using (var stream = new StreamReader(File.OpenRead(), Encoding))
        //    {
        //        str = stream.ReadToEnd();
        //    }
        //    using (var stream = new StreamWriter(new FileStream(File.FullName, FileMode.Create), encoding))
        //    {
        //        stream.Write(str);
        //    }
        //}

        /// <summary>
        ///     获取全名称
        /// </summary>
        public string GetEncodingScrutatorFileDirectory()
        {
            var encodingScrutatorFile = this;
            var str = new StringBuilder();
            var folder = encodingScrutatorFile.Parent;
            str.Append(encodingScrutatorFile.File.Name);
            while (folder != null)
            {
                str.Insert(0, folder.Name + "\\");
                folder = folder.Parent;
            }
            return str.ToString();
        }
    }
}