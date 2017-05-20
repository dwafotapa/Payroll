using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Payroll.Web.Controllers;
using Payroll.Web.Domain;
using Payroll.Web.Services;
using Xunit;

namespace Payroll.Web.UnitTests.Controllers
{
    public class PayrollControllerUnitTests
    {
        protected PayrollController _controller;
        protected Mock<IOptions<AppSettings>> _appSettingsMock = new Mock<IOptions<AppSettings>>();
        protected Mock<IPayrollFileService> _serviceMock = new Mock<IPayrollFileService>();
        protected string _fileName = "Payroll_032017.csv";

        public PayrollControllerUnitTests()
        {
            _controller = new PayrollController(_appSettingsMock.Object, _serviceMock.Object);
        }

        public class TheIndexPostMethod : PayrollControllerUnitTests
        {
            [Fact]
            public void ReturnsRedirectToActionResult_WhenFileIsInvalid()
            {
                _serviceMock.Setup(s => s.GeneratePayslips(null)).Throws(new InvalidDataException());

                var result = _controller.Index((FormFile)null);

                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Home", redirectToActionResult.ControllerName);
                Assert.Equal("Error", redirectToActionResult.ActionName);
            }

            [Fact]
            public void ReturnsRedirectToActionResult_WhenFileIsValid()
            {
                var formFile = new Mock<IFormFile>();
                var payslips = new List<Payslip>();
                formFile.Setup(f => f.FileName).Returns(_fileName);
                _serviceMock.Setup(s => s.GeneratePayslips(formFile.Object)).Returns(payslips);
                _serviceMock.Setup(s => s.Create(formFile.Object.FileName, payslips));

                var result = _controller.Index(formFile.Object);

                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Review", redirectToActionResult.ActionName);
                Assert.True(redirectToActionResult.RouteValues.ContainsKey("filename"));
                Assert.True(redirectToActionResult.RouteValues.Values.Contains(_fileName));
            }
        }

        public class TheReviewPostMethod : PayrollControllerUnitTests
        {
            // [Fact]
            // public void ReturnsRedirectToActionResult_WhenWorkflowIsInvalid()
            // {
            //     var result = _controller.Review(_fileName, 0);

            //     Assert.IsType<InvalidOperationException>(result);
            // }

            [Fact]
            public void ReturnsRedirectToActionResult_WhenWorkflowIsValidAndPayrollFileIsSaved()
            {
                _serviceMock.Setup(s => s.Save(_fileName));

                var result = _controller.Review(_fileName, Workflow.Save);
                
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Finish", viewResult.ActionName);
            }

            [Fact]
            public void ReturnsRedirectToActionResult_WhenWorkflowIsValidAndPayrollFileIsDeleted()
            {
                _serviceMock.Setup(s => s.Delete(_fileName));
                
                var result = _controller.Review(_fileName, Workflow.Delete);

                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName);
            }
        }
    }
}
