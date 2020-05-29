using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using ReferralSystem.Models;
using ReferralSystem.Repository;

namespace ReferralSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Privacy()
        {
             return View();
        }

        [HttpGet]
        public async Task<ActionResult> Demand()
        {
            return View();
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


            var abc = GetAllUsers().Result;

            foreach (var aa in abc)
            {
                userResult.Add(new UserModel() { DisplayName = aa.DisplayName});
            }

            return Json(userResult.ToDataSourceResult(request));
        }


        public async Task<IEnumerable<User>> GetAllUsers()
        {
            List<User> userResult = new List<User>();

            GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider());
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

            var _mongoRepository = new MongoRepository<Demand>(new MongoDbSettings { DatabaseName = "EmployeeReferralDb", ConnectionString = "mongodb://localhost:27017" });
            demands = _mongoRepository.Get();

            return Json(demands.ToDataSourceResult(request));
        }

        [HttpGet]
        public async Task<ActionResult> Position()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> CreatePosition(string id)
        {
            var _mongoRepository = new MongoRepository<Demand>(new MongoDbSettings { DatabaseName = "EmployeeReferralDb", ConnectionString = "mongodb://localhost:27017" });
            var demand = _mongoRepository.FindById(id);

            return View(demand);
        }

        [HttpGet]
        public async Task<ActionResult> DemandDetails(string id)
        {
            var _mongoRepository = new MongoRepository<Demand>(new MongoDbSettings { DatabaseName = "EmployeeReferralDb", ConnectionString = "mongodb://localhost:27017" });
            var demand = _mongoRepository.FindById(id);

            return View(demand);
        }

        public JsonResult DemandIds_Read()
        {
            var _mongoRepository = new MongoRepository<Demand>(new MongoDbSettings { DatabaseName = "EmployeeReferralDb", ConnectionString = "mongodb://localhost:27017" });
            var demands = _mongoRepository.Get();

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

            var _mongoRepository = new MongoRepository<Position>(new MongoDbSettings { DatabaseName = "EmployeeReferralDb", ConnectionString = "mongodb://localhost:27017" });
            _mongoRepository.InsertOne(_position);

            return RedirectToAction("Position");

        }

        public IActionResult Positions_Read([DataSourceRequest]DataSourceRequest request)
        {
            var _mongoRepository = new MongoRepository<Position>(new MongoDbSettings { DatabaseName = "EmployeeReferralDb", ConnectionString = "mongodb://localhost:27017" });
            var demands = _mongoRepository.Get();

            return Json(demands.ToDataSourceResult(request));
        }

    }
}
