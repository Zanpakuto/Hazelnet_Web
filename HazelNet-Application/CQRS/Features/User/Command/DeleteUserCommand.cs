using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Features.User.Command;

public record DeleteUserCommand(int UserId) : ICommand;