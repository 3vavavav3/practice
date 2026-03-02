namespace CarRepairRequests.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }

        public ICollection<Request> MasterRequests { get; set; }
        public ICollection<Request> ClientRequests { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
