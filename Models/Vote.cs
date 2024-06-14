using System.ComponentModel.DataAnnotations;

namespace LoadingAPI.Models
{
    // Definerer en stemmemodell
    public class Vote
    {
        // Konstruktør som sikrer at nødvendige egenskaper er initialisert
        public Vote()
        {
            UserId = string.Empty;
            Choice = string.Empty;
        }

        // Unik identifikator for stemmen
        public int VoteId { get; set; }
        
        // Identifikator for sesjonen som stemmen tilhører
        public int SessionId { get; set; }
        
        // Identifikator for brukeren som har avgitt stemmen
        [Required]
        [StringLength(50, ErrorMessage = "UserId cannot be longer than 50 characters.")]
        public string UserId { get; set; }
        
        // Valg som brukeren har stemt på
        [Required]
        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string Choice { get; set; }
    }
}
