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
    [BsonCollection("Books")]
    public class Books : Document
    {

        [BsonElement("Name")]
        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public IFormFile File { get; set; }


    }
}
