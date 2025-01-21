public class LoginResult
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public string? Token { get; set; }
}

public class Login
{
    public string Email { get; set; }
    public string Password { get; set; }
}