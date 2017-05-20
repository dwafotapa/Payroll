using System;
using Microsoft.Extensions.Options;

namespace Payroll.Web.Services
{
    public class TaxCalculatorService : ITaxCalculatorService
    {
        private readonly IOptions<AppSettings> _appSettingsOptions;

        public TaxCalculatorService(IOptions<AppSettings> appSettingsOptions)
        {
            _appSettingsOptions = appSettingsOptions;
        }

        public string GetFullName(string firstName, string lastName)
        {
            return string.Concat(firstName, " ", lastName);
        }

        
        public decimal CalculateGrossIncome(decimal annualSalary)
        {
            return Math.Round(annualSalary / 12, 0);
        }
        

        public decimal CalculateIncomeTax(decimal annualSalary)
        {
            if (annualSalary > _appSettingsOptions.Value.FourthTaxThreshold.Income)
            {
                return Math.Round((_appSettingsOptions.Value.FourthTaxThreshold.FixedRate + (annualSalary - _appSettingsOptions.Value.FourthTaxThreshold.Income) * _appSettingsOptions.Value.FourthTaxThreshold.VariableRate) / 12, 0);
            }

            if (annualSalary > _appSettingsOptions.Value.ThirdTaxThreshold.Income)
            {
                return Math.Round((_appSettingsOptions.Value.ThirdTaxThreshold.FixedRate + (annualSalary - _appSettingsOptions.Value.ThirdTaxThreshold.Income) * _appSettingsOptions.Value.ThirdTaxThreshold.VariableRate) / 12, 0);
            }

            if (annualSalary > _appSettingsOptions.Value.SecondTaxThreshold.Income)
            {
                return Math.Round((_appSettingsOptions.Value.SecondTaxThreshold.FixedRate + (annualSalary - _appSettingsOptions.Value.SecondTaxThreshold.Income) * _appSettingsOptions.Value.SecondTaxThreshold.VariableRate) / 12, 0);
            }

            if (annualSalary > _appSettingsOptions.Value.TaxFreeThreshold.Income)
            {
                return Math.Round((annualSalary - _appSettingsOptions.Value.TaxFreeThreshold.Income) * _appSettingsOptions.Value.SecondTaxThreshold.VariableRate / 12, 0);
            }

            return 0;
        }

        public decimal CalculateNetIncome(decimal grossIncome, decimal incomeTax)
        {
            return grossIncome - incomeTax;
        }

        public decimal CalculateSuper(decimal grossIncome, decimal superRate)
        {
            return Math.Round(grossIncome * superRate, 0);
        }
    }
}