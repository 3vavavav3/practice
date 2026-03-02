using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CarRepairRequests.Models
{
    public class Request
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public string CarType { get; set; }
        public string CarModel { get; set; }
        public string ProblemDescryption { get; set; }
        public string RequestStatus { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string RepairParts { get; set; }
        public int? MasterId { get; set; }
        public int? ClientId { get; set; }

        public User Master { get; set; }    
        public User Client { get; set; }      

        public ICollection<Comment> Comments { get; set; }
    }
}
