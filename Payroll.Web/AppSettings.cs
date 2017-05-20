using System;
using System.Collections.Generic;

namespace Payroll.Web
{
    public class AppSettings
    {
        public Uploads Uploads { get; set; }
        public TaxThreshold TaxFreeThreshold { get; set; }
        public TaxThreshold SecondTaxThreshold { get; set; }
        public TaxThreshold ThirdTaxThreshold { get; set; }
        public TaxThreshold FourthTaxThreshold { get; set; }
    }

    public class Uploads
    {
        public string Temp { get; set; }
        public string Saved { get; set; }
        public string Processed { get; set; }
    }

    public class TaxThreshold
    {
        public decimal Income { get; set; }
        public decimal FixedRate { get; set; }
        public decimal VariableRate { get; set; }
    }
}