using System.Collections.Generic;
using EncodingNormalior.Model;

namespace EncodingNormalizerVsx.Model
{
    public class EncodingScrutatorFolder : EncodingScrutatorFolderFile
    {
        public EncodingScrutatorFolder(EncodingNormalior.Model.EncodingScrutatorFolder encodingScrutatorFolder,EncodingScrutatorFolder parent)
            : base(encodingScrutatorFolder.Name,parent)
        {
            Folder = new List<IEncodingScrutatorFile>();
            foreach (var temp in encodingScrutatorFolder.File)
            {
                Folder.Add(new EncodingScrutatorFile(temp,parent));
            }

            foreach (var temp in encodingScrutatorFolder.Folder)
            {
                Folder.Add(new EncodingScrutatorFolder(temp,this));
            }
            SitpulationEncodingSetting = encodingScrutatorFolder.SitpulationEncodingSetting;
        }

        ///// <summary>
        ///// 把所有的文件写编码规范
        ///// </summary>
        //public void WriteSitpulationEncoding()
        //{
        //    foreach (var temp in Folder)
        //    {
        //        if (temp is EncodingScrutatorFile)
        //        {

        //        }
        //        else if (temp is EncodingScrutatorFolder)
        //        {
        //            if (temp.Check)
        //            {
        //                ((EncodingScrutatorFolder)temp).WriteSitpulationEncoding();
        //            }
        //        }
        //    }
        //}

        public SitpulationEncodingSetting SitpulationEncodingSetting { get; }
    }
}