using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("EventEnrolments")]
    public class EventEnrolment
    {
        [Key]
        [Column("enrolment_id")]
        public int EnrolmentId { get; set; }

        [Required]
        [Column("event_id")]
        public int EventId { get; set; }

        [Required]
        [Column("participant_id")]
        public int ParticipantId { get; set; }

        [Column("enrolment_date")]
        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Registered";

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [ForeignKey("ParticipantId")]
        public User? Participant { get; set; }

        public Result? Result { get; set; }
    }
}