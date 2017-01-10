using System.Collections.Generic;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     文件检测白名单设置
    /// </summary>
    public class InspectFileWhiteListSetting : ISetting
    {
        /// <summary>
        ///     设置或获取白名单
        /// </summary>
        public List<string> WhiteList { set; get; } = new List<string>();
    }
}