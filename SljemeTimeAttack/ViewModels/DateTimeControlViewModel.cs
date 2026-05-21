using System;

namespace SljemeTimeAttack.ViewModels
{
    public class DateTimeControlViewModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public DateTime? Value { get; set; }
        public string Placeholder { get; set; } = "Select date and time";
    }
}
