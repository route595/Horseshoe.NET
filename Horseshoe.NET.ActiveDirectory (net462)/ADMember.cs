namespace Horseshoe.NET.ActiveDirectory
{
    public class ADMember
    {
        public virtual string AdsPath { get; set; }  // overridden in ADObject
        public string RawCN { get; set; }
        public string RawOU { get; set; }
        public string RawDC { get; set; }
        public virtual string CN => ADUtil.ExtractCNFromRaw(RawCN);  // overridden in ADObject
        public string OU => ADUtil.ExtractOUFromRaw(RawOU);
        public string DC => ADUtil.ExtractDCFromRaw(RawDC);

        public override string ToString()
        {
            return Zap.String(CN) ?? Zap.String(AdsPath) ?? "[no-path]";
        }

        public static ADMember FromAdsPath(string adsPath)
        {
            ADUtil.ExtractMemberParts(adsPath, out string rawCn, out string rawOu, out string rawDc);
            return new ADMember
            {
                AdsPath = adsPath,
                RawCN = rawCn,
                RawOU = rawOu,
                RawDC = rawDc
            };
        }
    }
}