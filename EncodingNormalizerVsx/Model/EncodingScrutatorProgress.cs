using System;

namespace EncodingNormalizerVsx.Model
{
    public class EncodingScrutatorProgress : IProgress<EncodingNormalior.Model.EncodingScrutatorFile>
    {
        public EventHandler<EncodingNormalior.Model.EncodingScrutatorFile> ProgressChanged { set; get; }
        public EventHandler<Exception> ExceptChanged { set; get; }
        public EventHandler<EncodingScrutatorFile> WriteSitpulationFileChanged { set; get; }

        public void Report(EncodingNormalior.Model.EncodingScrutatorFile encodingScrutatorFile)
        {
            ProgressChanged?.Invoke(this, encodingScrutatorFile);
        }

        public void ReportExcept(Exception e)
        {
            ExceptChanged?.Invoke(this, e);
        }

        ///// <summary>
        ///// 需要处理总数
        ///// </summary>
        //public double Amount { set; get; }
        ///// <summary>
        ///// 当前数
        ///// </summary>
        //public double ProgressCount { set; get; }

        public void ReportWriteSitpulationFile(EncodingScrutatorFile encodingScrutatorFile)
        {
            WriteSitpulationFileChanged?.Invoke(this, encodingScrutatorFile);
        }
    }
}