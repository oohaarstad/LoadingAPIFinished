using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LoadingAPI.Models
{
    // Definerer en sesjonsmodell
    public class Session
    {
        // Konstruktør som sikrer at lister er initialisert
        public Session()
        {
            Scenarios = new List<Scenario>();
        }

        // Unik identifikator for sesjonen
        public int SessionId { get; set; }

        // Tittel på sesjonen
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
        public string Title { get; set; } = string.Empty; // Angir en standardverdi

        // Liste over scenarier tilknyttet denne sesjonen
        public List<Scenario> Scenarios { get; set; }

        // Angir om sesjonen er aktiv
        public bool IsActive { get; set; }

        // Nåværende valg i sesjonen
        [StringLength(100, ErrorMessage = "Current option cannot exceed 100 characters.")]
        public string CurrentOption { get; set; } = string.Empty; // Angir en standardverdi

        // Definerer en scenario-modell innenfor sesjonen
        public class Scenario
        {
            // Konstruktør som sikrer at lister er initialisert
            public Scenario()
            {
                Options = new List<string>();
            }

            // Unik identifikator for scenariet
            public int Id { get; set; }

            // Tittel på scenariet
            [Required]
            [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
            public string Title { get; set; } = string.Empty; // Angir en standardverdi

            // Liste over alternativer i scenariet
            public List<string> Options { get; set; }
        }
    }
}
