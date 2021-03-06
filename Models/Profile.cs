﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ReferralSystem.Service;

namespace ReferralSystem.Models
{
    [BsonIgnoreExtraElements]
    [BsonCollection("Profile")]
    public class ProfileModel : Document
    {
        [DisplayName("First Name")]
        public string CandidateName { get; set; }
        [DisplayName("Last Name")]
        public string CandidateSurname { get; set; }
       public string MobileNumber { get; set; }
       public string BlobURI { get; set; }
       public string AdditionalDetails { get; set; }
       public string ReferredBy { get; set; }
       public DateTime DateReferred { get; set; }
       public string JobID { get; set; }
       public string Experience { get; set; }
       public string Location { get; set; }
       public string Screening1Status { get; set; }
       public string Screening2Status { get; set; }
       public string FinalRoundStatus { get; set; }
       // public string ProfileStatus { get; set; }
        public string HRComment { get; set; }
        [DisplayName("Email")]
        public string EmailId { get; set; }



        [UIHint("AllRoles")]
        public string ProfileStatus { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Status
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; }

    }
}
