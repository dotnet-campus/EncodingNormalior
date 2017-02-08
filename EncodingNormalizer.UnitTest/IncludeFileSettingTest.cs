using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            var includeRegex = new List<Regex>();
            //全部匹配
            foreach (var temp in includeFileSetting.GetaIncludeRegexFile())
            {
                includeRegex.Add(new Regex(temp));
            }

            List<string> file = new List<string>()
            {
                "lindexi.txt",
                "1.lindexitxt",
                "lindexi.md",
                "lindexi.dox",
                "lindexi1.txt"
            };

            foreach (var temp in file)
            {
                Assert.AreEqual(includeRegex.Any(regex => regex.IsMatch(temp)), true);
            }

            file = new List<string>()
            {
                "lindexi.txt1"
            };
            foreach (var temp in file)
            {
                Assert.AreEqual(includeRegex.Any(regex => regex.IsMatch(temp)), false);
            }

            TestCase(includeRegex);
        }

        private static void TestCase(List<Regex> includeRegex)
        {
            List<string> file = new List<string>()
            {
                "lindexi.Txt"
            };
            foreach (var temp in file)
            {
                Assert.AreEqual(includeRegex.Any(regex => regex.IsMatch(temp)), true);
            }
        }

        private void TestCase()
        {
            
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
