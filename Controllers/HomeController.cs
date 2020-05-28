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

            demands.Add(new Demand() { Experience = "10", ClosingDate = "13/12/21", NoOfVacancies="5", Position ="SSC" });
            demands.Add(new Demand() { Experience = "5", ClosingDate = "06/01/20", NoOfVacancies = "1", Position = "SC" });

            return Json(demands.ToDataSourceResult(request));
        }
    }
}
