using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncodingNormalior.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncodingNormalizer.UnitTest
{
    [TestClass]
    public class InspectFileWhiteListSettingTest
    {
        [TestMethod]
        public void ReadWhiteListSetting()
        {
            if (!System.IO.File.Exists(File))
            {
                WriteFile();
            }

            InspectFileWhiteListSetting inspectFileWhiteListSetting = InspectFileWhiteListSetting.ReadWhiteListSetting(File);

            var file = inspectFileWhiteListSetting.FileRegexWhiteList;

            var folder = inspectFileWhiteListSetting.FolderWhiteList;

            string str = @"1.png
1.jpg
1.tmp
1.exe
1.mp4
1.snk
1.pfx
1.zip
1.rar
1.7z
1.cer";
            TestRegexFile(file, str, true);

            str = "1.png1";
            TestRegexFile(file, str, false);


            str = @".git
obj
bin
packages";

            foreach (var temp in str.Split('\n'))
            {
                Assert.AreEqual(folder.Any(t => t == temp), true);
            }
        }

        private static void TestRegexFile(IReadOnlyList<System.Text.RegularExpressions.Regex> file, string str, bool acequal)
        {
            foreach (var temp in str.Split('\n'))
            {
                Assert.AreEqual(file.Any(regex => regex.IsMatch(temp)), acequal);
            }
        }

        private void WriteFile()
        {
            string str = @"
.git\
obj\
bin\
packages\
*.png
*.jpg
*.tmp
*.exe
*.mp4
*.snk
*.pfx
*.zip
*.rar
*.7z
*.cer";
            using (StreamWriter stream = new StreamWriter(new FileStream(File, FileMode.Create)))
            {
                stream.Write(str);
            }

        }

        private const string File = Folder + "whiteList.txt";

        private const string Folder = "E:\\";
    }
}
