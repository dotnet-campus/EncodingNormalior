using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace EncodingNormalizerVsx
{
    public class FileInclude
    {
        public FileInclude()
        {

        }

        public string Name { set; get; }
    }

    public enum CriterionEncoding
    {
        Utf8,
        Unicode16,
        Gbk,
    }

    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Guid(GuidStrings.GuidDefinitionPage)]
    public class DefinitionPage : DialogPage
    {
        public DefinitionPage()
        {
            //OptionString = "Hello World";
            //OptionInteger = 567;
            //CustomSize = new Size(50, 50);

            CriterionEncoding = CriterionEncoding.Utf8;
            //FileInclude=new List<FileInclude>();
            //FileInclude=new FileInclude[1];
        }
        /// <summary>
        /// 规范格式
        /// </summary>
        [Category("规定编码")]
        [Description("规定的编码，如果是ascii 那么无论是选择utf8或GBK都判断为对")]
        public CriterionEncoding CriterionEncoding { set; get; }

        /// <summary>
        /// 不能做 <see cref="List{FileInclude}"/>
        /// </summary>
        [Category("包括文件")]
        [Description("包括文件的后缀名")]
        public FileInclude[] FileInclude { set; get; }



        /// <summary>
        /// Gets or sets the String type custom option value.
        /// </summary>
        /// <remarks>This value is shown in the options page.</remarks>
        [Category("String Options")]
        [Description("My string option")]
        public string OptionString { get; set; }

        ///// <summary>
        ///// Gets or sets the integer type custom option value.
        ///// </summary>
        ///// <remarks>This value is shown in the options page.</remarks>
        //[Category("Integer Options")]
        //[Description("My integer option")]
        //public int OptionInteger { get; set; }

        ///// <summary>
        ///// Gets or sets the Size type custom option value.
        ///// </summary>
        ///// <remarks>This value is shown in the options page.</remarks>
        //[Category("Expandable Options")]
        //[Description("My Expandable option")]
        //public Size CustomSize { get; set; }


        //protected override IWin32Window Window
        //{
        //    get
        //    {
        //        if (_definitionPage == null)
        //        {
        //            _definitionPage = new View.DefinitionPage();
        //        }
        //        _definitionPage.Owner = this;
        //        _definitionPage.InitializeLifetimeService();
        //        return _definitionPage;
        //    }
        //}
        //private View.DefinitionPage _definitionPage;
    }
}