using System;
using System.Collections.Generic;

namespace Payroll.Web
{
    public class AppSettings
    {
        public string TempFolderPath { get; set; }
        public string SavedFolderPath { get; set; }
        public string ProcessedFolderPath { get; set; }
        public TaxThreshold TaxFreeThreshold { get; set; }
        public TaxThreshold SecondTaxThreshold { get; set; }
        public TaxThreshold ThirdTaxThreshold { get; set; }
        public TaxThreshold FourthTaxThreshold { get; set; }
    }

    public class TaxThreshold
    {
        public decimal Income { get; set; }
        public decimal FixedRate { get; set; }
        public decimal VariableRate { get; set; }
    }
}