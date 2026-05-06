using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SljemeTimeAttack.Models
{
    public class RunNote
    {
        [Key]
        public int Id { get; set; }
        public int RunId { get; set; }
        [ForeignKey(nameof(RunId))]
        public Run Run { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }

        public RunNote()
        {
            Run = null!;
            Note = string.Empty;
        }

        public RunNote(int id, int runId, string note, DateTime createdDate)
        {
            Id = id;
            RunId = runId;
            Run = null!;
            Note = note;
            CreatedDate = createdDate;
        }
    }
}
