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

public class UpdateUserCommand : ICommand
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public UpdateUserCommand(int userId, string username, string email, string passwordHash)
    {
        UserId = userId;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
    }
}

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserCommand command)
    {
        var user = await _userRepository.Get(command.UserId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        user.Username = command.Username;
        user.EmailAddress = command.Email;
        user.PasswordHash = command.PasswordHash;

        await _userRepository.Update(user);
    }
}