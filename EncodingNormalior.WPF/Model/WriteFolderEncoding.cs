using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EncodingNormalizerVsx.Model
{
    public static class WriteFolderEncoding
    {
        /// <summary>
        ///     把所有的文件写编码规范
        /// </summary>
        public static void WriteSitpulationEncoding(IEnumerable<IEncodingScrutatorFile> encodingScrutatorFolder,
            EncodingScrutatorProgress progress, Encoding encoding)
        {
            foreach (var temp in encodingScrutatorFolder.Where(temp => temp.Check))
            {
                if (temp is EncodingScrutatorFile)
                {
                    var file = (EncodingScrutatorFile) temp;
                    progress.ReportWriteSitpulationFile(file);
                    try
                    {
                        //file.WriteSitpulationEncoding(encoding);
                        // 重写文件编码
                       
                        WriteSitpulationFileEncoding(file, encoding);
                        file.Check = false;
                    }
                    catch (Exception e)
                    {
                        progress.ReportExcept(e);
                    }
                }
                else if (temp is EncodingScrutatorFolder)
                {
                    WriteSitpulationEncoding(((EncodingScrutatorFolder) temp).Folder, progress, encoding);
                }
            }
        }

        private static void WriteSitpulationFileEncoding(EncodingScrutatorFile file,Encoding encoding)
        {
            if (file.Encoding.Equals(Encoding.ASCII))
            {
                if (Equals(encoding, Encoding.UTF8) ||
                    Equals(encoding, Encoding.GetEncoding("GBK")) ||
                    Equals(encoding, Encoding.ASCII)) //对 ASCII不写入
                {
                    return;
                }
            }
            string str;
            using (var stream = new StreamReader(file.File.OpenRead(), file.Encoding))
            {
                str = stream.ReadToEnd();
            }
            using (var stream = new StreamWriter(new FileStream(file.File.FullName, FileMode.Create), encoding))
            {
                stream.Write(str);
            }

        }
    }
}