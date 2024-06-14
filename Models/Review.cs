using System;
using System.ComponentModel.DataAnnotations;

namespace LoadingAPI.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Constructor to ensure required properties are initialized
        public Review(int sessionId, string userId, string text)
        {
            SessionId = sessionId;
            UserId = userId;
            Text = text;
        }
    }
}
