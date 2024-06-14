using Microsoft.AspNetCore.Mvc;
using LoadingAPI.Data;
using LoadingAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoadingAPI.Controllers
{
    // Angir at denne klassen er en API-kontroller med route "api/sessions"
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SessionsController> _logger;

        // Konstruktør som initialiserer databasekonteksten og loggeren
        public SessionsController(AppDbContext context, ILogger<SessionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Metode for å opprette en ny sesjon
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] Session session)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid session model received.");
                return BadRequest(ModelState);
            }

            _context.Sessions.Add(session);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving session: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
            
            _logger.LogInformation($"Session created with ID: {session.SessionId}");
            // Returnerer en 201 Created respons med en referanse til den nye sesjonen
            return CreatedAtAction(nameof(GetSession), new { id = session.SessionId }, session);
        }

        // Metode for å hente alle sesjoner
        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            var sessions = await _context.Sessions.ToListAsync();
            // Returnerer en 200 OK respons med listen over sesjoner
            return Ok(sessions);
        }

        // Metode for å hente en spesifikk sesjon basert på ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var session = await _context.Sessions.Include(s => s.Scenarios).FirstOrDefaultAsync(s => s.SessionId == id);
            if (session == null)
            {
                // Returnerer en 404 Not Found respons hvis sesjonen ikke finnes
                _logger.LogWarning($"Session with ID: {id} not found.");
                return NotFound();
            }
            // Returnerer en 200 OK respons med den spesifikke sesjonen
            return Ok(session);
        }

        // Metode for å starte en sesjon
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartSession(int id)
        {
            // Sletter alle stemmer før en ny sesjon starter
            var votes = await _context.Votes.ToListAsync();
            _context.Votes.RemoveRange(votes);
            await _context.SaveChangesAsync();

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                // Returnerer en 404 Not Found respons hvis sesjonen ikke finnes
                _logger.LogWarning($"Session with ID: {id} not found.");
                return NotFound();
            }

            // Angir at sesjonen er aktiv
            session.IsActive = true;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting session: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInformation($"Session with ID: {id} started.");
            // Returnerer en 200 OK respons med den oppdaterte sesjonen
            return Ok(session);
        }
    }
}
