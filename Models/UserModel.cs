using System;
using System.Collections.Generic;
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
    [BsonCollection("User")]
    public class UserModel : Document
    {       
        public string DisplayName { get; set; }
        public string Firstname  { get; set; }
        public string Lastname   { get; set; }
        public string EmailId    { get; set; } 
        public string IsActive   { get; set; }

        public string RoleName { get; set; }

        [BsonIgnore]
        [UIHint("AllRoles")]
        public Roles Role
        {
            get;
            set;
        }
    }

    [BsonIgnoreExtraElements]
    public class Roles
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

    }
}
