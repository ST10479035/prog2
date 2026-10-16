using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public int EventId { get; set; }

        [Required]
        [Column("organiser_id")]
        public int OrganiserId { get; set; }

        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("event_date")]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("location")]
        public string Location { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrganiserId")]
        public User? Organiser { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<EventEnrolment> Enrolments { get; set; } = new List<EventEnrolment>();
    }
}