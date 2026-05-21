using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Application.Interface;

namespace HazelNet_Application.CQRS.Features.User.Command;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{

    private readonly IUserRepository _repository;

    public DeleteUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Handle(DeleteUserCommand command)
    {
        await _repository.DeleteUserByIdAsync(command.UserId);
    }
}