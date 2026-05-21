using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Application.Interface;

namespace HazelNet_Application.CQRS.Features.User.Command;

public class UpdateUserPasswordCommandHandler : ICommandHandler<UpdateUserPasswordCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserPasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserPasswordCommand command)
    {
        // The repository should handle hashing the new password and verifying the current password.
        var success = await _userRepository.UpdatePasswordAsync(command.UserId, command.CurrentPassword, command.NewPassword);
            
        if (!success)
        {
            throw new Exception("Invalid current password or update failed.");
        }
    }
}
