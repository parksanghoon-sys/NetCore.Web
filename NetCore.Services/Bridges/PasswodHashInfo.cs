namespace NetCore.Services.Bridges
{
    public class PasswodHashInfo
    {
        public string? GUIDSalt { get; set; }
        public string? RNGSalt { get; set; }
        public string? PasswodHash { get; set; }
    }
}
