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
    public class UserModel : Document
    {

       
        public string DisplayName { get; set; }

        public string Id { get; set; }

 
    }
}
