using LogService.Data;
using LogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly LogDbContext _context;

        public LogsController(LogDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] Log log)
        {
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return Ok(log);
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _context.Logs.ToListAsync(); // ToListAsync() kullan
            return Ok(logs);
        }
    }
}