using HazelNet_Application.Interface;

namespace HazelNet_Application.Auth;

public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public record LoginQuery(string email, string password);
    public record LoginResult(bool Success, string? ErrorMessage);
    
    public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResult> Handle(LoginQuery query)
    {
        
            var storedHash = await _userRepository.GetPasswordHashAsync(query.email);
            
            if (storedHash is null)
            {
                // Return generic error to prevent enumeration
                return new LoginResult(false, "Invalid email or password.");
            }
            
            bool matched = _passwordHasher.Verify(query.password, storedHash);

            if (matched)
                return new  LoginResult(true, null);
           
            return new LoginResult(false, $"The password {query.password} is wrong");
        
       
    }
    
}