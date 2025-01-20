public static class EncryptHelper
{

    public static string GenerateSalt()
    {
        var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        var saltBytes = new byte[8];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes).Substring(0, 10);
    }

    public static string HashPassword(string password, string salt)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var combinedPassword = password + salt;
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combinedPassword));
        return Convert.ToBase64String(hashBytes);
    }
}