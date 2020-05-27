using ReferralSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;

namespace ReferralSystem.Models
{
    [BsonCollection("Employee")]
    public class Employee : Document
    {

        public string JobId { get; set; }

        public string BusinessUnit { get; set; }

        public string Position { get; set; }

        public string Experience { get; set; }

        public string Location { get; set; }

        public string NoOfVacancies { get; set; }

        public DateTime ClosingDate { get; set; }

        [DisplayName("Upload Resume")]
        public IFormFile File { get; set; }

    }
}
