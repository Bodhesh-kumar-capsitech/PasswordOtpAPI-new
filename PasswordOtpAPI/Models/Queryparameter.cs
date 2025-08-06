namespace PasswordOtpAPI.Models
{
    public class Queryparameter
    {
        public string? Search { get; set; }         
        public string? SortBy { get; set; } = "Email"; 
        public string? SortDir { get; set; } = "asc"; 
        public int Page { get; set; } = 1;           
        public int PageSize { get; set; } = 10;
    }
}
