using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ReferralSystem.Service;

namespace ReferralSystem.Models
{
    public class DemandModel
    {
       public int DemandID { get; set; }
       public string BusinessUnit { get; set; }
       public string ProjectName { get; set; }
       public string Position { get; set; }
       public string Band { get; set; }
       public string Experience { get; set; }
       public string Location { get; set; }
       public string NoOfVacancies { get; set; }
       public string RequesterEmail { get; set; }
       public string DemandDate { get; set; }
       public string ClosingDate { get; set; }
       public string Status { get; set; }
    }
}
