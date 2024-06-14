using Microsoft.AspNetCore.SignalR;
using LoadingAPI.Data;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LoadingAPI.Hubs
{
    // Definerer en SignalR-hub for å håndtere stemmegivning i sanntid
    public class VoteHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VoteHub> _logger;
        // ConcurrentDictionary for å holde oversikt over stemmer per sesjon
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, string>> Votes = new ConcurrentDictionary<int, ConcurrentDictionary<string, string>>();

        // Konstruktør som tar databasekonteksten og loggeren som parameter
        public VoteHub(AppDbContext context, ILogger<VoteHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Metode for å håndtere overgang til neste steg i en sesjon
        public async Task NextStep(int sessionId, int stepNumber)
        {
            _logger.LogInformation($"NextStep called with sessionId: {sessionId}, stepNumber: {stepNumber}");
            Votes[sessionId] = new ConcurrentDictionary<string, string>(); // Nullstiller stemmer for det nye steget

            try
            {
                await Clients.All.SendAsync("NextStep", sessionId, stepNumber); // Sender oppdatering til alle klienter
                _logger.LogInformation($"NextStep sent to clients for sessionId: {sessionId}, stepNumber: {stepNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending NextStep: {ex.Message}");
                throw;
            }
        }

        // Metode for å avslutte stemmegivningen og sende resultatene
        public async Task EndVoting(int sessionId)
        {
            _logger.LogInformation($"EndVoting called for sessionId: {sessionId}");

            try
            {
                // Henter og grupperer stemmer fra databasen
                var voteResults = _context.Votes
                    .Where(v => v.SessionId == sessionId)
                    .GroupBy(v => v.Choice)
                    .Select(group => new { option = group.Key, count = group.Count() })
                    .ToList();

                _logger.LogInformation($"Vote results for sessionId: {sessionId}");
                foreach (var result in voteResults)
                {
                    _logger.LogInformation($"Option: {result.option}, Count: {result.count}");
                }

                // Sender resultatene til alle klienter
                await Clients.All.SendAsync("VoteResults", voteResults);
                _logger.LogInformation($"Vote results sent to clients for sessionId: {sessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in EndVoting: {ex.Message}");
                throw;
            }
        }

        // Metode for å registrere en stemme fra en bruker
        public async Task SubmitVote(int sessionId, string userId, string choice)
        {
            var sessionVotes = Votes.GetOrAdd(sessionId, new ConcurrentDictionary<string, string>());
            sessionVotes[userId] = choice; // Registrerer stemmen

            _logger.LogInformation($"Vote recorded: sessionId={sessionId}, userId={userId}, choice={choice}");
            _logger.LogInformation($"Current votes for sessionId {sessionId}: {string.Join(", ", sessionVotes.Select(v => $"{v.Key}: {v.Value}"))}");

            try
            {
                await Clients.All.SendAsync("ReceiveVoteUpdate", $"User {userId} voted for {choice}"); // Sender stemmeoppdatering til alle klienter
                _logger.LogInformation($"Vote update sent to clients for sessionId: {sessionId}, userId: {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending vote update: {ex.Message}");
                throw;
            }
        }
    }
}
