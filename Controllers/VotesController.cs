using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using LoadingAPI.Data;
using LoadingAPI.Models;
using LoadingAPI.Hubs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LoadingAPI.Controllers
{
    // Angir at denne klassen er en API-kontroller med route "api/votes"
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<VoteHub> _hubContext;
        private readonly ILogger<VotesController> _logger;

        // Konstruktør som initialiserer databasekonteksten, SignalR-hub konteksten og loggeren
        public VotesController(AppDbContext context, IHubContext<VoteHub> hubContext, ILogger<VotesController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        // Metode for å sende inn en stemme for en bestemt sesjon
        [HttpPost("{sessionId}")]
        public async Task<IActionResult> SubmitVote(int sessionId, [FromBody] Vote vote)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid vote model received.");
                return BadRequest(ModelState);
            }

            vote.SessionId = sessionId; // Setter SessionId på stemmen
            _context.Votes.Add(vote); // Legger til stemmen i databasen

            try
            {
                await _context.SaveChangesAsync(); // Lagrer endringene
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving vote: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

            // Sender sanntidsoppdatering til alle klienter
            await _hubContext.Clients.All.SendAsync("ReceiveVoteUpdate", $"New vote for session {sessionId}: {vote.Choice}");

            _logger.LogInformation($"Vote submitted for session ID: {sessionId}, User ID: {vote.UserId}");

            // Returnerer en 201 Created respons med en referanse til den nye stemmen
            return CreatedAtAction(nameof(GetVote), new { id = vote.VoteId }, vote);
        }

        // Metode for å hente en bestemt stemme basert på ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVote(int id)
        {
            var vote = await _context.Votes.FindAsync(id); // Søker etter stemmen i databasen
            if (vote == null)
            {
                // Returnerer en 404 Not Found respons hvis stemmen ikke finnes
                _logger.LogWarning($"Vote with ID: {id} not found.");
                return NotFound();
            }
            // Returnerer en 200 OK respons med stemmen
            return Ok(vote);
        }
    }
}
