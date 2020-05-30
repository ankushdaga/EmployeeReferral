using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MongoDB.Bson;
using MongoDB.Driver;
using ReferralSystem.Models;
using ReferralSystem.Repository;
using ReferralSystem.Service;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ReferralSystem.Controllers
{
    // [Authorize(Roles = "Recruiter")]

    public class EmployeeController : Controller
    {
        private readonly IMongoRepository<Employee> _employeeRepo;

        private readonly IMongoRepository<Position> _positionRepo;


        private readonly IMongoRepository<ProfileModel> _prof;


        private readonly IConfiguration _configuration;

        public class CurrentUserInfo
        {
            public string Id { get; set; }
            public string Login { get; set; }
            public bool IsAuthenticated { get; set; }
        }

       // private readonly IIdentityService _identityService;
        public EmployeeController(IMongoRepository<Employee> employeeRepository, IConfiguration configuration , IMongoRepository<ProfileModel> prof , IMongoRepository<Position> position)
        {
            _employeeRepo = employeeRepository;
            _configuration = configuration;
            _prof = prof;
            _positionRepo = position;


        }

        public IActionResult Index()
        {

            return View();
        }


        public IActionResult GetAllReferrals([DataSourceRequest]DataSourceRequest request ,string AdditionalParam)
        {
            List<ProfileModel> userResult = new List<ProfileModel>();

            var empList = _prof.Get().Where(x=>x.JobID == AdditionalParam);

            //var abc  = ObjectId.Parse()

            foreach (var qq in empList)
            {
                userResult.Add(new ProfileModel
                {
                    BlobURI = qq.BlobURI,
                    CandidateName = qq.CandidateName + " " + qq.CandidateSurname,
                    DateReferred = qq.DateReferred,
                    ProfileStatus = qq.ProfileStatus,
                    JobID = qq.JobID,
                    Id = qq.Id
                });
            }

            return Json(userResult.ToDataSourceResult(request));
        }

        public JsonResult RemoteDataSource_GetProducts(string text)
        {
            var listStatus = new SelectList(Enum.GetValues(typeof(EnumStatus)).Cast<EnumStatus>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList(), "Value", "Text");
           
            return Json(listStatus.Items);
        }

        public IActionResult GetAllPositions([DataSourceRequest]DataSourceRequest request)
        {
            var empList = _positionRepo.Get();
            return Json(empList.ToDataSourceResult(request));
        }

        public ActionResult ReferCandidate(string id , string bu)
        {
            var widgetViewModel = new Employee();
            widgetViewModel.JobId = id;
            widgetViewModel.BusinessUnit = bu;

            return PartialView("_ReferCandidate", widgetViewModel);
        }


        public ActionResult UpdateReferralStatus(string id, string status,string name)
        {
            var widgetViewModel = new ProfileModel();
            widgetViewModel.Id = new ObjectId(id);
            widgetViewModel.ProfileStatus = status;
            widgetViewModel.CandidateName = name;
            // widgetViewModel.BusinessUnit = bu;

            return PartialView("_ReferralStatusUpdate", widgetViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> SaveReferralStatus(string Id , string ProfileStatus)
        {
            var entity = _prof.FindById(Id);
            string previousStatus = entity.ProfileStatus;


            if (entity != null && entity.ProfileStatus != ProfileStatus)
            {
                entity.ProfileStatus = ProfileStatus;
                _prof.ReplaceOne(entity);


                var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("ankush.daga@capita.co.uk", "Capita");
                var to = new EmailAddress(entity.ReferredBy, "");

                //List<EmailAddress> tos = new List<EmailAddress>
                //{
                //    new EmailAddress("pankaj.pawar@capita.co.uk", "Example User 2")
                //};

                //var subject = "Referral Status has been changed for " + entity.CandidateName + " "+ entity.CandidateSurname;
                //var htmlContent = "Referral status has been changed  from <strong>" + previousStatus + "</strong> to <strong> " + ProfileStatus + "</strong>";
                //var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
                //var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                //var response = await client.SendEmailAsync(msg);
            }


            return RedirectToAction("MyReferrals" , new { jobId = entity.JobID });
        }


        public JsonResult ValidateReferral(string name, string mobNo,string lastName)
        {
            var empList = _prof.Get();

            var isCandidateExist = empList.Where(x =>
                x.CandidateName.Trim() == name.Trim() && x.CandidateSurname.Trim() == lastName.Trim() &&
                x.MobileNumber.Trim() == mobNo.Trim());

            if (isCandidateExist.Any())
            {
                return Json(new { success = false, responseText = "Candidate is already referred." });

            }
            else
            {
                return Json(new { success = true, responseText = "Candidate does not exist." });

            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadReferral(Employee emp)
        {

                var empList = _prof.Get();

                var isCandidateExist = empList.Where(x =>
                    x.CandidateName.Trim() == emp.CandidateName.Trim() && x.CandidateSurname.Trim() == emp.CandidateSurname.Trim() &&
                    x.MobileNumber.Trim() == emp.MobileNumber.Trim());

            if (isCandidateExist.Any())
            {
                ModelState.AddModelError("Region", "Region is mandatory");

            }

            if (ModelState.IsValid)
            {
                if (!isCandidateExist.Any())
                {
                    BlobStorageService objBlobService = new BlobStorageService(_configuration);
                    byte[] fileData = new byte[emp.File.Length];
                    var abc = objBlobService.UploadFileToBlob(emp.File.FileName, emp.File);
                    decimal d = 1.23M;
                    var bookdata = new ProfileModel()
                    {
                        CandidateName = emp.CandidateName,
                        MobileNumber = emp.MobileNumber,
                        CandidateSurname = emp.CandidateSurname,
                        JobID = emp.JobId,
                        DateReferred = DateTime.Now.Date,
                        ReferredBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                        BlobURI = abc,
                        ProfileStatus = GetEnumDisplayName(EnumStatus.UnderReview)

                    };
                    _prof.InsertOne(bookdata);
                }

                return RedirectToAction("MyReferrals", new { jobId = emp.JobId });
            }

            return View(emp);
        }

        private void PopulateCategories()
        {

           var listStatus = Enum.GetValues(typeof(EnumStatus)).Cast<EnumStatus>();
            ViewData["categories"] = listStatus;
            ViewData["defaultCategory"] = listStatus.First();
        }

        public IActionResult MyReferrals(string jobId)
        {

            PopulateCategories();

            var widgetViewModel = new ProfileModel();
            widgetViewModel.JobID = jobId;
            return View(widgetViewModel);
        }

        public static string GetEnumDisplayName(Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .Name;
        }

        enum EnumStatus
        {
            [Display(Name = "Under Review")]
            UnderReview = 0,
            [Display(Name = "Under Evaluation")]
            UnderEvaluation = 1,
            [Display(Name = "Screen Rejected")]
            ScreenRejected = 2,
            [Display(Name = "Evaluation Rejected")]
            EvaluationRejected = 3,
            [Display(Name = "Selected")]
            Selected = 4,
            [Display(Name = "Offered")]
            Offered = 5,
            [Display(Name = "Onboarded")]
            Onboarded = 6,
            [Display(Name = "Offer Declined")]
            OfferDeclined = 7
        }
    }
}