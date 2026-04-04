using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;

namespace HazelNet_Application.CQRS.Commands;

public class CreateUserCommand : ICommand
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public CreateUserCommand(string username, string email, string passwordHash)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
    }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(CreateUserCommand command)
    {
        var user = new User
        {
            Username = command.Username,
            EmailAddress = command.Email,
            PasswordHash = command.PasswordHash
        };

        await _userRepository.Create(user);
    }
}