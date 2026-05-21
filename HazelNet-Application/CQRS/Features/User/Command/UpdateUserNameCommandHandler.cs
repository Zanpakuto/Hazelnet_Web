using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Application.Interface;

namespace HazelNet_Application.CQRS.Features.User.Command;

public class UpdateUserNameCommandHandler : ICommandHandler<UpdateUserNameCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserNameCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserNameCommand command)
    {
        await _userRepository.UpdateUserNameAsync(command.UserId, command.Name);
    }
}