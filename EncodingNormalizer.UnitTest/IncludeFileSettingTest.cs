using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EncodingNormalior.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncodingNormalizer.UnitTest
{
    [TestClass]
    public class IncludeFileSettingTest
    {
        [TestMethod]
        public void GetaIncludeRegexFile()
        {
            //测试本地文件夹
            if (!System.IO.File.Exists(Folder + File))
            {
                WriteFile();
            }

            IncludeFileSetting includeFileSetting = new IncludeFileSetting(new FileInfo(Folder + File));
            var includeRegex = includeFileSetting.GetaIncludeRegexFile();

        }

        private void WriteFile()
        {
            string str = @"
lindexi.txt
*.lindexitxt
lindexi.md
lindexi.dox
lindexi*.txt ";
            using (StreamWriter stream = new StreamWriter(new FileStream(Folder + File, FileMode.Create)))
            {
                stream.Write(str);
            }
        }

        private const string File = "includeFile.txt";

        private const string Folder = "E:\\";
    }
}
