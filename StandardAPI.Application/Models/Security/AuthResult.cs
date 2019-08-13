namespace StandardAPI.Application.Models.Security
{
    public  class AuthResult
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
