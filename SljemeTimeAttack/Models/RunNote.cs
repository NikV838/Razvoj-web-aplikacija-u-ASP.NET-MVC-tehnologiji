using System;

namespace SljemeTimeAttack.Models
{
    public class RunNote
    {
        public int Id { get; set; }
        public int RunId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }

        //public RunNote() { }

        public RunNote(int id, int runId, string note, DateTime createdDate)
        {
            Id = id;
            RunId = runId;
            Note = note;
            CreatedDate = createdDate;
        }
    }
}
