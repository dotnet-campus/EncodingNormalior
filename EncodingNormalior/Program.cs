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
        public static void Main(string[] args)
        {
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
