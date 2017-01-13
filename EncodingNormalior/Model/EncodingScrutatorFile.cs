using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncodingNormalior.Model
{
    public interface IEncodingScrutatorFile
    {
        string Name { get; }
        bool Check { set; get; }
        List<EncodingScrutatorFolder> Folder { set; get; }
    }

    /// <summary>
    ///     包括文件和编码
    /// </summary>
    public class EncodingScrutatorFile:IEncodingScrutatorFile
    {
        public EncodingScrutatorFile()
        {
        }

        public EncodingScrutatorFile(FileInfo file)
        {
            File = file;
            Name = file.Name;
        }

        /// <summary>
        /// 设置或获取是否忽略
        /// </summary>
        public bool Ignore { set; get; }

        /// <summary>
        ///     文件
        /// </summary>
        public FileInfo File { set; get; }

        /// <summary>
        ///     编码
        /// </summary>
        public Encoding Encoding { set; get; }

        public EncodingScrutatorFolder Parent { set; get; }

        /// <summary>
        ///     置信度
        ///     范围0-1,1表示确定，0表示不确定，注意：ASCII编码的置信度为0
        /// </summary>
        public double ConfidenceCount { set; get; } = 0;

        public string Name { get; }
        public bool Check { get; set; } = true;
        public List<EncodingScrutatorFolder> Folder { get; set; }
    }
}