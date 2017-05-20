using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Web.Domain;
using Payroll.Web.Services;

namespace Payroll.Web.Controllers
{
    public class PayrollController : Controller
    {
        private readonly IOptions<AppSettings> _appSettingsOptions;
        private readonly IPayrollFileService _payrollFileService;

        public PayrollController(IOptions<AppSettings> appSettingsOptions, IPayrollFileService payrollFileService)
        {
            _appSettingsOptions = appSettingsOptions;
            _payrollFileService = payrollFileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            try
            {
                var payslips = _payrollFileService.GeneratePayslips(file);
                _payrollFileService.Create(file.FileName, payslips);
                return RedirectToAction("Review", new { filename = file.FileName });
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Review(string fileName)
        {
            try
            {
                var filePath = _appSettingsOptions.Value.Uploads.Temp + fileName;
                var payslips = _payrollFileService.Read(filePath);
                ViewData["fileName"] = fileName;
                return View(payslips);
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult Review(string fileName, Workflow submit)
        {
            if (submit == Workflow.Save) return this.Save(fileName);

            else if (submit == Workflow.Delete) return this.Delete(fileName);
            
            throw new InvalidOperationException();
        }

        public IActionResult Finish(string fileName)
        {
            try
            {
                var filePath = _appSettingsOptions.Value.Uploads.Saved + fileName;                
                var payslips = _payrollFileService.Read(filePath);
                ViewData["fileName"] = fileName;
                return View(payslips);
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult Finish(string fileName, Workflow submit)
        {
            if (submit == Workflow.Run)
            {
                try
                {
                    _payrollFileService.Run(fileName);
                    return RedirectToAction("Success", new { filename = fileName });
                }
                catch(Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            throw new InvalidOperationException();
        }
        
        public IActionResult Success(string fileName)
        {
            ViewData["fileName"] = fileName;
            return View();
        }

        private IActionResult Save(string fileName)
        {
            try
            {
                _payrollFileService.Save(fileName);
                return RedirectToAction("Finish", new { filename = fileName });
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        private IActionResult Delete(string fileName)
        {
            try
            {
                _payrollFileService.Delete(fileName);
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
