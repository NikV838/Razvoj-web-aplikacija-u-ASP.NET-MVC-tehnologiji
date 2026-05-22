using System;

namespace SljemeTimeAttack.ViewModels
{
    public class RunDeleteViewModel
    {
        public int Id { get; set; }
        public string Track { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string CarName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string BestTime { get; set; } = string.Empty;
    }
}
