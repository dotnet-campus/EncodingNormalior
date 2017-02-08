using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncodingNormalior.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncodingNormalizer.UnitTest
{
    [TestClass]
    public class InspectEncoding
    {
        [TestMethod]
        public void InspectFolderEncoding()
        {
            EncodingScrutatorFolder encodingScrutatorFolder = new EncodingScrutatorFolder(new DirectoryInfo(Folder));

            EncodingNormalior.Model.InspectEncoding.InspectFolderEncodingAsync(encodingScrutatorFolder,
               new Progress<EncodingScrutatorFile>()).Wait();

            StringBuilder str=new StringBuilder();
            foreach (var temp in encodingScrutatorFolder.File)
            {
                str.Append(temp.Name + " " + temp.Encoding.EncodingName + "\r\n");
            }
            Debug.Write(str.ToString());
        }



        private const string Folder = "E:\\测试\\textFile\\";
        private const string File =Folder+ "InspectFolderEncoding.txt";
    }


}
