using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReferralSystem.Models;
using ReferralSystem.Repository;
using ReferralSystem.Service;

namespace ReferralSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IMongoRepository<Employee> _employeeRepo;

        private readonly IConfiguration _configuration;


        public EmployeeController(IMongoRepository<Employee> employeeRepository, IConfiguration configuration)
        {
            _employeeRepo = employeeRepository;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var abc = HttpContext.User;
            return View();
        }


        public IActionResult GetAllPositions([DataSourceRequest]DataSourceRequest request)
        {
            List<Employee> userResult = new List<Employee>();

            var empList = _employeeRepo.Get();


            return Json(empList.ToDataSourceResult(request));
        }

        public ActionResult ReferCandidate(string id , string bu)
        {
            var widgetViewModel = new Employee();
            widgetViewModel.JobId = id;
            widgetViewModel.BusinessUnit = bu;

            return PartialView("_ReferCandidate", widgetViewModel);
        }

        [HttpPost]
        public ActionResult UploadReferral(Employee emp)
        {
            BlobStorageService objBlobService = new BlobStorageService(_configuration);
            byte[] fileData = new byte[emp.File.Length];
            var abc = objBlobService.UploadFileToBlob(emp.File.FileName, emp.File);

            return RedirectToAction("Index");
        }

    }
}