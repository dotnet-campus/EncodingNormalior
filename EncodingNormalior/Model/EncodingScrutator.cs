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
        private Stream _stream;

        public EncodingScrutator(EncodingScrutatorFile encodingScrutatorFile)
        {
            EncodingScrutatorFile = encodingScrutatorFile;
        }

        public EncodingScrutator(FileInfo file)
        {
            EncodingScrutatorFile = new EncodingScrutatorFile
            {
                File = file
            };
        }

        private byte[] CountBuffer { set; get; }

        public EncodingScrutatorFile EncodingScrutatorFile { set; get; }

        /// <summary>
        ///     检测文件编码
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns>文件编码</returns>
        public EncodingScrutatorFile InspectFileEncoding() //(FileInfo file)
        {
            var file = EncodingScrutatorFile.File;
            //打开流
            Stream stream = file.OpenRead();
            _stream = stream;
            var headByte = ReadFileHeadbyte(stream);
            stream.Position = 0;

            //从文件获取编码
            var encoding = AutoEncoding(headByte);
            // uft8无签名
            if (encoding.Equals(Encoding.ASCII)) //GBK utf8
            {
                //if (IsGBK(stream))
                //{
                //    encoding = Encoding.GetEncoding("GBK");
                //}

                var countUtf8 = CountUtf8();
                if (countUtf8 == 0)
                {
                    encoding = Encoding.ASCII;
                }
                else
                {
                    var countGbk = CountGbk();
                    if (countUtf8 > countGbk)
                    {
                        encoding = Encoding.UTF8;
                        EncodingScrutatorFile.ConfidenceCount = (double) countUtf8/(countUtf8 + countGbk);
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding("GBK");
                        EncodingScrutatorFile.ConfidenceCount = (double) countGbk/(countUtf8 + countGbk);
                    }
                }
            }
            else
            {
                //EncodingScrutatorFile.Encoding = encoding;//不需要
                EncodingScrutatorFile.ConfidenceCount = 1;
            }
            stream.Dispose();
            EncodingScrutatorFile.Encoding = encoding;
            return EncodingScrutatorFile;
        }

        private int CountGbk()
        {
            var count = 0; //存在GBK的byte
            if (CountBuffer == null)
            {
                ReadStream();
            }
            var length = CountBuffer.Length; //总长度

            var buffer = CountBuffer;
            const char head = (char) 0x80; //小于127 通过 &head==0

            for (var i = 0; i < length; i++)
            {
                var firstByte = buffer[i]; //第一个byte，GBK有两个
                if ((firstByte & head) == 0) //如果是127以下，那么就是英文等字符，不确定是不是GBK
                {
                    continue; //文件全部都是127以下字符，可能是Utf-8 或ASCII
                }
                if (i + 1 >= length) //如果是大于127，需要两字符，如果只有一个，那么文件错了，但是我也没法做什么
                {
                    break;
                }
                var secondByte = buffer[i + 1]; //如果是GBK，那么添加GBK byte 2
                if (firstByte >= 161 && firstByte <= 247 &&
                    secondByte >= 161 && secondByte <= 254)
                {
                    count += 2;
                    i++;
                }
                //if (IsGbk(firstByte, secondByte))
                //{
                //    count += 2;
                //    i++;
                //}
            }
            return count;
        }


        private int CountUtf8() //(Stream stream)
        {
            var count = 0;
            if (CountBuffer == null)
            {
                ReadStream();
            }

            var length = CountBuffer.Length;


            var buffer = CountBuffer; // new byte[length];
            const char head = (char) 0x80;
            //while ((n = stream.Read(buffer, 0, n)) > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var temp = buffer[i];
                    if (temp < 128) //  !(temp&head)
                    {
                        //utf8 一开始如果byte大小在 0-127 表示英文等，使用一byte
                        //length++; 我们记录的是和CountGBK比较
                        continue;
                    }
                    var tempHead = head;
                    var wordLength = 0; //单词长度，一个字使用多少个byte

                    while ((temp & tempHead) != 0) //存在多少个byte
                    {
                        wordLength++;
                        tempHead >>= 1;
                    }

                    if (wordLength <= 1)
                    {
                        //utf8最小长度为2
                        continue;
                    }

                    wordLength--; //去掉最后一个，可以让后面的 point大于wordLength
                    if (wordLength + i >= length)
                    {
                        break;
                    }
                    var point = 1; //utf8的这个word 是多少 byte
                    //utf8在两字节和三字节的编码，除了最后一个 byte 
                    //其他byte 大于127 
                    //所以 除了最后一个byte，其他的byte &head >0
                    for (; point <= wordLength; point++)
                    {
                        var secondChar = buffer[i + point];
                        if ((secondChar & head) == 0)
                        {
                            break;
                        }
                    }

                    if (point > wordLength)
                    {
                        count += wordLength + 1;
                        i += wordLength;
                    }
                }
            }
            return count;
        }

        private void ReadStream()
        {
            var stream = _stream;
            stream.Position = 0;
            var length = (int) stream.Length;
            CountBuffer = new byte[length];
            stream.Read(CountBuffer, 0, length);
        }

        /// <summary>
        ///     输入文件是不是GBK编码
        /// </summary>
        /// <param name="stream">文件</param>
        /// <returns>true 是GBK编码，false不是GBK编码</returns>
        private bool IsGBK(Stream stream)
        {
            //无签名 utf8 判断为 GBK
            long length = 0;
            var isGBK = false; //如果所有的byte都是不大于127那么是ascii，这时是什么都好
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
                    var temp2 = buffer[i + 1]; //http://en.wikipedia.org/wiki/GBK
                    if (IsGbk(temp, temp2))
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
            if (!isGBK) //如果没有中文或GBK的在ascii外字符，判断ASCII
            {
                return false;
            }
            return length == stream.Length;
        }

        /// <summary>
        ///     判断输入的两byte是不是GBK
        /// </summary>
        /// <param name="firstByte">第一个字符</param>
        /// <param name="secondByte">第二个</param>
        /// <returns>true 是GBK</returns>
        private static bool IsGbk(byte firstByte, byte secondByte)
        {
            //firstByte >= 161 && firstByte <= 247 &&
            //secondByte >= 161 && secondByte <= 254
            return (firstByte >= 0xA1 && firstByte <= 0xA9 && secondByte >= 0xA1 && secondByte <= 0xFE) ||
                   (firstByte >= 0xB0 && firstByte <= 0xF7 && secondByte >= 0xA1 && secondByte <= 0xFE) ||
                   (firstByte >= 0x81 && firstByte <= 0xA0 && secondByte >= 0x40 && secondByte <= 0xFE &&
                    secondByte != 0x7F) ||
                   (firstByte >= 0xAA && firstByte <= 0xFE && secondByte >= 0x40 && secondByte <= 0xA0 &&
                    secondByte != 0x7F) ||
                   (firstByte >= 0xA8 && firstByte <= 0xA9 && secondByte >= 0x40 && secondByte <= 0xA0 &&
                    secondByte != 0x7F) ||
                   (firstByte >= 0xAA && firstByte <= 0xAF && secondByte >= 0xA1 && secondByte <= 0xFE) ||
                   (firstByte >= 0xF8 && firstByte <= 0xFE && secondByte >= 0xA1 && secondByte <= 0xFE) ||
                   (firstByte >= 0xA1 && firstByte <= 0xA7 && secondByte >= 0x40 && secondByte <= 0xA0 &&
                    secondByte != 0x7F);
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

            return Encoding.ASCII; //如果返回ASCII可能是GBK 无签名utf8
        }
    }
}