using System.IO;
using System.Text;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     包括文件和编码
    /// </summary>
    public class EncodingScrutatorFile
    {
        public EncodingScrutatorFile()
        {
        }

        public EncodingScrutatorFile(FileInfo file)
        {
            File = file;
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

        /// <summary>
        ///     置信度
        ///     范围0-1,1表示确定，0表示不确定，注意：ASCII编码的置信度为0
        /// </summary>
        public double ConfidenceCount { set; get; } = 0;
    }
}