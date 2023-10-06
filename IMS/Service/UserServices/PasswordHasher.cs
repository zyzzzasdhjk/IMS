using System;
using System.Security.Cryptography;
using System.Text;

namespace IMS.Service.UserServices;

public class PasswordHasher
{
    private const int SaltSize = 16; // 盐的字节数

    public static string HashPassword(string password)
    {
        // 生成随机盐
        var salt = GenerateSalt();

        // 将盐和密码拼接，然后进行哈希
        var combinedBytes = Encoding.UTF8.GetBytes(salt + password);
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes) + "|" + salt;
        }
    }

    public static bool CheckPassword(string password, string hashWithSalt)
    {
        var items = hashWithSalt.Split('|');
        if (items.Length != 2)
        {
            return false;
        }

        var hash = items[0];
        var salt = items[1];

        // 将用户输入的密码和盐拼接，然后进行哈希
        var combinedBytes = Encoding.UTF8.GetBytes(salt + password);
        using (var sha256 = SHA256.Create())
        {
            var computedHashBytes = sha256.ComputeHash(combinedBytes);
            var computedHash = Convert.ToBase64String(computedHashBytes);
            return hash == computedHash;
        }
    }

    private static string GenerateSalt()
    {
        using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
        {
            var saltBytes = new byte[SaltSize];
            rngCryptoServiceProvider.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}