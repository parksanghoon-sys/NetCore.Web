using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

class Program
{ 

    private static void Main(string[] args)
    {
        Console.Write("Enter a UserId: ");
        string? userId = Console.ReadLine();

        Console.Write("Enter a password: ");
        string? password = Console.ReadLine();

        string guidSalt = Guid.NewGuid().ToString();
        
        string rngSalt = GetRNGSalt();

        // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
        // Pbkdf2
        // Password-based key derivation function2
     
        var hashed = GetPasswordHash(userId, password, guidSalt, rngSalt);
        // 데이터 베이스에 있는 비밀번호 정보와 지금 입력한 비밀번호 정보를 비교해서 같은 해시 값이 나오면 로그인
        bool check = IsCheckThePasswordInfo(userId, password, guidSalt, rngSalt, hashed);
        Console.WriteLine($"usderId: {userId}");
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Salt: {rngSalt}");
        Console.WriteLine($"Hashed: {hashed}");
        Console.WriteLine($"guidSalt: {guidSalt}");
        Console.WriteLine($"check: {(check ? "비밀번호 일치" : "불일치")}");
    }
    private static string GetRNGSalt()
    {
        // generate a 128-bit salt using a secure PRNG
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }        
        return  Convert.ToBase64String(salt);
    }
    private static string GetPasswordHash(string userId, string password,string guidSalt, string rngSalt)
    {
        return  Convert.ToBase64String(KeyDerivation.Pbkdf2(
         password: userId + password + guidSalt,
         salt: Encoding.UTF8.GetBytes(rngSalt),
         prf: KeyDerivationPrf.HMACSHA512,
         iterationCount: 45000, // 10000, 25000, 45000
         numBytesRequested: 256 / 8));
    }
    private static bool IsCheckThePasswordInfo(string userId, string password, string guidSalt, string  rngSalt, string passwordHash)
    {
        return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
    }
}



