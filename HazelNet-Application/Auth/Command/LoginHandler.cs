using HazelNet_Application.Interface;

namespace HazelNet_Application.Auth;

public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public record LoginQuery(string email, string password);
    public record LoginResult(bool Success, int? userID, string? username, string? ErrorMessage);
    
    public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResult> Handle(LoginQuery query)
    {
        
            var user  = await _userRepository.GetUserByEmailAsync(query.email);
            
            if (user is null)
            {
                // Return generic error to prevent enumeration
                return new LoginResult(false, null, null, "Invalid email or password.");
            }
            
            bool matched = _passwordHasher.Verify(query.password, user.PasswordHash);

            if (matched)
                return new  LoginResult(true, user.Id, user.Username, null);

            return new LoginResult(false, null, null,$"The password {query.password} is wrong");
        
       
    }
    
}