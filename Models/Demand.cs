using System;
using ReferralSystem.Service;

namespace ReferralSystem.Models
{
    [BsonCollection("Demands")]
    public class Demand : Document
    {
        public string DemandId { get; set; }
        public string BusinessUnit { get; set; }
       public string ProjectName { get; set; }
       public string Role { get; set; }
       public string Band { get; set; }
       public string Experience { get; set; }
       public string Location { get; set; }
       public string NoOfVacancies { get; set; }
       public string RequesterEmailID { get; set; }

        private DateTime _DateTime { get; set; }

        public DateTime DemandDate
        {
            get { return _DateTime.Date; } set { this._DateTime = value.Date; }
        }

        public DateTime ClosingDate { get; set; }
       public string Status { get; set; }
       public string JobDescription { get; set; }
       public string Skills { get; set; }

    }
}
