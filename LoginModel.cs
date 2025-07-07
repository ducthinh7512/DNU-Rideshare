namespace DNUrideshare.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; } // "Customer" hoặc "Admin"
    }
}