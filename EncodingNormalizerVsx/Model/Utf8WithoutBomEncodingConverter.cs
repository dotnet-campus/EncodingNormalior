using System.Text;

namespace EncodingNormalizerVsx.Model
{
    public class Utf8WithoutBomEncodingConverter : EncodingConverter, IEncodingConverter
    {
        /// <inheritdoc />
        public Utf8WithoutBomEncodingConverter() : base(new UTF8Encoding(false))
        {
        }

        public override string EncodingName { get; } = "Utf8 without Bom";
    }
}