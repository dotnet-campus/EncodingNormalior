using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncodingNormalior.Model;

namespace EncodingNormalizerVsx.Model
{
    public class EncodingScrutatorFolderFile : IEncodingScrutatorFile
    {
        public EncodingScrutatorFolderFile(string name)//, SitpulationEncodingSetting sitpulationEncodingSetting)
        {
            Name = name;
            //SitpulationEncodingSetting = sitpulationEncodingSetting;
        }

        public EncodingScrutatorFolderFile(string name, EncodingScrutatorFolder parent)
        {
            Name = name;
            Parent = parent;
        }

        public string Name { get; }
        public bool Check { get; set; } = true;
        public List<IEncodingScrutatorFile> Folder { get; set; }
        public EncodingScrutatorFolder Parent { get; set; }
    }


}
