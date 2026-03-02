namespace CarRepairRequests.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int? MasterId { get; set; }
        public int? RequestId { get; set; }

        public User Master { get; set; }
        public Request Request { get; set; }
    }
}
