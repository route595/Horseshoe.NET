using System;

namespace Horseshoe.NET.Mathematics.Finance
{
    public class AltCreditAccountPayoffInfo
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? APR { get; set; }
        public decimal? PaymentAmount { get; set; }
    }
}
