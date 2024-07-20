using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class TaskItem
    {

        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}