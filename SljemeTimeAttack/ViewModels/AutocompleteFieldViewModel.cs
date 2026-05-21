namespace SljemeTimeAttack.ViewModels
{
    public class AutocompleteFieldViewModel
    {
        public string Label { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public int? SelectedId { get; set; }
        public string SelectedText { get; set; } = string.Empty;
        public string SearchUrl { get; set; } = string.Empty;
        public string Placeholder { get; set; } = string.Empty;
    }
}
