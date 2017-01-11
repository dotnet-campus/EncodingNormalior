using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     用户设置的规定编码
    /// </summary>
    public class SitpulationEncodingSetting : ISetting
    {
        /// <summary>
        ///     设置或获取用户规定的编码，默认编码为 <see cref="Encoding.UTF8" /> 和 ASCII
        /// </summary>
        public Encoding SitpulationEncoding { set; get; } = Encoding.UTF8;

        public static SitpulationEncodingSetting DefaultSitpulationEncodingSetting { set; get; } = new SitpulationEncodingSetting
        {
            SitpulationEncoding = Encoding.UTF8
        };

        /// <summary>
        /// 是否符合规定编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <remarks>如果是ASCII，那么无法判断他是默认的GBK或UTF-8 返回的是true</remarks>
        public bool ConformtotheDefaultEncoding(Encoding encoding)
        {
            if (Equals(encoding, SitpulationEncoding))
            {
                return true;
            }

            if (encoding.Equals(Encoding.ASCII) &&
                SitpulationEncoding.Equals(Encoding.UTF8) &&
                SitpulationEncoding.Equals(Encoding.GetEncoding("GBK")))
            {
                return true;
            }

            return false;
        }
    }
}