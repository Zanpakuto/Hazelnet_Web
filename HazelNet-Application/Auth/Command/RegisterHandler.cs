using HazelNet_Application.Interface;
using HazelNet_Domain.Models;

namespace HazelNet_Application.Auth;

public class RegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public record RegisterUserCommand(string Username, string email, string Password);
    public record RegisterResult(bool Success, string? ErrorMessage);
    
    public RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResult> Handle(RegisterUserCommand command)
    {
        if (await _userRepository.EmailExistAsync(command.email))
            return new  RegisterResult(false, $"Email {command.email} already exists");
        
        var hash = _passwordHasher.Hash(command.Password);

        var user = new User
        {
            Username = command.Username,
            EmailAddress = command.email,
            PasswordHash = hash
        };

        await _userRepository.RegisterUserAsync(user);
        return new  RegisterResult(true, null);
    }
}