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

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers() => Ok(await _db.Users.ToListAsync());

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return user  == null ? NotFound() : Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string login,string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            return user == null ? Unauthorized() : Ok(user);
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
                .Where(r => r.CarModel.Contains(text) || r.ProblemDescryption.Contains(text))
                .ToListAsync();
            return Ok(results);

        }

        [HttpPost("requests")]
        public async Task<IActionResult> CreateRequest(Request request)
        {
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPut("requests/{id}")]
        public async Task<IActionResult> UpdateRequest(int id, Request request)
        {
            if (id != request.Id) return BadRequest();
            _db.Entry (request).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("requests/{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
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
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
            return Ok(comment);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            int completed = await _db.Requests.CountAsync(r => r.RequestStatus == "Завершена");

            var completedReqs = await _db.Requests
                .Where(r => r.CompletionDate != null)
                .ToListAsync();

            double avgTime = completedReqs.Any()
                ? Math.Round(completedReqs.Average(r => (r.CompletionDate.Value - r.StartDate).TotalHours), 1)
                : 0;

            var byProblem = await _db.Requests
                .GroupBy(r => r.ProblemDescryption)
                .Select(g => new { Problem = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.Problem, v => v.Count);

            return Ok(new { completedCount = completed, avgTime, byProblem });
        }
    }
}
