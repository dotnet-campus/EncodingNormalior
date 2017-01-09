using System;
using System.IO;
using System.Text;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     检测编码
    /// </summary>
    public class EncodingScrutator
    {
        /// <summary>
        ///     检测文件编码
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns>文件编码</returns>
        public Encoding InspectFileEncoding(FileInfo file)
        {
            //打开流
            Stream stream = file.OpenRead();
            byte[] headByte = ReadFileHeadbyte(stream);

            //从文件获取编码
            Encoding encoding= AutoEncoding(headByte);

            //gbk utf7 uft8无签名
            if (encoding.Equals(Encoding.ASCII))
            {
                if (IsGBK(stream))
                {
                    return Encoding.GetEncoding("GBK");
                }
            }


            stream.Dispose();
            //return Encoding.Default;
            return encoding;
        }

        private bool IsGBK(Stream stream)
        {
            return true;
        }

        /// <summary>
        /// 读取文件的头4个byte
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>文件头4个byte</returns>
        private byte[] ReadFileHeadbyte(Stream stream)
        {
            int headAmount = 4;
            byte[] buffer = new byte[headAmount];
            stream.Read(buffer, 0, headAmount);
            stream.Position = 0;
            return buffer;
        }



        private static Encoding AutoEncoding(byte[] bom)
        {
            if (bom.Length != 4)
            {
                throw new ArgumentException("EncodingScrutator.AutoEncoding 参数大小不等于4");
            }

            // Analyze the BOM

            //gbk 71 66 75 32
            //gbk 编码 177 224 194 235
            //gbk aa  97 97 0 0
            //gbk aa文 97 97 206 196
            //gbk aa文三 97 97 206 196 200 253

            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;//85 116 102 55    //utf7 aa 97 97 0 0
            //utf7 编码 = 43 102 120 90

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;//无签名 117 116 102 56
                                          // 130 151 160 231
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE

            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            return Encoding.ASCII;//ascii 116 104 105 115
        }
    }
}