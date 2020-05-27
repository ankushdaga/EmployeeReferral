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
        public string Firstname  { get; set; }
        public string Lastname   { get; set; }
        public string EmailId    { get; set; }
        public string Username   { get; set; }
        public string Password   { get; set; }
        public string Role       { get; set; }
        public string IsActive   { get; set; }
    }
}
