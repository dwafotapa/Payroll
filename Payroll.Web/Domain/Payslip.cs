using System;

namespace Payroll.Web.Domain
{
    public class Payslip
    {
        public string Name { get; set; }
        public string PayPeriod { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal NetIncome { get; set; }
        public decimal Super { get; set; }

        public override string ToString()
        {
            return string.Concat(Name, ", ", PayPeriod, ", ", GrossIncome, ", ", IncomeTax, ", ", NetIncome, ", ", Super);
        }
    }
}
