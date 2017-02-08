using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     获取文件的编码
    /// </summary>
    public static class InspectEncoding
    {
        /// <summary>
        ///     获取文件夹所有文件的编码，异步函数
        /// </summary>
        public static async Task InspectFolderEncodingAsync(EncodingScrutatorFolder encodingScrutatorFolder, IProgress<EncodingScrutatorFile> progress)
        {
            encodingScrutatorFolder.Progress = progress;
            await new Task(encodingScrutatorFolder.InspectFolderEncoding);
        }
    }
}
