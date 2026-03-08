using CarRepairRequests.Models;
using System.Text.Json.Serialization;

public class Request
{
    public int Id { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("carType")]
    public string CarType { get; set; }

    [JsonPropertyName("carModel")]
    public string CarModel { get; set; }

    [JsonPropertyName("problemDescription")]
    public string ProblemDescription { get; set; }

    [JsonPropertyName("requestStatus")]
    public string RequestStatus { get; set; }

    [JsonPropertyName("completionDate")]
    public DateTime? CompletionDate { get; set; }

    [JsonPropertyName("repairParts")]
    public string RepairParts { get; set; }

    [JsonPropertyName("masterId")]
    public int? MasterId { get; set; }

    [JsonPropertyName("clientId")]
    public int? ClientId { get; set; }

    // Навигационные свойства
    public User Master { get; set; }
    public User Client { get; set; }
    public ICollection<Comment> Comments { get; set; }
}