using System.Security.Cryptography;
using HazelNet_Application.Interface;
using System.Text;
using HazelNet_Application.Interface;
using Konscious.Security.Cryptography;


namespace HazelNet_Infrastracture.Command;

public class PasswordHasher :  IPasswordHasher
{
    private const int SaltSize = 16;
    private const int Iterations = 4;
    private const int MemorySize = 65536;
    private const int DegParallelism = 4;
    private const int HashSize = 64;
    
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.DegreeOfParallelism = DegParallelism;
        argon2.MemorySize = MemorySize;
        argon2.Salt = salt;
        argon2.Iterations = Iterations;


        var hash = argon2.GetBytes(HashSize);
        return $"{Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('$');
        if (parts.Length != 2) return false;
        
        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);
        
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.DegreeOfParallelism = DegParallelism;
        argon2.MemorySize = MemorySize;
        argon2.Salt = salt;
        argon2.Iterations = Iterations;
        
        var newHash = argon2.GetBytes(HashSize);
        return CryptographicOperations.FixedTimeEquals(storedHash, newHash);
    }
}