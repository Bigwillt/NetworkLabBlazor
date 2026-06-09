namespace NetworkLabBlazor.Models
{
    public class AddressGroup
    {
        public string Device { get; set; } = string.Empty;
        public List<AddressRow> Rows { get; set; } = new();
    }

    public class AddressRow
    {
        public string Interface { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public string Subnet { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
    }
}
