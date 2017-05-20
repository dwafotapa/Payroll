using System;
using Microsoft.Extensions.Options;
using Moq;
using Payroll.Web.Services;
using Xunit;

namespace Payroll.Web.UnitTests.Services
{
    public class TaxCalculatorServiceUnitTests
    {
        protected TaxCalculatorService _service;
        protected Mock<IOptions<AppSettings>> _appSettingsMock = new Mock<IOptions<AppSettings>>();
        protected AppSettings _appSettings;

        public TaxCalculatorServiceUnitTests()
        {
            _service = new TaxCalculatorService(_appSettingsMock.Object);
            _appSettings = new AppSettings
            {
                TaxFreeThreshold = new TaxThreshold { Income = 18200, FixedRate = 0, VariableRate = 0.19M },
                SecondTaxThreshold = new TaxThreshold { Income = 37000, FixedRate = 3572, VariableRate = 0.325M },
                ThirdTaxThreshold = new TaxThreshold { Income = 80000, FixedRate = 17547, VariableRate = 0.37M },
                FourthTaxThreshold = new TaxThreshold { Income = 180000, FixedRate = 54547, VariableRate = 0.45M }
            };
        }

        public class TheCalculateIncomeTaxMethod : TaxCalculatorServiceUnitTests
        {
            [Fact]
            public void ReturnsZero_WhenAnnualSalaryIsBelowTaxFreeThreshold()
            {
                var annualSalary = 18000;
                _appSettingsMock.Setup(o => o.Value).Returns(_appSettings);

                var result = _service.CalculateIncomeTax(annualSalary);

                Assert.Equal(0, result);
            }

            [Fact]
            public void Returns922_WhenAnnualSalaryIs60050AndOverSecondTaxThreshold()
            {
                var annualSalary = 60050;
                _appSettingsMock.Setup(o => o.Value).Returns(_appSettings);

                var result = _service.CalculateIncomeTax(annualSalary);

                Assert.Equal(922, result);
            }

            [Fact]
            public void Returns2696_WhenAnnualSalaryIs120000AndOverThirdTaxThreshold()
            {
                var annualSalary = 120000;
                _appSettingsMock.Setup(o => o.Value).Returns(_appSettings);

                var result = _service.CalculateIncomeTax(annualSalary);

                Assert.Equal(2696, result);
            }
        }
    }
}
