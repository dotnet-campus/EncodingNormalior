using System.Collections.Generic;
using EncodingNormalior.Model;

namespace EncodingNormalizerVsx.Model
{
    public interface IEncodingScrutatorFile
    {
        string Name { get; }
        bool Check { set; get; }
        List<IEncodingScrutatorFile> Folder { set; get; }

        EncodingScrutatorFolder Parent { set; get; }

        //SitpulationEncodingSetting SitpulationEncodingSetting { set; get; }
    }
}