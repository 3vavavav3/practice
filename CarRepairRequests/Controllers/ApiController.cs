using CarRepairRequests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRepairRequests.Models;

namespace CarRepairRequests.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ApiController(AppDbContext db)
        {
            _db = db;
        }

        private bool IsOperatorOrManager(string userType)
        {
            return userType == "Оператор" || userType == "Менеджер";
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _db.Users.ToListAsync());
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            user.Id = 0;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
            return user == null ? Unauthorized() : Ok(user);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests(string userType, int? userId)
        {
            IQueryable<Request> query = _db.Requests;

            if (userType == "Заказчик" && userId.HasValue)
            {
                query = query.Where(r => r.ClientId == userId);
            }
            else if (userType == "Автомеханик" && userId.HasValue)
            {
                query = query.Where(r => r.MasterId == userId || r.RequestStatus == "Новая заявка");
            }

            return Ok(await query.ToListAsync());
        }

        [HttpGet("requests/{id}")]
        public async Task<IActionResult> GetRequest(int id)
        {
            var request = await _db.Requests.FindAsync(id);
            return request == null ? NotFound() : Ok(request);
        }

        [HttpGet("requests/search")]
        public async Task<IActionResult> SearchRequests(string text)
        {
            var results = await _db.Requests
                .Where(r => r.CarModel.Contains(text) || r.ProblemDescription.Contains(text))
                .ToListAsync();
            return Ok(results);
        }

        [HttpPost("requests")]
        public async Task<IActionResult> CreateRequest([FromBody] Request request)
        {
            request.Id = 0;
            if (request.StartDate == DateTime.MinValue)
                request.StartDate = DateTime.Now;

            await _db.Requests.AddAsync(request);
            await _db.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPut("requests/{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] RequestUpdateModel model, string userType)
        {
            var existingRequest = await _db.Requests.FindAsync(id);
            if (existingRequest == null) return NotFound();

            if (userType == "Автомеханик")
            {
                existingRequest.RequestStatus = model.RequestStatus;
                existingRequest.RepairParts = model.RepairParts;
            }
            else
            {
                existingRequest.RequestStatus = model.RequestStatus;
                existingRequest.RepairParts = model.RepairParts;
                existingRequest.MasterId = model.MasterId;
            }

            if (model.RequestStatus == "Завершена" && existingRequest.RequestStatus != "Завершена")
            {
                existingRequest.CompletionDate = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return Ok(existingRequest);
        }

        [HttpDelete("requests/{id}")]
        public async Task<IActionResult> DeleteRequest(int id, string userType)
        {
            if (userType != "Оператор" && userType != "Менеджер")
                return Unauthorized();

            var request = await _db.Requests.FindAsync(id);
            if (request == null) return NotFound();

            _db.Requests.Remove(request);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments(int? requestId)
        {
            if (requestId.HasValue)
                return Ok(await _db.Comments.Where(c => c.RequestId == requestId).ToListAsync());
            return Ok(await _db.Comments.ToListAsync());
        }

        [HttpPost("comments")]
        public async Task<IActionResult> CreateComment(Comment comment, int? masterId)
        {
            if (!masterId.HasValue) return Unauthorized();

            comment.MasterId = masterId;
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
            return Ok(comment);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(string userType)
        {
            if (userType != "Оператор" && userType != "Менеджер")
                return Unauthorized();

            var requests = await _db.Requests.ToListAsync();

            return Ok(new
            {
                completedCount = requests.Count(r => r.RequestStatus == "Завершена"),
                byProblem = requests
                    .Where(r => !string.IsNullOrEmpty(r.ProblemDescription))
                    .GroupBy(r => r.ProblemDescription)
                    .ToDictionary(g => g.Key, g => g.Count())
            });
        }

        [HttpGet("manager/reports")]
        public async Task<IActionResult> GetManagerReports(string userType)
        {
            if (userType != "Менеджер") return Unauthorized();

            var requests = await _db.Requests.ToListAsync();

            return Ok(new
            {
                totalRequests = requests.Count,
                completedRequests = requests.Count(r => r.RequestStatus == "Завершена"),
                inProgress = requests.Count(r => r.RequestStatus == "В процессе"),
                masters = requests.Where(r => r.MasterId != null).GroupBy(r => r.MasterId).Count()
            });
        }

    }



    public class LoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class RequestUpdateModel
    {
        public string RequestStatus { get; set; }
        public string RepairParts { get; set; }
        public int? MasterId { get; set; }
    }
}