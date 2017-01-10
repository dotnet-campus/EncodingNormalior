using System.Text;

namespace EncodingNormalior.Model
{
    /// <summary>
    ///     用户设置的规定编码
    /// </summary>
    public class SitpulationEncodingSetting : ISetting
    {
        /// <summary>
        ///     设置或获取用户规定的编码，默认编码为 <see cref="Encoding.UTF8" />
        /// </summary>
        public Encoding SitpulationEncoding { set; get; } = Encoding.UTF8;

        public static SitpulationEncodingSetting DefaultSitpulationEncodingSetting { set; get; } = new SitpulationEncodingSetting
        {
            SitpulationEncoding = Encoding.UTF8
        };
    }
}