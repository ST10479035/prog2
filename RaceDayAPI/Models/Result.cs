using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("Results")]
    public class Result
    {
        [Key]
        [Column("result_id")]
        public int ResultId { get; set; }

        [Required]
        [Column("enrolment_id")]
        public int EnrolmentId { get; set; }

        [Column("finish_time_seconds")]
        public int? FinishTimeSeconds { get; set; }

        [Column("position")]
        public int? Position { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Finished";

        [ForeignKey("EnrolmentId")]
        public EventEnrolment? Enrolment { get; set; }
    }
}