using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Payroll.Web.Domain;

namespace Payroll.Web.Services
{
    public class PayrollFileService : IPayrollFileService
    {
        private readonly IOptions<AppSettings> _appSettingsOptions;
        private readonly ITaxCalculatorService _taxCalculatorService;

        public PayrollFileService(IOptions<AppSettings> appSettingsOptions, ITaxCalculatorService taxCalculatorService)
        {
            _appSettingsOptions = appSettingsOptions;
            _taxCalculatorService = taxCalculatorService;
        }

        public IEnumerable<Payslip> GeneratePayslips(IFormFile file)
        {
            var payslips = new List<Payslip>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var cells = line.Split(',');

                    decimal annualSalary;
                    decimal superRate;

                    if (this.IsFormFileDataValid(cells, out annualSalary, out superRate))
                    {
                        var firstName = cells[0];
                        var lastName = cells[1];
                        var paymentStartDate = cells[4];
                        var payslip = new Payslip();
                        payslip.Name = _taxCalculatorService.GetFullName(firstName, lastName);
                        payslip.PayPeriod = paymentStartDate;
                        payslip.GrossIncome = _taxCalculatorService.CalculateGrossIncome(annualSalary);
                        payslip.IncomeTax = _taxCalculatorService.CalculateIncomeTax(annualSalary);
                        payslip.NetIncome = _taxCalculatorService.CalculateNetIncome(payslip.GrossIncome, payslip.IncomeTax);
                        payslip.Super = _taxCalculatorService.CalculateSuper(payslip.GrossIncome, superRate);
                        payslips.Add(payslip);
                    }
                }
            }
            return payslips;
        }

        public void Create(string fileName, IEnumerable<Payslip> payslips)
        {
            System.IO.File.WriteAllLines(_appSettingsOptions.Value.TempFolderPath + fileName, payslips.Select(p => p.ToString()));
        }

        public IEnumerable<Payslip> Read(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            var payslips = new List<Payslip>();
            foreach (var line in lines)
            {
                var cells = line.Split(',');
                var payslip = new Payslip
                {
                    Name = cells[0],
                    PayPeriod = cells[1],
                    GrossIncome = Decimal.Parse(cells[2]),
                    IncomeTax = Decimal.Parse(cells[3]),
                    NetIncome = Decimal.Parse(cells[4]),
                    Super = Decimal.Parse(cells[5])
                };
                payslips.Add(payslip);
            }
            return payslips;
        }

        public void Save(string fileName)
        {
            var sourceFilePath = _appSettingsOptions.Value.TempFolderPath + fileName;
            var destinationFilePath = _appSettingsOptions.Value.SavedFolderPath + fileName;
            if (File.Exists(destinationFilePath)) File.Delete(destinationFilePath);
            File.Move(sourceFilePath, destinationFilePath);
        }
        
        public void Delete(string fileName)
        {
            var filePath = _appSettingsOptions.Value.TempFolderPath + fileName;
            File.Delete(filePath);
        }

        public void Run(string fileName)
        {
            var sourceFilePath = _appSettingsOptions.Value.SavedFolderPath + fileName;
            var destinationFilePath = _appSettingsOptions.Value.ProcessedFolderPath + fileName;
            if (File.Exists(destinationFilePath)) File.Delete(destinationFilePath);
            File.Move(sourceFilePath, destinationFilePath);
        }

        private bool IsFormFileDataValid(string[] cells, out decimal annualSalary, out decimal superRate)
        {
            if (cells.Length != 5
                || String.IsNullOrEmpty(cells[0])
                || String.IsNullOrEmpty(cells[1])
                || !Decimal.TryParse(cells[2], out annualSalary)
                || !Decimal.TryParse(cells[3].TrimEnd(new[] {'%'}), out superRate)
                || String.IsNullOrEmpty(cells[4]))
            {
                throw new InvalidDataException();
            }

            superRate /= 100;
            return true;
        }
    }
}