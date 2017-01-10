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
            var headByte = ReadFileHeadbyte(stream);

            //从文件获取编码
            var encoding = AutoEncoding(headByte);
            stream.Position = 0;

            // uft8无签名
            if (encoding.Equals(Encoding.ASCII))//GBK utf8
            {
                if (IsGBK(stream))
                {
                    encoding = Encoding.GetEncoding("GBK");
                }
            }

            stream.Dispose();
            return encoding;
        }
        /// <summary>
        /// 输入文件是不是GBK编码
        /// </summary>
        /// <param name="stream">文件</param>
        /// <returns>true 是GBK编码，false不是GBK编码</returns>
        private static bool IsGBK(Stream stream)
        {
            long length = 0;
            bool isGBK = false;//如果所有的byte都是不大于127那么是ascii，这时是什么都好
            var buffer = new byte[1024];
            var n = 0;
            while ((n = stream.Read(buffer, 0, 1024)) > 0)
            {
                for (var i = 0; i < n; i++)
                {
                    var temp = buffer[i];
                    if (temp < 128)
                    {
                        length++;
                        continue;
                    }
                    if (i + 1 == n)
                    {
                        break;
                    }
                    var temp2 = buffer[i + 1];//http://en.wikipedia.org/wiki/GBK
                    if ((temp >= 0xA1 && temp <= 0xA9 && temp2 >= 0xA1 && temp2 <= 0xFE) ||
                        (temp >= 0xB0 && temp <= 0xF7 && temp2 >= 0xA1 && temp2 <= 0xFE) ||
                        (temp >= 0x81 && temp <= 0xA0 && temp2 >= 0x40 && temp2 <= 0xFE && temp2 != 0x7F) ||
                        (temp >= 0xAA && temp <= 0xFE && temp2 >= 0x40 && temp2 <= 0xA0 && temp2 != 0x7F) ||
                        (temp >= 0xA8 && temp <= 0xA9 && temp2 >= 0x40 && temp2 <= 0xA0 && temp2 != 0x7F) ||
                        (temp >= 0xAA && temp <= 0xAF && temp2 >= 0xA1 && temp2 <= 0xFE) ||
                        (temp >= 0xF8 && temp <= 0xFE && temp2 >= 0xA1 && temp2 <= 0xFE) ||
                        (temp >= 0xA1 && temp <= 0xA7 && temp2 >= 0x40 && temp2 <= 0xA0 && temp2 != 0x7F))
                    {
                        length += 2;
                        i++;
                        isGBK = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            stream.Position = 0;
            if (!isGBK)//如果没有中文或GBK的在ascii外字符，判断ASCII
            {
                return false;
            }
            return length == stream.Length;
        }

        /// <summary>
        ///     读取文件的头4个byte
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="headAmount">读取长度</param>
        /// <returns>文件头4个byte</returns>
        private byte[] ReadFileHeadbyte(Stream stream, int headAmount = 4)
        {
            //var headAmount = 4;
            var buffer = new byte[headAmount];
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

            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7; //85 116 102 55    //utf7 aa 97 97 0 0
            //utf7 编码 = 43 102 120 90

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8; //无签名 117 116 102 56
            // 130 151 160 231
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE

            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            return Encoding.ASCII; //如果返回ASCII可能是GBK
        }
    }
}