using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ReferralSystem.Models;
using ReferralSystem.Repository;

namespace ReferralSystem.Controllers
{
   // [Authorize(Roles = "Employee")]

    public class HomeController : Controller
    {
        private readonly IMongoRepository<UserModel> _user;
        private readonly IMongoRepository<Position> _position;

        private readonly IMongoRepository<Demand> _demand;


        public HomeController(IMongoRepository<UserModel> userrRepository, IMongoRepository<Position> positionRepo, IMongoRepository<Demand> DemandRepo)
        {
            _user = userrRepository;
            _position = positionRepo;
            _demand = DemandRepo;
        }

        public async  Task<IActionResult> Index()
        {



            if (User.Identity.IsAuthenticated)
            {
                //var authorization = this.Request.Headers["Authorization"].ToString();
                //// var tokenstring = authorization.Substring("Bearer ".Length).Trim();
                //var handler = new JwtSecurityTokenHandler();

                var ss = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (_user.Get().Any(x => x.EmailId.Equals(ss)))
                {
                    return View();
                }
                else
                {
                    if (ss != null)
                    {
                        var bookdata = new UserModel()
                        {
                            EmailId = ss,
                           // Role = "Employee",
                            DisplayName = ss.Substring(0, ss.IndexOf('@')).Replace('.', ' '),
                            IsActive = "True"
                        };
                        _user.InsertOne(bookdata);

                    }

                    
                }
             //   _user.InsertOne(bookdata);
            }

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Privacy()
        {
             return View();
        }


        [HttpGet]
        public async Task<ActionResult> Roles()
        {
           
            return View();
        }

        private void PopulateRoleNames()
        {
            var roles = new List<Roles>();
            roles.Add(new Roles()
            {
                RoleName = "Employee",
                RoleId = 1
            });
            roles.Add(new Roles()
            {
                RoleName = "Recruiter",
                RoleId = 2

            });

            ViewData["RoleNames"] = roles;

        }


        private IEnumerable<User> Read()
        {
            List<User> positions = new List<User>();
            positions.Add(new User() { DisplayName = "Ankush"});

            return positions;
        }


        public IActionResult Users_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<UserModel> userResult = new List<UserModel>();

            

            //  var abc = GetAllUsers().Result;


            return Json(userResult.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult UpdateRoles(UserModel emp)
        {
            var empList = _user.Get().Where(x=>x.EmailId == emp.EmailId).FirstOrDefault();

            if (empList != null)
            {
                empList.RoleName = emp.RoleName;
                _user.ReplaceOne(empList);
            }

            return RedirectToAction("Roles");
        }


        public ActionResult UpdateRole(string id, string bu, string emailId)
        {
            var widgetViewModel = new UserModel();
            widgetViewModel.DisplayName = id;
            widgetViewModel.RoleName = bu;
            widgetViewModel.EmailId = emailId;

            return PartialView("_RoleDetails", widgetViewModel);
        }

        public async Task<IActionResult> DownloadResume(string blobUri)
        { 
            Uri myUri = new Uri(blobUri, UriKind.Absolute);

        string accessKey =
                "";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("uploads");
            var fileName = new CloudBlockBlob(myUri).Name;
            var blockBlob = cloudBlobContainer.GetBlobReference(fileName);
           
            MemoryStream memStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(memStream);


            HttpResponse response = HttpContext.Response;
            //response.ContentType = blockBlob.Properties.ContentType;
            response.ContentType = blockBlob.Properties.ContentType;
            response.Headers.Add("Content-Disposition", "Attachment; filename=" + fileName);
            response.Headers.Add("Content-Length", blockBlob.Properties.Length.ToString());
            return File(memStream.ToArray(), blockBlob.Properties.ContentType, fileName);
        }


        public IActionResult Manage_Roles([DataSourceRequest]DataSourceRequest request)
        {
            List<UserModel> positions = new List<UserModel>();
            var empList = _user.Get();
            foreach (var aa in empList)
            {
                positions.Add(new UserModel
                {
                    EmailId = aa.EmailId,
                    DisplayName = aa.DisplayName,
                    RoleName = aa.RoleName
                });
            }

            //  PopulateRoleNames();

            var roles = new List<Roles>();
            roles.Add(new Roles()
            {
                RoleName = "Employee",
                RoleId = 1
            });
            roles.Add(new Roles()
            {
                RoleName = "Recruiter",
                RoleId = 2

            });

            ViewData["RoleNames"] =
                new SelectList(roles, "RoleId", "RoleName");

            //ViewData["RoleNames"] = roles;

            return Json(positions.ToDataSourceResult(request));

        }


        public async Task<IEnumerable<User>> GetAllUsers()
        {

            string owner = (User.FindFirst(ClaimTypes.X500DistinguishedName))?.Value;


            List<User> userResult = new List<User>();

            GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider());

            IGraphServiceUsersCollectionPage users1 = await graphClient.Users.Request().Top(500).GetAsync(); // The hard coded Top(500) is what allows me to pull all the users, the blog post did this on a param passed in

            IGraphServiceUsersCollectionPage users = await graphClient.Users.Request().Top(500).GetAsync(); // The hard coded Top(500) is what allows me to pull all the users, the blog post did this on a param passed in
            userResult.AddRange(users);

            while (users.NextPageRequest != null)
            {
                users = await users.NextPageRequest.GetAsync();
                userResult.AddRange(users);
            }

            return userResult;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Demands_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<Demand> demands = new List<Demand>();
            demands = _demand.Get();
            return Json(demands.ToDataSourceResult(request));
        }

        [HttpGet]
        public async Task<ActionResult> Demand()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Position()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> CreatePosition(string id)
        {
            var demand = _demand.FindById(id);

            return View(demand);
        }


        public JsonResult DemandIds_Read()
        {
            var demands = _demand.Get();

            return Json(demands);
        }

        [HttpPost]
        public ActionResult SavePosition(Demand demand)
        {
            Position _position = new Position()
            {
                Band = demand.Band,
                BusinessUnit = demand.BusinessUnit,
                ClosingDate = demand.ClosingDate,
                DemandDate = demand.DemandDate,
                Experience = demand.Experience,
                Location = demand.Location,
                NoOfVacancies = demand.NoOfVacancies,
                ProjectName = demand.ProjectName,
                RequesterEmailID = demand.RequesterEmailID,
                Role = demand.Role,
                Status = demand.Status,
                DemandId = demand.DemandId,
                Skills = demand.Skills,
                JobDescription = demand.JobDescription
            };

            this._position.InsertOne(_position);

            return RedirectToAction("Position");

        }

        public IActionResult Positions_Read([DataSourceRequest]DataSourceRequest request)
        {
            var demands = _position.Get();
            return Json(demands.ToDataSourceResult(request));
        }

    }
}
