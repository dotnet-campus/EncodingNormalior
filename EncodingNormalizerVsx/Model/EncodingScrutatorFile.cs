using System.IO;
using System.Text;

namespace EncodingNormalizerVsx.Model
{
    public class EncodingScrutatorFile:EncodingScrutatorFolderFile
    {
        private Encoding _encoding;

        public EncodingScrutatorFile(EncodingNormalior.Model.EncodingScrutatorFile encodingScrutatorFile, EncodingScrutatorFolder parent) : base(encodingScrutatorFile.Name,parent)
        {
            File = encodingScrutatorFile.File;
            _encoding = encodingScrutatorFile.Encoding;
            Encoding = encodingScrutatorFile.Encoding.EncodingName;

        }

        public string Encoding { get; set; }

        public FileInfo File { set; get; }

        public void WriteSitpulationEncoding(Encoding encoding)
        {
            string str;
            using (StreamReader stream=new StreamReader(File.OpenRead(),_encoding))
            {
                str = stream.ReadToEnd();
            }
            using (StreamWriter stream = new StreamWriter(File.OpenWrite(), encoding))
            {
                stream.Write(str);
            }
        }

        /// <summary>
        /// 获取全名称
        /// </summary>
        /// <param name="encodingScrutatorFile"></param>
        public string GetEncodingScrutatorFileDirectory()
        {
            EncodingScrutatorFile encodingScrutatorFile = this;
            StringBuilder str = new StringBuilder();
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