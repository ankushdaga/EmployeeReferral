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
    public class Profile
    {
       public string CandidateName { get; set; }
       public string UniqueID { get; set; }
       public string BlobURI { get; set; }
       public string AdditionalDetails { get; set; }
       public string ReferredBy { get; set; }
       public string DateReferred { get; set; }
       public string JobID { get; set; }
       public string Experience { get; set; }
       public string Location { get; set; }
       public string Screening1Status { get; set; }
       public string Screening2Status { get; set; }
       public string FinalRoundStatus { get; set; }
       public string ProfileStatus { get; set; }
       public string HRComment { get; set; }
    }
}
