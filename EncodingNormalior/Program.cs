using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EncodingNormalior.Model;

namespace EncodingNormalior
{
    public class Program
    {

        //private static void GetWildcardRegexString()
        //{
        //    //IncludeFileSetting includeFile=new IncludeFileSetting();
        //    List<string > str=new List<string>()
        //    {
        //        "*.png",
        //        "Encoding.cs",
        //        "林德熙*.doc",
        //        "林*德熙*.doc",
        //    };

        //    foreach (var temp in str)
        //    {
        //        Console.WriteLine(IncludeFileSetting.GetWildcardRegexString(temp));
        //    }

        //}
        public static void Main(string[] args)
        {
           // GetWildcardRegexString();

            EncodingScrutatorFolder encodingScrutatorFolder=new EncodingScrutatorFolder(new DirectoryInfo("E:\\程序\\公司\\EncodingNormalior"));
            encodingScrutatorFolder.InspectFolderEncoding();
            using (StreamWriter stream=new StreamWriter(
                new FileStream("E:\\1.txt",FileMode.Create)))
            {
                stream.Write(Print(encodingScrutatorFolder));
            }


            //2017年1月10日16:09:17
            //测试
            //ReadByte();
            //EncodingWrite();
            string folder = "E:\\程序\\公司\\EncodingNormalior\\EncodingNormalior\\textFile";
            foreach (var temp in Directory.GetFiles(folder))
            {
                var file = new FileInfo(temp);
                EncodingScrutator encodingScrutator = new EncodingScrutator(file);
                var encoding = encodingScrutator.InspectFileEncoding();
                Console.WriteLine(file.Name + " " + encoding.Encoding.EncodingName + " 置信度 " + encoding.ConfidenceCount + "\r\n");
                //if (encoding.ConfidenceCount == 0.5)
                //{
                //    ReadFile(encoding);
                //}
            }
        }

        private static string Print(EncodingScrutatorFolder encodingScrutatorFolder,string white="",StringBuilder str=null)
        {
            if (str == null)
            {
                str = new StringBuilder();
            }
            foreach (var temp in encodingScrutatorFolder.Folder)
            {
                Print(temp,white+temp.Name+"/",str);
            }
            //StringBuilder str=new StringBuilder();
           
            str.Append(white);
            foreach (var temp in encodingScrutatorFolder.File)
            {
                str.Append(temp.File.Name+" ");
                if (temp.Ignore)
                {
                    str.Append("文件忽略");
                }
                else
                {
                    str.Append("编码 " + temp.Encoding.EncodingName + " 置信度 " + temp.ConfidenceCount);
                }
                str.Append("\r\n");
            }

            //using (StreamWriter stream=new StreamWriter(
            //    new FileStream("E:\\1.txt",FileMode.Create)))
            //{
            //    stream.Write(str.ToString());
            //}

            //Console.WriteLine(str.ToString());
            return str.ToString();
        }

        //private static void ReadFile(EncodingScrutatorFile encoding)
        //{
        //    using (StreamReader stream=new StreamReader(encoding.File.Open(FileMode.Open),Encoding.UTF8))
        //    {
        //        string str = stream.ReadToEnd();
        //    }

        //    using (StreamReader stream = new StreamReader(encoding.File.Open(FileMode.Open), Encoding.GetEncoding("GBK")))
        //    {
        //        string str = stream.ReadToEnd();
        //    }
        //}

        //private static void ReadByte()
        //{
        //    string file = "E:\\程序\\公司\\EncodingNormalior\\EncodingNormalior\\textFile\\GBK 3.txt";
        //    Stream stream=new FileStream(file,FileMode.Open);
        //    int n =(int) stream.Length;
        //    byte[] buffer=new byte[n];
        //    stream.Read(buffer, 0, n);
        //    //aa 文206 196
        //    //aa文三 97 97 206 196 200 253
        //}

        ///// <summary>
        ///// 写入编码各种
        ///// </summary>
        //private static void EncodingWrite()
        //{
        //    string str = "这是{0}编码";
        //    List<Encoding> encoding=new List<Encoding>()
        //    {
        //        Encoding.ASCII,
        //        Encoding.BigEndianUnicode,
        //        Encoding.UTF8,
        //        Encoding.GetEncoding("gbk"),

        //    };

        //    string file = "E:\\程序\\公司\\EncodingNormalior\\EncodingNormalior\\textFile\\";
        //    foreach (var temp in encoding)
        //    {
        //        using (StreamWriter stream =new StreamWriter(
        //            new FileStream(file+temp.EncodingName+".txt",FileMode.OpenOrCreate),temp))
        //        {
        //            stream.Write(str, temp);
        //        }
        //    }
        //}
    }


}
