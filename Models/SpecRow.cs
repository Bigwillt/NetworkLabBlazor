namespace NetworkLabBlazor.Models
{
    public class SpecRow
    {
        public required string Task { get; set; }
        public required string Specification { get; set; }

        // NEW: optional header row
        public bool IsHeader { get; set; } = false;
    }

}